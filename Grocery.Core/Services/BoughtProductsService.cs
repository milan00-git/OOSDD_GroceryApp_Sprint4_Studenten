using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGroceryListRepository _groceryListRepository;
        public BoughtProductsService(IGroceryListItemsRepository groceryListItemsRepository, IGroceryListRepository groceryListRepository, IClientRepository clientRepository, IProductRepository productRepository)
        {
            _groceryListItemsRepository=groceryListItemsRepository;
            _groceryListRepository=groceryListRepository;
            _clientRepository=clientRepository;
            _productRepository=productRepository;
        }
        public List<BoughtProducts> Get(int? productId)
        {
            var result = new List<BoughtProducts>();    // Nieuwe lijst voor Client met: client, boodschappenlijst & product

            if (productId == null) return result;

            var product = GetProduct(productId.Value);
            if (product == null) return result;

            var items = GetFilteredItems(product.Id);
            var listIds = GetUniekeListIds(items);

            foreach (var listId in listIds)
            {
                var gekocht = MaakGekochteProducten(listId, product);
                if (gekocht != null)
                {
                    result.Add(gekocht);
                }
            }

            return result;
        }

        // Ophalen product
        private Product? GetProduct(int productId)
        {
            return _productRepository.Get(productId);
        }

        // Items filteren op hoeveelheid boven 0
        private List<GroceryListItem> GetFilteredItems(int productId)
        {
            var result = new List<GroceryListItem>();
            var allItems = _groceryListItemsRepository.GetAll();

            foreach (var item in allItems)
            {
                if (item.ProductId == productId && item.Amount > 0)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        // Unieke GroceryListIds ophalen
        private List<int> GetUniekeListIds(List<GroceryListItem> items)
        {
            var ids = new List<int>();
            foreach (var item in items)
            {
                if (!ids.Contains(item.GroceryListId))
                {
                    ids.Add(item.GroceryListId);
                }
            }

            return ids;
        }

        // Maakt een BoughtProducts object voor client en list
        private BoughtProducts? MaakGekochteProducten(int listId, Product product)
        {
            var list = _groceryListRepository.Get(listId);
            if (list == null) return null;

            var client = _clientRepository.Get(list.ClientId);
            if (client == null) return null;

            return new BoughtProducts(client, list, product);
        }
    }
}
