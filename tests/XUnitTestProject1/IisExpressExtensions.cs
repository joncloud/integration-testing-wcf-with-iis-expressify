using IISExpressify;
using System;

namespace XUnitTestProject1
{
    static class IisExpressExtensions
    {
        public static WcfClientBuilder<T> WcfClient<T>(this IisExpress iisExpress, string path)
        {
            var builder = new UriBuilder(iisExpress.BaseUri);
            builder.Path = path;

            return new WcfClientBuilder<T>(builder.Uri);
        }
    }
}
