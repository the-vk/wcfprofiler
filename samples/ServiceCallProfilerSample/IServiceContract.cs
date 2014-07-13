using System.ServiceModel;

namespace ServiceCallProfilerSample
{
	[ServiceContract]
	interface IServiceContract
	{
		[OperationContract]
		int SampleMethod(int counter, out int doubledCounter, ref int tripledCounter);
	}
}
