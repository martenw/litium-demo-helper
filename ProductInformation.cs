using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.Demo.Services;
using Litium.Foundation;
using Litium.Foundation.GUI;
using Litium.Products;
using Litium.Xunit;
using Xunit;

namespace Litium.Accelerator.Demo
{
    public class ProductInformation : ApplicationTestBase
    {
        private List<string[]> GetCategoryTrees(BaseProduct baseProduct)
        {
            var categoryString = baseProduct.Fields.GetValue<string>("category_path");

            // In this example the format is: bikes;bikes/parts;bikes/parts/tires
            // so first split by ; to get all the categories the product should be added to
            // then split each category string by / to get the list to use for each category
            if (string.IsNullOrEmpty(categoryString)) return null;

            var result = new List<string[]>();
            foreach (var cat in categoryString.Split(';')) result.Add(cat.Split('/').Select(s => s.Trim()).ToArray());

            return result;
        }

        [Fact]
        public void MoveProductsIntoCategories()
        {
            var productDemoService = IoC.Resolve<IProductDemoService>();
            var categoryDemoService = IoC.Resolve<ICategoryDemoService>();
            var assortmentService = IoC.Resolve<AssortmentService>();
            var assortment = assortmentService.GetAll().First();
            Assert.NotNull(assortment);

            using (FoundationContext.Current.SystemToken.Use())
            {
                foreach (var baseProduct in productDemoService.GetAllBaseProducts())
                {
                    // Get which category the product should be placed in, 
                    // fetch from a field of the baseproduct from import.
                    // TODO - This method will likely need to be modified for every use.
                    var categoryTrees = GetCategoryTrees(baseProduct);
                    if (categoryTrees == null || !categoryTrees.Any())
                        continue;

                    foreach (var categoryTree in categoryTrees)
                    {
                        var category = categoryDemoService.GetOrCreateCategory(assortment, categoryTree);
                        if (category == null)
                            continue;

                        productDemoService.AddToCategory(baseProduct, category);
                    }
                }
            }
        }

        [Fact]
        public void PublishEverything()
        {
            var categoryDemoService = IoC.Resolve<ICategoryDemoService>();
            var categoryService = IoC.Resolve<CategoryService>();
            var assortmentService = IoC.Resolve<AssortmentService>();

            using (FoundationContext.Current.SystemToken.Use())
            {
                foreach (var assortment in assortmentService.GetAll())
                    foreach (var assortmentTopLevelCategory in categoryService.GetChildCategories(Guid.Empty, assortment.SystemId))
                        categoryDemoService.PublishRecursive(assortmentTopLevelCategory);
            }
        }
    }
}