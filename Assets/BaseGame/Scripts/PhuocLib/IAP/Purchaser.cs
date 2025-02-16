using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using TW.Utility.DesignPattern;

public class Purchaser : Singleton<Purchaser>, IStoreListener
{
    #region Unity IAP
    private CrossPlatformValidator validator;
    private IStoreController m_Controller;
    private static IExtensionProvider m_StoreExtensionProvider;
    private IAppleExtensions m_AppleExtensions;
    private ITransactionHistoryExtensions m_TransactionHistoryExtensions;
    private IGooglePlayStoreExtensions m_GooglePlayStoreExtensions;

    private bool m_IsGooglePlayStoreSelected;
    private bool m_IsSamsungAppsStoreSelected;
    private bool m_IsCloudMoolahStoreSelected;
    private bool m_PurchaseInProgress;
    #endregion

    #region Product ID
    public const string starter_pack = "pack1";
    public const string no_ads = "noads";
    public const string super_no_ads = "snoads";

    public const string epic_item = "epicitem";

    public const string gem_1 = "gem1";
    public const string gem_2 = "gem2";
    public const string gem_3 = "gem3";
    public const string gem_4 = "gem4";
    public const string gem_5 = "gem5";
    public const string gem_6 = "gem6";

    #endregion

    private UnityAction m_OnBuySuccessCallback;
    private UnityAction m_OnBuyFailedCallback;

    protected override void Awake()
    { 
        InitializePurchasing();
    }
    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var module = StandardPurchasingModule.Instance();
        var builder = ConfigurationBuilder.Instance(module);
        Debug.Log("Platform " + Application.platform + " store " + module.appStore);

        string appIdentifier;
        appIdentifier = Application.identifier;
#if !UNITY_EDITOR
#endif
        builder.AddProduct(super_no_ads, ProductType.Consumable);

        builder.AddProduct(starter_pack, ProductType.Consumable);

        builder.AddProduct(no_ads, ProductType.NonConsumable);

        builder.AddProduct(epic_item, ProductType.NonConsumable);

        builder.AddProduct(gem_1, ProductType.NonConsumable);
        builder.AddProduct(gem_2, ProductType.NonConsumable);
        builder.AddProduct(gem_3, ProductType.NonConsumable);
        builder.AddProduct(gem_4, ProductType.NonConsumable);
        builder.AddProduct(gem_5, ProductType.NonConsumable);
        builder.AddProduct(gem_6, ProductType.NonConsumable);

        try
        {
            //validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), appIdentifier);
        }
        catch (Exception e)
        {

        }   

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }
    private bool IsInitialized()
    {
        return m_Controller != null && m_StoreExtensionProvider != null;
    }
    private void OnDeferred(Product item)
    {
        //Debug.Log("Purchase deferred: " + item.definition.id);
    }
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        //Debug.Log("OnInitialized: PASS");

        m_Controller = controller;
        m_StoreExtensionProvider = extensions;
        m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
        m_GooglePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();

        m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);
#if SUBSCRIPTION_MANAGER
        Dictionary<string, string> introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();
#endif
        Debug.Log("Available items:");
        foreach (var item in controller.products.all)
        {
            if (item.availableToPurchase)
            {
                //Debug.Log(string.Join(" - ",
                //    new[]
                //    {
                //                item.metadata.localizedTitle,
                //                item.metadata.localizedDescription,
                //                item.metadata.isoCurrencyCode,
                //                item.metadata.localizedPrice.ToString(),
                //                item.metadata.localizedPriceString,
                //                item.transactionID,
                //                item.receipt
                //    }));
#if INTERCEPT_PROMOTIONAL_PURCHASES
                        // Set all these products to be visible in the user's App Store according to Apple's Promotional IAP feature
                        // https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/StoreKitGuide/PromotingIn-AppPurchases/PromotingIn-AppPurchases.html
                        m_AppleExtensions.SetStorePromotionVisibility(item, AppleStorePromotionVisibility.Show);
#endif
                string productID = item.definition.id;
#if SUBSCRIPTION_MANAGER
                        bool isSubscribed = false;
                        // this is the usage of SubscriptionManager class
                        if(item.receipt != null) {
                            if(item.definition.type == ProductType.Subscription) {
                                if(checkIfProductIsAvailableForSubscriptionManager(item.receipt)) {
                                    string intro_json = (introductory_info_dict == null || !introductory_info_dict.ContainsKey(item.definition.storeSpecificId)) ? null : introductory_info_dict[item.definition.storeSpecificId];
                                    SubscriptionManager p = new SubscriptionManager(item, intro_json);
                                    SubscriptionInfo info = p.getSubscriptionInfo();
                                    Debug.Log("product id is: " + info.getProductId());
                                    Debug.Log("purchase date is: " + info.getPurchaseDate());
                                    Debug.Log("subscription next billing date is: " + info.getExpireDate());
                                    Debug.Log("is subscribed? " + info.isSubscribed().ToString());
                                    Debug.Log("is expired? " + info.isExpired().ToString());
                                    Debug.Log("is cancelled? " + info.isCancelled());
                                    Debug.Log("product is in free trial peroid? " + info.isFreeTrial());
                                    Debug.Log("product is auto renewing? " + info.isAutoRenewing());
                                    Debug.Log("subscription remaining valid time until next billing date is: " + info.getRemainingTime());
                                    Debug.Log("is this product in introductory price period? " + info.isIntroductoryPricePeriod());
                                    Debug.Log("the product introductory localized price is: " + info.getIntroductoryPrice());
                                    Debug.Log("the product introductory price period is: " + info.getIntroductoryPricePeriod());
                                    Debug.Log("the number of product introductory price period cycles is: " + info.getIntroductoryPricePeriodCycles());
                                    if(info.isSubscribed() == Result.True && info.isExpired() == Result.False) {
                                        isSubscribed = true;
                                    }
                                } else {
                                    Debug.Log("This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
                                }
                            } else {
                                Debug.Log("the product is not a subscription product");
                            }
                        } else {
                            Debug.Log("the product should have a valid receipt");
                        }
                        switch(productID) {
                            case kPremium_Weekly: {
                                    ProfileManager.MyProfile.SetPremium(PremiumMember.WEEK, isSubscribed);
                                }
                                break;
                            case kPremium_Monthly: {
                                    ProfileManager.MyProfile.SetPremium(PremiumMember.MONTH, isSubscribed);
                                }
                                break;
                            case kPremium_Quarterly: {
                                    ProfileManager.MyProfile.SetPremium(PremiumMember.QUARTER, isSubscribed);
                                }
                                break;
                        }
#endif
            }
        }
        LoadProductDetails(InAppPurchaseManager.Instance.IAPPackageList);
    }
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("Billing failed to initialize!");
        switch (error)
        {
            case InitializationFailureReason.AppNotKnown:
                Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
                break;
            case InitializationFailureReason.PurchasingUnavailable:
                // Ask the user if billing is disabled in device settings.
                Debug.Log("Billing disabled!");
                break;
            case InitializationFailureReason.NoProductsAvailable:
                // Developer configuration error; check product metadata.
                Debug.Log("No products available for purchase!");
                break;
        }
    }
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log("Billing failed to initialize!");
        switch (error)
        {
            case InitializationFailureReason.AppNotKnown:
                Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
                break;
            case InitializationFailureReason.PurchasingUnavailable:
                // Ask the user if billing is disabled in device settings.
                Debug.Log("Billing disabled!");
                break;
            case InitializationFailureReason.NoProductsAvailable:
                // Developer configuration error; check product metadata.
                Debug.Log("No products available for purchase!");
                break;
        }
    }
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        m_PurchaseInProgress = false;
        OnBuyIAPFail(product.definition.storeSpecificId);
    }
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        bool validPurchase = true; // Presume valid for platforms with no R.V.
        m_PurchaseInProgress = false;
        Debug.Log("Purchase success " + m_IsGooglePlayStoreSelected);
        if (m_IsGooglePlayStoreSelected ||
            Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer ||
            Application.platform == RuntimePlatform.tvOS)
        {
            try
            {
                Debug.Log("Receipt is valid. Contents:");
                var result = validator.Validate(args.purchasedProduct.receipt);
                foreach (IPurchaseReceipt productReceipt in result)
                {
                    Debug.Log(productReceipt.productID);
                    Debug.Log(productReceipt.purchaseDate);
                    Debug.Log(productReceipt.transactionID);

                    GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
                    if (null != google)
                    {
                        Debug.Log(google.purchaseState);
                        Debug.Log(google.purchaseToken);
                    }

                    AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
                    if (null != apple)
                    {
                        Debug.Log(apple.originalTransactionIdentifier);
                        Debug.Log(apple.subscriptionExpirationDate);
                        Debug.Log(apple.cancellationDate);
                        Debug.Log(apple.quantity);
                    }
                }
            }
            catch (IAPSecurityException ex)
            {
                Debug.Log("Invalid receipt, not unlocking content. " + ex);
                validPurchase = false;
            }
        }
        if (validPurchase)
        {
            Debug.Log("Receipt " + args.purchasedProduct.receipt);
            string productID = args.purchasedProduct.definition.id;
            OnBuyIAPSuccess(productID);
            decimal num = args.purchasedProduct.metadata.localizedPrice;
            string currencyCode = args.purchasedProduct.metadata.isoCurrencyCode;
            num = num * 0.63m;
        }

        //EventManager.TriggerEvent("TurnOffLoading");
        return PurchaseProcessingResult.Complete;
    }

    public void LoadProductDetails(List<IAPPackage> shopPackages)
    {
        //Debug.Log("Start load product detail.");
        for (int i = 0; i < shopPackages.Count; i++)
        {
            IAPPackage iPack = shopPackages[i];
            //this.LogWarning("LoadProductDetails " + iPack.productID);
            string s = iPack.productID;
            Product pd = m_Controller.products.WithID(s);
            if (pd != null)
            {
                string iPrice = pd.metadata.localizedPriceString;
                if (!string.IsNullOrEmpty(iPrice))
                {
                    iPack.localizedPriceString = iPrice;
                    iPack.localizedPrice = pd.metadata.localizedPrice;
                    iPack.currencyCode = pd.metadata.isoCurrencyCode;
                    iPack.isConnectedToStore = true;
                }
            }
        }
        Debug.LogWarning($"End load product detail.");
        Debug.LogWarning($"Details {shopPackages.Count}");
    }
    public void BuyIAPProduct(string productID, UnityAction buySuccessCallback, UnityAction buyFailCallback)
    {
        m_OnBuySuccessCallback = buySuccessCallback;
        m_OnBuyFailedCallback = buyFailCallback;
#if UNITY_EDITOR
        //OnBuyIAPSuccess(productID);
#else
        if (IsInitialized()) {
            if (m_PurchaseInProgress == true) return;
            Product product = m_Controller.products.WithID(productID);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase) {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_Controller.InitiatePurchase(product);
                m_PurchaseInProgress = true;
            } else {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                OnBuyIAPFail(productID);
            }
        } else {
            Debug.Log("BuyProductID FAIL. Not initialized.");
            OnBuyIAPFail(productID);
        }
#endif

    }
    public void OnBuyIAPSuccess(string productID)
    {
        if (m_OnBuySuccessCallback != null)
        {
            m_OnBuySuccessCallback();
        }
        InAppPurchaseManager.Instance.OnBuyIAPSuccess(productID);
        //EventManager.TriggerEvent("BuyIAPSuccess");
    }
    public void OnBuyIAPFail(string productID)
    {
        if (m_OnBuyFailedCallback != null)
        {
            m_OnBuyFailedCallback();
        }
        //EventManager.TriggerEvent("BuyIAPFail");
        //EventManager.TriggerEvent("TurnOffLoading");
    }
}