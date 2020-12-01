using System;
using System.Collections.Generic;
using System.Linq;
using Litium.FieldFramework;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Products;

namespace Litium.Accelerator.Demo.Services
{
    public class ProductDemoService : IProductDemoService

    {
        private readonly BaseProductService _baseProductService;
        private readonly CategoryService _categoryService;
        private readonly FieldTemplateService _fieldTemplateService;
        private readonly VariantService _variantService;

        public ProductDemoService(BaseProductService baseProductService, FieldTemplateService fieldTemplateService, VariantService variantService, CategoryService categoryService)
        {
            _baseProductService = baseProductService;
            _fieldTemplateService = fieldTemplateService;
            _variantService = variantService;
            _categoryService = categoryService;
        }

        public List<BaseProduct> GetAllBaseProducts()
        {
            var result = new List<BaseProduct>();
            foreach (var fieldTemplate in _fieldTemplateService.GetAll())
                result.AddRange(_baseProductService.GetByTemplate(fieldTemplate.SystemId));
            return result;
        }

        public void AddToCategory(BaseProduct baseProduct, Category category)
        {
            var linkAlreadyExist = category.ProductLinks.Any(l => l.BaseProductSystemId.Equals(baseProduct.SystemId));
            if (linkAlreadyExist)
                return;

            var variants = _variantService.GetByBaseProduct(baseProduct.SystemId).ToList();
            var variantSystemIds = new HashSet<Guid>(variants.Select(v => v.SystemId));
            var link = new CategoryToProductLink(baseProduct.SystemId)
            {
                BaseProductSystemId = baseProduct.SystemId,
                ActiveVariantSystemIds = variantSystemIds
            };

            if (baseProduct.GetMainCategory(null) == null)
                link.MainCategory = true;

            var tmpCategory = category.MakeWritableClone();
            tmpCategory.ProductLinks.Add(link);
            _categoryService.Update(tmpCategory);
        }
    }
}