using StocksApi.DTO;
using StocksApi.IRepositoryContracts;
using StocksApi.IServiceContracts;
using StocksApi.Models;
using StocksApi.Repositories;

namespace StocksApi.Services
{
    public class StockSellOrderServices:ISellOrderService
    {
        private readonly IStockRepository _repo;
        public StockSellOrderServices(IStockRepository stockRepository)
        {
            this._repo = stockRepository;
        }
        public async Task<SellOrderResponse> CreateSellOrder(SellOrderRequest sellOrderRequest,Guid userId)
        {
            if (sellOrderRequest == null)
                throw new ArgumentNullException(nameof(sellOrderRequest));

            //ValidationHelper.ModelValidation(sellOrderRequest);

            SellOrder sellOrder = sellOrderRequest.ToSellOrder();
            sellOrder.SellOrderID = Guid.NewGuid();
            sellOrder.UserID = userId; // Assign the user ID

            SellOrder SellOrderFromRepo = await _repo.CreateSellOrder(sellOrder, userId);
            return sellOrder.ToSellOrderResponse();
            //if (sellOrderRequest == null)
            //{
            //    throw new ArgumentNullException(nameof(sellOrderRequest));
            //}
            //SellOrder order = sellOrderRequest.ToSellOrder();
            //SellOrder resp = await _repo.CreateSellOrder(order);
            //return resp.ToSellOrderResponse();
        }

        public async Task<List<SellOrderResponse>> GetSellOrders(Guid userId)
        {
            List<SellOrder> sellOrders = await _repo.GetSellOrders(userId);
            return sellOrders.Select(temp => temp.ToSellOrderResponse()).ToList();
            //List<SellOrder> resp = await _repo.GetSellOrders();
            //return resp.Select(x => x.ToSellOrderResponse()).ToList();
        }
    }
}
