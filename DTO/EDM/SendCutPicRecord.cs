namespace DTO.EDM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SendCutPicRecord")]
    public partial class SendCutPicRecord
    {
        public int Id { get; set; }

        public int? MachineNo { get; set; }

        [StringLength(50)]
        public string BoardBatch { get; set; }

        public int? BoardPartId { get; set; }

        [StringLength(50)]
        public string Pic { get; set; }

        public DateTime? SendTime { get; set; }
    }
}
