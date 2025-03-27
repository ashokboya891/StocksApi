using StocksApi.DTO;

namespace StocksApi.IServiceContracts
{
    public interface ISellOrderService
    {
        Task<List<SellOrderResponse>> GetSellOrders(Guid userId);
        Task<SellOrderResponse> CreateSellOrder(SellOrderRequest buyOrder,Guid userId);
    }
}
