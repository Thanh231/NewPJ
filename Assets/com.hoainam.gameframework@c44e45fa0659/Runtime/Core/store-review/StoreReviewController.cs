public class StoreReviewController : SingletonMonoBehaviour<StoreReviewController>
{
    #if UNITY_EDITOR || UNITY_STANDALONE
    private IStoreReview impl = new StoreReviewImpl_editor();
    #elif UNITY_ANDROID
    private IStoreReview impl = new StoreReviewImpl_android();
    #elif UNITY_IOS
    private IStoreReview impl = new StoreReviewImpl_ios();
    #endif
    
    public void Review()
    {
        impl.Review();
    }
}