namespace DTO.EDM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Stack_table
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string StackId { get; set; }

        [StringLength(50)]
        public string Batch { get; set; }

        [Required]
        [StringLength(50)]
        public string PartID { get; set; }

        public int Len { get; set; }

        public int Width { get; set; }

        public int Thin { get; set; }

        [Required]
        [StringLength(50)]
        public string Material { get; set; }

        public int Pos { get; set; }

        public int Pattern { get; set; }

        [Required]
        public string Map { get; set; }

        public int? Status { get; set; }

        public int? About { get; set; }

        public DateTime? DateTime { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
