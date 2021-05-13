namespace DTO.EDM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class StackInfo_table
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string StackId { get; set; }

        [Required]
        [StringLength(50)]
        public string Batch { get; set; }

        public int PlateAmount { get; set; }

        public int CurrentCount { get; set; }

        public int Status { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
