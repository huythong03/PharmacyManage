using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PharmacyWeb.Areas.Identity.Pages.Account
{
	public class ResetPasswordModel : PageModel
	{
		private readonly UserManager<IdentityUser> _userManager;

		public ResetPasswordModel(UserManager<IdentityUser> userManager)
		{
			_userManager = userManager;
		}

		[BindProperty]
		public InputModel Input { get; set; }

		public class InputModel
		{
			[Required(ErrorMessage = "Vui lòng nhập email.")]
			[EmailAddress(ErrorMessage = "Email không hợp lệ.")]
			public string Email { get; set; }

			[Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
			[StringLength(100, ErrorMessage = "Mật khẩu phải dài từ {2} đến {1} ký tự.", MinimumLength = 6)]
			[DataType(DataType.Password)]
			public string Password { get; set; }

			[DataType(DataType.Password)]
			[Display(Name = "Xác Nhận Mật Khẩu")]
			[Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
			public string ConfirmPassword { get; set; }

			[Required]
			public string Code { get; set; }
		}

		public IActionResult OnGet(string code = null)
		{
			if (code == null)
			{
				return BadRequest("Mã xác nhận là bắt buộc để đặt lại mật khẩu.");
			}
			Input = new InputModel { Code = code };
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var user = await _userManager.FindByEmailAsync(Input.Email);
			if (user == null)
			{
				// Không tiết lộ email không tồn tại
				return RedirectToPage("./ResetPasswordConfirmation");
			}

			var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
			if (result.Succeeded)
			{
				return RedirectToPage("./ResetPasswordConfirmation");
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
			return Page();
		}
	}
}