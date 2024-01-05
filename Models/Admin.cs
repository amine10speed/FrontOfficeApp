using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontOfficeApp.Models
{
    public class Admin
    {
        public int AdminId { get; set; } // Clé primaire
        public string NomUtilisateur { get; set; }
        public string MotDePasse { get; set; }
        // Vous pouvez également ajouter des méthodes ou des constructeurs ici
    }

}
