using System;
using Litium.Accelerator.Demo.Services;
using Litium.FieldFramework;
using Litium.Foundation;
using Litium.Foundation.GUI;
using Litium.Xunit;
using Xunit;

namespace Litium.Accelerator.Demo
{
    public class PimDataModel : ApplicationTestBase
    {
        // Example fields defined below, replace:
        [Theory]
        [InlineData("Brand", true, "Product information", SystemFieldTypeConstants.TextOption, "ProductWithVariants", "Brand1;Brand2;Brand3;null", false)]
        [InlineData("TextFieldSample", false, "Product information", SystemFieldTypeConstants.Text, "ProductWithVariants", "", true)]
        [InlineData("OptionFieldSample", false, "Product information", SystemFieldTypeConstants.TextOption, "ProductWithVariants", "Yes;No;null", false)]
        [InlineData("SizeOptionFieldSample", false, "Product information", SystemFieldTypeConstants.TextOption, "ProductWithVariants", "203;622-29;630;305;559;622-28;635;584-26;507;609;428;406;null", false)]
        [InlineData("ImportImageUrl", false, "ImportFields", SystemFieldTypeConstants.Text, "ProductWithVariants", "", false)]
        [InlineData("CategoryPaths", true, "ImportFields", SystemFieldTypeConstants.Text, "ProductWithVariants", "", false)]
        public void CreateFields(string fieldName, bool baseProductfield, string fieldGroup, string fieldType, string templateName, string options, bool multiCulture)
        {
            var demoService = IoC.Resolve<IFieldDemoService>();

            using (FoundationContext.Current.SystemToken.Use())
            {
                switch (fieldType)
                {
                    case SystemFieldTypeConstants.Decimal:
                        if (!string.IsNullOrEmpty(options))
                            throw new Exception("Remove defined options for int field");
                        demoService.AddDecimalField(fieldName, baseProductfield, fieldGroup, templateName, multiCulture);
                        break;
                    case SystemFieldTypeConstants.Int:
                        if (!string.IsNullOrEmpty(options))
                            throw new Exception("Remove defined options for int field");
                        demoService.AddIntField(fieldName, baseProductfield, fieldGroup, templateName, multiCulture);
                        break;
                    case SystemFieldTypeConstants.Text:
                        if (!string.IsNullOrEmpty(options))
                            throw new Exception("Remove defined options for text field");
                        demoService.AddTextField(fieldName, baseProductfield, fieldGroup, templateName, multiCulture);
                        break;
                    case SystemFieldTypeConstants.TextOption:
                        demoService.AddTextOptionField(fieldName, baseProductfield, fieldGroup, templateName, options);
                        break;
                    default:
                        throw new Exception($"Field type not implemented:{fieldType}");
                }
            }
        }
    }
}