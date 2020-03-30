using System;
using System.Linq;
using JetBrains.Annotations;
using Litium.Blocks;
using Litium.Globalization;
using Litium.Websites;

namespace Litium.Accelerator.Demo.Services
{
    public class PagesDemoService : IPagesDemoService
    {
        private readonly BlockService _blockService;
        private readonly PageService _pageService;

        public PagesDemoService(PageService pageService, BlockService blockService)
        {
            _pageService = pageService;
            _blockService = blockService;
        }

        public void PublishContent([NotNull] Website website, [NotNull] Channel channel)
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
                PublishRecursive(page, channel);
        }

        private void PublishRecursive(Page page, Channel channel)
        {
            var pageConnectionExists = page.ChannelLinks.Any(link => link.ChannelSystemId.Equals(channel.SystemId));
            if (!pageConnectionExists)
            {
                var writePage = page.MakeWritableClone();
                writePage.ChannelLinks.Add(new PageToChannelLink(channel.SystemId));
                _pageService.Update(writePage);
            }

            // Publish all blocks on the page
            foreach (var blockContainer in page.Blocks)
            foreach (var blockItem in blockContainer.Items.OfType<BlockItemLink>())
                PublishBlock(blockItem.BlockSystemId, channel);

            foreach (var childPage in _pageService.GetChildPages(page.SystemId))
                PublishRecursive(childPage, channel);
        }

        private void PublishBlock(Guid blockSystemId, Channel channel)
        {
            var block = _blockService.Get(blockSystemId);
            if (block == null)
                return;

            var blockConnectionExists = block.ChannelLinks.Any(link => link.ChannelSystemId.Equals(channel.SystemId));
            if (blockConnectionExists)
                return;

            var writeBlock = block.MakeWritableClone();
            writeBlock.ChannelLinks.Add(new BlockToChannelLink(channel.SystemId));
            _blockService.Update(writeBlock);
        }
    }
}