namespace ServiceCallProfilerSample
{
	class ServiceImplementation : IServiceContract
	{
		public int SampleMethod(int counter, out int doubledCounter, ref int tripledCounter)
		{
			doubledCounter = counter*2;
			tripledCounter = counter*3;
			return counter;
		}
	}
}
