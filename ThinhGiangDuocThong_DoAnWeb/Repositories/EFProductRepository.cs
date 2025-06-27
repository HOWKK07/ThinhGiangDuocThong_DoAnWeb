using Microsoft.EntityFrameworkCore;
using ThinhGiangDuocThong_DoAnWeb.Models;

namespace ThinhGiangDuocThong_DoAnWeb.Repositories
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// L?y v? to�n b? danh s�ch s?n ph?m, bao g?m th�ng tin danh m?c v� h�nh ?nh
        /// </summary>
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .ToListAsync();
        }

        /// <summary>
        /// L?y th�ng tin s?n ph?m theo ID, bao g?m th�ng tin danh m?c v� h�nh ?nh
        /// </summary>
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// L?y danh s�ch s?n ph?m theo CategoryId, bao g?m th�ng tin danh m?c v� h�nh ?nh
        /// </summary>
        public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Product>> SearchByNameAsync(string term)
        {
            return await _context.Products
                .Include(p => p.Images)
                .Where(p => p.Name.Contains(term))
                .OrderBy(p => p.Name)
                .Take(7)
                .ToListAsync();
        }
    }
}