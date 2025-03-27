using StocksApi.DTO;
using StocksApi.Models;

namespace StocksApi.IRepositoryContracts
{
    public interface IStockRepository
    {

        Task<List<BuyOrder>> GetBuyOrders(Guid userId);

        Task<List<SellOrder>> GetSellOrders(Guid userId);

        Task<BuyOrder> CreateBuyOrder(BuyOrder buyOrder,Guid userID);

        Task<SellOrder> CreateSellOrder(SellOrder sellOrder, Guid userId);
    }
}
