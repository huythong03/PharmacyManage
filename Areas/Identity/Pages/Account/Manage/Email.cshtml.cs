using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PharmacyWeb.Areas.Identity.Pages.Account.Manage
{
	public class EmailModel : PageModel
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly IEmailSender _emailSender;
		private readonly ILogger<EmailModel> _logger;

		public EmailModel(
			UserManager<IdentityUser> userManager,
			SignInManager<IdentityUser> signInManager,
			IEmailSender emailSender,
			ILogger<EmailModel> logger)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_emailSender = emailSender;
			_logger = logger;
		}

		[TempData]
		public string StatusMessage { get; set; }

		public string Email { get; set; }

		public bool IsEmailConfirmed { get; set; }

		[BindProperty]
		public InputModel Input { get; set; }

		public class InputModel
		{
			[Required]
			[EmailAddress]
			[Display(Name = "Email mới")]
			public string NewEmail { get; set; }
		}

		private async Task LoadAsync(IdentityUser user)
		{
			var email = await _userManager.GetEmailAsync(user);
			Email = email;

			var emailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
			IsEmailConfirmed = emailConfirmed;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			await LoadAsync(user);
			return Page();
		}

		public async Task<IActionResult> OnPostChangeEmailAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			if (!ModelState.IsValid)
			{
				await LoadAsync(user);
				return Page();
			}

			var email = await _userManager.GetEmailAsync(user);
			if (Input.NewEmail != email)
			{
				try
				{
					var userId = await _userManager.GetUserIdAsync(user);
					var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
					var callbackUrl = Url.Page(
						"/Account/ConfirmEmailChange",
						pageHandler: null,
						values: new { userId = userId, email = Input.NewEmail, code = code },
						protocol: Request.Scheme);
					var htmlMessage = $"Vui lòng xác nhận email mới bằng cách <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>nhấn vào đây</a>.";
					await _emailSender.SendEmailAsync(
						Input.NewEmail,
						"Xác nhận thay đổi email",
						htmlMessage);

					StatusMessage = "Đã gửi email xác nhận. Vui lòng kiểm tra email của bạn để xác nhận thay đổi.";
					return RedirectToPage();
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Không thể gửi email xác nhận thay đổi email cho {Email}", Input.NewEmail);
					StatusMessage = "Không thể gửi email xác nhận. Vui lòng thử lại sau.";
					return RedirectToPage();
				}
			}

			StatusMessage = "Email mới phải khác email hiện tại.";
			return RedirectToPage();
		}

		public async Task<IActionResult> OnPostSendVerificationEmailAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			try
			{
				var userId = await _userManager.GetUserIdAsync(user);
				var email = await _userManager.GetEmailAsync(user);
				var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
				var callbackUrl = Url.Page(
					"/Account/ConfirmEmail",
					pageHandler: null,
					values: new { userId = userId, code = code },
					protocol: Request.Scheme);
				var htmlMessage = $"Vui lòng xác nhận email của bạn bằng cách <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>nhấn vào đây</a>.";
				await _emailSender.SendEmailAsync(
					email,
					"Xác nhận email",
					htmlMessage);

				StatusMessage = "Đã gửi email xác nhận. Vui lòng kiểm tra email của bạn.";
				return RedirectToPage();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Không thể gửi email xác nhận cho {Email}", Email);
				StatusMessage = "Không thể gửi email xác nhận. Vui lòng thử lại sau.";
				return RedirectToPage();
			}
		}
	}
}