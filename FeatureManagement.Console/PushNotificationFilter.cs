using System.Threading.Tasks;
using Microsoft.FeatureManagement;

namespace FeatureManagement.Console
{
	public class PushNotificationContext
	{
		public string OsVersion { get; set; }
	}

	[FilterAlias("OperatingSystem")]
	public class PushNotificationFilter : IContextualFeatureFilter<PushNotificationContext>
	{
		public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext featureFilterContext, PushNotificationContext appContext)
		{
			return Task.FromResult(appContext.OsVersion == "14.5");
		}
	}
}