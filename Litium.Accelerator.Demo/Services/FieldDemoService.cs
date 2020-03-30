using System;
using System.Collections.Generic;
using System.Linq;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Products;

namespace Litium.Accelerator.Demo.Services
{
    public class FieldDemoDemoService : IFieldDemoService
    {
        private readonly FieldDefinitionService _fieldDefinitionService;
        private readonly FieldTemplateService _fieldTemplateService;

        public FieldDemoDemoService(FieldDefinitionService fieldDefinitionService, FieldTemplateService fieldTemplateService)
        {
            _fieldDefinitionService = fieldDefinitionService;
            _fieldTemplateService = fieldTemplateService;
        }

        public void AddTextField(string fieldName, bool baseProductfield, string fieldGroup, string templateName, bool multiCulture)
        {
            var fieldDefinition = new FieldDefinition<ProductArea>(fieldName, SystemFieldTypeConstants.Text)
            {
                CanBeGridColumn = false,
                CanBeGridFilter = false,
                MultiCulture = multiCulture,
                Localizations =
                {
                    ["en-US"] = {Name = fieldName, Description = fieldName},
                    ["sv-SE"] = {Name = fieldName, Description = fieldName}
                }
            };

            var field = CreateField(fieldDefinition);
            AddToTemplate(templateName, baseProductfield, fieldGroup, field);
        }

        public void AddTextOptionField(string fieldName, bool baseProductfield, string fieldGroup, string templateName, string options)
        {
            if (string.IsNullOrEmpty(options))
                throw new Exception("Cannot create options field withot options");

            var optionList = options.Split(';').Distinct().Select(option => new TextOption.Item
            {
                Value = option,
                Name = new Dictionary<string, string> { { "en-US", option }, { "sv-SE", option } }
            }).ToList();

            var fieldDefinition = new FieldDefinition<ProductArea>(fieldName, SystemFieldTypeConstants.TextOption)
            {
                CanBeGridColumn = true,
                CanBeGridFilter = true,
                Localizations =
                {
                    ["en-US"] = {Name = fieldName, Description = fieldName},
                    ["sv-SE"] = {Name = fieldName, Description = fieldName}
                },
                Option = new TextOption
                {
                    MultiSelect = false,
                    Items = optionList
                }
            };
            var field = CreateField(fieldDefinition);
            AddToTemplate(templateName, baseProductfield, fieldGroup, field);
        }

        private FieldDefinition CreateField(FieldDefinition fieldDefinition)
        {
            var currentField = _fieldDefinitionService.Get<ProductArea>(fieldDefinition.Id);
            if (currentField != null)
            {
                // Update options of the field even if the field already exists
                if (fieldDefinition.Option != null)
                {
                    currentField = currentField.MakeWritableClone();
                    currentField.Option = fieldDefinition.Option;
                    _fieldDefinitionService.Update(currentField);
                }

                return currentField;
            }

            _fieldDefinitionService.Create(fieldDefinition);
            return _fieldDefinitionService.Get<ProductArea>(fieldDefinition.Id);
        }

        private void AddToTemplate(string templateName, bool baseProductfield, string fieldGroup, FieldDefinition currentField)
        {
            var template = _fieldTemplateService.Get<ProductFieldTemplate>(templateName);
            if (template == null)
                throw new Exception("Cannot find field template " + templateName);

            template = template.MakeWritableClone();

            var groupList = baseProductfield ? template.ProductFieldGroups.ToList() : template.VariantFieldGroups.ToList();
            if (!groupList.Any(g => g.Id.Equals(fieldGroup)))
            {
                groupList.Add(new FieldTemplateFieldGroup
                {
                    Fields = new List<string> { currentField.Id },
                    Id = fieldGroup,
                    Collapsed = false,
                    Localizations =
                    {
                        ["en-US"] = {Name = fieldGroup},
                        ["sv-SE"] = {Name = fieldGroup}
                    }
                });
            }
            else
            {
                var group = groupList.First(g => g.Id.Equals(fieldGroup));
                if (!group.Fields.Contains(currentField.Id))
                    group.Fields.Add(currentField.Id);
            }


            if (baseProductfield)
                template.ProductFieldGroups = groupList;
            else
                template.VariantFieldGroups = groupList;

            _fieldTemplateService.Update(template);
        }
    }
}