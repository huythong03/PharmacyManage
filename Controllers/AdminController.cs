using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PharmacyWeb.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController : Controller
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}

		// GET: /Admin/Roles
		public async Task<IActionResult> Roles()
		{
			var roles = await _roleManager.Roles.ToListAsync();
			return View(roles);
		}

		// GET: /Admin/Roles/Create
		public IActionResult CreateRole()
		{
			return View();
		}

		// POST: /Admin/Roles/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateRole(string roleName)
		{
			if (!string.IsNullOrEmpty(roleName))
			{
				var roleExists = await _roleManager.RoleExistsAsync(roleName);
				if (!roleExists)
				{
					var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
					if (result.Succeeded)
					{
						TempData["Success"] = $"Đã tạo vai trò '{roleName}' thành công!";
						return RedirectToAction(nameof(Roles));
					}
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}
				}
				else
				{
					ModelState.AddModelError("", $"Vai trò '{roleName}' đã tồn tại.");
				}
			}
			return View();
		}

		// GET: /Admin/Roles/Delete
		public async Task<IActionResult> DeleteRole(string id)
		{
			var role = await _roleManager.FindByIdAsync(id);
			if (role == null)
			{
				TempData["Error"] = "Không tìm thấy vai trò để xóa!";
				return RedirectToAction(nameof(Roles));
			}
			return View(role);
		}

		// POST: /Admin/Roles/Delete
		[HttpPost, ActionName("DeleteRole")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteRoleConfirmed(string id)
		{
			var role = await _roleManager.FindByIdAsync(id);
			if (role != null)
			{
				var result = await _roleManager.DeleteAsync(role);
				if (result.Succeeded)
				{
					TempData["Success"] = $"Đã xóa vai trò '{role.Name}' thành công!";
				}
				else
				{
					TempData["Error"] = "Có lỗi khi xóa vai trò!";
				}
			}
			return RedirectToAction(nameof(Roles));
		}

		// GET: /Admin/Users
		public async Task<IActionResult> Users()
		{
			var users = await _userManager.Users.ToListAsync();
			var userRoles = new List<UserRoleViewModel>();
			foreach (var user in users)
			{
				var roles = await _userManager.GetRolesAsync(user);
				userRoles.Add(new UserRoleViewModel
				{
					UserId = user.Id,
					UserName = user.UserName,
					Email = user.Email,
					Roles = roles.ToList()
				});
			}
			return View(userRoles);
		}

		// GET: /Admin/Users/Edit
		public async Task<IActionResult> EditUser(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				TempData["Error"] = "Không tìm thấy người dùng!";
				return RedirectToAction(nameof(Users));
			}
			var roles = await _userManager.GetRolesAsync(user);
			var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
			ViewBag.AllRoles = allRoles;
			return View(new UserRoleViewModel
			{
				UserId = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				Roles = roles.ToList()
			});
		}

		// POST: /Admin/Users/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditUser(UserRoleViewModel model, string[] selectedRoles)
		{
			var user = await _userManager.FindByIdAsync(model.UserId);
			if (user == null)
			{
				TempData["Error"] = "Không tìm thấy người dùng!";
				return RedirectToAction(nameof(Users));
			}

			var currentRoles = await _userManager.GetRolesAsync(user);
			var rolesToAdd = selectedRoles.Except(currentRoles).ToList();
			var rolesToRemove = currentRoles.Except(selectedRoles).ToList();

			var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
			if (!removeResult.Succeeded)
			{
				TempData["Error"] = "Có lỗi khi xóa vai trò!";
				return View(model);
			}

			var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
			if (!addResult.Succeeded)
			{
				TempData["Error"] = "Có lỗi khi thêm vai trò!";
				return View(model);
			}

			TempData["Success"] = $"Đã cập nhật vai trò cho '{user.UserName}' thành công!";
			return RedirectToAction(nameof(Users));
		}
	}

	public class UserRoleViewModel
	{
		public string UserId { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public List<string> Roles { get; set; }
	}
}