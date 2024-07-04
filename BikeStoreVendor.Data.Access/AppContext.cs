using Microsoft.EntityFrameworkCore;

namespace BikeStoreVendor.Data.Access
{
    public class AppContext : DbContext
    {
        public AppContext() { }
        public AppContext(DbContextOptions<AppContext> options) : base(options) { }
    }
}
