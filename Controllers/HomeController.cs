﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using FrontOfficeApp.Models;
using FrontOfficeApp.Data;
using Microsoft.EntityFrameworkCore;
using System;
using FrontOfficeApp.Controllers;

public class HomeController : Controller
{
    private readonly BibliothequeContext _context;

    public HomeController(BibliothequeContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> OurBooksSearch(string search)
    {
        var query = _context.Livres.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(l => EF.Functions.Like(l.Titre, $"%{search}%"));
        }

        var livres = await query.ToListAsync();
        return View("OurBooks", livres);
    }

    public async Task<IActionResult> OurBooks()
    {
        var livres = await _context.Livres.ToListAsync();

        foreach (var livre in livres)
        {
            var reservationsController = new ReservationsController(_context);
            var nextAvailableDate = await reservationsController.CalculateNextAvailableDate(livre.ISBN);

            // Assuming Livre model has a property to hold the next available date
            // If not, you might need to extend your model or use a ViewModel
            livre.NextAvailableDate = nextAvailableDate;
        }

        return View(livres); // Pass the list of books with their next available dates to the view
    }


    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
   
   
	public IActionResult Register()
	{
		return View();
	}


	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Register(
	[Bind("NomUtilisateur,MotDePasse,Nom,Prenom,Adresse,Email")] Adherent adherent,
	string ConfirmPassword)
	{
		if (ModelState.IsValid)
		{
			if (adherent.MotDePasse != ConfirmPassword)
			{
				ModelState.AddModelError("ConfirmPassword", "The password and confirmation password do not match.");
				return RedirectToAction("Register", "Home"); // Return to the view with the model to show the error
			}

			_context.Add(adherent);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Login));
		}
		return View(adherent); // Return the model with validation errors if any
	}

	[HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        // Vérifiez les informations d'identification de l'utilisateur.
        var adherent = await _context.Adherents
            .FirstOrDefaultAsync(a => a.Email == email && a.MotDePasse == password);

        if (adherent != null)
        {
            // Créez une identité et un principal.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, adherent.Email),
                new Claim("AdherentID", adherent.AdherentID.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                // Permettre la persistance de la session après la fermeture du navigateur.
                IsPersistent = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "Invalid email or password");
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Index));
    }

}
