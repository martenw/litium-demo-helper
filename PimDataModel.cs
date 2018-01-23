﻿using System;
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
        [InlineData("Brand", true, "Product information", SystemFieldTypeConstants.TextOption, "ProductWithVariants", "Brand1;Brand2;Brand3;null")]
        [InlineData("TextFieldSample", false, "Product information", SystemFieldTypeConstants.Text, "ProductWithVariants", "")]
        [InlineData("OptionFieldSample", false, "Product information", SystemFieldTypeConstants.TextOption, "ProductWithVariants", "Yes;No;null")]
        [InlineData("SizeOptionFieldSample", false, "Product information", SystemFieldTypeConstants.TextOption, "ProductWithVariants", "203;622-29;630;305;559;622-28;635;584-26;507;609;428;406;null")]
        [InlineData("ImportImageUrl", false, "ImportFields", SystemFieldTypeConstants.Text, "ProductWithVariants", "")]
        [InlineData("CategoryPaths", true, "ImportFields", SystemFieldTypeConstants.Text, "ProductWithVariants", "")]
        public void CreateFields(string fieldName, bool baseProductfield, string fieldGroup, string fieldType, string templateName, string options)
        {
            var demoService = IoC.Resolve<IFieldDemoService>();

            using (FoundationContext.Current.SystemToken.Use())
            {
                switch (fieldType)
                {
                    case SystemFieldTypeConstants.Text:
                        if (!string.IsNullOrEmpty(options))
                            throw new Exception("Remove defined options for text field");
                        demoService.AddTextField(fieldName, baseProductfield, fieldGroup, templateName);
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