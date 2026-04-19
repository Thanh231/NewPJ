
public enum IAPPurchaseFailReason
{
    //from IAP plugin
    PurchasingUnavailable,
    ExistingPurchasePending,
    ProductUnavailable,
    SignatureInvalid,
    UserCancelled,
    PaymentDeclined,
    DuplicateTransaction,
    ValidationFailure,
    StoreNotConnected,
    PurchaseMissing,
    Unknown,

    //from our code
    NotInitializedController,
    ValidateReceiptFail,
    ErrorFromSamsungStore,
    UnknownFromPlugin
}
