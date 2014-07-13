using System;
using System.Diagnostics;
using System.ServiceModel;

using log4net.Config;

namespace ServiceCallProfilerSample
{
	class Program
	{
		static void Main(string[] args)
		{
			XmlConfigurator.Configure();

			var serviceHost = new ServiceHost(typeof (ServiceImplementation));
			serviceHost.Open();

			var channelFactory = new ChannelFactory<IServiceContract>("client");
			var client = channelFactory.CreateChannel();

			var stopwatch = Stopwatch.StartNew();

			const int rounds = 10000;
			for (var i = 0; i < rounds; ++i)
			{
				int doubledCounter;
				var tripledCounter = 0;
				client.SampleMethod(i, out doubledCounter, ref tripledCounter);
			}

			stopwatch.Stop();

			serviceHost.Close();

			Console.WriteLine("Executed {0} calls to service in {1} ms. Average duration is {2} ms.", rounds, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / rounds);

			Console.ReadLine();
		}
	}
}
