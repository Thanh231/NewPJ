
#if UNITY_EDITOR || UNITY_ANDROID

#if USE_IN_APP_REVIEW

using System.Collections;
using Google.Play.Review;

#endif

public class StoreReviewImpl_android : IStoreReview
{
    public void Review()
    {
#if USE_IN_APP_REVIEW
        StoreReviewController.instance.StartCoroutine(InAppReview());
#else
        NativeController.instance.OpenStorePage();
#endif
    }

#if USE_IN_APP_REVIEW
    
    // Google doesn't specify how many times you can call the API,
    // but it is recommended to call it at most 3 times a year.
    // must upload to play store to test.
    
    // when in-app review popup is shown, it triggers OnApplicationPause() and OnApplicationFocus()
    private IEnumerator InAppReview()
    {
        var reviewManager = new ReviewManager();
        var requestFlowOperation = reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;

        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            NativeController.instance.OpenStorePage();
        }
        else
        {
            var playReviewInfo = requestFlowOperation.GetResult();
            var launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);
            yield return launchFlowOperation;

            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                NativeController.instance.OpenStorePage();
            }
        }
    }
    
#endif
}

#endif