using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

using log4net;
using Newtonsoft.Json;

namespace wcfprofiler
{
	public class ServiceCallProfiler : BehaviorExtensionElement
	{
		protected override object CreateBehavior()
		{
			return new ServiceCallProfilerEndpointBehavior();
		}

		public override Type BehaviorType
		{
			get { return typeof(ServiceCallProfilerEndpointBehavior); }
		}
	}

	class ServiceCallProfilerEndpointBehavior : IEndpointBehavior
	{
		public void Validate(ServiceEndpoint endpoint)
		{
		}

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			foreach (var operation in endpoint.Contract.Operations)
			{
				operation.Behaviors.Add(new ServiceCallProfilerOperationBehavior());
			}
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			foreach (var operation in endpoint.Contract.Operations)
			{
				operation.Behaviors.Add(new ServiceCallProfilerOperationBehavior());
			}
		}
	}

	class ServiceCallProfilerOperationBehavior : IOperationBehavior
	{
		public void Validate(OperationDescription operationDescription)
		{
		}

		public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
		{
			dispatchOperation.ParameterInspectors.Add(new ServiceCallProfilerParameterInspector());
		}

		public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
		{
			clientOperation.ParameterInspectors.Add(new ServiceCallProfilerParameterInspector());
		}

		public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
		{
		}
	}

	class ServiceCallProfilerParameterInspector : IParameterInspector
	{
		private class State
		{
			public object[] Inputs;
			public Stopwatch Stopwatch;
		}

		private static readonly ILog Log = LogManager.GetLogger("ServiceCallProfiler");

		public object BeforeCall(string operationName, object[] inputs)
		{
			return new State {Inputs = inputs, Stopwatch = Stopwatch.StartNew()};
		}

		public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
		{
			var state = (State) correlationState;
			var stopwatch = state.Stopwatch;
			stopwatch.Stop();
			var inputs = state.Inputs;

			var login = OperationContext.Current != null && OperationContext.Current.ServiceSecurityContext != null
				? OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name
				: "unknown";
			LogicalThreadContext.Properties["login"] = login;
			LogicalThreadContext.Properties["method"] = operationName;
			LogicalThreadContext.Properties["inputs"] = JsonConvert.SerializeObject(inputs);
			LogicalThreadContext.Properties["outputs"] = JsonConvert.SerializeObject(outputs);
			LogicalThreadContext.Properties["return"] = JsonConvert.SerializeObject(returnValue);
			Log.Debug(stopwatch.Elapsed.Ticks);
		}
	}
}
