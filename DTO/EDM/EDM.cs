using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DTO.EDM
{
    public partial class EDM : DbContext
    {
        public EDM()
            : base("name=EDM")
        {
        }

        public virtual DbSet<Batch_table> Batch_table { get; set; }
        public virtual DbSet<Board_table> Board_table { get; set; }
        public virtual DbSet<ScanQrCodeRecord> ScanQrCodeRecord { get; set; }
        public virtual DbSet<Stack_table> Stack_table { get; set; }
        public virtual DbSet<StackInfo_table> StackInfo_table { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Board_table>()
                .Property(e => e.Upi)
                .IsUnicode(false);
        }
    }
}
