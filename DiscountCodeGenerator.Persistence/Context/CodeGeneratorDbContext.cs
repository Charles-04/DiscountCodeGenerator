using DiscountCodeGenerator.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscountCodeGenerator.Data.Context
{
    public class CodeGeneratorDbContext : DbContext
    {
        public CodeGeneratorDbContext(DbContextOptions<CodeGeneratorDbContext> options):base(options)
        {
            
        }
        public DbSet<DiscountCode> DiscountCodes => Set<DiscountCode>();
    }
}
