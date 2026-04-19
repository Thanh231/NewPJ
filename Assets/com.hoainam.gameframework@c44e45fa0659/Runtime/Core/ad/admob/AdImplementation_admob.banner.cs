
#if USE_ADMOB

using GoogleMobileAds.Api;

public partial class AdImplementation_admob
{
	private BannerView bannerView;

	private void LoadBanner()
	{
		var unitId = GetAdUnitID_banner();
		if (string.IsNullOrEmpty(unitId))
		{
			return;
		}

		if (bannerView != null)
		{
			bannerView.Destroy();
			bannerView = null;
		}

		bannerView = new BannerView(unitId, AdSize.Banner, AdPosition.Bottom);
		bannerView.LoadAd(new AdRequest());
	}

	public void HideBanner()
	{
		bannerView.Hide();
	}

	public void ShowBanner()
	{
		if (bannerView != null)
		{
			bannerView.Show();
		}
		else
		{
			LoadBanner();
		}
	}
}

#endif