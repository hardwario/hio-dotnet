using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization.Common
{
    public class ChesterAttributeTests
    {
        [Fact]
        public void DefaultValues_ShouldBeCorrect()
        {
            // Arrange
            var chesterAttribute = new ChesterAttribute();

            // Act & Assert
            Assert.Equal(Defaults.Unknown, chesterAttribute.VendorName);
            Assert.Equal(Defaults.Unknown, chesterAttribute.ProductName);
            Assert.Equal(Defaults.Unknown, chesterAttribute.HwVariant);
            Assert.Equal(Defaults.Unknown, chesterAttribute.HwRevision);
            Assert.Equal(Defaults.Unknown, chesterAttribute.FwName);
            Assert.Equal(Defaults.Unknown, chesterAttribute.FwVersion);
            Assert.Equal(Defaults.UnknownSerialNumber, chesterAttribute.SerialNumber);
        }

        [Fact]
        public void SetValues_ShouldBeAssignedCorrectly()
        {
            // Arrange
            var chesterAttribute = new ChesterAttribute
            {
                VendorName = "VendorX",
                ProductName = "ProductY",
                HwVariant = "VariantZ",
                HwRevision = "RevA",
                FwName = "FirmwareB",
                FwVersion = "1.0.0",
                SerialNumber = "1234567890"
            };

            // Act & Assert
            Assert.Equal("VendorX", chesterAttribute.VendorName);
            Assert.Equal("ProductY", chesterAttribute.ProductName);
            Assert.Equal("VariantZ", chesterAttribute.HwVariant);
            Assert.Equal("RevA", chesterAttribute.HwRevision);
            Assert.Equal("FirmwareB", chesterAttribute.FwName);
            Assert.Equal("1.0.0", chesterAttribute.FwVersion);
            Assert.Equal("1234567890", chesterAttribute.SerialNumber);
        }
    }
}
