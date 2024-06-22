//using ExcelToDB.Models;
using Microsoft.EntityFrameworkCore;

//namespace ExcelToDB.Models
//{
//    
//}

namespace ExcelToDB.Models
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options)
            : base(options)
        {
        }
        public DbSet<Student> Students { get; set; }

    }

}

