using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ClientService
    {
        private readonly IClientRepository _repo;

        public ClientService(IClientRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Client>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<Client?> GetByIdAsync(string id) => await _repo.GetByIdAsync(id);

        public async Task CreateAsync(ClientDto dto)
        {
            var client = new Client
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address
            };

            await _repo.CreateAsync(client);
        }

        public async Task UpdateAsync(string id, ClientDto dto)
        {
            var client = await _repo.GetByIdAsync(id);
            if (client == null) throw new Exception("Client not found");

            client.FirstName = dto.FirstName;
            client.LastName = dto.LastName;
            client.Email = dto.Email;
            client.Phone = dto.Phone;
            client.Address = dto.Address;

            await _repo.UpdateAsync(client);
        }

        public async Task DeleteAsync(string id) => await _repo.DeleteAsync(id);
    }
}
