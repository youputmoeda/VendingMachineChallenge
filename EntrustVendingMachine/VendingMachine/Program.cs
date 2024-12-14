using EntrustVendingMachine.Enums;
using EntrustVendingMachine.Extensions;
using EntrustVendingMachine.Models;
using EntrustVendingMachine.Services;
using EntrustVendingMachine.Utilities;

VendingMachineService vendingMachine = new VendingMachineService();

List<TypesOfCoin> coins = Enum.GetValues(typeof(TypesOfCoin)).Cast<TypesOfCoin>().ToList();

// Carregar produtos
Result<int>? productResult = vendingMachine.LoadProductsOnMachine(
[
    new("Soda", 1.50m, 10),
    new("Chips", 1.00m, 5),
    new("Candy", 0.75m, 20)
]);

if (!productResult.IsSuccess)
{
    Console.WriteLine($"Error loading products: {productResult.Error}");
    return;
}

Console.WriteLine($"Successfully loaded {productResult.Value} products.");

// Carregar moedas
var coinResult = vendingMachine.LoadCoinsOnMachine(
[
    new(TypesOfCoin.OnePound, 10),
    new(TypesOfCoin.FiftyPence, 20),
    new(TypesOfCoin.TwentyPence, 30)
]);

if (!coinResult.IsSuccess)
{
    Console.WriteLine($"Error loading coins: {coinResult.Error}");
    return;
}
Console.WriteLine($"Successfully loaded {coinResult.Value} coins.");

while (true)
{
    Console.WriteLine("\n--- Vending Machine Menu ---");
    Console.WriteLine("1. Buy a Product");
    Console.WriteLine("2. Reload Products");
    Console.WriteLine("3. Reload Coins");
    Console.WriteLine("4. View Machine Status");
    Console.WriteLine("5. Exit");
    Console.Write("Choose an option: ");

    string choice = Console.ReadLine()?.Trim();

    switch (choice)
    {
        case "1": // Comprar Produto
            BuyProduct(vendingMachine, coins);
            break;
        case "2": // Recarregar Produtos
            ReloadProducts(vendingMachine);
            break;
        case "3": // Recarregar Moedas
            ReloadCoins(vendingMachine);
            break;
        case "4": // Ver Status da Máquina
            ViewMachineStatus(vendingMachine);
            break;
        case "5": // Sair
            Console.WriteLine("Thank you for using the vending machine!");
            return;
        default:
            Console.WriteLine("Invalid option. Please choose a valid menu option.");
            break;
    }
}

void BuyProduct(VendingMachineService vendingMachine, List<TypesOfCoin> coins)
{
    Console.WriteLine("Select a product (or type 'exit' to return to menu):");
    vendingMachine.ProductStatus().ForEach(Console.WriteLine);
    string productName = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(productName) || productName.ToLower() == "exit")
        return;

    Result<Product> selectResult = vendingMachine.SelectProduct(productName);
    if (!selectResult.IsSuccess)
    {
        Console.WriteLine(selectResult.Error);
        return;
    }

    bool moneyIsAllInserted = false;
    while (!moneyIsAllInserted)
    {
        Console.WriteLine("Insert one of these coins:");
        for (int index = 0; index < coins.Count; index++)
            Console.WriteLine("{0}). {1}", index, coins[index].GetName());

        string userInput = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(userInput))
        {
            Console.WriteLine("Invalid input. Please enter a coin index or 'exit'.");
            continue;
        }
        else if (userInput.ToLower() == "exit")
        {
            Console.WriteLine("Returning all inserted coins...");
            List<TypesOfCoin> returnedCoins = vendingMachine.ReturnInsertedCoins();
            Console.WriteLine("Coins returned: " + string.Join(", ", returnedCoins.Select(c => c.GetName())));
            return;
        }
        else if (int.TryParse(userInput, out int selectedIndex) && selectedIndex >= 0 && selectedIndex < coins.Count)
        {
            Result<decimal> insertResult = vendingMachine.InsertMoney(coins[selectedIndex]);
            if (!insertResult.IsSuccess)
            {
                Console.WriteLine(insertResult.Error);
                return;
            }
            moneyIsAllInserted = insertResult.Value >= selectResult.Value.Price;
        }
        else
        {
            Console.WriteLine("Invalid input. Please select a valid coin index.");
        }
    }

    Console.WriteLine("Enjoy your product!");
}

void ReloadProducts(VendingMachineService vendingMachine)
{
    Console.WriteLine("Enter product details (or type 'done' to finish):");
    while (true)
    {
        Console.Write("Product name: ");
        string name = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(name) || name.ToLower() == "done")
            break;

        Console.Write("Price (e.g., 1.50): ");
        if (!decimal.TryParse(Console.ReadLine()?.Trim(), out decimal price) || price <= 0)
        {
            Console.WriteLine("Invalid price. Try again.");
            continue;
        }

        Console.Write("Stock quantity: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out int stock) || stock <= 0)
        {
            Console.WriteLine("Invalid quantity. Try again.");
            continue;
        }

        Result<int> result = vendingMachine.LoadProductsOnMachine(new List<Product> { new(name, price, stock) });
        if (!result.IsSuccess)
        {
            Console.WriteLine($"Error adding product: {result.Error}");
        }
        else
        {
            Console.WriteLine($"Added {stock} units of '{name}' at £{price:F2}.");
        }
    }
}

void ReloadCoins(VendingMachineService vendingMachine)
{
    Console.WriteLine("Enter coin details (or type 'done' to finish):");
    while (true)
    {
        Console.WriteLine("Select a coin type:");
        for (int index = 0; index < coins.Count; index++)
            Console.WriteLine("{0}). {1}", index, coins[index].GetName());

        string userInput = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(userInput) || userInput.ToLower() == "done")
            break;

        if (!int.TryParse(userInput, out int selectedIndex) || selectedIndex < 0 || selectedIndex >= coins.Count)
        {
            Console.WriteLine("Invalid coin type. Try again.");
            continue;
        }

        Console.Write("Quantity to add: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out int quantity) || quantity <= 0)
        {
            Console.WriteLine("Invalid quantity. Try again.");
            continue;
        }

        var result = vendingMachine.LoadCoinsOnMachine(new List<Coin> { new(coins[selectedIndex], quantity) });
        if (!result.IsSuccess)
        {
            Console.WriteLine($"Error adding coins: {result.Error}");
        }
        else
        {
            Console.WriteLine($"Added {quantity} coins of type '{coins[selectedIndex].GetName()}'.");
        }
    }
}

void ViewMachineStatus(VendingMachineService vendingMachine)
{
    Console.WriteLine("\nAvailable products:");
    foreach (var productLine in vendingMachine.ProductStatus())
    {
        Console.WriteLine(productLine);
    }

    Console.WriteLine("\nAvailable coins:");
    foreach (var coinLine in vendingMachine.CoinsStatus())
    {
        Console.WriteLine(coinLine);
    }
}
