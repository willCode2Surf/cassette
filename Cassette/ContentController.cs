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
		readonly CoverCollectionView CoverCollection;

		UIView OriginalView;
		RectangleF OriginalFrame;
		BigCoverView CoverView;
		UIImageView ControlView;

		public ContentController (RectangleF frame)
		{
			View = new UIView (frame);

			CoverView = new BigCoverView ();
			ControlView = new UIImageView (UIImage.FromFile ("player.png"));
			View.AddSubview (CoverCollection = new CoverCollectionView (frame));

			CoverCollection.CoverTapped += CoverCollectionCoverTapped;
			CoverView.Tapped += _ => TransitionFromBigCoverBackToCoverCollection ();
		}

		void TransitionFromBigCoverBackToCoverCollection ()
		{
			UIView.Animate (0.1, () => {
				ControlView.Alpha = 0;
			}, () => {
				ControlView.RemoveFromSuperview ();
			});

			UIView.Animate (0.2, 0.1, UIViewAnimationOptions.CurveEaseOut, () => {
				CoverView.Frame = OriginalFrame;
				CoverCollection.Alpha = 1;
			}, () => {
				CoverView.RemoveFromSuperview ();
				OriginalView.Alpha = 1;
			});
		}

		void CoverCollectionCoverTapped (Cover cover, UIView view)
		{
			OriginalView = view;
			OriginalFrame = CoverCollection.ConvertRectToView (view.Frame, View);

			CoverView.Frame = OriginalFrame;
			CoverView.Image = cover.CoverImage;
		
			View.AddSubview (CoverView);

			OriginalView.Alpha = 0;

			UIView.Animate (0.2, () => {
				CoverCollection.Alpha = 0;
				CoverView.Frame = new RectangleF (0, 0, View.Frame.Width, View.Frame.Width);
			});

			ControlView.Alpha = 0;
			View.AddSubview (ControlView);
			UIView.Animate (0.1, 0.25, UIViewAnimationOptions.CurveLinear, () => {
				ControlView.Alpha = 1;
			}, () => {});
		}
	}
	
}
