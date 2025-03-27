using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using StocksApi.DatabaseContext;
using StocksApi.DTO;
using StocksApi.IRepositoryContracts;
using StocksApi.Models;

namespace StocksApi.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _db;
        public StockRepository(ApplicationDbContext con) 
        {
            _db = con;
        }
        public async Task<BuyOrder> CreateBuyOrder(BuyOrder buyOrder,Guid userId)
        {
            buyOrder.UserID = userId; // Assign the logged-in user
            var existingorder = _db.BuyOrders.FirstOrDefault(t => t.UserID == userId && t.StockSymbol == buyOrder.StockSymbol);
            if (existingorder != null)
            {
                existingorder.Quantity += buyOrder.Quantity;
                existingorder.Price += buyOrder.Price;
                _db.BuyOrders.Update(existingorder);
            }
            else
            {


                _db.BuyOrders.Add(buyOrder);
                await _db.SaveChangesAsync();
                return buyOrder;
            }
            _db.SaveChanges();
            return buyOrder;
            // if (buyOrder == null) throw new ArgumentNullException(" buyOrder data is null here..>!");
            // _context.BuyOrders.Add(buyOrder);
            //await _context.SaveChangesAsync();
            // return buyOrder;
        }

        public async  Task<SellOrder> CreateSellOrder(SellOrder sellOrder,Guid userId)
        {
            sellOrder.UserID = userId; // Assign the logged-in user

            // Check if the user has an existing sell order for the same stock
            var existingOrder = await _db.BuyOrders
                .FirstOrDefaultAsync(t => t.UserID == userId && t.StockSymbol == sellOrder.StockSymbol);
            if (existingOrder == null)
            {
                throw new InvalidOperationException("No mtaching record of this stock in your portofolio");
            }
            if (sellOrder.Quantity > existingOrder.Quantity)
            {
                throw new InvalidOperationException("Sell quantity exceeds available quantity.");
            }
            existingOrder.Quantity -= sellOrder.Quantity;
            if (existingOrder.Quantity == 0)
            {
                _db.BuyOrders.Remove(existingOrder);
            }
            else
            {
                _db.BuyOrders.Update(existingOrder);
            }
            // Add the new sell order
            _db.SellOrders.Add(sellOrder);

            await _db.SaveChangesAsync();
            return sellOrder;
            //if (sellOrder == null) throw new ArgumentNullException(" sellOrder data is null here..>!");
            //_context.SellOrders.Add(sellOrder);
            //await _context.SaveChangesAsync();
            //return sellOrder;
        }

        public async Task<List<BuyOrder>> GetBuyOrders(Guid userId)
        {
            // Get only the logged-in user's buy orders
            return await _db.BuyOrders
                .Where(order => order.UserID == userId) // Filter by user ID
                .OrderByDescending(order => order.DateAndTimeOfOrder)
                .ToListAsync();
            //return await _context.BuyOrders.ToListAsync();
        }

    

        public async Task<List<SellOrder>> GetSellOrders(Guid userId)
        {
            // Get only the logged-in user's sell orders
            return await _db.SellOrders
                .Where(order => order.UserID == userId) // Filter by user ID
                .OrderByDescending(order => order.DateAndTimeOfOrder)
                .ToListAsync();
            //return await _context.SellOrders.ToListAsync();
        }
    }
}
