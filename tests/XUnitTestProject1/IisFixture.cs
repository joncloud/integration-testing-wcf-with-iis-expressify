using IISExpressify;
using System;
using System.Collections.Generic;
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

    public class IisFixture : IDisposable
    {
        static string GetHostingDirectory() =>
            new DirectoryInfo(
                Path.Combine(
                    Environment.CurrentDirectory,
                    @"..\..\..\..\..\src\WcfService1"
                )
            ).FullName;
        
        readonly IisExpress _iisExpress;
        readonly FileSwap _fileSwap;
        public IisFixture()
        {
            var hostingDirectory = GetHostingDirectory();

            _fileSwap = new WebConfig(Path.Combine(hostingDirectory, "Web.config"))
                .ReplaceConnectionString("SchoolDbContext", "Initial Catalog=.;Data Source=SchoolDbContext2;Integrated Security=true;")
                .Swap();

            _iisExpress = IisExpress.Https()
                .PhysicalPath(hostingDirectory)
                .Port(44300)
                .HideSystray()
                .Start();
        }

        public void Dispose()
        {
            _iisExpress.Dispose();
            _fileSwap.Dispose();
        }

        internal WcfClientBuilder<T> WcfClient<T>(string path) => _iisExpress.WcfClient<T>(path);
    }
}
