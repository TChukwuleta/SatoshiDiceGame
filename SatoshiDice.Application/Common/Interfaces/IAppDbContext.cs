using Microsoft.EntityFrameworkCore;
using SatoshiDice.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Domain.Entities.Player> Players { get; set; }
        DbSet<Transaction> Transactions { get; set; }
        DbSet<ApplicationUser> ApplicationUsers { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
