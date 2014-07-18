using System;
using System.Collections.Generic;
using System.Configuration;
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
			public object[] Outputs;
			public object Return;
			public string Login;
			public string Method;
			public Stopwatch Stopwatch;
		}

		private static readonly ILog Log = LogManager.GetLogger("ServiceCallProfiler");

		private static readonly Action<State>[] LogActions;

		static ServiceCallProfilerParameterInspector()
		{
			var config = ConfigurationManager.GetSection("serviceCallProfiler") as ServiceCallProfilerConfigurationSection;
			var skipData = false;
			var skipLargeData = false;
			if (config != null)
			{
				skipData = config.SkipData;
				skipLargeData = config.SkipLargeData;
			}

			var actions = new List<Action<State>>
			{
				StoreLogin,
				StoreMethod
			};

			if (!skipData)
			{
				if (skipLargeData)
				{
					actions.Add(StoreSmallInputs);
					actions.Add(StoreSmallOutputs);
					actions.Add(StoreSmallReturn);
				}
				else
				{
					actions.Add(StoreInputs);
					actions.Add(StoreOutputs);
					actions.Add(StoreReturn);
				}
			}

			LogActions = actions.ToArray();
		}

		public object BeforeCall(string operationName, object[] inputs)
		{
			return new State {Inputs = inputs, Stopwatch = Stopwatch.StartNew()};
		}

		public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
		{
			var state = (State) correlationState;
			state.Stopwatch.Stop();
			state.Outputs = outputs;
			state.Return = returnValue;
			

			var login = OperationContext.Current != null && OperationContext.Current.ServiceSecurityContext != null
				? OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name
				: "unknown";

			state.Login = login;
			state.Method = operationName;

			for (var i = 0; i < LogActions.Length; ++i)
			{
				LogActions[i](state);
			}
			
			Log.Debug(state.Stopwatch.Elapsed.Ticks);
		}

		private static void StoreLogin(State state)
		{
			LogicalThreadContext.Properties["login"] = state.Login;
		}

		private static void StoreMethod(State state)
		{
			LogicalThreadContext.Properties["method"] = state.Method;
		}

		private static void StoreInputs(State state)
		{
			LogicalThreadContext.Properties["inputs"] = JsonConvert.SerializeObject(state.Inputs);
		}

		private static void StoreSmallInputs(State state)
		{
			LogicalThreadContext.Properties["inputs"] = JsonConvert.SerializeObject(state.Inputs, new LargeDataConverter());
		}

		private static void StoreOutputs(State state)
		{
			LogicalThreadContext.Properties["outputs"] = JsonConvert.SerializeObject(state.Outputs);
		}

		private static void StoreSmallOutputs(State state)
		{
			LogicalThreadContext.Properties["outputs"] = JsonConvert.SerializeObject(state.Outputs, new LargeDataConverter());
		}

		private static void StoreReturn(State state)
		{
			LogicalThreadContext.Properties["return"] = JsonConvert.SerializeObject(state.Return);
		}

		private static void StoreSmallReturn(State state)
		{
			LogicalThreadContext.Properties["return"] = JsonConvert.SerializeObject(state.Return, new LargeDataConverter());
		}
	}
}
