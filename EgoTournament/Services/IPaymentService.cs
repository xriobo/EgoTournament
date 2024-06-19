using EgoTournament.Models.Enums;
using EgoTournament.Models.Paypal;

namespace EgoTournament.Services
{
    public interface IPaymentService
    {
        Task<string> CreateOrderAsync(PaymentDto payment);

        Task<bool> CaptureOrderAsync(string orderId);
    }
}
