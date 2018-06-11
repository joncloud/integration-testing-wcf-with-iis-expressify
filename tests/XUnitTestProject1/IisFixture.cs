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
        public IisFixture()
        {
            var hostingDirectory = GetHostingDirectory();

            _iisExpress = IisExpress.Https()
                .PhysicalPath(hostingDirectory)
                .Port(44300)
                .HideSystray()
                .Start();
        }

        public void Dispose()
        {
            _iisExpress.Dispose();
        }

        internal WcfClientBuilder<T> WcfClient<T>(string path) => _iisExpress.WcfClient<T>(path);
    }
}
