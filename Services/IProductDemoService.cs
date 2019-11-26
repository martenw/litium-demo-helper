using System.Collections.Generic;
using Litium.Products;
using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Demo.Services
{
    [Service(ServiceType = typeof(IProductDemoService), Lifetime = DependencyLifetime.Singleton)]
    public interface IProductDemoService
    {
        List<BaseProduct> GetAllBaseProducts();
        void AddToCategory(BaseProduct baseProduct, Category category);
    }
}