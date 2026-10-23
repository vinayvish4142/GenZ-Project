using EntityModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositorys.Authenticate
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByHashAsync(string hash);
        Task AddAsync(RefreshToken token);
        Task SaveAsync();
    }
}
