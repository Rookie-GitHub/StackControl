namespace DTO.EDM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Batch_table
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string BatchId { get; set; }

        public int AllNum { get; set; }

        public int ComNum { get; set; }

        public int Status { get; set; }

        public DateTime DateTime { get; set; }
    }
}
