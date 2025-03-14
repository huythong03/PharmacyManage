using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PharmacyWeb.Areas.Identity.Pages.Account.Manage
{
	public class IndexModel : PageModel
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;

		public IndexModel(
			UserManager<IdentityUser> userManager,
			SignInManager<IdentityUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public string Username { get; set; }

		[TempData]
		public string StatusMessage { get; set; }

		[BindProperty]
		public InputModel Input { get; set; }

		public class InputModel
		{
			[Required(ErrorMessage = "Địa Chỉ Email là bắt buộc.")]
			[EmailAddress(ErrorMessage = "Địa Chỉ Email không hợp lệ.")]
			[Display(Name = "Địa Chỉ Email")]
			public string Email { get; set; }

			[Phone(ErrorMessage = "Số Điện Thoại không hợp lệ.")]
			[Display(Name = "Số Điện Thoại")]
			public string PhoneNumber { get; set; }
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Không thể tải thông tin người dùng với ID '{_userManager.GetUserId(User)}'.");
			}

			Username = await _userManager.GetUserNameAsync(user);

			Input = new InputModel
			{
				Email = await _userManager.GetEmailAsync(user),
				PhoneNumber = await _userManager.GetPhoneNumberAsync(user)
			};

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Không thể tải thông tin người dùng với ID '{_userManager.GetUserId(User)}'.");
			}

			if (!ModelState.IsValid)
			{
				Username = await _userManager.GetUserNameAsync(user);
				Input = new InputModel
				{
					Email = await _userManager.GetEmailAsync(user),
					PhoneNumber = await _userManager.GetPhoneNumberAsync(user)
				};
				return Page();
			}

			// Cập nhật Email
			var email = await _userManager.GetEmailAsync(user);
			if (Input.Email != email)
			{
				var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
				if (!setEmailResult.Succeeded)
				{
					StatusMessage = "Đã xảy ra lỗi khi cập nhật địa chỉ email.";
					return RedirectToPage();
				}
			}

			// Cập nhật PhoneNumber
			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
			if (Input.PhoneNumber != phoneNumber)
			{
				var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
				if (!setPhoneResult.Succeeded)
				{
					StatusMessage = "Đã xảy ra lỗi khi cập nhật số điện thoại.";
					return RedirectToPage();
				}
			}

			await _signInManager.RefreshSignInAsync(user);
			StatusMessage = "Hồ sơ của bạn đã được cập nhật thành công.";
			return RedirectToPage();
		}
	}
}