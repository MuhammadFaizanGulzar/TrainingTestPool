using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using Task_4_NETMVCandRazorPages.Models.Domain;

namespace Task_4_NETMVCandRazorPages.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        
        }

        public DbSet<Item> Items { get; set; }
   
    }
}
