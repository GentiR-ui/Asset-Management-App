namespace AssetManagementSystem.Application.Exceptions;

public sealed class BadRequestException(string message) : Exception(message);
public sealed class NotFoundException(string message) : Exception(message);
public sealed class ConflictException(string message) : Exception(message);
public sealed class UnauthorizedException(string message) : Exception(message);
public sealed class ForbiddenException(string message) : Exception(message);

public sealed class ValidationException(IReadOnlyDictionary<string, string[]> errors)
    : Exception("One or more validation errors occurred.")
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}
