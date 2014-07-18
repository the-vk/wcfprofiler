using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace wcfprofiler.tools
{

    public class ServiceCallProfilerLogParser
    {
	    public static IEnumerable<ServiceCallEvent> Parse(Stream inputStream, bool skipData)
	    {
		    var settings = new XmlReaderSettings
		    {
			    ConformanceLevel = ConformanceLevel.Fragment,
				CheckCharacters = false
		    };
		    var sanitizedReader = new XmlSanitizingStream(inputStream);
			var reader = XmlReader.Create(sanitizedReader, settings);
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
							    if (!skipData)
							    {
								    currentEvent.Inputs = reader.ReadElementContentAsString();
							    }
								else
								{
									reader.Skip();
								}
							    break;
							case "outputs":
							    if (!skipData)
							    {
								    currentEvent.Outputs = reader.ReadElementContentAsString();
							    }
							    else
							    {
								    reader.Skip();
							    }
							    break;
							case "return":
							    if (!skipData)
							    {
								    currentEvent.Return = reader.ReadElementContentAsString();
							    }
							    else
							    {
								    reader.Skip();
							    }
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
