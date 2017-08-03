using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseMonitoringSystem.Tests.Core.Common.Extensions
{
    using global::Core.Common.Extensions;
    using Shouldly;
    using Xunit;

    public class ObjectExtension_Tests
    {
        [Fact]
        public void To_Tests()
        {
            "42".To<int>().ShouldBeOfType<int>().ShouldBe(42);
            "42".To<Int32>().ShouldBeOfType<Int32>().ShouldBe(42);

            "28173829281734".To<long>().ShouldBeOfType<long>().ShouldBe(28173829281734);
            "28173829281734".To<Int64>().ShouldBeOfType<Int64>().ShouldBe(28173829281734);

            "2.0".To<double>().ShouldBe(2.0);
            "0.2".To<double>().ShouldBe(0.2);
            (2.0).To<int>().ShouldBe(2);

            "false".To<bool>().ShouldBeOfType<bool>().ShouldBe(false);
            "True".To<bool>().ShouldBeOfType<bool>().ShouldBe(true);

            "2260afec-bbfd-42d4-a91a-dcb11e09b17f".To<Guid>().ShouldBeOfType<Guid>().ShouldBe(new Guid("2260afec-bbfd-42d4-a91a-dcb11e09b17f"));

            Assert.Throws<FormatException>(() => "test".To<bool>());
            Assert.Throws<FormatException>(() => "test".To<int>());
        }
    }
}
