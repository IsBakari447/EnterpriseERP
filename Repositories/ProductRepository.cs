using EnterpriseERP.Data;
using EnterpriseERP.Interfaces;
using EnterpriseERP.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateAsync(int id, Product product)
        {
            var existing = await _context.Products.FindAsync(id);

            if (existing == null)
                return null;

            existing.Name = product.Name;
            existing.Category = product.Category;
            existing.PurchasePrice = product.PurchasePrice;
            existing.SalePrice = product.SalePrice;
            existing.Quantity = product.Quantity;

            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
