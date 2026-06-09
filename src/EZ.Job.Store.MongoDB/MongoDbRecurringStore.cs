using EZ.Job.Core;
using MongoDB.Driver;

namespace EZJob.Store.MongoDB;

public sealed class MongoDbRecurringStore : IRecurringStore
{
    private readonly IMongoCollection<RecurringDefinitionDocument> _collection;

    public MongoDbRecurringStore(string connectionString, string databaseName = "ez_jobs", string collectionName = "ez_recurring_definitions")
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _collection = database.GetCollection<RecurringDefinitionDocument>(collectionName);

        var index = new CreateIndexModel<RecurringDefinitionDocument>(
            Builders<RecurringDefinitionDocument>.IndexKeys.Ascending(r => r.CreatedAtUtc));
        _collection.Indexes.CreateOne(index);
    }

    public async ValueTask AddOrUpdateAsync(RecurringDefinition definition, CancellationToken cancellationToken = default)
    {
        var filter = Builders<RecurringDefinitionDocument>.Filter.Eq(r => r.Id, definition.Id.ToString());
        var doc = ToDocument(definition);

        await _collection.ReplaceOneAsync(filter, doc, new ReplaceOptions { IsUpsert = true }, cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<RecurringDefinitionDocument>.Filter.Eq(r => r.Id, id.ToString());
        await _collection.DeleteOneAsync(filter, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<RecurringDefinition?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<RecurringDefinitionDocument>.Filter.Eq(r => r.Id, id.ToString());
        var doc = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        return doc is not null ? FromDocument(doc) : null;
    }

    public async ValueTask<IEnumerable<RecurringDefinition>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var docs = await _collection.Find(FilterDefinition<RecurringDefinitionDocument>.Empty)
            .SortBy(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return docs.ConvertAll(FromDocument);
    }

    public async ValueTask SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken = default)
    {
        var filter = Builders<RecurringDefinitionDocument>.Filter.Eq(r => r.Id, id.ToString());
        var update = Builders<RecurringDefinitionDocument>.Update.Set(r => r.IsActive, isActive);

        await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private static RecurringDefinitionDocument ToDocument(RecurringDefinition definition)
    {
        return new RecurringDefinitionDocument
        {
            Id = definition.Id.ToString(),
            TypeName = definition.TypeName,
            MethodName = definition.MethodName,
            ArgumentTypes = definition.ArgumentTypes,
            Arguments = definition.Arguments,
            CronExpression = definition.CronExpression,
            IsActive = definition.IsActive,
            CreatedAtUtc = definition.CreatedAtUtc,
            LastExecutionUtc = definition.LastExecutionUtc
        };
    }

    private static RecurringDefinition FromDocument(RecurringDefinitionDocument doc)
    {
        return new RecurringDefinition(
            Id: Guid.Parse(doc.Id),
            TypeName: doc.TypeName,
            MethodName: doc.MethodName,
            ArgumentTypes: doc.ArgumentTypes ?? [],
            Arguments: doc.Arguments ?? [],
            CronExpression: doc.CronExpression,
            IsActive: doc.IsActive,
            CreatedAtUtc: doc.CreatedAtUtc,
            LastExecutionUtc: doc.LastExecutionUtc);
    }
}

internal sealed class RecurringDefinitionDocument
{
    public string Id { get; set; } = "";
    public string TypeName { get; set; } = "";
    public string MethodName { get; set; } = "";
    public string[]? ArgumentTypes { get; set; }
    public object?[]? Arguments { get; set; }
    public string CronExpression { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastExecutionUtc { get; set; }
}
