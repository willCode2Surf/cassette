using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Xamarin.Juice;

namespace Cassette
{
	public class ContentController : UIViewController
	{
		CoverCollectionView CoverCollection;
		PlayerViewController PlayerViewController;

		public ContentController (RectangleF frame)
		{
			View = new UIView (frame);

			PlayerViewController = new PlayerViewController (frame);
			PlayerViewController.Dismissed += () => {
				PlayerViewController.DismissViewController (false, delegate{});
			};

			CoverCollection = new CoverCollectionView (frame);
			CoverCollection.CoverTapped += CoverCollectionCoverTapped;

			View.AddSubview (CoverCollection);

			RdioClient.SharedClient.AuthorizeRequestCompleted += (state) => {
				if (state == AuthorizeState.Authorized) {
					RdioClient.SharedClient.GetHeavyRotationCoversCompleted += HandleGetHeavyRotationCoversCompleted;
					RdioClient.SharedClient.GetHeavyRotationCovers (start: 0, count: 50);
				}
			};
		}

		void CoverCollectionCoverTapped (Cover cover, UIView view)
		{
			var coverOrigin = CoverCollection.ConvertRectToView (view.Frame, View);
			var screen = View.GetImageRepresentation ();
			PlayerViewController.PrepareTransition (cover, coverOrigin, screen);
			PresentViewController (PlayerViewController, false, delegate {});
		}

		void HandleGetHeavyRotationCoversCompleted (List<Cover> covers)
		{
			RdioClient.SharedClient.GetHeavyRotationCoversCompleted -= HandleGetHeavyRotationCoversCompleted;
			CoverCollection.DataSource = new CoverCollectionView.CoverDataSource (covers);
		}
	}
	
}
