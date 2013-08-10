using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Xamarin.Juice;

namespace Cassette
{

	public class Cover
	{
		public readonly string ImagePath;

		Lazy<UIImage> CoverImageLazy;

		public UIImage CoverImage {
			get {
				return CoverImageLazy.Value;
			}
		}

		public string Artist { get; private set; }

		public List<string> Tracks { get; private set; }

		public Cover (string imagePath, bool fromUrl, string artist, List<string> tracks)
		{
			ImagePath = imagePath;
			if (fromUrl) {
				CoverImageLazy = new Lazy<UIImage> (() => FromUrl (ImagePath));
			} else {
				CoverImageLazy = new Lazy<UIImage> (() => UIImage.FromFile (ImagePath));
			}

			Artist = artist;
			Tracks = tracks;

		}

	 	UIImage FromUrl (string uri)
		{
			using (var url = new NSUrl (uri))
				using (var data = NSData.FromUrl (url))
					return UIImage.LoadFromData (data);
		}
	}
	
}
