using Labotec.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq;

namespace Labotec.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAll()
    {
        var users = await _userManager.Users
            .OrderBy(u => u.UserName)
            .ToListAsync();

        var result = new List<UserReadDto>(users.Count);
        foreach (var user in users)
        {
            result.Add(await ToReadDto(user));
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserReadDto>> GetById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();
        return Ok(await ToReadDto(user));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserReadDto>> Update(string id, [FromBody] UserUpdateDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.UserName))
        {
            user.UserName = dto.UserName;
        }

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            user.Email = dto.Email;
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return BadRequest(updateResult.Errors);
        }

        if (dto.Roles is not null)
        {
            var requestedRoles = dto.Roles.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            foreach (var role in requestedRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    return BadRequest(new { message = $"El rol '{role}' no existe." });
                }
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = currentRoles.Except(requestedRoles, StringComparer.OrdinalIgnoreCase);
            var rolesToAdd = requestedRoles.Except(currentRoles, StringComparer.OrdinalIgnoreCase);

            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
            {
                return BadRequest(removeResult.Errors);
            }

            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
            {
                return BadRequest(addResult.Errors);
            }
        }

        if (dto.Lockout.HasValue)
        {
            var lockoutEnd = dto.Lockout.Value
                ? DateTimeOffset.UtcNow.AddYears(100)
                : (DateTimeOffset?)null;

            var lockResult = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);
            if (!lockResult.Succeeded)
            {
                return BadRequest(lockResult.Errors);
            }
        }

        return Ok(await ToReadDto(user));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == id)
        {
            return BadRequest(new { message = "No puedes eliminar tu propia cuenta." });
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }

    private async Task<UserReadDto> ToReadDto(IdentityUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
        var isLocked = lockoutEnd.HasValue && lockoutEnd.Value.UtcDateTime > DateTime.UtcNow;

        return new UserReadDto(
            user.Id,
            user.UserName,
            user.Email,
            roles.ToList().AsReadOnly(),
            isLocked,
            lockoutEnd);
    }
}
