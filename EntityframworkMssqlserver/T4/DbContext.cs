
using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Model
{
  public partial class DbContext : System.Data.Entity.DbContext
  {
        public DbContext() : base("name=hz")
        {
        }
        static DbContext() {
            System.Data.Entity.Database.SetInitializer<Model.DbContext>(null);
        }    
    /// <summary>
    /// 
    /// <summary>
    public virtual DbSet<Log> Log { get; set; }
    }
}
