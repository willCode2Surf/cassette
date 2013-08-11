using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Cassette
{

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{

		UIWindow window;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds) {
				RootViewController = new TopLevelController {
					ContentController = new ContentController (UIScreen.MainScreen.Bounds)
				}
			};
			window.MakeKeyAndVisible ();

			var loginScreenController = new LoginScreenController (UIScreen.MainScreen.Bounds);
			window.RootViewController.PresentViewController (loginScreenController, false, delegate{});

			return true;
		}
	}
}

