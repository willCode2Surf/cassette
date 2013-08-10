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

			RdioHelper.AuthorizeRequestCompleted += (RdioHelper.AuthorizeState state) => {
				if (state == RdioHelper.AuthorizeState.Authorized) {
					RdioHelper.GetHeavyRotationCoversCompleted += HandleGetHeavyRotationCoversCompleted;
					RdioHelper.Instance.GetHeavyRotationCovers ();
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
			RdioHelper.GetHeavyRotationCoversCompleted -= HandleGetHeavyRotationCoversCompleted;
			CoverCollection.DataSource = new CoverCollectionView.CoverDataSource (covers);
		}
	}
	
}
