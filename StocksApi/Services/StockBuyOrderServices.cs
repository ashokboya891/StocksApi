using StocksApi.DTO;
using StocksApi.IRepositoryContracts;
using StocksApi.IServiceContracts;
using StocksApi.Models;

namespace StocksApi.Services
{
    public class StockBuyOrderServices : IBuyOrderService
    {
        private readonly IStockRepository _repo;
        public StockBuyOrderServices(IStockRepository stockRepository) 
        {
            this._repo= stockRepository;
        }
        public async Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest buyOrder,Guid userId)
        {
            if(buyOrder == null)
            {
                throw new ArgumentNullException(nameof(buyOrder));
            }
            BuyOrder order= buyOrder.ToBuyOrder();
            order.UserID= userId;

            BuyOrder resp =await   _repo.CreateBuyOrder(order, userId);
            return resp.ToBuyOrderResponse();
        }

        public async Task<List<BuyOrderResponse>> GetBuyOrders(Guid userId)
        {
           List<BuyOrder> resp=await _repo.GetBuyOrders(userId);
            return resp.Select(x => x.ToBuyOrderResponse()).ToList();
        }
    }
}
