using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace FeatureManagement.Console.CustomFeatureProvider
{
	class Program
	{
		static IServiceCollection ConfigureServices()
		{
			var services = new ServiceCollection();

			services
				.AddScoped<IFeatureDefinitionProvider, RedisFeatureDefinitionProvider>()
				.AddFeatureManagement();

			return services;
		}

		static async Task Main(string[] args)
		{
			var services = ConfigureServices();
			var serviceProvider = services.BuildServiceProvider();
			var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

			if (await featureManager.IsEnabledAsync("SendEmail"))
			{
				System.Console.WriteLine("Email service active!");
			}
			else
			{
				System.Console.WriteLine("Email service passive!");
			}

			if (await featureManager.IsEnabledAsync("SendPush"))
			{
				System.Console.WriteLine("Push service active!");
			}
			else
			{
				System.Console.WriteLine("Push service passive!");
			}
		}
	}
}
