using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FrontOfficeApp.Data;
using FrontOfficeApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Components.RenderTree;
using System.Security.Claims;


namespace FrontOfficeApp.Controllers
{
    public class AdherentsController : Controller
    {
        private readonly BibliothequeContext _context;

        public AdherentsController(BibliothequeContext context)
        {
            _context = context;
        }

        // GET: Adherents
        public async Task<IActionResult> Index()
        {
            return View(await _context.Adherents.ToListAsync());
        }

        // GET BORROW BOOKS 
        // GET: Adherents/BorrowBooks
        public async Task<IActionResult> BorrowBooks()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            var userId = User.Claims.FirstOrDefault(c => c.Type == "AdherentID")?.Value;

            if (!int.TryParse(userId, out int adherentId))
            {
                // Handle the case where user ID is not found or invalid
                return RedirectToAction("Index", "Home");
            }

            var emprunts = await _context.Emprunts
                .Where(e => e.AdherentID == adherentId)
                .Include(e => e.Livre) // Assuming Emprunt has a navigation property to Livre
                .ToListAsync();

            return View(emprunts); // Pass the list of emprunts to the BorrowBooks view
        }


        // GET RESERVATIONS BOOKS 
       

        public async Task<IActionResult> ReservationBooks()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            var userId = User.Claims.FirstOrDefault(c => c.Type == "AdherentID")?.Value;

            if (!int.TryParse(userId, out int adherentId))
            {
                // Handle the case where user ID is not found or invalid
                return RedirectToAction("Index", "Home");
            }

            var Reservations = await _context.Reservations
                .Where(e => e.AdherentID == adherentId)
                .Include(e => e.Livre) // Assuming Emprunt has a navigation property to Livre
                .ToListAsync();

            return View(Reservations); // Pass the list of emprunts to the BorrowBooks view
        }
        // GET profil BOOKS 
        public async Task<IActionResult> Profils()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "AdherentID")?.Value;
            if (!int.TryParse(userId, out int adherentId))
            {
                // Gérer le cas où l'ID de l'utilisateur n'est pas trouvé ou est invalide
                return RedirectToAction("Login", "Home");
            }

            var adherent = await _context.Adherents
                .FirstOrDefaultAsync(e => e.AdherentID == adherentId);

            if (adherent == null)
            {
                // Gérer le cas où l'adhérent n'est pas trouvé
                return RedirectToAction("Login", "Home");
            }

            return View(adherent); // Passer l'adhérent à la vue
        }





        // GET: Adherents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adherent = await _context.Adherents
                .FirstOrDefaultAsync(m => m.AdherentID == id);
            if (adherent == null)
            {
                return NotFound();
            }

            return View(adherent);
        }

        // GET: Adherents/Create
        public IActionResult Create()
        {
            return View();
        }

		// POST: Adherents/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

		

        // GET: Adherents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adherent = await _context.Adherents.FindAsync(id);
            if (adherent == null)
            {
                return NotFound();
            }
            return View(adherent);
        }

        // POST: Adherents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAdherent(Adherent model)
        {
            // Ensures that the user is authenticated before attempting to update
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); // Or your login action
            }


            // Ensures the current user is updating their own profile
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Make sure this claim type matches your setup
            if (model.AdherentID.ToString() != currentUserId)
            {
                // If the ID does not match, forbid the operation
                return Forbid();
            }

            // Retrieves the adherent from the database
            var adherent = await _context.Adherents.FindAsync(model.AdherentID);
            if (adherent == null)
            {
                // If no adherent is found, return a NotFound result
                return NotFound();
            }

            // Maps the updated information from the model to the adherent entity
            adherent.Prenom = model.Prenom;
            adherent.Nom = model.Nom;
            adherent.NomUtilisateur = model.NomUtilisateur;
            adherent.Email = model.Email;
            adherent.Adresse = model.Adresse;

            // Tries to save changes to the database
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // If a concurrency error occurs, display an error message
                ModelState.AddModelError("", "Someone else may have updated this profile. Please try again.");
                return View(model);
            }
            catch (Exception)
            {
                // If any other exception occurs, display a generic error message
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                return View(model);
            }

            // If the update is successful, redirect to the profile page with a success message
            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction("Profile"); // Assuming you have a "Profile" view to redirect to
        }

        // GET: Adherents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adherent = await _context.Adherents
                .FirstOrDefaultAsync(m => m.AdherentID == id);
            if (adherent == null)
            {
                return NotFound();
            }

            return View(adherent);
        }

        // POST: Adherents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var adherent = await _context.Adherents.FindAsync(id);
            if (adherent != null)
            {
                _context.Adherents.Remove(adherent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdherentExists(int id)
        {
            return _context.Adherents.Any(e => e.AdherentID == id);
        }



        public async Task<IActionResult> GetHTMLPageAsPdf(long id)
        {
            var reservation = _context.Reservations
                                     .Include(r => r.Livre)
                                     .FirstOrDefault(r => r.ReservationID == id);
            
            var userId = User.Claims.FirstOrDefault(c => c.Type == "AdherentID")?.Value;
            if (!int.TryParse(userId, out int adherentId))
            {
                return RedirectToAction("Login", "Home");
            }
            var adherent = await _context.Adherents
                                         .FirstOrDefaultAsync(e => e.AdherentID == adherentId);
            if (reservation == null)
            {
                return NotFound("Reservation not found.");
            }
            if (reservation.Livre == null)
            {
                return NotFound("Book details not found for the reservation.");
            }
            if (adherent == null)
            {
                return NotFound("User not found.");
            }
            var Renderer = new IronPdf.ChromePdfRenderer();

            // Create HTML content for the PDF
            var htmlContent = $@"
<html>
<head>
    <title>Reservation Details</title>
</head>
<body>
    <h1>Reservation receipt</h1>
    <p><strong>Lastname:</strong> {adherent.Nom}</p>   
<p><strong>Firstname:</strong> {adherent.Prenom}</p>
<p><strong>Email:</strong> {adherent.Email}</p>
    <p><strong>Book Name:</strong> {reservation.Livre.Titre}</p>
    <p><strong>Reservation Date:</strong> {reservation.DateReservation.ToString("d")}</p>
</body>
</html>";
            using var PDF = Renderer.RenderHtmlAsPdf(htmlContent);
            var contentLength = PDF.BinaryData.Length;
            Response.Headers["Content-Length"] = contentLength.ToString();
            Response.Headers.Add("Content-Disposition", "inline; filename=Reservation_" + id + ".pdf");
            return File(PDF.BinaryData, "application/pdf");
        }


    }
}
