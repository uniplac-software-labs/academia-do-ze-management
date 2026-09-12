using System;

namespace AcademiaDoZe.Infrastructure.Exceptions;

public class InfrastructureException : Exception
{
    public string ErrorCode { get; }

    public InfrastructureException(string message) : base(message) => ErrorCode = "ERRO_INFRAESTRUTURA";
    public InfrastructureException(string message, Exception innerException) : base(message, innerException) => ErrorCode = "ERRO_INFRAESTRUTURA";
    public InfrastructureException(string errorCode, string message) : base(message) => ErrorCode = errorCode;
    public InfrastructureException(string errorCode, string message, Exception innerException) : base(message, innerException) => ErrorCode = errorCode;
}