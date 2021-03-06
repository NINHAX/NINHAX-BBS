using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace NHX.BBS.Logic.Model
{
    public partial class BBSDatabaseContext : DbContext
    {
        public BBSDatabaseContext()
        {
        }

        public BBSDatabaseContext(DbContextOptions<BBSDatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Thread> Threads { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Data Source=(localdb)\ProjectsV13;Initial Catalog='BBS Database';Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Post", "BBS");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Timestamp)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.HasOne(d => d.CreatorNavigation)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.Creator)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PostCreator");

                entity.HasOne(d => d.ThreadNavigation)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.Thread)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ParentThread");
            });

            modelBuilder.Entity<Thread>(entity =>
            {
                entity.ToTable("Thread", "BBS");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Timestamp)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.HasOne(d => d.CreatorNavigation)
                    .WithMany(p => p.Threads)
                    .HasForeignKey(d => d.Creator)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ThreadCreator");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "BBS");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
