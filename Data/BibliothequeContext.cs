using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrontOfficeApp.Models; // Assurez-vous que ce namespace correspond à celui de vos modèles
using Microsoft.EntityFrameworkCore;

namespace FrontOfficeApp.Data
{
    public class BibliothequeContext : DbContext
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Employe> Employes { get; set; }
        public DbSet<Adherent> Adherents { get; set; }
        public DbSet<Livre> Livres { get; set; }
        public DbSet<Emprunt> Emprunts { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Assurez-vous que la chaîne de connexion est correcte pour votre environnement FrontOfficeApp
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=Bibliotheque;Trusted_Connection=True;Encrypt=False;");
        }
    }
}
