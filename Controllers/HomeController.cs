﻿using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Login()
    {
        return View();
    }

    public IActionResult OurBooks()
    {
        return View();
    }

	public IActionResult Register()
	{
		return View();
	}
}

