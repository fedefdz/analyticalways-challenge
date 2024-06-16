using AcmeSchool.Domain.ValueObjects;

namespace AcmeSchool.Application.Services.PaymentGateway
{
    public abstract record PaymentRequest
    {
        public required Guid PaymentId { get; init; }
        public required PaymentMethod PaymentMethod { get; init; }
        public required decimal Amount { get; init; }
        public required DateTime PaymentDate { get; init; }
        public string? Description { get; init; }
    } 
}
