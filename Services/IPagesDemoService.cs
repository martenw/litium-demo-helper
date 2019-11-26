using Litium.Globalization;
using Litium.Runtime.DependencyInjection;
using Litium.Websites;

namespace Litium.Accelerator.Demo.Services
{
    [Service(ServiceType = typeof(IPagesDemoService), Lifetime = DependencyLifetime.Singleton)]
    public interface IPagesDemoService
    {
        void PublishContent(Website website, Channel channel);
    }
}