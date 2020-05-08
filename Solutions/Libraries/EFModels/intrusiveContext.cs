using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DatabaseREST.Models
{
    public class intrusiveContextReadOnly : intrusiveContext
    {
        public intrusiveContextReadOnly(DbContextOptions<intrusiveContextReadOnly> options)
            : base(options)
        {
        }


        public override int SaveChanges()
        {
            throw new InvalidOperationException("This context is read only!");
        }
    }

    public partial class intrusiveContext : DbContext
    {
        public intrusiveContext()
        {
        }

        public intrusiveContext(DbContextOptions<intrusiveContext> options)
            : base(options)
        {
        }

        protected intrusiveContext(DbContextOptions options)
        : base(options)
        {
        }

        public virtual DbSet<Abilities> Abilities { get; set; }
        public virtual DbSet<Accounts> Accounts { get; set; }
        public virtual DbSet<HasLearned> HasLearned { get; set; }
        public virtual DbSet<ItemColors> ItemColors { get; set; }
        public virtual DbSet<ItemTypes> ItemTypes { get; set; }
        public virtual DbSet<Items> Items { get; set; }
        public virtual DbSet<Maps> Maps { get; set; }
        public virtual DbSet<Matches> Matches { get; set; }
        public virtual DbSet<PlayedMatch> PlayedMatch { get; set; }
        public virtual DbSet<Players> Players { get; set; }
        public virtual DbSet<Servers> Servers { get; set; }
        public virtual DbSet<Testtable> Testtable { get; set; }
        public virtual DbSet<Wears> Wears { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=212.10.51.254;user=root;port=30003;database=intrusive;", x => x.ServerVersion("8.0.19-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Abilities>(entity =>
            {
                entity.HasKey(e => e.AbilityName)
                    .HasName("PRIMARY");

                entity.ToTable("abilities");

                entity.Property(e => e.AbilityName)
                    .HasColumnName("ability_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Cost).HasColumnName("cost");
            });

            modelBuilder.Entity<Accounts>(entity =>
            {
                entity.HasKey(e => e.AccountId)
                    .HasName("PRIMARY");

                entity.ToTable("accounts");

                entity.HasIndex(e => e.Email)
                    .HasName("email")
                    .IsUnique();

                entity.Property(e => e.AccountId)
                    .HasColumnName("account_id")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.FirstName)
                    .HasColumnName("first_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.LastName)
                    .HasColumnName("last_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasColumnName("password_hash")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<HasLearned>(entity =>
            {
                entity.HasKey(e => new { e.AbilityName, e.PlayerId })
                    .HasName("PRIMARY");

                entity.ToTable("has_learned");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("player_id");

                entity.Property(e => e.AbilityName)
                    .HasColumnName("ability_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("player_id")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.HasOne(d => d.AbilityNameNavigation)
                    .WithMany(p => p.HasLearned)
                    .HasForeignKey(d => d.AbilityName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("has_learned_ibfk_2");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.HasLearned)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("has_learned_ibfk_1");
            });

            modelBuilder.Entity<ItemColors>(entity =>
            {
                entity.HasKey(e => e.ColorName)
                    .HasName("PRIMARY");

                entity.ToTable("item_colors");

                entity.Property(e => e.ColorName)
                    .HasColumnName("color_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Blue).HasColumnName("blue");

                entity.Property(e => e.Green).HasColumnName("green");

                entity.Property(e => e.Red).HasColumnName("red");
            });

            modelBuilder.Entity<ItemTypes>(entity =>
            {
                entity.HasKey(e => e.TypeValue)
                    .HasName("PRIMARY");

                entity.ToTable("item_types");

                entity.Property(e => e.TypeValue)
                    .HasColumnName("type_value")
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<Items>(entity =>
            {
                entity.HasKey(e => e.ItemId)
                    .HasName("PRIMARY");

                entity.ToTable("items");

                entity.HasIndex(e => e.ItemColor)
                    .HasName("item_color");

                entity.HasIndex(e => e.ItemType)
                    .HasName("item_type");

                entity.HasIndex(e => e.OwnerId)
                    .HasName("owner_id");

                entity.Property(e => e.ItemId).HasColumnName("item_id");

                entity.Property(e => e.AquireDate)
                    .HasColumnName("aquire_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.ItemColor)
                    .IsRequired()
                    .HasColumnName("item_color")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ItemName)
                    .IsRequired()
                    .HasColumnName("item_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ItemType)
                    .IsRequired()
                    .HasColumnName("item_type")
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.OwnerId)
                    .IsRequired()
                    .HasColumnName("owner_id")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Quality).HasColumnName("quality");

                entity.HasOne(d => d.ItemColorNavigation)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.ItemColor)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("items_ibfk_1");

                entity.HasOne(d => d.ItemTypeNavigation)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.ItemType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("items_ibfk_2");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("items_ibfk_3");
            });

            modelBuilder.Entity<Maps>(entity =>
            {
                entity.HasKey(e => e.MapName)
                    .HasName("PRIMARY");

                entity.ToTable("maps");

                entity.Property(e => e.MapName)
                    .HasColumnName("map_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<Matches>(entity =>
            {
                entity.HasKey(e => e.MatchId)
                    .HasName("PRIMARY");

                entity.ToTable("matches");

                entity.HasIndex(e => e.MapName)
                    .HasName("map_name");

                entity.Property(e => e.MatchId).HasColumnName("match_id");

                entity.Property(e => e.Begun)
                    .HasColumnName("begun")
                    .HasColumnType("datetime");

                entity.Property(e => e.Difficulty).HasColumnName("difficulty");

                entity.Property(e => e.Ended)
                    .HasColumnName("ended")
                    .HasColumnType("datetime");

                entity.Property(e => e.MapName)
                    .IsRequired()
                    .HasColumnName("map_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.HasOne(d => d.MapNameNavigation)
                    .WithMany(p => p.Matches)
                    .HasForeignKey(d => d.MapName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("matches_ibfk_1");
            });

            modelBuilder.Entity<PlayedMatch>(entity =>
            {
                entity.HasKey(e => new { e.MatchId, e.PlayerId })
                    .HasName("PRIMARY");

                entity.ToTable("played_match");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("player_id");

                entity.Property(e => e.MatchId).HasColumnName("match_id");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("player_id")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Deaths).HasColumnName("deaths");

                entity.Property(e => e.Kills).HasColumnName("kills");

                entity.Property(e => e.Score).HasColumnName("score");

                entity.HasOne(d => d.Match)
                    .WithMany(p => p.PlayedMatch)
                    .HasForeignKey(d => d.MatchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("played_match_ibfk_2");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayedMatch)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("played_match_ibfk_1");
            });

            modelBuilder.Entity<Players>(entity =>
            {
                entity.HasKey(e => e.PlayerId)
                    .HasName("PRIMARY");

                entity.ToTable("players");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("player_id")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Experience)
                    .HasColumnName("experience")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.Player)
                    .WithOne(p => p.Players)
                    .HasForeignKey<Players>(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("players_ibfk_1");

                //Ignore derived attributes
                entity.Ignore(e => e.Level);
                entity.Ignore(e => e.Skillpoints);
                entity.Ignore(e => e.SkillpointsUsed);
                entity.Ignore(e => e.LevelProgress);

            });

            modelBuilder.Entity<Servers>(entity =>
            {
                entity.HasKey(e => e.Secret)
                    .HasName("PRIMARY");

                entity.ToTable("servers");

                entity.Property(e => e.Secret)
                    .HasColumnName("secret")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<Testtable>(entity =>
            {
                entity.ToTable("testtable");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Message)
                    .HasColumnName("message")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.NewMessage)
                    .HasColumnName("newMessage")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<Wears>(entity =>
            {
                entity.HasKey(e => new { e.ItemId, e.PlayerId })
                    .HasName("PRIMARY");

                entity.ToTable("wears");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("player_id");

                entity.Property(e => e.ItemId).HasColumnName("item_id");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("player_id")
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.Wears)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("wears_ibfk_2");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.Wears)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("wears_ibfk_1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
