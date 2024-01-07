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

            var userIdString = User.Claims.FirstOrDefault(c => c.Type == "AdherentID")?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                // Handle the error if the user ID claim is not found or cannot be parsed
                return RedirectToAction("OurBooks", "Home");
            }

            var livre = await _context.Livres
                .FirstOrDefaultAsync(l => l.ISBN == ISBN);

            if (livre == null || livre.Quantite <= 0)
            {
                // Handle the case where the book doesn't exist or there are no copies left to borrow
                TempData["Error"] = "This book is not available.";
                return RedirectToAction("OurBooks", "Home");
            }

            // Check if the user has already borrowed this book and not returned it
            var existingEmprunt = await _context.Emprunts
                .AnyAsync(e => e.ISBN == ISBN && e.AdherentID == userId && e.DateRetourReel == null);

            if (existingEmprunt)
            {
                // If the user has already borrowed this book, do not allow another borrow
                TempData["Error"] = "You have already borrowed this book.";
                return RedirectToAction("OurBooks", "Home");
            }

            // Decrement the quantity of the book
            livre.Quantite--;

            var emprunt = new Emprunt
            {
                AdherentID = userId,
                ISBN = ISBN,
                DateEmprunt = DateTime.Now,
                DateRetourPrevu = DateTime.Now.AddDays(14), // Set the return date to 14 days from now
            };

            _context.Emprunts.Add(emprunt);
            await _context.SaveChangesAsync();

            TempData["Success"] = "You have successfully borrowed the book.";
            return RedirectToAction("OurBooks", "Home");
        }




    }
}
