using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SimplCommerce.Infrastructure.Cache;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Catalog.Areas.Catalog.ViewModels;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Core.Services;

namespace SimplCommerce.Module.Catalog.Services
{
    public class CategoryService : ICategoryService
    {
        private const string CategoryEntityTypeId = "Category";
        private const string CATEGORY_LIST_CACHE_KEY = "CategoryList";
        private const string CATEGORY_DETAIL_CACHE_PREFIX = "Category_";

        private readonly IRepository<Category> _categoryRepository;
        private readonly IEntityService _entityService;
        private readonly IRedisCacheService _cache;
        private readonly IConfiguration _configuration;

        public CategoryService(
            IRepository<Category> categoryRepository, 
            IEntityService entityService,
            IRedisCacheService cache,
            IConfiguration configuration)
        {
            _categoryRepository = categoryRepository;
            _entityService = entityService;
            _cache = cache;
            _configuration = configuration;
        }

        public async Task<IList<CategoryListItem>> GetAll()
        {
            // Try get from cache
            var cachedCategories = await _cache.GetAsync<IList<CategoryListItem>>(CATEGORY_LIST_CACHE_KEY);
            if (cachedCategories != null && cachedCategories.Count > 0)
            {
                return cachedCategories;
            }

            // Get from database
            var categories = await _categoryRepository.Query().Where(x => !x.IsDeleted).ToListAsync();
            var categoriesList = new List<CategoryListItem>();
            foreach (var category in categories)
            {
                var categoryListItem = new CategoryListItem
                {
                    Id = category.Id,
                    IsPublished = category.IsPublished,
                    IncludeInMenu = category.IncludeInMenu,
                    Name = category.Name,
                    DisplayOrder = category.DisplayOrder,
                    ParentId = category.ParentId
                };

                var parentCategory = category.Parent;
                while (parentCategory != null)
                {
                    categoryListItem.Name = $"{parentCategory.Name} >> {categoryListItem.Name}";
                    parentCategory = parentCategory.Parent;
                }

                categoriesList.Add(categoryListItem);
            }

            var result = categoriesList.OrderBy(x => x.Name).ToList();

            // Set cache
            var expirationMinutes = _configuration.GetValue<int>("Redis:CacheExpirationMinutes:CategoryList", 1440);
            await _cache.SetAsync(
                CATEGORY_LIST_CACHE_KEY,
                result,
                TimeSpan.FromMinutes(expirationMinutes)
            );

            return result;
        }

        public async Task Create(Category category)
        {
            using (var transaction = _categoryRepository.BeginTransaction())
            {
                category.Slug = _entityService.ToSafeSlug(category.Slug, category.Id, CategoryEntityTypeId);
                _categoryRepository.Add(category);
                await _categoryRepository.SaveChangesAsync();

                _entityService.Add(category.Name, category.Slug, category.Id, CategoryEntityTypeId);
                await _categoryRepository.SaveChangesAsync();

                transaction.Commit();
            }

            // Invalidate category cache
            await InvalidateCategoryCacheAsync();
        }

        public async Task Update(Category category)
        {
            category.Slug = _entityService.ToSafeSlug(category.Slug, category.Id, CategoryEntityTypeId);
            _entityService.Update(category.Name, category.Slug, category.Id, CategoryEntityTypeId);
            await _categoryRepository.SaveChangesAsync();

            // Invalidate category cache
            await InvalidateCategoryCacheAsync(category.Id);
        }

        public async Task Delete(Category category)
        {
            await _entityService.Remove(category.Id, CategoryEntityTypeId);
            category.IsDeleted = true;
            _categoryRepository.SaveChanges();

            // Invalidate category cache
            await InvalidateCategoryCacheAsync(category.Id);
        }

        private async Task InvalidateCategoryCacheAsync(long? categoryId = null)
        {
            // Clear category list cache
            await _cache.RemoveAsync(CATEGORY_LIST_CACHE_KEY);

            // Clear specific category cache if ID provided
            if (categoryId.HasValue)
            {
                await _cache.RemoveAsync($"{CATEGORY_DETAIL_CACHE_PREFIX}{categoryId.Value}");
            }
        }
    }
}
