using Microsoft.EntityFrameworkCore;
using AgendaApi.Models;

namespace AgendaApi.Data
{
    public class AgendaContext : DbContext
    {
        public AgendaContext(DbContextOptions<AgendaContext> options)
            : base(options)
        {
        }

        public DbSet<Evento> Eventos => Set<Evento>();
    }
}