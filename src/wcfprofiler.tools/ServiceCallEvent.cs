using System;

namespace wcfprofiler.tools
{
	public class ServiceCallEvent
	{
		public DateTime TimeStamp { get; set; }
		public string Thread { get; set; }
		public TimeSpan Elapsed { get; set; }
		public string Login { get; set; }
		public string Method { get; set; }
		public string Inputs { get; set; }
		public string Outputs { get; set; }
		public string Return { get; set; }

		public override string ToString()
		{
			return String.Format("{0} - {1}", Method, Elapsed);
		}
	}
}
