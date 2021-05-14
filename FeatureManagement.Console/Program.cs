using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace FeatureManagement.Console
{
	class Program
	{
		static IConfiguration SetupConfiguration()
		{
			var builder = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

			return builder.Build();
		}

		static IServiceCollection ConfigureServices()
		{
			var services = new ServiceCollection();
			var config = SetupConfiguration();

			services.AddSingleton(config);
			services.AddFeatureManagement(config.GetSection("Features"))
					.AddFeatureFilter<PushNotificationFilter>();

			return services;
		}

		static async Task Main(string[] args)
		{
			var services = ConfigureServices();
			var serviceProvider = services.BuildServiceProvider();
			var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

			if (await featureManager.IsEnabledAsync("SendEmail"))
			{
				SendMail();
			}

			var supportedContext = new PushNotificationContext { OsVersion = "14.5" };
			var unsupportedContext = new PushNotificationContext { OsVersion = "11.4" };

			if (await featureManager.IsEnabledAsync("SendPush", supportedContext))
			{
				SendPush();
			}

			if (!await featureManager.IsEnabledAsync("SendPush", unsupportedContext))
			{
				System.Console.WriteLine("Unsupported operating system.");
			}
		}

		static void SendMail()
		{
			System.Console.WriteLine("E-mail sent!");
		}

		static void SendPush()
		{
			System.Console.WriteLine("Push sent!");
		}
	}
}
