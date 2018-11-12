using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Litium.FieldFramework;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Globalization;
using Litium.Products;
using Litium.Studio.Plugins.Suggestions;

namespace Litium.Accelerator.Demo.Services
{
    public class CategoryDemoService : ICategoryDemoService
    {
        private readonly Dictionary<string, Category> _categoryLookup;
        private readonly CategoryService _categoryService;
        private readonly FieldTemplateService _fieldTemplateService;
        private readonly ISuggestionService _suggestionService;
        private readonly VariantService _variantService;
	    private readonly ChannelService _channelService;
	    private CategoryFieldTemplate _fieldTemplate;

        public CategoryDemoService(CategoryService categoryService,
            FieldTemplateService fieldTemplateService, ISuggestionService suggestionService, 
            VariantService variantService, ChannelService channelService)
        {
            _categoryService = categoryService;
            _fieldTemplateService = fieldTemplateService;
            _suggestionService = suggestionService;
            _variantService = variantService;
	        _channelService = channelService;
	        _categoryLookup = new Dictionary<string, Category>();
        }

        private CategoryFieldTemplate FieldTemplate
        {
            get
            {
                if (_fieldTemplate == null)
                {
                    _fieldTemplate = (CategoryFieldTemplate) _fieldTemplateService
                        .GetAll()
                        .FirstOrDefault(t => t is CategoryFieldTemplate);

                    if (_fieldTemplate is null)
                        throw new Exception("No category field template found");
                }

                return _fieldTemplate;
            }
        }

        public Category GetOrCreateCategory(Assortment assortment, string[] categoryTree)
        {
            if (!categoryTree.Any())
                return null;

	        var categoryKey = assortment.Id + string.Join("---", categoryTree);

			try
	        {
		        if (_categoryLookup.ContainsKey(categoryKey))
			        return _categoryLookup[categoryKey];

		        this.Log().Info($"Creating category '{categoryKey}'");

		        var categoryName = categoryTree.First();
		        if (string.IsNullOrEmpty(categoryName))
			        return null;

		        var rootCategories = _categoryService.GetChildCategories(Guid.Empty, assortment.SystemId);
		        var rootCategory = rootCategories.FirstOrDefault(c => c.Id?.Equals(categoryName, StringComparison.InvariantCultureIgnoreCase) ?? false);
		        if (rootCategory == null)
			        rootCategory = CreateCategory(categoryName, Uri.EscapeDataString(categoryName), assortment, null);

		        if (rootCategory == null)
			        throw new Exception("Cannot find or create root category: " + categoryName);

		        var category = GetOrCreateCategoriesRecursive(assortment, rootCategory, categoryTree.Skip(1).ToList());
		        _categoryLookup.Add(categoryKey, category);

		        return category;
	        }
	        catch (Exception exception)
	        {
		        this.Log().Error($"Creating category '{categoryKey}'", exception);
				return null;
	        }
        }

        public void PublishRecursive(Category category)
        {
            foreach (var channel in _channelService.GetAll())
            {
                var categoryConnectionExists = category.ChannelLinks.Any(link => link.ChannelSystemId.Equals(channel.SystemId));
                if (!categoryConnectionExists)
                {
                    var writeCategory = category.MakeWritableClone();
                    writeCategory.ChannelLinks.Add(new CategoryToChannelLink(channel.SystemId));
                    _categoryService.Update(writeCategory);
                }

                foreach (var productLink in category.ProductLinks)
                {
                    foreach (var variant in _variantService.GetByBaseProduct(productLink.BaseProductSystemId))
                    {
                        var variantConnectionExists = variant.ChannelLinks.Any(link => link.ChannelSystemId.Equals(channel.SystemId));
						if (!variantConnectionExists)
                        {
                            var writeVariant = variant.MakeWritableClone();
                            writeVariant.ChannelLinks.Add(new VariantToChannelLink(channel.SystemId));
                            _variantService.Update(writeVariant);
                        }
                    }
                }

                foreach (var childCategory in category.GetChildren())
                    PublishRecursive(childCategory);
            }
        }

        private Category GetOrCreateCategoriesRecursive(Assortment assortment, Category parent, List<string> categoryTree)
        {
            if (!categoryTree.Any())
                return parent;

            var categoryName = categoryTree.First();
	        if (string.IsNullOrEmpty(categoryName))
                return parent;

	        var categoryId = $"{parent?.SystemId}_{Uri.EscapeDataString(categoryName)}"; // Get unique id per category level

	        Category category = null;
            if (parent != null)
            {
                var childCategories = _categoryService.GetChildCategories(parent.SystemId).ToList();
                category = childCategories.FirstOrDefault(c => c.Id.Equals(categoryId, StringComparison.InvariantCultureIgnoreCase));
            }

            if (category == null)
                category = CreateCategory(categoryName, categoryId, assortment, parent);

            if (categoryTree.Count == 1)
                return category;

            return GetOrCreateCategoriesRecursive(assortment, category, categoryTree.Skip(1).ToList());
        }

        private Category CreateCategory(string categoryName, string categoryId, Assortment assortment, Category parent)
        {
	        try
	        {
		        var category = new Category(FieldTemplate.SystemId, assortment.SystemId)
		        {
			        Id = categoryId,
			        ParentCategorySystemId = parent?.SystemId ?? Guid.Empty
		        };

		        foreach (var assortmentLocalization in assortment.Localizations)
		        {
			        var culture = CultureInfo.GetCultureInfo(assortmentLocalization.Key);
			        category.Fields.AddOrUpdateValue("_name", culture, categoryName);
			        category.Fields.AddOrUpdateValue("_url", culture, _suggestionService.Suggest(culture, categoryName));
		        }

		        _categoryService.Create(category);
		        return category;
	        }
	        catch (Exception exception)
	        {
		        this.Log().Error($"Cannot create category '{categoryName}' ('{categoryId}') in parent category '{parent?.Id ?? "NULL"}'", exception);
		        throw;
	        }
        }
    }
}