using Xunit;
using EZJob.Store.MongoDB;
using Xunit;

namespace EZ.Job.Store.MongoDB.Tests;

public sealed class MongoDbJobStoreTests
{
    private const string ConnectionString = "mongodb://localhost:27017";

    [Fact(Skip = "Requires MongoDB container")]
    public async Task AddAsync_should_store_job()
    {
        var store = new MongoDbJobStore(ConnectionString);
        var job = new EZ.Job.Core.Job("test-id", "T", "M", [], [], EZ.Job.Core.JobStatus.Enqueued, System.DateTime.UtcNow, null, null, null, null);

        await store.AddAsync(job);
        var result = await store.GetAsync("test-id");

        Assert.NotNull(result);
        Assert.Equal("test-id", result!.Id);
    }
}
