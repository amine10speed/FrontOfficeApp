using FrontOfficeApp.Data;
using FrontOfficeApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace FrontOfficeApp.Controllers
{
    public class EmpruntsController : Controller
    {
        private readonly BibliothequeContext _context;

        public EmpruntsController(BibliothequeContext context)
        {
            _context = context;
        }

        // Assuming you have a route setup to accept ISBN as a parameter for the book to be borrowed.
        [HttpPost]
        public async Task<IActionResult> AddEmprunt(string ISBN)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            var livre = await _context.Livres
                .FirstOrDefaultAsync(l => l.ISBN == ISBN);

            if (livre == null || livre.Quantite <= 0)
            {
                // Handle the case where the book doesn't exist or there are no copies left to borrow
                return View("Error"); // Replace with your error view
            }

            // Decrement the quantity of the book
            livre.Quantite--;

            var userId = User.Claims.FirstOrDefault(c => c.Type == "AdherentID")?.Value; // Adjust the claim type if necessary

            var emprunt = new Emprunt
            {
                AdherentID = int.Parse(userId),
                ISBN = ISBN,
                DateEmprunt = DateTime.Now,
                DateRetourPrevu = DateTime.Now.AddDays(14), // Adjust the number of days as necessary
            };

            _context.Emprunts.Add(emprunt);
            await _context.SaveChangesAsync();

           

            return RedirectToAction("OurBooks", "Home"); // Redirect to the books list page
        }
    }
}
