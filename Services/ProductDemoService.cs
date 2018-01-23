using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Products;

namespace Litium.Accelerator.Demo.Services
{
    public class ProductDemoService : IProductDemoService

    {
        private readonly BaseProductService _baseProductService;
        private readonly FieldTemplateService _fieldTemplateService;
        private readonly VariantService _variantService;

        public ProductDemoService(BaseProductService baseProductService, FieldTemplateService fieldTemplateService, VariantService variantService)
        {
            _baseProductService = baseProductService;
            _fieldTemplateService = fieldTemplateService;
            _variantService = variantService;
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
            var linkAlreadyExist = baseProduct.CategoryLinks.Any(l => l.CategorySystemId.Equals(category.SystemId));
            if (linkAlreadyExist)
                return;

            var tmpBaseProduct = baseProduct.MakeWritableClone();
            var variants = _variantService.GetByBaseProduct(baseProduct.SystemId).ToList();
            var variantSystemIds = new HashSet<Guid>(variants.Select(v => v.SystemId));
            var link = new BaseProductToCategoryLink(category.SystemId)
            {
                ActiveVariantSystemIds = variantSystemIds
            };
            tmpBaseProduct.CategoryLinks.Add(link);
            _baseProductService.Update(tmpBaseProduct);
        }
    }
}