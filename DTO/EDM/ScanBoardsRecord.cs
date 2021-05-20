namespace DTO.EDM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ScanBoardsRecord")]
    public partial class ScanBoardsRecord
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Upi { get; set; }

        [StringLength(50)]
        public string Batch { get; set; }

        public int Station { get; set; }

        public DateTime? ScanTime { get; set; }
    }
}
