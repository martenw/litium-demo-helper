using System.Globalization;
using Litium.Accelerator.Demo.Services;
using Litium.FieldFramework;
using Litium.Foundation;
using Litium.Foundation.GUI;
using Litium.Globalization;
using Litium.Websites;
using Litium.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace Litium.Accelerator.Demo
{
    public class PageInformation : ApplicationTestBase
    {
        public PageInformation(ITestOutputHelper output)
        {
            _output = output;
        }

        private readonly ITestOutputHelper _output;

        /// <summary>
        ///     Publish every page and block in every channel
        /// </summary>
        [Fact]
        public void PublishEverything()
        {
            var websiteService = IoC.Resolve<WebsiteService>();
            var pagesDemoService = IoC.Resolve<IPagesDemoService>();
            var channelService = IoC.Resolve<ChannelService>();

            using (FoundationContext.Current.SystemToken.Use("Automated publish using demo service"))
            {
                foreach (var website in websiteService.GetAll())
                foreach (var channel in channelService.GetAll())
                {
                    _output.WriteLine($"Publishing content for channel '{channel.Fields[SystemFieldDefinitionConstants.Name, CultureInfo.CurrentUICulture]}' on website '{website.Fields[SystemFieldDefinitionConstants.Name, CultureInfo.CurrentUICulture]}'");
                    pagesDemoService.PublishContent(website, channel, _output);
                }
            }

            _output.WriteLine("DONE - REMEMBER TO RESTART APPLICATION FOR CHANGES TO SHOW ON WEBSITE");
        }
    }
}