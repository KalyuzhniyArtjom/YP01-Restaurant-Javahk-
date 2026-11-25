using DjavaLib.Models;
using System.Collections.Generic;

namespace DjavaLib.Data
{
    public interface ICartRepository
    {
        List<CartItem> GetCartItems(int clientId);
        void AddToCart(int clientId, int dishId);
        void UpdateQuantity(int cartId, int quantity);
        void RemoveFromCart(int cartId);
        void ClearCart(int clientId);
        CartSummary GetCartSummary(int clientId);
    }
}