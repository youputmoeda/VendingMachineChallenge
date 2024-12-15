using EntrustVendingMachine.Enums;
using EntrustVendingMachine.Extensions;
using EntrustVendingMachine.Models;
using EntrustVendingMachine.Services;
using EntrustVendingMachine.Utilities;
using VendingMachine.Enums;

/// <summary>
/// Entry point for the Vending Machine application.
/// Provides a menu-driven console interface to interact with the vending machine.
/// </summary>
/// <remarks>
/// This program allows users to load predefined or custom products and coins,
/// purchase products, reload/unload inventory, and view machine status.
/// </remarks>

VendingMachineService vendingMachine = new VendingMachineService();

List<TypesOfCoin> coins = Enum.GetValues(typeof(TypesOfCoin)).Cast<TypesOfCoin>().ToList();

Console.WriteLine("Welcome to the Vending Machine!");
Console.WriteLine("Would you like to:");
Console.WriteLine("1. Load predefined products and coins");
Console.WriteLine("2. Enter products and coins manually");
Console.Write("Choose an option (1 or 2): ");

string setupChoice = Console.ReadLine()?.Trim();
if (setupChoice == "1")
{
    // Carregar produtos predefinidos
    var productResult = vendingMachine.LoadProductsOnMachine(new List<Product>
    {
        new("Soda", 1.50m, 10),
        new("Chips", 1.00m, 5),
        new("Candy", 0.75m, 20)
    });

    if (!productResult.IsSuccess)
    {
        Console.WriteLine($"Error loading products: {productResult.Error}");
        return;
    }
    Console.WriteLine($"Successfully loaded {productResult.Value} products.");

    // Carregar moedas predefinidas
    var coinResult = vendingMachine.LoadCoinsOnMachine(new List<Coin>
    {
        new(TypesOfCoin.OnePound, 10),
        new(TypesOfCoin.FiftyPence, 20),
        new(TypesOfCoin.TwentyPence, 30)
    });

    if (!coinResult.IsSuccess)
    {
        Console.WriteLine($"Error loading coins: {coinResult.Error}");
        return;
    }
    Console.WriteLine($"Successfully loaded {coinResult.Value} coins.");
}
else if (setupChoice == "2")
{
    Console.WriteLine("\n--- Load Products ---");
    ReloadProducts(vendingMachine);

    Console.WriteLine("\n--- Load Coins ---");
    ReloadCoins(vendingMachine);
}
else
{
    Console.WriteLine("Invalid option. Exiting...");
    return;
}

while (true)
{
    Console.WriteLine("\n--- Vending Machine Menu ---");
    Console.WriteLine("1. Buy a Product");
    Console.WriteLine("2. Reload Products");
    Console.WriteLine("3. Reload Coins");
    Console.WriteLine("4. Remove Products");
    Console.WriteLine("5. Remove Coins");
    Console.WriteLine("6. View Machine Status");
    Console.WriteLine("7. Exit");
    Console.Write("Choose an option: ");

    string choice = Console.ReadLine()?.Trim();

    switch (choice)
    {
        case "1":
            BuyProduct(vendingMachine, coins);
            break;
        case "2":
            ReloadProducts(vendingMachine);
            break;
        case "3":
            ReloadCoins(vendingMachine);
            break;
        case "4":
            UnloadProducts(vendingMachine);
            break;
        case "5":
            UnloadCoins(vendingMachine);
            break;
        case "6":
            ViewMachineStatus(vendingMachine);
            break;
        case "7":
            Console.WriteLine("Thank you for using the vending machine!");
            return;
        default:
            Console.WriteLine("Invalid option. Please choose a valid menu option.");
            break;
    }
}

/// <summary>
/// Handles the product purchase process by guiding the user through product selection,
/// coin insertion, and transaction completion.
/// </summary>
/// <param name="vendingMachine">The instance of the vending machine service to perform operations on.</param>
/// <param name="coins">A list of available coin types for the vending machine.</param>
void BuyProduct(VendingMachineService vendingMachine, List<TypesOfCoin> coins)
{
    Console.WriteLine("Select a product (or type 'exit' to return to menu):");
    var products = vendingMachine.ProductStatus(includeIndex: true, includeTotal: false);
    products.ForEach(Console.WriteLine);
    string userInput = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(userInput) || userInput.ToLower() == "exit")
        return;

    if (!int.TryParse(userInput, out int productIndex) || productIndex < 0 || productIndex >= products.Count)
    {
        Console.WriteLine("Invalid product index. Please try again.");
        return;
    }

    string productName = vendingMachine.GetProductNameByIndex(productIndex);
    Result<Product> selectResult = vendingMachine.SelectProduct(productName);
    if (!selectResult.IsSuccess)
    {
        Console.WriteLine(selectResult.Error);
        return;
    }

    Console.WriteLine($"You selected: {selectResult.Value.Name} - £{selectResult.Value.Price:F2}");

    bool moneyIsAllInserted = false;
    while (!moneyIsAllInserted)
    {
        Console.WriteLine("Insert one of these coins:");
        for (int index = 0; index < coins.Count; index++)
            Console.WriteLine("{0}) {1}", index, coins[index].GetName());

        userInput = Console.ReadLine()?.Trim();

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
            if (!insertResult.IsSuccess && insertResult.ErrorCode != ErrorCode.InsufficientFunds)
            {
                Console.WriteLine(insertResult.Error);
                return;
            }

            else if (insertResult.ErrorCode == ErrorCode.InsufficientFunds)
                Console.WriteLine(insertResult.Error);

            moneyIsAllInserted = insertResult.Value >= selectResult.Value.Price;

            if (!moneyIsAllInserted)
                Console.WriteLine(
                    $"Missing Amount: {selectResult.Value.Price - insertResult.Value}"
                );
        }
        else
        {
            Console.WriteLine("Invalid input. Please select a valid coin index.");
        }
    }

    Console.WriteLine($"Enjoy your {selectResult.Value.Name}!");
}

/// <summary>
/// Allows the user to reload products into the vending machine via the console.
/// </summary>
/// <param name="vendingMachine">The instance of the vending machine service to reload products.</param>
/// <remarks>
/// The user is prompted to input the product name, price, and stock quantity.
/// </remarks>
void ReloadProducts(VendingMachineService vendingMachine)
{
    Console.WriteLine("Enter product details (or type 'done' to finish):");
    while (true)
    {
        Console.Write("Product name: ");
        string name = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(name) || name.ToLower() == "done")
            break;

        Console.Write("Price (e.g., 1,50): ");
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

/// <summary>
/// Allows the user to reload coins into the vending machine via the console.
/// </summary>
/// <param name="vendingMachine">The instance of the vending machine service to reload coins.</param>
/// <remarks>
/// The user is prompted to select coin types and quantities to add to the machine.
/// </remarks>
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

/// <summary>
/// Allows the user to remove products from the vending machine.
/// </summary>
/// <param name="vendingMachine">The instance of the vending machine service to unload products from.</param>
/// <remarks>
/// If the user attempts to remove more stock than available, they are prompted
/// to confirm whether to remove all available stock.
/// </remarks>
void UnloadProducts(VendingMachineService vendingMachine)
{
    Console.WriteLine("Enter product details to remove (or type 'done' to finish):");
    while (true)
    {
        Console.Write("Product name: ");
        string name = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(name) || name.ToLower() == "done")
            break;

        Console.Write("Quantity to remove: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out int quantity) || quantity <= 0)
        {
            Console.WriteLine("Invalid quantity. Try again.");
            continue;
        }

        var result = vendingMachine.UnloadProductsFromMachine(new List<Product> { new(name, 0, quantity) });
        if (!result.IsSuccess)
        {
            Console.WriteLine($"Error removing product: {result.Error}");
        }
        else
        {
            Console.WriteLine($"Removed {result.Value} units of '{name}'.");
        }
    }
}

/// <summary>
/// Allows the user to remove coins from the vending machine.
/// </summary>
/// <param name="vendingMachine">The instance of the vending machine service to unload coins from.</param>
/// <remarks>
/// If the user attempts to remove more coins than available for a specific type,
/// they are prompted to confirm whether to remove all available coins.
/// </remarks>
void UnloadCoins(VendingMachineService vendingMachine)
{
    Console.WriteLine("Enter coin details to remove (or type 'done' to finish):");
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

        Console.Write("Quantity to remove: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out int quantity) || quantity <= 0)
        {
            Console.WriteLine("Invalid quantity. Try again.");
            continue;
        }

        var result = vendingMachine.UnloadCoinsFromMachine(new List<Coin> { new(coins[selectedIndex], quantity) });
        if (!result.IsSuccess)
        {
            Console.WriteLine($"Error removing coins: {result.Error}");
        }
        else
        {
            Console.WriteLine($"Removed {result.Value} coins of type '{coins[selectedIndex].GetName()}'.");
        }
    }
}


/// <summary>
/// Displays the current status of the vending machine, including available products and coins.
/// </summary>
/// <param name="vendingMachine">The instance of the vending machine service to retrieve status from.</param>
/// <remarks>
/// Product status includes name, price, stock, and total value. Coin status includes
/// type, quantity, and total money available in the machine.
/// </remarks>
void ViewMachineStatus(VendingMachineService vendingMachine)
{
    Console.WriteLine("\nAvailable products:");
    foreach (var productLine in vendingMachine.ProductStatus(includeIndex: false, includeTotal: true))
    {
        Console.WriteLine(productLine);
    }

    Console.WriteLine("\nAvailable coins:");
    foreach (var coinLine in vendingMachine.CoinsStatus())
    {
        Console.WriteLine(coinLine);
    }
}
