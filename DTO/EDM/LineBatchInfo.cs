namespace DTO.EDM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LineBatchInfo")]
    public partial class LineBatchInfo
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Batch { get; set; }

        public int Status { get; set; }

        public int Station { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
