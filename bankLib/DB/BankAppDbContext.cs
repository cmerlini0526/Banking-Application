using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace bankLib.DB;

public partial class BankAppDbContext : DbContext
{
    public BankAppDbContext()
    {
    }

    public BankAppDbContext(DbContextOptions<BankAppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<CheckRequest> CheckRequests { get; set; }

    public virtual DbSet<TransactionHistory> TransactionHistories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    private string GetConnection()
    {
        FileStream connectionFile = new FileStream("db.txt", FileMode.Open, FileAccess.Read);
        StreamReader connectionReader = new StreamReader(connectionFile);
        string output = connectionReader.ReadLine();
        connectionReader.Close();
        connectionFile.Close();
        return output;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnection());
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccId).HasName("PK__accounts__A471AFFADB50B081");

            entity.ToTable("accounts");

            entity.Property(e => e.AccId).HasColumnName("accID");
            entity.Property(e => e.AccBalance).HasColumnName("accBalance");
            entity.Property(e => e.AccBranch)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("accBranch");
            entity.Property(e => e.AccIsActive).HasColumnName("accIsActive");
            entity.Property(e => e.AccName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("accName");
            entity.Property(e => e.AccOwnerId).HasColumnName("accOwnerID");
            entity.Property(e => e.AccType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("accType");

            entity.HasOne(d => d.AccOwner).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.AccOwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_userID");
        });

        modelBuilder.Entity<CheckRequest>(entity =>
        {
            entity.HasKey(e => e.ReqId).HasName("pk_reqID");

            entity.ToTable("check_requests");

            entity.Property(e => e.ReqId).HasColumnName("reqID");
            entity.Property(e => e.AccId).HasColumnName("accID");
            entity.Property(e => e.ReqAccepted).HasColumnName("reqAccepted");
            entity.Property(e => e.ReqOpenDate)
                .HasColumnType("datetime")
                .HasColumnName("reqOpenDate");
            entity.Property(e => e.ReqRespondDate)
                .HasColumnType("datetime")
                .HasColumnName("reqRespondDate");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.Acc).WithMany(p => p.CheckRequests)
                .HasForeignKey(d => d.AccId)
                .HasConstraintName("fk_reqAccID");

            entity.HasOne(d => d.User).WithMany(p => p.CheckRequests)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_reqUserID");
        });

        modelBuilder.Entity<TransactionHistory>(entity =>
        {
            entity.HasKey(e => e.TransId).HasName("PK__transact__DB107E474348779B");

            entity.ToTable("transaction_history");

            entity.Property(e => e.TransId).HasColumnName("transID");
            entity.Property(e => e.AccId).HasColumnName("accID");
            entity.Property(e => e.TransChange).HasColumnName("transChange");
            entity.Property(e => e.TransDate)
                .HasColumnType("datetime")
                .HasColumnName("transDate");
            entity.Property(e => e.TransOldBal).HasColumnName("transOldBal");
            entity.Property(e => e.TransType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("transType");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.Acc).WithMany(p => p.TransactionHistories)
                .HasForeignKey(d => d.AccId)
                .HasConstraintName("fk_accID");

            entity.HasOne(d => d.User).WithMany(p => p.TransactionHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_transUserID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__CB9A1CDF42B75747");

            entity.ToTable("users");

            entity.HasIndex(e => e.UserName, "UQ__users__66DCF95CD14B2217").IsUnique();

            entity.HasIndex(e => e.UserName, "uc_userName").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("userID");
            entity.Property(e => e.UserIsAdmin).HasColumnName("userIsAdmin");
            entity.Property(e => e.UserName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("userName");
            entity.Property(e => e.UserPass)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("userPass");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
