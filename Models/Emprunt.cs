using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontOfficeApp.Models
{
    public class Emprunt
    {
        [Key]
        public int EmpruntID { get; set; }

        [ForeignKey("Adherent")]
        public int AdherentID { get; set; }
        public virtual Adherent Adherent { get; set; }

        [ForeignKey("Livre")]
        public string ISBN { get; set; }
        public virtual Livre Livre { get; set; }

        public DateTime DateEmprunt { get; set; }
        public DateTime DateRetourPrevu { get; set; }
        public DateTime? DateRetourReel { get; set; } // Peut être null si le livre n'est pas encore retourné

        // Ajoutez d'autres propriétés si nécessaire, par exemple pour gérer les retards ou les pénalités
    }

}
