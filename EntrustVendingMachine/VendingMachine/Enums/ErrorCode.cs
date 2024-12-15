namespace VendingMachine.Enums
{
    /// <summary>
    /// Represents error codes used to identify specific errors in the vending machine.
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// No error has occurred.
        /// </summary>
        None,

        /// <summary>
        /// The list of products provided is empty or null.
        /// </summary>
        ProductListEmpty,

        /// <summary>
        /// The list of coins provided is empty or null.
        /// </summary>
        CoinListEmpty,

        /// <summary>
        /// The specified product does not exist in the vending machine.
        /// </summary>
        ProductDoesNotExist,

        /// <summary>
        /// The specified product is out of stock.
        /// </summary>
        ProductOutOfStock,

        /// <summary>
        /// No product has been selected for purchase.
        /// </summary>
        NoProductSelected,

        /// <summary>
        /// The user has not inserted enough funds to purchase the selected product.
        /// </summary>
        InsufficientFunds,

        /// <summary>
        /// The machine cannot provide exact change for the transaction.
        /// </summary>
        CannotGiveChange,

        /// <summary>
        /// An error occurred while loading coins into the vending machine.
        /// </summary>
        CoinLoadingError,

        /// <summary>
        /// An error occurred while delivering the selected product.
        /// </summary>
        ProductDeliveryError,

        /// <summary>
        /// A general error occurred in the vending machine.
        /// </summary>
        MachineError,

        /// <summary>
        /// The specified coin type is invalid or unrecognized.
        /// </summary>
        InvalidCoinType
    }
}