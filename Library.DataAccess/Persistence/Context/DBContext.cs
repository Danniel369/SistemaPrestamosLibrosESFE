using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Options;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Library.DataAccess.Persistence.Context
{
    public class DBContext : DbContext
    {
        public DbSet<Categories> Categories { get; set; }
        public DbSet<AcquisitionTypes> Acquisition_Types { get; set; }
        public DbSet<Authors> Authors{ get; set; }
        public DbSet<Books> Books { get; set; }
        public DbSet<Catalogs> Catalogs { get; set; }
        public DbSet<Countries> Countries { get; set; }
        public DbSet<Editions> Editions { get; set; }
        public DbSet<Editorials> Editorials { get; set; }
        public DbSet<LoanDates> Loan_Dates { get; set; }
        public DbSet<Loans> Loans { get; set; }
        public DbSet<LoansTeacher> Teacher { get; set; }
        public DbSet<Personal> Personal { get; set; }
        public DbSet<LoanTypes> Loan_Types { get; set; }
        public DbSet<ReservationStatus> Reservation_Status { get; set; }
        public DbSet<Posts> Posts { get; set; }
        public DbSet<PostsImages> Posts_Images { get; set; }
        public DbSet<PostsCategories> Posts_Categories { get; set; }
        public DbSet<PostsDocs> Posts_Docs { get; set; }
        public DbSet<PinnedPosts> Pinned_Posts { get; set; }
        public DbSet<Users> Users { get; set; } 
        public DbSet<Users_Roles> Users_Roles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {

            ///BooksImport/ImportBooks
            //optionBuilder.UseSqlServer(@"Data Source= PC-FERNANDO;Initial Catalog=LIBRARY;Integrated Security=True; Trust Server Certificate=true");
            //optionBuilder.UseSqlServer(@"Data Source= KATHY\KATHY; Initial Catalog=LIBRARY;Integrated Security=True; Trust Server Certificate=true");
            //optionBuilder.UseSqlServer(@"Data Source=AREVPC;Initial Catalog=LIBRARY;User ID=sa;Password=emmanuelle;Trust Server Certificate=true");
            //optionBuilder.UseSqlServer(@"Data Source=Marroquin55N\MSSQLSERVER01;Initial Catalog=LIBRARY;User ID=Norberto9;Password=1234;Trust Server Certificate=True");
            optionBuilder.UseSqlServer(@"Data Source=DESKTOP-NFDMETJ;Initial Catalog=library6;Integrated Security=True;Encrypt=False");

            //optionBuilder.UseSqlServer(@"Data Source=THEDESTRUCTORGY\SQLEXPRESS;Initial Catalog=LIBRARY;User ID=sa;Password=rodrigo;Trust Server Certificate=true");
            //optionBuilder.UseSqlServer(@"Data Source=DESKTOP-JV2NN3U\SQLEXPRESS; Initial Catalog=LIBRARY;User ID=sa;Password=12345; Trust Server Certificate=true");
        }
    }
}
