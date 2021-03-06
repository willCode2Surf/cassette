using System;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Xamarin.Juice;
using RdioSdk.iOS;

namespace Cassette
{	
	public class CoverCollectionView : UICollectionView
	{
		public static readonly NSString CoverCellId = (NSString) "CoverCellId";

		public event Action<Cover, UIView> CoverTapped = delegate {};

		public CoverCollectionView (RectangleF frame) : base (frame, new UICollectionViewFlowLayout ())
		{
			RegisterClassForCell (typeof(CoverCell), CoverCellId);

			//DataSource = new CoverDataSource ();
			var @delegate = new CoverLayoutDelegate ();
			Delegate = @delegate;

			@delegate.CoverCellSelected += (cover, view) => CoverTapped (cover, view);
			BackgroundColor = UIColor.Clear;
		}

		class CoverCell : UICollectionViewCell
		{
			Cover cover;
			UIImageView ImageView;
			UIActivityIndicatorView ActivityIndicator;

			public static readonly SizeF DefaultSize;

			static CoverCell ()
			{
				var size = UIScreen.MainScreen.Bounds.Width / 2;
				DefaultSize = new SizeF (size, size);
			}

			public CoverCell (IntPtr handle) : base (handle)
			{
				Frame = new RectangleF (PointF.Empty, DefaultSize);

				ImageView = new UIImageView (Frame);

				ActivityIndicator = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.WhiteLarge) {
					Center = Frame.Center (),
					HidesWhenStopped = true,
					AutoresizingMask = UIViewAutoresizing.FlexibleMargins
				};

				ContentView.AddSubviews (
					ImageView,
					ActivityIndicator
				);
			}

			async void BeginLoadCoverImage ()
			{
				ImageView.Alpha = 0;

				ActivityIndicator.StartAnimating ();
				ImageView.Image = await cover.GetCoverImageAsync ();
				ActivityIndicator.StopAnimating ();

				UIView.Animate (0.5, () => ImageView.Alpha = 1);
			}

			public Cover Cover {
				get { return cover; }
				set {
					cover = value;

					switch (cover.CoverImageStatus) {
					case TaskStatus.Created:
						BeginLoadCoverImage ();
						break;
					case TaskStatus.RanToCompletion:
						ImageView.Image = cover.CoverImage;
						break;
					default:
						// Do nothing.
						break;
					}
				}
			}
		}

		class CoverLayoutDelegate : UICollectionViewDelegateFlowLayout
		{
			public event Action<Cover, UIView> CoverCellSelected = delegate {};

			public override SizeF GetSizeForItem (UICollectionView view, UICollectionViewLayout layout, NSIndexPath path)
			{
				return CoverCell.DefaultSize;
			}

			public override float GetMinimumInteritemSpacingForSection (UICollectionView view, UICollectionViewLayout layout, int section)
			{
				return 0;
			}

			public override float GetMinimumLineSpacingForSection (UICollectionView view, UICollectionViewLayout layout, int section)
			{
				return 0;
			}

			public override void ItemSelected (UICollectionView view, NSIndexPath path)
			{
				var cell = (CoverCell) view.CellForItem (path);
				CoverCellSelected (cell.Cover, cell);
			}
		}

		public class CoverDataSource : UICollectionViewDataSource
		{
			const int TotalCovers = 24;

			//List<Cover> Covers =
			//	Enumerable
			//		.Range (0, TotalCovers)
			//		.Select (i => new Cover (string.Format ("covers/{0}.png", i)))
			//		.ToList ();
			public List<Cover> Covers { get; private set; }

			public CoverDataSource(List<Cover> covers)
			{
				Covers = covers;
			}

			public override int GetItemsCount (UICollectionView collectionView, int section)
			{
				return Covers.Count;
			}

			public override UICollectionViewCell GetCell (UICollectionView view, NSIndexPath path)
			{
				var cell = (CoverCell) view.DequeueReusableCell (CoverCellId, path);
				cell.Cover = Covers [path.Row];
				return cell;
			}
		}
	}
	
}
