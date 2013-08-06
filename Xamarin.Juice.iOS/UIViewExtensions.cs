using System;

using MonoTouch.UIKit;

namespace Xamarin.Juice
{
	public static class UIViewExtensions
	{
		public static UIImage GetImageRepresentation (this UIView view)
		{
			UIImage image;

			UIGraphics.BeginImageContextWithOptions (view.Bounds.Size, view.Opaque, 0);
			view.Layer.RenderInContext (UIGraphics.GetCurrentContext ());
			image = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			return image;
		}
	}
}

