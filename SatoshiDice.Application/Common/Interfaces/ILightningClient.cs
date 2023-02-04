using Grpc.Core;
using SatoshiDice.Domain.Enums;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Common.Interfaces
{
    public interface ILightningClient
    {
        Task<string> CreateInvoice(long satoshis, string message, UserType userType);
        Task<long> GetChannelOutboundBalance(UserType userType);
        Task<long> GetChannelInboundBalance(UserType userType);
        Task<long> GetWalletBalance(UserType userType);
        void SendLightning(string paymentRequest, UserType userType);
    }
}
