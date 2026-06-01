namespace EZJob.Store.MongoDB;

public class MongoDbStoreOptions
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "ez_jobs";
    public string CollectionName { get; set; } = "ez_jobs";
}
