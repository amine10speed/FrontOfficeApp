using FrontOfficeApp.Data;
using FrontOfficeApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;

namespace FrontOfficeApp.Controllers
{
    public class EmpruntsController : Controller
    {
        private readonly BibliothequeContext _context;

        public EmpruntsController(BibliothequeContext context)
        {
            _context = context;
        }

        public void SendEmail(string email, string subject, string messageBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("admin", "anouar7moussaoui@gmail.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = messageBody };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate("safaabatrahi7@gmail.com", "ljdy xbhw azyr dphn");
                client.Send(message);
                client.Disconnect(true);
            }
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
            
            var adherent = await _context.Adherents.FindAsync(userId);
        if (adherent != null)
        {
            SendEmail(
                adherent.Email,
                "Book Borrowed",
                $"Dear {adherent.Prenom} {adherent.Nom}, you have borrowed the book '{livre.Titre}'. Please return it by {emprunt.DateRetourPrevu.ToShortDateString()}."
            );
        }


            TempData["Success"] = "You have successfully borrowed the book.";
            return RedirectToAction("OurBooks", "Home");
        }




    }
}
