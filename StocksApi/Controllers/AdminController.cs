using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StocksApi.DatabaseContext;

namespace StocksApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> user, RoleManager<ApplicationRole> roleManager)
        {
            this._userManager = user;
            _roleManager = roleManager;
        }
        [Authorize(policy: "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var users = _userManager.Users.ToList(); // Get all users

            var usersWithRoles = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                usersWithRoles.Add(new
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles
                });
            }

            return Ok(usersWithRoles);
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{userId}")]
        public async Task<IActionResult> EditRoles(Guid userId, [FromBody] List<string> selectedRoles)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound("User not found.");

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Ensure selected roles exist
            foreach (var role in selectedRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = role });
                }
            }

            // Add new roles that are not already assigned
            var rolesToAdd = selectedRoles.Except(currentRoles);
            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
                return BadRequest("Failed to add roles.");

            // Remove roles that are not selected anymore
            var rolesToRemove = currentRoles.Except(selectedRoles);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
                return BadRequest("Failed to remove roles.");

            var updatedRoles = await _userManager.GetRolesAsync(user);
            return Ok(new { user.Id, user.UserName, UpdatedRoles = updatedRoles });
        }

        //[Authorize(policy: "RequireAdminRole")]
        //[HttpPost("edit-roles/{username}")]
        //public async Task<IActionResult> EditRoles(string username, [FromQuery] string roles)
        //{
        //    if (string.IsNullOrWhiteSpace(roles))
        //        return BadRequest("You must select at least one role.");

        //    var selectedRoles = roles.Split(',').Select(r => r.Trim()).ToArray();

        //    var user = await _userManager.FindByNameAsync(username);
        //    if (user == null)
        //        return NotFound("User not found.");

        //    // Ensure all roles exist
        //    foreach (var role in selectedRoles)
        //    {
        //        if (!await _roleManager.RoleExistsAsync(role))
        //        {
        //            var roleResult = await _roleManager.CreateAsync(new IdentityRole(role));
        //            if (!roleResult.Succeeded)
        //            {
        //                return BadRequest($"Failed to create role '{role}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
        //            }
        //        }
        //    }

        //    var currentRoles = await _userManager.GetRolesAsync(user);

        //    var addResult = await _userManager.AddToRolesAsync(user, selectedRoles.Except(currentRoles));
        //    if (!addResult.Succeeded)
        //        return BadRequest("Failed to add user to roles.");

        //    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles.Except(selectedRoles));
        //    if (!removeResult.Succeeded)
        //        return BadRequest("Failed to remove user from roles.");

        //    return Ok(await _userManager.GetRolesAsync(user));
        //}


        //[Authorize(policy: "RequireAdminRole")]
        //[HttpPost("edit-roles/{username}")]
        //public async Task<IActionResult> EditRoles(string username, [FromQuery] string roles)
        //{
        //    if (string.IsNullOrEmpty(roles))
        //        return BadRequest("You must select at least one role.");

        //    // Split incoming roles by comma and remove whitespace
        //    var selectedRoles = roles.Split(',').Select(r => r.Trim()).ToArray();

        //    // Find user by username
        //    var user = await _userManager.FindByNameAsync(username);
        //    if (user == null)
        //        return NotFound("User not found.");

        //    // Get current roles of the user
        //    var currentRoles = await _userManager.GetRolesAsync(user);

        //    // Add new roles that the user doesn't already have
        //    var addRolesResult = await _userManager.AddToRolesAsync(user, selectedRoles.Except(currentRoles));
        //    if (!addRolesResult.Succeeded)
        //        return BadRequest("Failed to add user to roles.");

        //    // Remove roles that are no longer selected
        //    var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles.Except(selectedRoles));
        //    if (!removeRolesResult.Succeeded)
        //        return BadRequest("Failed to remove user from roles.");

        //    // Return the updated roles for confirmation
        //    var updatedRoles = await _userManager.GetRolesAsync(user);
        //    return Ok(updatedRoles);
        //}


        //[Authorize(policy: "RequireAdminRole")]
        //[HttpGet("users-with-roles")]
        //public async Task<IActionResult> GetRoGetUsersWithRolesles()
        //{
        // var roles =   await _userManager.GetRolesAsync();
        //    return roles;
        //}
    }
}
