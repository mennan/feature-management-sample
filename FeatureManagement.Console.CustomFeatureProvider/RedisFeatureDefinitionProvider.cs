using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.FeatureManagement;
using StackExchange.Redis;

namespace FeatureManagement.Console.CustomFeatureProvider
{
	public class RedisFeatureDefinitionProvider : IFeatureDefinitionProvider
	{
		public async IAsyncEnumerable<FeatureDefinition> GetAllFeatureDefinitionsAsync()
		{
			var features = await GetAllFeatures();

			foreach (var feature in features)
			{
				yield return GetFeatureDefinition(feature.Key, feature.Value);
			}
		}

		public async Task<FeatureDefinition> GetFeatureDefinitionAsync(string featureName)
		{
			var features = await GetAllFeatures();

			if (features.TryGetValue(featureName, out var isFeatureEnabled))
			{
				return GetFeatureDefinition(featureName, isFeatureEnabled);
			}

			return null;
		}

		private async Task<Dictionary<string, bool>> GetAllFeatures()
		{
			var redis = ConnectionMultiplexer.Connect("localhost");
			var db = redis.GetDatabase();
			var featuresString = await db.StringGetAsync("Features");
			var features = JsonSerializer.Deserialize<Dictionary<string, bool>>(featuresString);

			return features;
		}

		private FeatureDefinition GetFeatureDefinition(string featureName, bool isEnabled)
		{
			var featureDefinition = new FeatureDefinition
			{
				Name = featureName
			};

			if (isEnabled)
				featureDefinition.EnabledFor = new[]
				{
					new FeatureFilterConfiguration
					{
						Name = "AlwaysOn"
					}
				};

			return featureDefinition;
		}
	}
}