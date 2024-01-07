using FrontOfficeApp.Data;
using FrontOfficeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FrontOfficeApp.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly BibliothequeContext _context;

        public ReservationsController(BibliothequeContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddReservation(string ISBN)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            var userId = User.Claims.FirstOrDefault(c => c.Type == "AdherentID")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "An error occurred while processing your request.";
                return RedirectToAction("OurBooks", "Home");
            }

            // Check for an existing reservation for the same book by the same user
            bool alreadyReserved = await _context.Reservations.AnyAsync(r =>
                r.ISBN == ISBN &&
                r.AdherentID == int.Parse(userId)); 

            if (alreadyReserved)
            {
                TempData["Error"] = "You have already reserved this book.";
                return RedirectToAction("OurBooks", "Home");
            }

            var nextAvailableDate = await CalculateNextAvailableDate(ISBN);

            var reservation = new Reservation
            {
                AdherentID = int.Parse(userId),
                ISBN = ISBN,
                DateReservation = DateTime.Now,
                DatePrevuRetrait = nextAvailableDate
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Book reserved successfully for " + nextAvailableDate.ToString("dd-MM-yyyy");
            return RedirectToAction("OurBooks", "Home");
        }


        public async Task<DateTime> CalculateNextAvailableDate(string ISBN)
        {
            // Get all expected return dates from Emprunts for the given ISBN where the book hasn't been returned yet
            var returnDates = await _context.Emprunts
                .Where(e => e.ISBN == ISBN && e.DateRetourReel == null)
                .Select(e => e.DateRetourPrevu)
                .ToListAsync();

            // Get all expected retrieval dates from Reservations for the given ISBN
            var retrievalDates = await _context.Reservations
                .Where(r => r.ISBN == ISBN)
                .Select(r => r.DatePrevuRetrait)
                .ToListAsync();

            // Find the first return date that is not in the retrieval dates
            var nextAvailableDate = returnDates.FirstOrDefault(returnDate => !retrievalDates.Contains(returnDate));

            if (nextAvailableDate == default)
            {
                // If no suitable date is found, set a default date (e.g., 14 days from now)
                nextAvailableDate = DateTime.Today.AddDays(14);
            }

            return nextAvailableDate;
        }

    }

}
