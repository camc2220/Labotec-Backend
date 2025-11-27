namespace Labotec.Api.DTOs;

/// <summary>
/// Información legible de un usuario para los paneles administrativos.
/// </summary>
/// <param name="Id">Identificador interno del usuario.</param>
/// <param name="UserName">Nombre de usuario.</param>
/// <param name="Email">Correo registrado.</param>
/// <param name="Roles">Listado de roles asignados.</param>
/// <param name="IsLocked">Indica si el usuario está bloqueado actualmente.</param>
/// <param name="LockoutEnd">Fecha en la que termina el bloqueo (si aplica).</param>
public record UserReadDto(
    string Id,
    string? UserName,
    string? Email,
    IReadOnlyCollection<string> Roles,
    bool IsLocked,
    DateTimeOffset? LockoutEnd);

/// <summary>
/// Solicitud para modificar datos básicos de un usuario.
/// </summary>
/// <param name="UserName">Nuevo nombre de usuario (opcional).</param>
/// <param name="Email">Nuevo correo electrónico (opcional).</param>
/// <param name="Roles">Lista completa de roles que debe poseer el usuario (opcional).</param>
/// <param name="Lockout">True para bloquear, false para desbloquear, null para mantener el estado.</param>
public record UserUpdateDto(
    string? UserName,
    string? Email,
    IReadOnlyCollection<string>? Roles,
    bool? Lockout);
