
using Core.Entities;
using Core.Interfaces;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        public Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            throw new System.NotImplementedException();
        }
    }
}
