using StocksApi.DTO;
using StocksApi.Models;

namespace StocksApi.IServiceContracts
{
    public interface IBuyOrderService
    {
        Task<List<BuyOrderResponse>> GetBuyOrders(Guid userId);
        Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest buyOrder,Guid userId);
   
    }
}
