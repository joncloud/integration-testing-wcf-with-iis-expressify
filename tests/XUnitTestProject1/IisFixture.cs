using IISExpressify;
using System;
using System.IO;

namespace XUnitTestProject1
{
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
                .ReplaceConnectionString("SchoolDbContext", Test.ConnectionString)
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
