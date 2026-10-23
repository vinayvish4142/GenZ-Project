using GenZApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModel.Models
{ 
    public class RefreshToken
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string TokenHash { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public ApplicationUser User { get; set; } = null!;
    }

    
}
