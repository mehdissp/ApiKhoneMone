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



        public DbSet<BookMark> BookMarks { get; set; }
        public DbSet<RealEstates_SpecialFeature> RealEstates_SpecialFeatures { get; set; }
        public DbSet<RealEstates_Facilities> RealEstates_Facilities { get; set; }


        public DbSet<RealEstatesRent_SpecialFeature> RealEstatesRent_SpecialFeatures { get; set; }
        public DbSet<RealEstatesRent_Facilities> RealEstatesRent_Facilities { get; set; }

        public DbSet<Image> Images { get; set; }



        

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
            modelBuilder.Entity<Role>(b =>
            {

                b.Property(x => x.Name).HasMaxLength(250).IsRequired();

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
                b.Property(p => p.IsHasElevator).HasDefaultValueSql("0");
                b.Property(p => p.IsHasPool).HasDefaultValueSql("0");
                b.Property(p => p.DescriptionRows).HasMaxLength(450);
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
    }
}
