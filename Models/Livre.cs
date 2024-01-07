using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontOfficeApp.Models
{
    public class Livre
    {
        [Key]
        public string ISBN { get; set; }
        public string Titre { get; set; }
        public string Auteur { get; set; }
        public int AnneePublication { get; set; }
        public int Quantite { get; set; }
        public string ImageCouvertureUrl { get; set; }

        [NotMapped]
       
        public string WebFriendlyImageCoverUrl
        {
            get
            {
                // Replace backslashes with forward slashes and ensure the path starts with a leading slash
                var pathWithForwardSlashes = ImageCouvertureUrl.Replace("\\", "/");
                if (!pathWithForwardSlashes.StartsWith("/"))
                {
                    pathWithForwardSlashes = "/" + pathWithForwardSlashes;
                }
                // Ensure the 'images' directory is correctly cased as 'Images'
                return pathWithForwardSlashes.Replace("images", "Images");
            }
        }
    }


}
