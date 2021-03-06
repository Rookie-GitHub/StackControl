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
        public virtual DbSet<LineBatchInfo> LineBatchInfo { get; set; }
        public virtual DbSet<ScanBoardsRecord> ScanBoardsRecord { get; set; }
        public virtual DbSet<ScanQrCodeRecord> ScanQrCodeRecord { get; set; }
        public virtual DbSet<SendCutPicRecord> SendCutPicRecord { get; set; }
        public virtual DbSet<Stack_table> Stack_table { get; set; }
        public virtual DbSet<StackInfo_table> StackInfo_table { get; set; }
        public virtual DbSet<ScanBoards_View> ScanBoards_View { get; set; }
        public virtual DbSet<StacksInfo_View> StacksInfo_View { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Board_table>()
                .Property(e => e.Upi)
                .IsUnicode(false);

            modelBuilder.Entity<ScanBoards_View>()
                .Property(e => e.Upi)
                .IsUnicode(false);
        }
    }
}
