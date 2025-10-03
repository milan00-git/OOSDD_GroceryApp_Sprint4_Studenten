using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository, IProductRepository productRepository)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            return _groceriesRepository.Add(item);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id)
        {
            return _groceriesRepository.Get(id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            return _groceriesRepository.Update(item);
        }

        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)   
        {
            // Alle grocery list items ophalen
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();

            // Dictionary gebruike om de kwantiteit per product bij te houden
            Dictionary<int, int> productKwantiteit = new Dictionary<int, int>();

            for (int i = 0; i < groceryListItems.Count; i++)
            {
                int productId = groceryListItems[i].ProductId;
                int amount = groceryListItems[i].Amount;

                if (productKwantiteit.ContainsKey(productId))
                {
                    productKwantiteit[productId] += amount;
                }
                else
                {
                    productKwantiteit[productId] = amount;
                }
            }

            // Top 5 producten vinden bij kwantiteit
            List<BestSellingProducts> BestVerkochteProducten = new List<BestSellingProducts>();
            for (int j = 0; j < topX; j++)
            {
                // Startwaarde zetten, er zijn geen producten in de lijst 'bestVerkochteProducten'
                int maxProductId = -1;
                int maxAmount = -1;

                // Product vinden met de max kwantiteit
                foreach (var paar in productKwantiteit)
                {
                    if (paar.Value > maxAmount)
                    {
                        maxAmount = paar.Value;
                        maxProductId = paar.Key;
                    }
                }

                // Geen producten meer
                if (maxProductId == -1) break;

                // Product details ophalen
                var product = _productRepository.Get(maxProductId);
                if (product != null)
                {
                    // j + 1 is de ranking
                    BestVerkochteProducten.Add(new BestSellingProducts(product.Id, product.Name, product.Stock, maxAmount, j + 1));
                }

                // Gevonden product verwijderen voor de volgende iteratie om de volgende beste te vinden
                // (mocht er een nieuw product komen).
                productKwantiteit.Remove(maxProductId);
            }

            return BestVerkochteProducten;
        }

        private void FillService(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "", 0);
            }
        }
    }
}
