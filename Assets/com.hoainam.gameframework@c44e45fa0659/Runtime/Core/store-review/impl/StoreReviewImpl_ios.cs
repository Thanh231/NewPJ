
#if UNITY_EDITOR || UNITY_IOS

using UnityEngine.iOS;

public class StoreReviewImpl_ios : IStoreReview
{
    public void Review()
    {
#if USE_IN_APP_REVIEW
        InAppReview();
#else
        NativeController.instance.OpenStorePage();
#endif
    }

    // apple only allows display in-app review dialog 3 times per year
    // this popup won't show when tested in TestFlight
    // we can test it by directly deploying via Xcode or live build on App Store
    
    // when in-app review popup is shown, it doesn't trigger OnApplicationPause() and OnApplicationFocus()
    private void InAppReview()
    {
        var canInAppReview = Device.RequestStoreReview();
        if (!canInAppReview)
        {
            NativeController.instance.OpenStorePage();
        }
    }
}

#endif