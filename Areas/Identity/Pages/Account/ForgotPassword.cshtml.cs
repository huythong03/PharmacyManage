using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PharmacyWeb.Areas.Identity.Pages.Account
{
	public class ForgotPasswordModel : PageModel
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IEmailSender _emailSender;

		public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender)
		{
			_userManager = userManager;
			_emailSender = emailSender;
		}

		[BindProperty]
		public InputModel Input { get; set; }

		public class InputModel
		{
			[Required(ErrorMessage = "Vui lòng nhập email.")]
			[EmailAddress(ErrorMessage = "Email không hợp lệ.")]
			public string Email { get; set; }
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(Input.Email);
				if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
				{
					// Không tiết lộ rằng email không tồn tại hoặc chưa xác nhận
					return RedirectToPage("./ForgotPasswordConfirmation");
				}

				var code = await _userManager.GeneratePasswordResetTokenAsync(user);
				var callbackUrl = Url.Page(
					"/Account/ResetPassword",
					pageHandler: null,
					values: new { area = "Identity", code },
					protocol: Request.Scheme);

				await _emailSender.SendEmailAsync(
					Input.Email,
					"Đặt Lại Mật Khẩu",
					$"Vui lòng đặt lại mật khẩu bằng cách nhấp vào <a href='{callbackUrl}'>liên kết này</a>.");

				return RedirectToPage("./ForgotPasswordConfirmation");
			}

			return Page();
		}
	}
}