using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories
{
    public class MySqlClientRepository : IClientRepository
    {
        private readonly ClientDbContext _context;

        public MySqlClientRepository(ClientDbContext context)
        {
            _context = context;
        }

        public async Task<List<Client>> GetAllAsync() =>
            await _context.Clients.ToListAsync();

        public async Task<Client?> GetByIdAsync(string id) =>
            await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);

        public async Task CreateAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }
        }
    }
}
