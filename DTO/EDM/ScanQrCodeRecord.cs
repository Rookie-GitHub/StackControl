namespace DTO.EDM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ScanQrCodeRecord")]
    public partial class ScanQrCodeRecord
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string StackId { get; set; }

        public int Status { get; set; }

        public DateTime? ScanTime { get; set; }
    }
}
