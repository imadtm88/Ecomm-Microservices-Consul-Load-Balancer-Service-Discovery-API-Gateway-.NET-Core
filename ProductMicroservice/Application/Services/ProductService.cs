using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Product>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<Product?> GetByIdAsync(string id) => await _repo.GetByIdAsync(id);

        public async Task CreateAsync(ProductDto dto)
        {
            var product = new Product(dto.Name, dto.Price);
            await _repo.CreateAsync(product);
        }

        public async Task UpdateAsync(string id, ProductDto dto)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product == null) throw new Exception("Not found");

            product.Update(dto.Name, dto.Price);
            await _repo.UpdateAsync(product);
        }

        public async Task DeleteAsync(string id) => await _repo.DeleteAsync(id);
    }
}
