using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace XUnitTestProject1
{
    public class WebConfig
    {
        readonly string _source;
        readonly XDocument _document;
        public WebConfig(string source)
        {
            _source = source;
            _document = XDocument.Load(_source);

            // <configuration>
            //   <connectionStrings>
            //     <add name="..." connectionString="..."
        }

        public WebConfig ReplaceConnectionString(string name, string connectionString)
        {
            _document.Root
                .Element("connectionStrings")
                .Elements("add")
                .First(add => add.Attribute("name").Value == name)
                .Attribute("connectionString")
                .SetValue(connectionString);
            return this;
        }

        public FileSwap Swap()
        {
            var target = Path.Combine(
                Path.GetTempPath(), 
                Guid.NewGuid().ToString()
            );
            _document.Save(target);

            return new FileSwap(_source, target);
        }
    }
}
