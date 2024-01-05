using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontOfficeApp.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationID { get; set; }

        [ForeignKey("Adherent")]
        public int AdherentID { get; set; }
        public virtual Adherent Adherent { get; set; }

        [ForeignKey("Livre")]
        public string ISBN { get; set; }
        public virtual Livre Livre { get; set; }

        public DateTime DateReservation { get; set; }
        public DateTime? DatePrevuRetrait { get; set; } // Date prévue pour la récupération du livre

        // Vous pouvez ajouter d'autres propriétés selon les besoins de votre application
    }
}

