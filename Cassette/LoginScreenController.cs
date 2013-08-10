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

		public LoginScreenController ()
		{
			View = new UIView (UIScreen.MainScreen.Bounds);
			View.AddSubview (new UIImageView (UIImage.FromFile ("purple.png")));

			float width = UIScreen.MainScreen.ApplicationFrame.Width;
			float delta = 20f; 
			float buttonWidth = width - ( 2 * delta );
			float buttonHeigth = 2 * delta;

			loginButton = UIButton.FromType(UIButtonType.RoundedRect);
			loginButton.Frame = new RectangleF(delta, delta, buttonWidth, buttonHeigth);;
			loginButton.SetTitle("Login to Rdio", UIControlState.Normal);

			loginButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
			loginButton.TouchUpInside += (sender, e) => 
			{
				RdioHelper.Instance.AuthorizeFromController(this);
			};

			RdioHelper.AuthorizeRequestCompleted += (RdioHelper.AuthorizeState state) => {
				switch(state) 
				{
					case RdioHelper.AuthorizeState.Authorized:
					{	
						this.PresentingViewController.DismissViewController(false,null);
						break;
					}
						case RdioHelper.AuthorizeState.AuthorizeFailure:
					{
						new UIAlertView("Authentication Failed", "Failed to login to Rdio" , null, "ok", null).Show(); 
						this.View.AddSubview(loginButton);
						break;
					}
						case RdioHelper.AuthorizeState.AuthorizeCancelled:
					{
						new UIAlertView("Authentication Cancelled", "You cancelled your authentication", null, "ok", null).Show(); 
						this.PresentingViewController.DismissViewController(false,null);
						break;
					}
					default:
						break;
				}
			};
		}

		public override void ViewDidAppear (bool animated)
		{
			if (RdioHelper.Instance.IsAuthorized) {
				// After the user authenticates with the Rdio login screen we're done
				this.PresentingViewController.DismissViewController(false,null);
			} else if (!string.IsNullOrEmpty (RdioHelper.Instance.AuthToken)) {
				// On first launch if the user has previously authenticated
				RdioHelper.Instance.AuthorizeWithToken (RdioHelper.Instance.AuthToken);
			} else {
				// On first launch with no authentication stored 
				this.View.AddSubview(loginButton);
			}
		}
	}
}

