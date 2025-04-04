﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PharmacyWeb.Areas.Identity.Pages.Account;

public class LogoutModel : PageModel
{
	private readonly SignInManager<IdentityUser> _signInManager;

	public LogoutModel(SignInManager<IdentityUser> signInManager)
	{
		_signInManager = signInManager;
	}

	public async Task<IActionResult> OnPost(string? returnUrl = null)
	{
		await _signInManager.SignOutAsync();
		return LocalRedirect(returnUrl ?? Url.Content("~/"));
	}
}