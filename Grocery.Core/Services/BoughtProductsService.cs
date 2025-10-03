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

            // Een lege lijst geven als er geen product is
            if (productId == null)
            {
                return result;
            }

            var product = _productRepository.Get(productId.Value);
            if (product == null) return result;

            // Alle items ophalen
            var allItems = _groceryListItemsRepository.GetAll();

            // Kijken per product of de hoeveelheid > 0
            var gefilterdeItems = new List<GroceryListItem>();
            foreach (var item in allItems)
            {
                if (item.ProductId == product.Id && item.Amount > 0)
                {
                    gefilterdeItems.Add(item);
                }
            }

            var uniekeListIds = new List<int>();
            foreach (var item in gefilterdeItems)
            {
                if (!uniekeListIds.Contains(item.GroceryListId))
                {
                    uniekeListIds.Add(item.GroceryListId);
                }
            }

            foreach (var listId in uniekeListIds)
            {
                var list = _groceryListRepository.Get(listId);
                if (list == null) continue;

                var client = _clientRepository.Get(list.ClientId);
                if (client == null) continue;

                result.Add(new BoughtProducts(client, list, product));
            }

            return result;
        }
    }
}
