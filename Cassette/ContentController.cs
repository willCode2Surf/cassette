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
		
		BigCoverView BigCoverView;
		UIView CoverCollectionSelectedView;

		PlayerViewController PlayerViewController;

		public ContentController (RectangleF frame)
		{
			View = new UIView (frame);

			PlayerViewController = new PlayerViewController (frame);
			PlayerViewController.Dismissed += () => {
				PlayerViewController.DismissViewController (false, () => {
					TransitionFromBigCoverBackToCoverCollection ();
				});
			};

			BigCoverView = new BigCoverView ();
			View.AddSubview (CoverCollection = new CoverCollectionView (frame));

			CoverCollection.CoverTapped += CoverCollectionCoverTapped;
		}

		void TransitionFromBigCoverBackToCoverCollection ()
		{
			UIView.Animate (0.2, 0.1, UIViewAnimationOptions.CurveEaseOut, () => {
				BigCoverView.Frame = CoverCollectionSelectedViewFrame;
				CoverCollection.Alpha = 1;
			}, () => {
				BigCoverView.RemoveFromSuperview ();
				CoverCollectionSelectedView.Alpha = 1;
			});
		}

		RectangleF CoverCollectionSelectedViewFrame {
			get {
				return CoverCollection.ConvertRectToView (CoverCollectionSelectedView.Frame, View);
			}
		}

		void CoverCollectionCoverTapped (Cover cover, UIView view)
		{
			CoverCollectionSelectedView = view;
			BigCoverView.Frame = CoverCollectionSelectedViewFrame;
			BigCoverView.Image = cover.CoverImage;

			PlayerViewController.Cover = cover;

			View.AddSubview (BigCoverView);

			CoverCollectionSelectedView.Alpha = 0;

			UIView.Animate (0.2, () => {
				CoverCollection.Alpha = 0;
				BigCoverView.Frame = new RectangleF (0, 0, View.Frame.Width, View.Frame.Width);
			}, () => {
				PresentViewController (PlayerViewController, false, delegate {});
			});
		}
	}
	
}
