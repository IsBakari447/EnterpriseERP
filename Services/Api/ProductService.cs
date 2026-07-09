using EnterpriseERP.Interfaces;
using EnterpriseERP.Models;

namespace EnterpriseERP.Services.Api
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public Task<List<Product>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<Product?> GetByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }

        public Task<Product> CreateAsync(Product product)
        {
            return _repository.CreateAsync(product);
        }

        public Task<Product?> UpdateAsync(int id, Product product)
        {
            return _repository.UpdateAsync(id, product);
        }

        public Task<bool> DeleteAsync(int id)
        {
            return _repository.DeleteAsync(id);
        }
    }
}
