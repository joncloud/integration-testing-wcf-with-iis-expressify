using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace XUnitTestProject1
{
    class WcfClient<T>
    {
        readonly Binding _binding;
        readonly EndpointAddress _remoteAddress;
        public WcfClient(Binding binding, EndpointAddress remoteAddress)
        {
            _binding = binding;
            _remoteAddress = remoteAddress;
        }

        public void Do(Action<T> fn)
        {
            Use(svc =>
            {
                fn(svc);
                return 0;
            });
        }

        public TResult Use<TResult>(Func<T, TResult> fn)
        {
            var channelFactory = new ChannelFactory<T>(
                _binding,
                _remoteAddress
            );

            try
            {
                return fn(channelFactory.CreateChannel());
            }
            finally
            {
                // Isn't there some special logic here?
                channelFactory.Close();
            }
        }
    }
}
