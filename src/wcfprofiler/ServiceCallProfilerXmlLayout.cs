using System.Xml;
using log4net;
using log4net.Core;
using log4net.Layout;

namespace wcfprofiler
{
	public class ServiceCallProfilerXmlLayout : XmlLayoutBase
	{
		protected override void FormatXml(XmlWriter writer, LoggingEvent loggingEvent)
		{
			writer.WriteStartElement("event");

			writer.WriteAttributeString("timestamp", XmlConvert.ToString(loggingEvent.TimeStamp, XmlDateTimeSerializationMode.Utc));
			writer.WriteAttributeString("thread", loggingEvent.ThreadName);

			writer.WriteStartElement("elapsed");
			writer.WriteValue(loggingEvent.RenderedMessage);
			writer.WriteEndElement();

			var properties = LogicalThreadContext.Properties;

			writer.WriteStartElement("login");
			writer.WriteValue(properties["login"]);
			writer.WriteEndElement();

			writer.WriteStartElement("method");
			writer.WriteValue(properties["method"]);
			writer.WriteEndElement();

			writer.WriteStartElement("inputs");
			writer.WriteCData((string)properties["inputs"]);
			writer.WriteEndElement();

			writer.WriteStartElement("outputs");
			writer.WriteCData((string)properties["outputs"]);
			writer.WriteEndElement();

			writer.WriteStartElement("return");
			writer.WriteCData((string)properties["return"]);
			writer.WriteEndElement();

			writer.WriteEndElement();
		}
	}
}
