namespace AssetManagementSystem.Application.Exceptions;

public class ConflictException(string message) : Exception(message)
{
}