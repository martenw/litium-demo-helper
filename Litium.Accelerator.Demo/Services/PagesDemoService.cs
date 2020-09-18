using System;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Litium.Blocks;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Websites;
using Xunit.Abstractions;

namespace Litium.Accelerator.Demo.Services
{
    public class PagesDemoService : IPagesDemoService
    {
        private readonly DraftBlockService _draftBlockService;
        private readonly DraftPageService _draftPageService;
        private readonly PageService _pageService;

        public PagesDemoService(PageService pageService, DraftBlockService draftBlockService, DraftPageService draftPageService)
        {
            _pageService = pageService;
            _draftBlockService = draftBlockService;
            _draftPageService = draftPageService;
        }

        public void PublishContent([NotNull] Website website, [NotNull] Channel channel, ITestOutputHelper output)
        {
            if (website == null)
                throw new ArgumentNullException(nameof(website));
            if (channel == null)
                throw new ArgumentNullException(nameof(channel));

            var isChannelSite = channel.WebsiteSystemId.Equals(website.SystemId);
            if (!isChannelSite)
                return;

            var rootPages = _pageService.GetChildPages(Guid.Empty, website.SystemId);

            foreach (var page in rootPages)
                PublishRecursive(page, channel, output);
        }

        private void PublishRecursive(Page page, Channel channel, ITestOutputHelper output)
        {
            var draftPage = _draftPageService.Get(page.SystemId).MakeWritableClone();
            var pageConnectionExists = draftPage.ChannelLinks.Any(link => link.ChannelSystemId.Equals(channel.SystemId));
            if (!pageConnectionExists)
            {
                // var writePage = page.MakeWritableClone();
                draftPage.ChannelLinks.Add(new DraftPageToChannelLink(channel.SystemId));
                output.WriteLine($"Connecting page '{draftPage.Fields[SystemFieldDefinitionConstants.Name, CultureInfo.CurrentUICulture]}' to channel '{channel.Fields[SystemFieldDefinitionConstants.Name, CultureInfo.CurrentUICulture]}'");
            }

            // Publish all blocks on the page
            foreach (var blockContainer in draftPage.Blocks)
            foreach (var blockItem in blockContainer.Items.OfType<BlockItemLink>())
                PublishBlock(blockItem.BlockSystemId, channel);

            _draftPageService.Update(draftPage);
            _draftPageService.Publish(draftPage);

            foreach (var childPage in _pageService.GetChildPages(page.SystemId))
                PublishRecursive(childPage, channel, output);
        }

        private void PublishBlock(Guid blockSystemId, Channel channel)
        {
            var draftBlock = _draftBlockService.Get(blockSystemId).MakeWritableClone();
            if (draftBlock == null)
                return;

            var blockConnectionExists = draftBlock.ChannelLinks.Any(link => link.ChannelSystemId.Equals(channel.SystemId));
            if (blockConnectionExists)
                return;

            draftBlock.ChannelLinks.Add(new DraftBlockToChannelLink(channel.SystemId));
            _draftBlockService.Update(draftBlock);
            _draftBlockService.Publish(draftBlock);
        }
    }
}