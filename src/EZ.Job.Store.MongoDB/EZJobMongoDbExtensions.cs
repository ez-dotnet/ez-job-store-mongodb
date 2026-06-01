using EZ.Job.Core;
using EZJob.Store.MongoDB;

namespace Microsoft.Extensions.DependencyInjection;

public static class EZJobMongoDbExtensions
{
    public static EZJobBuilder AddMongoDbStore(this EZJobBuilder builder, string connectionString)
    {
        return AddMongoDbStore(builder, o =>
        {
            o.ConnectionString = connectionString;
        });
    }

    public static EZJobBuilder AddMongoDbStore(this EZJobBuilder builder, Action<MongoDbStoreOptions> configure)
    {
        var options = new MongoDbStoreOptions();
        configure(options);

        builder.Services.AddSingleton<IJobStore>(_ =>
            new MongoDbJobStore(options.ConnectionString, options.DatabaseName, options.CollectionName));

        return builder;
    }
}
