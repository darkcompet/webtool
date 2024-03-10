namespace App;

using System.Net;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

public static class RedisConfig {
	/// [Redis] For cache in-memory
	/// Ref: https://docs.redis.com/latest/rs/references/client_references/client_csharp/
	/// Connection string: https://stackexchange.github.io/StackExchange.Redis/Configuration.html#configuration-options
	public static void ConfigureRedisDk(this IServiceCollection services, AppSetting appSetting) {
		var multiplexer = ConnectionMultiplexer.Connect(appSetting.database.redis);
		services.AddSingleton<IConnectionMultiplexer>(multiplexer);

		// Config lock for redis master instances
		// https://github.com/samcook/RedLock.net
		// https://stackoverflow.com/questions/59630836/idistributedcache-with-redis-cache-performing-atomic-operations
		var factory = RedLockFactory.Create(new List<RedLockMultiplexer> {
			multiplexer
		});
		services.AddSingleton<IDistributedLockFactory, RedLockFactory>(x => factory);
	}
}
