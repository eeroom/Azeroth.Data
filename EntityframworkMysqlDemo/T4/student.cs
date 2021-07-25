using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Model
{
    /// <summary>
    /// 
    /// <summary>
    [Table("student")]
    public partial class student
    {
        /// <summary>
        ///
        /// </summary>
        [Required]
        [StringLength(36)]
        [Key]
        public String Id {set;get;}
        /// <summary>
        ///
        /// </summary>
        [Required]
        [StringLength(192)]
        public String Name {set;get;}
    }
}
