using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Bai1.Areas.Admin.Models;
using System.Threading.Tasks;
using Bai1.Models;

[Authorize]
public class UserController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleDarkMode([FromBody] DarkModeRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        user.IsDarkMode = request.IsDarkMode;
        await _userManager.UpdateAsync(user);

        return Ok(new { success = true });
    }
}

public class DarkModeRequest
{
    public bool IsDarkMode { get; set; }
}
