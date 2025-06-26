using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThinhGiangDuocThong_DoAnWeb.Models;
using ThinhGiangDuocThong_DoAnWeb.Areas.Admin.Models;

namespace ThinhGiangDuocThong_DoAnWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    Address = user.Address,
                    Age = user.Age,
                    PhoneNumber = user.PhoneNumber,
                    Roles = string.Join(", ", roles)
                });
            }

            return View(userViewModels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    Address = model.Address,
                    Age = model.Age,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Assign role if specified
                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }

                    TempData["Success"] = "Tài khoản đã được tạo thành công!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Address = user.Address,
                Age = user.Age,
                PhoneNumber = user.PhoneNumber,
                CurrentRole = roles.FirstOrDefault()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.FullName = model.FullName;
                user.Address = model.Address;
                user.Age = model.Age;
                user.PhoneNumber = model.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Update role if changed
                    if (!string.IsNullOrEmpty(model.NewRole) && model.NewRole != model.CurrentRole)
                    {
                        if (!string.IsNullOrEmpty(model.CurrentRole))
                        {
                            await _userManager.RemoveFromRoleAsync(user, model.CurrentRole);
                        }
                        await _userManager.AddToRoleAsync(user, model.NewRole);
                    }

                    TempData["Success"] = "Tài khoản đã được cập nhật thành công!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Address = user.Address,
                Age = user.Age,
                PhoneNumber = user.PhoneNumber,
                Roles = string.Join(", ", roles)
            };

            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Address = user.Address,
                Age = user.Age,
                PhoneNumber = user.PhoneNumber,
                Roles = string.Join(", ", roles)
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Tài khoản đã được xóa thành công!";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa tài khoản!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
