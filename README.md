WCF Profiler
============

WCF Profiles is a set of tools to give you insight on WCF service performance metrics.

Installation
============

You can install the latest stable release using nuget package wcfprofiler.

Usage
=====

Drop assembly to bin folder, add behaviors to service configuration - and that's it!

ServiceCallProfiler
-------------------

ServiceCallProfiler is a set of endpoint and operation behaviors to log every call to WCF service.
Information is sent to log4net logger ServiceCallProfiler.
Add behavior as shown on example below to enable profiling:

	<system.serviceModel>
		<services>
			<service name="ServiceCallProfilerSample.ServiceImplementation">
				<endpoint address="soap.udp://localhost:16544/" binding="udpBinding" contract="ServiceCallProfilerSample.IServiceContract" behaviorConfiguration="profilerBehavior" />
			</service>
		</services>
		<client>
			<endpoint name="client" address="soap.udp://localhost:16544/" binding="udpBinding" contract="ServiceCallProfilerSample.IServiceContract" />
		</client>
		<behaviors>
			<endpointBehaviors>
				<behavior name="profilerBehavior">
					<serviceCallProfiler />
				</behavior>
			</endpointBehaviors>
		</behaviors>
		<extensions>
			<behaviorExtensions>
				<add name="serviceCallProfiler" type="wcfprofiler.ServiceCallProfiler, wcfprofiler"/>
			</behaviorExtensions>
		</extensions>
	</system.serviceModel>

### Configuration

ServiceCallProfiler has two options to disable logging large datasets (byte[] arrays and strings longer 1024 symbols) and to disable logging data completely.
Example:

	<configuration>
		<configSections>
			<section name="serviceCallProfiler" type="wcfprofiler.ServiceCallProfilerConfigurationSection, wcfprofiler"/>
		</configSections>
		<serviceCallProfiler skipData="true" skipLargeData="true" />
	</configuration>

### Performance

Profiling overhead is fairly low - ServiceCallProfiler adds about 0.05 ms to each call duration on Intel i5-4200U, your mileage may vary.
Some log4net configuration is required to achieve low overhead:

* File logger should be configured to use log4net.Appender.FileAppender+ExclusiveLock
* It's recommended to use custom XML layout wcfprofiler.ServiceCallProfilerXmlLayout to render logging event with minimal overhead (compared to default log4net.Layout.XmlLayout)

Sample log4net configuration:

	<log4net>
		<appender name="ServiceCallProfilerAppender" type="log4net.Appender.RollingFileAppender">
			<file type="log4net.Util.PatternString" value="logs\" />
			<appendtofile value="true" />
			<rollingstyle value="Date" />
			<datePattern value="dd-MM-yyyy'.log'" />
			<maxsizerollbackups value="10" />
			<maximumfilesize value="20MB" />
			<staticlogfilename value="false" />
			<immediateFlush value="true" />
			<layout type="wcfprofiler.ServiceCallProfilerXmlLayout, wcfprofiler" />
		</appender>
		<logger name="ServiceCallProfiler">
			<appender-ref ref="ServiceCallProfilerAppender" />
		</logger>
	</log4net>

### Log4net properties

ServiceCallProfiler sets log4net properties:

* login - name of primary identity from service security context if it exsits, or 'unknown' if it does not
* method - name of called method
* inputs - method arguments serialized to json
* outputs - method outs and refs arguments serialized to json
* return - return value serialized to json

Call duration is logged as count of ticks and passed to log4net as message content.

### Tools

Assembly wcfprofiler.tools contains handy and high performant parser ServiceCallProfilerLogParser to handle output of wcfprofiler.ServiceCallProfilerXmlLayout.

License
=======

This project is licensed under the [MIT license](https://github.com/the-vk/wcfprofiler/blob/master/LICENSE).
