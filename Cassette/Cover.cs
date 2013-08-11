using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Xamarin.Juice;

namespace Cassette
{
	public class Cover
	{
		public readonly string ImageUrl;

		UIImage _coverImage;
		Task<UIImage> _coverImageTask;

		/// <summary>
		/// Gets the cover image. This blocks if the image isn't downloaded yet; please use GetCoverImageAsync.
		/// </summary>
		/// <value>The cover image.</value>
		public UIImage CoverImage {
			get {
				return _coverImage = _coverImage ?? FromUrl (ImageUrl);
			}
		}

		public string Artist { get; private set; }

		public List<string> Tracks { get; private set; }

		public Cover (string imageUrl, string artist, List<string> tracks)
		{
			ImageUrl = imageUrl;
			Artist = artist;
			Tracks = tracks;

			_coverImageTask = new Task<UIImage> (() => CoverImage);
		}

		public TaskStatus CoverImageStatus {
			get { return _coverImageTask.Status; }
		}

		public Task<UIImage> GetCoverImageAsync ()
		{
			if (_coverImageTask.Status == TaskStatus.Created)
				_coverImageTask.Start ();

			return _coverImageTask;
		}

	 	UIImage FromUrl (string uri)
		{
			using (var url = new NSUrl (uri))
				using (var data = NSData.FromUrl (url))
					return UIImage.LoadFromData (data);
		}
	}
	
}
