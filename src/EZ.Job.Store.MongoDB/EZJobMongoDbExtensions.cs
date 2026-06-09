using EZ.Job.Core;
using EZJob.Store.MongoDB;

namespace Microsoft.Extensions.DependencyInjection;

public static class EZJobMongoDbExtensions
{
    public static IEZJobBuilder AddMongoDbStore(this IEZJobBuilder builder, string connectionString)
    {
        return AddMongoDbStore(builder, o =>
        {
            o.ConnectionString = connectionString;
        });
    }

    public static IEZJobBuilder AddMongoDbStore(this IEZJobBuilder builder, Action<MongoDbStoreOptions> configure)
    {
        var options = new MongoDbStoreOptions();
        configure(options);

        builder.Services.AddSingleton<IJobStore>(_ =>
            new MongoDbJobStore(options.ConnectionString, options.DatabaseName, options.CollectionName));
        builder.Services.AddSingleton<IRecurringStore>(_ =>
            new MongoDbRecurringStore(options.ConnectionString, options.DatabaseName, options.RecurringCollectionName));

        return builder;
    }
}
