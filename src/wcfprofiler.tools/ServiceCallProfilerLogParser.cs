using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace wcfprofiler.tools
{

    public class ServiceCallProfilerLogParser
    {
	    public static IEnumerable<ServiceCallEvent> Parse(Stream inputStream)
	    {
		    var settings = new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Fragment};
		    var reader = XmlReader.Create(inputStream, settings);
		    ServiceCallEvent currentEvent = null;
		    reader.Read();
		    while (!reader.EOF)
		    {
			    switch (reader.NodeType)
			    {
					case XmlNodeType.Element:
					    switch (reader.Name)
					    {
						    case "event":
								currentEvent = new ServiceCallEvent
								{
									TimeStamp = DateTime.Parse(reader.GetAttribute("timestamp")),
									Thread = reader.GetAttribute("thread")
								};
							    reader.Read();
							    break;
							case "login":
							    currentEvent.Login = reader.ReadElementContentAsString();
							    break;
							case "method":
							    currentEvent.Method = reader.ReadElementContentAsString();
							    break;
							case "inputs":
							    currentEvent.Inputs = reader.ReadElementContentAsString();
							    break;
							case "outputs":
							    currentEvent.Outputs = reader.ReadElementContentAsString();
							    break;
							case "return":
							    currentEvent.Return = reader.ReadElementContentAsString();
							    break;
							case "elapsed":
							    currentEvent.Elapsed = TimeSpan.FromTicks(reader.ReadElementContentAsLong());
							    break;
					    }
					    break;
					case XmlNodeType.EndElement:
						if (reader.Name == "event") yield return currentEvent;
					    reader.Read();
						break;
					default:
					    reader.Read();
						break;
			    }
		    }
	    }
    }
}
