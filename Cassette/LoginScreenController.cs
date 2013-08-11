using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;

namespace Cassette
{
	public class LoginScreenController : UIViewController
	{
		UIButton loginButton;

		public LoginScreenController (RectangleF frame)
		{
			View = new UIView (frame);
			View.AddSubview (new UIImageView (UIImage.FromFile ("purple.png")));

			float width = frame.Width;
			float delta = 20f; 
			float buttonWidth = width - (2 * delta);
			float buttonHeigth = 2 * delta;

			loginButton = UIButton.FromType (UIButtonType.RoundedRect);
			loginButton.Frame = new RectangleF (delta, delta, buttonWidth, buttonHeigth);
			loginButton.SetTitle ("Login to Rdio", UIControlState.Normal);

			loginButton.SetTitleColor (UIColor.Black, UIControlState.Normal);
			loginButton.TouchUpInside += (sender, e) => {
				RdioClient.SharedClient.AuthorizeFromController (this);
			};

			RdioClient.SharedClient.AuthorizeRequestCompleted += (state) => {
				switch (state) {
				case AuthorizeState.Authorized:
					PresentingViewController.DismissViewController (false, null);
					break;
				case AuthorizeState.Failed:
					new UIAlertView ("Authentication Failed", "Failed to login to Rdio", null, "ok", null).Show (); 
					this.View.AddSubview (loginButton);
					break;
				case AuthorizeState.Cancelled:
					new UIAlertView ("Authentication Cancelled", "You cancelled your authentication", null, "ok", null).Show (); 
					PresentingViewController.DismissViewController (false, null);
					break;
				default:
					break;
				}
			};
		}

		public override void ViewDidAppear (bool animated)
		{
			if (RdioClient.SharedClient.IsAuthorized) {
				// After the user authenticates with the Rdio login screen we're done
				PresentingViewController.DismissViewController (false, null);
			} else if (!string.IsNullOrEmpty (RdioClient.SharedClient.AuthToken)) {
				// On first launch if the user has previously authenticated
				RdioClient.SharedClient.AuthorizeWithToken (RdioClient.SharedClient.AuthToken);
			} else {
				// On first launch with no authentication stored 
				View.AddSubview (loginButton);
			}
		}
	}
}

