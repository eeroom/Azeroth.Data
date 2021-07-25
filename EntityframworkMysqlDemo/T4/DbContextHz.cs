
using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Model
{
  public partial class DbContextHz : System.Data.Entity.DbContext
  {
        public DbContextHz() : base("name=hz")
        {
        }    
    /// <summary>
    /// 
    /// <summary>
    public virtual DbSet<student> student { get; set; }
    }
}
