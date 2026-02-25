namespace BackendSWP391.Application.Exceptions;

[Serializable]
public class BadRequestException(string message) : Exception(message);

