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
        public IActionResult ReservationBooks()
        {
            return View();
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
        [HttpPost , ActionName("UpdateAdherent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAdherent([Bind("AdherentID,NomUtilisateur,Nom,Prenom,Adresse,Email")] Adherent adherent)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "AdherentID")?.Value;

            if (!int.TryParse(userId, out int parsedUserId) || adherent.AdherentID != parsedUserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adherent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdherentExists(adherent.AdherentID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View("Profils", adherent); // Make sure this view can handle the adherent model
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


        

    }
}
