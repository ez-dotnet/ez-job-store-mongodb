using EZ.Job.Core;
using MongoDB.Driver;

namespace EZJob.Store.MongoDB;

public sealed class MongoDbJobStore : IJobStore
{
    private readonly IMongoCollection<JobDocument> _collection;

    static MongoDbJobStore()
    {
        global::MongoDB.Bson.Serialization.BsonSerializer.RegisterSerializer(
            typeof(DateTime),
            new global::MongoDB.Bson.Serialization.Serializers.DateTimeSerializer(DateTimeKind.Utc));
    }

    public MongoDbJobStore(string connectionString, string databaseName = "ez_jobs", string collectionName = "ez_jobs")
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _collection = database.GetCollection<JobDocument>(collectionName);

        var index = new CreateIndexModel<JobDocument>(
            Builders<JobDocument>.IndexKeys.Ascending(j => j.CreatedAt));
        _collection.Indexes.CreateOne(index);
    }

    public async ValueTask AddAsync(Job job, CancellationToken cancellationToken = default)
    {
        var doc = ToDocument(job);
        await _collection.InsertOneAsync(doc, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Job?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<JobDocument>.Filter.Eq(j => j.Id, id);
        var doc = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        return doc is not null ? FromDocument(doc) : null;
    }

    public async ValueTask<IEnumerable<Job>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var docs = await _collection.Find(FilterDefinition<JobDocument>.Empty)
            .SortBy(j => j.CreatedAt)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return docs.ConvertAll(FromDocument);
    }

    public async ValueTask UpdateStatusAsync(string id, JobStatus status, string? error = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<JobDocument>.Filter.Eq(j => j.Id, id);
        var update = Builders<JobDocument>.Update
            .Set(j => j.Status, (int)status)
            .Set(j => j.Error, error);

        await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<IEnumerable<Job>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        var filter = Builders<JobDocument>.Filter.Eq(j => j.Status, (int)JobStatus.Enqueued);
        var docs = await _collection.Find(filter)
            .SortBy(j => j.CreatedAt)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return docs.ConvertAll(FromDocument);
    }

    private static JobDocument ToDocument(Job job)
    {
        return new JobDocument
        {
            Id = job.Id,
            TypeName = job.TypeName,
            MethodName = job.MethodName,
            ArgumentTypes = job.ArgumentTypes,
            Arguments = job.Arguments,
            Status = (int)job.Status,
            CreatedAt = job.CreatedAt,
            Error = job.Error,
            RecurringJobId = job.RecurringJobId
        };
    }

    private static Job FromDocument(JobDocument doc)
    {
        return new Job(
            Id: doc.Id,
            doc.TypeName,
            doc.MethodName,
            doc.ArgumentTypes ?? [],
            doc.Arguments ?? [],
            (JobStatus)doc.Status,
            doc.CreatedAt,
            doc.Error,
            doc.RecurringJobId);
    }
}

internal sealed class JobDocument
{
    public string Id { get; set; } = "";
    public string TypeName { get; set; } = "";
    public string MethodName { get; set; } = "";
    public string[]? ArgumentTypes { get; set; }
    public object?[]? Arguments { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Error { get; set; }
    public string? RecurringJobId { get; set; }
}
