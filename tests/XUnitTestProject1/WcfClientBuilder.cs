using System;
using System.ServiceModel;

namespace XUnitTestProject1
{
    struct WcfClientBuilder<T>
    {
        readonly Uri _uri;
        public WcfClientBuilder(Uri uri)
        {
            _uri = uri;
        }

        public WcfClient<T> BasicHttpClient()
        {
            var basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            var remoteAddress = new EndpointAddress(_uri);

            return new WcfClient<T>(basicHttpBinding, remoteAddress);
        }
    }
}
