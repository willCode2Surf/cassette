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
		UIImageView ControlView;

		public event Action Dismissed = delegate {};

		public PlayerViewController (RectangleF frame)
		{
			View = new UIView (frame);
			View.AddSubview (CoverView = new BigCoverView (new RectangleF (0, 0, frame.Width, frame.Width)));
			View.AddSubview (ControlView = new UIImageView (UIImage.FromFile ("player.png")));

			CoverView.Tapped += _ => Dismiss ();
		}

		void Dismiss ()
		{
			UIView.Animate (0.1, () => {
				ControlView.Alpha = 0;
			}, () => {
				// Notify listeners so we can be fully dismissed
				Dismissed ();
			});
		}

		public override void ViewDidAppear (bool animated)
		{
			ControlView.Alpha = 0;
			UIView.Animate (0.1, () => {
				ControlView.Alpha = 1;
			}, () => {
				base.ViewDidAppear (false);
			});
		}

		public Cover Cover {
			set {
				CoverView.Image = value.CoverImage;
			}
		}
	}
	
}
