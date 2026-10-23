using EntityModel.Models;
using GenZApp.Models;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositorys.Authenticate
{
    public class RefreshTokenRepository:IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByHashAsync(string hash)
        {
            return await _context.RefreshTokens.Include(x => x.User).FirstOrDefaultAsync(x => x.TokenHash == hash);
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
