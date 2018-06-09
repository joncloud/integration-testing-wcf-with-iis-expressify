using System;
using System.ServiceModel;
using WcfService1;
using Xunit;

namespace XUnitTestProject1
{
    [Collection("IIS")]
    public class IntegrationTest1
    {
        readonly IisFixture _iisFixture;
        public IntegrationTest1(IisFixture iisFixture) => _iisFixture = iisFixture;

        [Fact]
        public void GetData_ShouldEchoInputValue()
        {
            var value = 123;
            var expected = $"You entered: {value}";

            var actual = _iisFixture.WcfClient<IService1>("Service1.svc")
                .BasicHttpClient()
                .Use(svc => svc.GetData(value));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetDataUsingDataContract_ShouldThrowGivenNullComposite()
        {
            _iisFixture.WcfClient<IService1>("Service1.svc")
                .BasicHttpClient()
                .Do(AssertService);

            void AssertService(IService1 service)
            {
                Assert.Throws<FaultException>(() => service.GetDataUsingDataContract(null));
            }
        }

        [Fact]
        public void GetDataUsingDataContract_ShouldSuffixStringValue()
        {
            var composite = new CompositeType
            {
                BoolValue = true,
                StringValue = "abc"
            };
            var actual = _iisFixture.WcfClient<IService1>("Service1.svc")
                .BasicHttpClient()
                .Use(svc => svc.GetDataUsingDataContract(composite));

            Assert.Equal(composite.BoolValue, actual.BoolValue);
            Assert.Equal(composite.StringValue + "Suffix", actual.StringValue);
        }
    }
}
