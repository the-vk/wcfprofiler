using System.Configuration;

namespace wcfprofiler
{
	public class ServiceCallProfilerConfigurationSection : ConfigurationSection
	{
		[ConfigurationProperty("skipData", DefaultValue = "false", IsRequired = false)]
		public bool SkipData
		{
			get { return (bool) this["skipData"]; }
			set { this["skipData"] = value; }
		}

		[ConfigurationProperty("skipLargeData", DefaultValue = "false", IsRequired = false)]
		public bool SkipLargeData
		{
			get { return (bool) this["skipLargeData"]; }
			set { this["skipLargeData"] = value; }
		}
	}
}
