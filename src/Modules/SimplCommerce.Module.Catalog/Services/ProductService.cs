using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SimplCommerce.Infrastructure.Cache;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Core.Services;

namespace SimplCommerce.Module.Catalog.Services
{
    public class ProductService : IProductService
    {
        private const string ProductEntityTypeId = nameof(Product);
        private const string PRODUCT_LIST_CACHE_KEY = "ProductList";
        private const string PRODUCT_DETAIL_CACHE_PREFIX = "Product_";

        private readonly IRepository<Product> _productRepository;
        private readonly IEntityService _entityService;
        private readonly IRedisCacheService _cache;
        private readonly IConfiguration _configuration;

        public ProductService(
            IRepository<Product> productRepository, 
            IEntityService entityService,
            IRedisCacheService cache,
            IConfiguration configuration)
        {
            _productRepository = productRepository;
            _entityService = entityService;
            _cache = cache;
            _configuration = configuration;
        }

        public void Create(Product product)
        {
            using (var transaction = _productRepository.BeginTransaction())
            {
                product.Slug = _entityService.ToSafeSlug(product.Slug, product.Id, ProductEntityTypeId);
                _productRepository.Add(product);
                _productRepository.SaveChanges();

                _entityService.Add(product.Name, product.Slug, product.Id, ProductEntityTypeId);
                _productRepository.SaveChanges();

                transaction.Commit();
            }

            // Invalidate product cache
            _ = InvalidateProductCacheAsync();
        }

        public void Update(Product product)
        {
            var slug = _entityService.Get(product.Id, ProductEntityTypeId);
            if (product.IsVisibleIndividually)
            {
                product.Slug = _entityService.ToSafeSlug(product.Slug, product.Id, ProductEntityTypeId);
                if (slug != null)
                {
                    _entityService.Update(product.Name, product.Slug, product.Id, ProductEntityTypeId);
                }
                else
                {
                    _entityService.Add(product.Name, product.Slug, product.Id, ProductEntityTypeId);
                }
            }
            else
            {
                if (slug != null)
                {
                    _entityService.Remove(product.Id, ProductEntityTypeId);
                }
            }
            _productRepository.SaveChanges();

            // Invalidate product cache
            _ = InvalidateProductCacheAsync(product.Id);
        }

        public async Task Delete(Product product)
        {
            product.IsDeleted = true;
            await _entityService.Remove(product.Id, ProductEntityTypeId);
            _productRepository.SaveChanges();

            // Invalidate product cache
            await InvalidateProductCacheAsync(product.Id);
        }

        private async Task InvalidateProductCacheAsync(long? productId = null)
        {
            // Clear product list cache
            await _cache.RemoveAsync(PRODUCT_LIST_CACHE_KEY);

            // Clear specific product cache if ID provided
            if (productId.HasValue)
            {
                await _cache.RemoveAsync($"{PRODUCT_DETAIL_CACHE_PREFIX}{productId.Value}");
            }
        }
    }
}
