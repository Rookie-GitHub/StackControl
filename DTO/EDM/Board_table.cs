namespace DTO.EDM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Board_table
    {
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string BatchId { get; set; }

        public int Array { get; set; }

        [Required]
        [StringLength(30)]
        public string Upi { get; set; }

        public int? Status { get; set; }
    }
}
