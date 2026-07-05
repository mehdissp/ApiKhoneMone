using Microsoft.EntityFrameworkCore;
using JWTApi.Domain.Entities;
using System.Reflection.Metadata;

namespace JWTApi.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();





        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Menu> Menus { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }
        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<IpLock> IpLocks { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<UserPackage> UserPackages { get; set; }
        public DbSet<ExtraProject> ExtraProjects { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<RealEstates> RealEstates { get; set; }
        public DbSet<RealEstatesRent> RealEstatesRents { get; set; }
        public DbSet<SearchMatch> SearchMatches { get; set; }
        public DbSet<SearchRequest> SearchRequests { get; set; }

        public DbSet<Facilities> Facilities { get; set; }

        public DbSet<BookMark> BookMarks { get; set; }
        public DbSet<RealEstates_SpecialFeature> RealEstates_SpecialFeatures { get; set; }
        public DbSet<RealEstates_Facilities> RealEstates_Facilities { get; set; }
        public DbSet<IndependentAgentProfile> IndependentAgentProfiles { get; set; }
        public DbSet<RealEstateAgentProfile> RealEstateAgentProfiles { get; set; }


        public DbSet<RealEstatesRent_SpecialFeature> RealEstatesRent_SpecialFeatures { get; set; }
        public DbSet<RealEstatesRent_Facilities> RealEstatesRent_Facilities { get; set; }
        public DbSet<Warning> Warnings { get; set; }

        public DbSet<Image> Images { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<WalletLog> WalletLogs { get; set; }

        public DbSet<AdPriceRanges> AdPriceRanges { get; set; }
        public DbSet<Story> Stories { get; set; }

        public DbSet<Violation> Violations { get; set; }

        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<TrainingData> TrainingData { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Attachment>()
    .HasKey(d => d.Id);


            modelBuilder.Entity<Image>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(p => p.IsBanner).HasDefaultValueSql("0");

            });
            //---------------Wallet--------
            modelBuilder.Entity<Wallet>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(p => p.Currency).HasMaxLength(20);
                b.Property(p => p.UserId).IsRequired();
                b.Property(p => p.Balance).IsRequired();
                b.Property(p => p.PendingBalance).IsRequired();
                b.Property(p => p.IsActive).HasDefaultValueSql("1");
                b.Property(p => p.IsLocked).HasDefaultValueSql("0");
                b.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");
                b.HasOne(r => r.User).WithOne(s => s.Wallet);
            });
            //--------------AdPriceRanges--
            modelBuilder.Entity<AdPriceRanges>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(p => p.Description).HasMaxLength(500);
                b.Property(p => p.RoleId).IsRequired();
                b.Property(p => p.CategoryId).IsRequired();
                b.Property(p => p.Title).HasMaxLength(100);
                b.Property(p => p.IsActive).HasDefaultValueSql("1");
            });
            

            //---------------Transaction--------
            modelBuilder.Entity<Transaction>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(p => p.WalletId).IsRequired();
                b.Property(p => p.TransactionCode).IsRequired().HasMaxLength(50); ;
                b.Property(p => p.Type).IsRequired();
                b.Property(p => p.Amount).IsRequired();
                b.Property(p => p.ReferenceId).IsRequired();
                b.Property(p => p.Description).HasMaxLength(500);
                b.Property(p => p.Status).IsRequired().HasDefaultValueSql("0");
                b.Property(p => p.PaymentMethod).HasMaxLength(200);
                b.Property(p => p.IpAddress).HasMaxLength(200);
                b.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");
                b.HasOne(p => p.Wallet)
                .WithMany(t => t.Transactions)
                .HasForeignKey(p => p.WalletId);
            });
            //-------------WalletLog------
            modelBuilder.Entity<WalletLog>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(p => p.WalletId).IsRequired();
                b.Property(p => p.Action).IsRequired().HasMaxLength(500); 
                b.Property(p => p.IpAddress).IsRequired().HasMaxLength(250);
                b.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");
                b.HasOne(p => p.Wallet)
                .WithMany(t => t.Logs)
                .HasForeignKey(p => p.WalletId);
            });
            modelBuilder.Entity<Warning>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(p => p.DescriptionRows).HasMaxLength(500);
                b.Property(p => p.Name).HasMaxLength(150);
                b.HasOne(p => p.Category)
   .WithMany(t => t.Warnings)
   .HasForeignKey(p => p.CategoryId);

            });
            //------------

            modelBuilder.Entity<UserRole>().HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<RolePermission>().HasKey(x => new { x.RoleId, x.PermissionId });
            modelBuilder.Entity<LoginAttempt>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.IPAddress).HasMaxLength(50).IsRequired();
                b.Property(x => x.Username).HasMaxLength(200);
                b.Property(x => x.Reason).HasMaxLength(200);
                b.HasIndex(x => new { x.UserId, x.AttemptTime });
                b.HasIndex(x => x.AttemptTime);
            });

            modelBuilder.Entity<IpLock>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.IPAddress).HasMaxLength(50).IsRequired();
                b.HasIndex(x => x.IPAddress).IsUnique();
            });

            modelBuilder.Entity<ChatMessage>(b =>
            {
                b.HasKey(x => x.Id);
           
            });
            modelBuilder.Entity<TrainingData>().HasData(GetTrainingData());
            modelBuilder.Entity<TrainingData>(b =>
            {
                b.HasKey(x => x.Id);

            });

            modelBuilder.Entity<Role>(b =>
            {

                b.Property(x => x.Name).HasMaxLength(250).IsRequired();

            });
            //*------------  RealEstateAgentProfile
            modelBuilder.Entity<RealEstateAgentProfile>(b =>
            {

                b.HasKey(x => x.Id);
                b.Property(u => u.AgentCode).HasMaxLength(25).IsRequired();
                b.Property(u => u.LicenseNumber).HasMaxLength(20);
                b.Property(u => u.NationalCartNumber).HasMaxLength(15);
                b.Property(u => u.OfficeAddress).HasMaxLength(200);
                b.HasOne(r => r.User).WithOne(s => s.RealEstateAgentProfile);
            });
            //*------------  IndependentAgentProfile
            modelBuilder.Entity<IndependentAgentProfile>(b =>
            {

                b.HasKey(x => x.Id);
                b.Property(u => u.BusinessLicense).HasMaxLength(25).IsRequired();

            });
            //---------------payment -----------------
            modelBuilder.Entity<Payment>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");

            });

            // ---------------- User ----------------
            modelBuilder.Entity<User>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Name).HasMaxLength(100).IsRequired();
                b.Property(u => u.Email).HasMaxLength(200);
                b.Property(u => u.FullName).HasMaxLength(200);
                b.Property(u => u.MobileNumber).HasMaxLength(200).IsRequired();
                b.Property(u => u.PasswordHash).IsRequired();
                b.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");
                b.Property(r => r.IsActive).HasDefaultValue(true);
                b.Property(p => p.IsDeleted).HasDefaultValueSql("0");
                b.Property(p => p.IsMobileVerified).HasDefaultValueSql("0");
                b.Property(u => u.NationalCode).HasMaxLength(10);


                b.HasMany(u => u.ExtraProjects)
                 .WithOne(p => p.User)
                 .HasForeignKey(p => p.UserId);
                b.HasMany(p => p.UserPackages)
               .WithOne(t => t.User)
               .HasForeignKey(t => t.UserId);

                b.HasMany(u => u.RealEstates)
                 .WithOne(p => p.User)
                 .HasForeignKey(p => p.UserId);

            });

            // ---------------- Project ----------------
            modelBuilder.Entity<Project>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Name).HasMaxLength(200).IsRequired();
                b.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");

            });

            //------------Category
            modelBuilder.Entity<Category>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Name).HasMaxLength(200).IsRequired();
                b.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");
                b.Property(p => p.IsDeleted).HasDefaultValueSql("0");

                b.HasMany(p => p.Facilities)
                 .WithOne(t => t.Category)
                 .HasForeignKey(t => t.CategoryId);

                b.HasMany(p => p.Warnings)
             .WithOne(t => t.Category)
             .HasForeignKey(t => t.CategoryId);


                b.HasMany(p => p.SpecialFeature)
                 .WithOne(t => t.Category)
                 .HasForeignKey(t => t.CategoryId);

                b.HasMany(p => p.RealEstates)
                 .WithOne(t => t.Category)
                 .HasForeignKey(t => t.CategoryId);

                b.HasMany(p => p.RealEstatesRents)
           .WithOne(t => t.Category)
           .HasForeignKey(t => t.CategoryId);

            });
            //------------RealEstates
            modelBuilder.Entity<RealEstates>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Title).HasMaxLength(200).IsRequired();
                b.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");
                b.Property(p => p.IsPrivate).HasDefaultValueSql("0");
                b.Property(p => p.IsShowLocation).HasDefaultValueSql("0");
                b.Property(p => p.IsDeleted).HasDefaultValueSql("0");
                b.Property(p => p.IsHasElevator).HasDefaultValueSql("0");
                b.Property(p => p.IsHasParking).HasDefaultValueSql("0");
                b.Property(p => p.IsHaLoan).HasDefaultValueSql("0");
                b.Property(p => p.IsRenovated).HasDefaultValueSql("0");
                b.Property(p => p.IsHasElevator).HasDefaultValueSql("0");
                b.Property(p => p.IsHasPool).HasDefaultValueSql("0");
                b.Property(p => p.DescriptionRows).HasMaxLength(450);
                b.Property(p => p.Address).HasMaxLength(450);
                b.Property(p => p.AdditionalInformation).HasMaxLength(250);
                b.Property(p => p.Latitude)
                    .HasColumnType("decimal(10,8)");
                b.Property(p => p.Longitude)
                    .HasColumnType("decimal(11,8)");
                b.HasOne(p => p.Category)
              .WithMany(t => t.RealEstates)
              .HasForeignKey(p => p.CategoryId);

                //            b.HasMany(p => p.Matches)
                //.WithOne(t => t.RealEstates)
                //.HasForeignKey(t => t.RealEstateId);
                // مهمترین ایندکس - برای بیشتر کوئری‌ها
                b.HasIndex(p => new {
                    p.Status,
                    p.IsDeleted,
                    p.CategoryId
                }).HasDatabaseName("IX_RealEstates_Main");

                // برای جستجوی منطقه
                b.HasIndex(p => new {
                    p.RegionId,
                    p.Status,
                    p.IsDeleted
                }).HasDatabaseName("IX_RealEstates_Region");

                // برای املاک کاربر
                b.HasIndex(p => new {
                    p.UserId,
                    p.Status,
                    p.IsDeleted
                }).HasDatabaseName("IX_RealEstates_User");

            });

            //******Story
            modelBuilder.Entity<Story>(b =>
            {
                b.HasKey(p => p.Id);
  
                b.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");
                b.Property(p => p.ImagePath)
     .HasMaxLength(500);
                b.Property(p => p.Desc)
    .HasMaxLength(500);

                b.HasOne(p => p.RealEstates)
            .WithMany(t => t.Stories)
            .HasForeignKey(p => p.RealEstatesId)
            .OnDelete(DeleteBehavior.Cascade);




                b.Property(p => p.ExpiresAt)
    .HasComputedColumnSql("DATEADD(HOUR, 24, CreatedAt)")
    .ValueGeneratedOnAddOrUpdate();

    //            b.Property(p => p.IsExpired)
    //.HasComputedColumnSql("CAST(CASE WHEN GETUTCDATE() >= DATEADD(HOUR, 24, CreatedAt) THEN 1 ELSE 0 END AS BIT)")
    //.ValueGeneratedOnAddOrUpdate();
                // ایندکس‌ها برای بهبود عملکرد
                // ایندکس روی ExpiresAt برای حذف سریع استوری‌های منقضی شده
                b.HasIndex(p => p.ExpiresAt);

                // ایندکس ترکیبی برای جستجوی استوری‌های فعال یک ملک
                b.HasIndex(p => new { p.RealEstatesId, p.ExpiresAt });

                //            b.HasMany(p => p.Matches)
                //.WithOne(t => t.RealEstates)
                //.HasForeignKey(t => t.RealEstateId);


            });
            //-----------------RealEstatesRent
            modelBuilder.Entity<RealEstatesRent>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Title).HasMaxLength(200).IsRequired();
                b.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");
                b.Property(p => p.IsPrivate).HasDefaultValueSql("0");
                b.Property(p => p.IsShowLocation).HasDefaultValueSql("0");
                b.Property(p => p.IsDeleted).HasDefaultValueSql("0");
                b.Property(p => p.IsHasElevator).HasDefaultValueSql("0");
                b.Property(p => p.IsHasParking).HasDefaultValueSql("0");
                b.Property(p => p.IsHasElevator).HasDefaultValueSql("0");
                b.Property(p => p.IsHasPool).HasDefaultValueSql("0");
                b.Property(p => p.DescriptionRows).HasMaxLength(450);
                b.Property(p => p.AdditionalInformation).HasMaxLength(250);
                b.Property(p => p.Latitude)
                    .HasColumnType("decimal(10,8)");
                b.Property(p => p.Longitude)
                    .HasColumnType("decimal(11,8)");
                b.HasOne(p => p.Category)
              .WithMany(t => t.RealEstatesRents)
              .HasForeignKey(p => p.CategoryId);


                //                b.HasMany(p => p.Matches)
                //.WithOne(t => t.RealEstatesRent)
                //.HasForeignKey(t => t.RealEstateRentId);


            });
            //------------search request
            modelBuilder.Entity<SearchRequest>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Title).HasMaxLength(200).IsRequired();
                b.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");
                b.Property(p => p.IsActive).HasDefaultValueSql("1");
                b.Property(p => p.IsDeleted).HasDefaultValueSql("0");

                b.HasOne(p => p.Category)
              .WithMany(t => t.SearchRequests)
              .HasForeignKey(p => p.CategoryId);


                b.HasMany(p => p.Matches)
.WithOne(t => t.SearchRequest)
.HasForeignKey(t => t.SearchRequestId);


            });

            //------------search request
            modelBuilder.Entity<SearchMatch>(b =>
            {
                b.HasKey(p => p.Id);

                b.Property(p => p.MatchedAt).HasDefaultValueSql("GETDATE()");
                b.Property(p => p.IsNotified).HasDefaultValueSql("0");
                b.Property(p => p.IsRejected).HasDefaultValueSql("0");
                b.Property(p => p.IsSeenByUser).HasDefaultValueSql("0");


                //    b.HasOne(p => p.RealEstates)
                //  .WithMany(t => t.Matches)
                //  .HasForeignKey(p => p.RealEstateId);


                //    b.HasOne(p => p.RealEstatesRent)
                //.WithMany(t => t.Matches)
                //.HasForeignKey(p => p.RealEstateRentId);


            });
            //------------BookMark
            modelBuilder.Entity<BookMark>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.DescriptionRows).HasMaxLength(550);
                b.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");

                b.HasOne(p => p.RealEstates)
              .WithMany(t => t.BookMark)
              .HasForeignKey(p => p.RealEstatesId);


            });

            //--------------Violation
            modelBuilder.Entity<Violation>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.DescriptionRows).HasMaxLength(550);
                b.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");
                b.Property(p => p.IsDeleted).HasDefaultValueSql("0");

                b.HasOne(p => p.RealEstates)
              .WithMany(t => t.Violations)
              .HasForeignKey(p => p.RealEstatesId);


            });

            //---------------Facilities
            modelBuilder.Entity<Facilities>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Name).HasMaxLength(200).IsRequired();
                b.Property(p => p.Icon).HasMaxLength(150);
                b.Property(p => p.DescriptionRows).HasMaxLength(450);
                b.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");

                b.HasOne(p => p.Category)
                .WithMany(t => t.Facilities)
                .HasForeignKey(p => p.CategoryId);

            });

            //----------------SpecialFeature
            modelBuilder.Entity<SpecialFeature>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Name).HasMaxLength(200).IsRequired();
                b.Property(p => p.Icon).HasMaxLength(150);
                b.Property(p => p.DescriptionRows).HasMaxLength(450);
                b.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");

                b.HasOne(p => p.Category)
                .WithMany(t => t.SpecialFeature)
                .HasForeignKey(p => p.CategoryId);

                b.HasOne(p => p.Parent)
                 .WithMany(p => p.Children)
                 .HasForeignKey(p => p.ParentId)
                 .IsRequired(false) // Parent می‌تواند null باشد (برای ریشه)
                 .OnDelete(DeleteBehavior.Restrict); // یا Cascade بسته به نیاز

            });
            //*-------- region
            //----------------SpecialFeature
            modelBuilder.Entity<Region>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Name).HasMaxLength(200).IsRequired();
                b.Property(p => p.Latitude).HasMaxLength(200).IsRequired();
                b.Property(p => p.Latitude)
                    .HasColumnType("decimal(10,8)");  // یا decimal(11,8) برای Longitude
                b.Property(p => p.Longitude)
                    .HasColumnType("decimal(11,8)");
                b.Property(p => p.HasRegion).HasDefaultValueSql("0");
                b.HasOne(p => p.Parent)
                 .WithMany(p => p.Children)
                 .HasForeignKey(p => p.ParentId)
                 .IsRequired(false) // Parent می‌تواند null باشد (برای ریشه)
                 .OnDelete(DeleteBehavior.Restrict); // یا Cascade بسته به نیاز

            });
            //--------------------RealEstates_SpecialFeature

            modelBuilder.Entity<RealEstates_SpecialFeature>(b =>
            {
                b.HasKey(rm => new { rm.RealEstatesId, rm.SpecialFeatureId });
            });


            //------------------RealEstatesRent_SpecialFeature
            modelBuilder.Entity<RealEstatesRent_SpecialFeature>(b =>
            {
                b.HasKey(rm => new { rm.RealEstatesRentId, rm.SpecialFeatureId });
            });
            //---------------------RealEstates_Facilities
            modelBuilder.Entity<RealEstates_Facilities>(b =>
            {
                b.HasKey(rm => new { rm.RealEstatesId, rm.FacilitiesId });
            });
            //---------------------RealEstates_Facilities
            modelBuilder.Entity<RealEstatesRent_Facilities>(b =>
            {
                b.HasKey(rm => new { rm.RealEstatesRentId, rm.FacilitiesId });
            });
            //--------------

            modelBuilder.Entity<ProjectUser>(b =>
            {
                b.HasKey(p => p.Id);

            });
            // ---------------- Package ----------------
            modelBuilder.Entity<Package>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Name).HasMaxLength(200).IsRequired();
                b.Property(s => s.MaxProjects).IsRequired();
                b.Property(s => s.MaxUsers).IsRequired();
                b.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");

                b.HasMany(p => p.UserPackages)
                 .WithOne(t => t.Package)
                 .HasForeignKey(t => t.PackageId);

            });
            // ---------------- UserPackage ----------------
            modelBuilder.Entity<UserPackage>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");


            });

            // ---------------- ExtraProject ----------------
            modelBuilder.Entity<ExtraProject>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");


            });

            // ---------------- Todo ----------------

            //-----------------TagProject ---------------




            modelBuilder.Entity<RoleMenu>(b =>
            {
                b.HasKey(rm => new { rm.RoleId, rm.MenuId });

                b.HasOne(rm => rm.Role)
                 .WithMany(r => r.RoleMenus)
                 .HasForeignKey(rm => rm.RoleId);

                b.HasOne(rm => rm.Menu)
                 .WithMany(m => m.RoleMenus)
                 .HasForeignKey(rm => rm.MenuId);

                //    b.HasOne(rm => rm.Permission)
                //.WithMany(m => m.RoleMenus)
                //.HasForeignKey(rm => rm.PermissionId);


            });

            // ---------------- Menu ----------------

        }

        private List<TrainingData> GetTrainingData()
        {
            var data = new List<TrainingData>();
            // 1000 نمونه داده مشاوره املاک
            for (int i = 0; i < 1000; i++)
            {
                data.Add(new TrainingData
                {
                    Id = i + 1,
                    Question = $"سوال نمونه {i + 1} درباره ملک",
                    Answer = $"پاسخ نمونه {i + 1} برای مشاوره",
                    Category = i % 3 == 0 ? "قیمت" : i % 3 == 1 ? "منطقه" : "متراژ",
                    Confidence = 0.9f
                });
            }
            return data;
        }
    }
}
