using System.Collections.Generic;
using Litium.Products;
using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Demo.Services
{
    [Service(
        ServiceType = typeof(ICategoryDemoService),
        Lifetime = DependencyLifetime.Singleton)]
    public interface ICategoryDemoService
    {
        Category GetOrCreateCategory(Assortment assortment, string[] categoryTree);
        void PublishRecursive(Category category);
    }
}