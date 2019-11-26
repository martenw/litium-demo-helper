using System;
using Litium.Accelerator.Demo.Services;
using Litium.Foundation;
using Litium.Foundation.GUI;
using Litium.Globalization;
using Litium.Products;
using Litium.Websites;
using Litium.Xunit;
using Xunit;

namespace Litium.Accelerator.Demo
{
    public class PageInformation : ApplicationTestBase
    {
        /// <summary>
        /// Publish every page and block in every channel
        /// </summary>
        [Fact]
        public void PublishEverything()
        {
            var websiteService = IoC.Resolve<WebsiteService>();
            var pagesDemoService = IoC.Resolve<IPagesDemoService>();
            var channelService = IoC.Resolve<ChannelService>();
            
            using (FoundationContext.Current.SystemToken.Use())
            {
                foreach (var website in websiteService.GetAll())
                {
                   foreach (var channel in channelService.GetAll())
                   {
                       pagesDemoService.PublishContent(website, channel);
                   }
                }
            }
        }
    }
}