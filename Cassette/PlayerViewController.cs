using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Xamarin.Juice;

namespace Cassette
{
	class PlayerViewController : UIViewController
	{
		BigCoverView CoverView;
		RectangleF CoverOrigin, CoverDestination;
		UIImageView ControlView;
		UIView BackgroundView;

		public event Action Dismissed = delegate {};

		public PlayerViewController (RectangleF frame)
		{
			View = new UIView (frame);

			BackgroundView = new UIView (frame);

			CoverDestination = new RectangleF (0, 0, frame.Width, frame.Width);
			CoverView = new BigCoverView (CoverDestination);
			CoverView.Tapped += _ => Dismiss ();

			ControlView = new UIImageView (UIImage.FromFile ("player.png"));

			View.AddSubviews (
				BackgroundView,
				CoverView,
				ControlView
			);
		}

		public void PrepareTransition (Cover cover, RectangleF coverOrigin, UIImage previousScreen)
		{
			CoverView.Image = cover.CoverImage;
			CoverOrigin = coverOrigin;
			BackgroundView.BackgroundColor = UIColor.FromPatternImage (previousScreen);
		}

		void Dismiss ()
		{
			UIView.Animate (0.2, () => {
				ControlView.Alpha = 0;
				CoverView.Frame = CoverOrigin;
				BackgroundView.Alpha = 1;
			}, () => {
				// Notify listeners so we can be fully dismissed
				Dismissed ();
			});
		}

		public override void ViewDidAppear (bool animated)
		{
			ControlView.Alpha = 0;
			CoverView.Frame = CoverOrigin;
			BackgroundView.Alpha = 1;

			UIView.Animate (0.2, () => {
				CoverView.Frame = CoverDestination;
				BackgroundView.Alpha = 0;
			}, () => {
				UIView.Animate (0.2, () => {
					ControlView.Alpha = 1;
				}, () => {
					base.ViewDidAppear (false);
				});
			});
		}
	}
	
}
