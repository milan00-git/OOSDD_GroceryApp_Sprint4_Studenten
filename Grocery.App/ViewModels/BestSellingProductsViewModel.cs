using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    public partial class BestSellingProductsViewModel : BaseViewModel
    {
        private readonly IGroceryListItemsService _groceryListItemsService;
        public ObservableCollection<BestSellingProducts> Products { get; set; }

        public BestSellingProductsViewModel(IGroceryListItemsService groceryListItemsService)
        {
            _groceryListItemsService = groceryListItemsService;
            Products = new ObservableCollection<BestSellingProducts>();
            Load();
        }

        public void Load()
        {
            Products.Clear();
            var bestSelling = _groceryListItemsService.GetBestSellingProducts(5);

            for (int i = 0; i < 5; i++)
            {
                if (i < bestSelling.Count)
                {
                    // Juiste ranking meegeven
                    var product = bestSelling[i];
                    product.Ranking = i + 1;
                    Products.Add(bestSelling[i]);
                }
                else
                {
                    Products.Add(new BestSellingProducts(
                        0,           // ProductId
                        "...",       // Name (placeholder)
                        0,           // Stock
                        0,           // NrOfSells
                        i + 1        // Ranking
                    ));
                }
            }
        }

        public override void OnAppearing()
        {
            Load();
        }

        public override void OnDisappearing()
        {
            Products.Clear();
        }
    }
}
