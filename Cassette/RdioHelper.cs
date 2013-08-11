using System;
using RdioSdk.iOS;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MonoTouch.ObjCRuntime;


namespace Cassette
{
	public sealed class RdioHelper
	{
		public delegate void AuthorizeDelegate (AuthorizeState state);
		public static event AuthorizeDelegate AuthorizeRequestCompleted;
		public delegate void GetHeavyRotationCoversDelegate (List<Cover> covers);
		public static event GetHeavyRotationCoversDelegate GetHeavyRotationCoversCompleted;

		MyRequestDelegate getHeavyRotationResult;

		static readonly RdioHelper instance = new RdioHelper ();
		const string RDIO_TOKEN_KEY = "rdioAccessToken";

		Rdio rdio;
		MyRdioDelegate rdioDelegate;

		public enum AuthorizeState
		{ 			
			Unknown,
			Authorized,
			AuthorizeFailure,
			AuthorizeCancelled,
			LoggedOut
		};

		RdioHelper ()
		{
			rdioDelegate = new MyRdioDelegate ();
			rdio = new Rdio ("45uk6pq7vbxdb2zunug9r7mu", "Vbe54ghj2N", rdioDelegate);

			getHeavyRotationResult = new MyRequestDelegate ();
			getHeavyRotationResult.LoadedData += (o) => {

				var covers = new List<Cover>();
				var results = o as NSMutableArray;

				if (results != null) {
					for (uint i = 0; i < results.Count; i++) {
						using (var album = new NSDictionary(results.ValueAt (i))) {

							string artist = album.ObjectForKey(new NSString ("artist")).ToString ();
							string bigIcon = album.ObjectForKey(new NSString ("bigIcon")).ToString ();
							var tracks = new List<string>();
							var tracksArray = album.ObjectForKey(new NSString("trackKeys")) as NSMutableArray;
							if (tracksArray != null)
							{
								foreach (var track in NSArray.FromArray <NSString> (tracksArray)) {
									tracks.Add(track);
								}
							}

							var cover = new Cover (bigIcon, artist, tracks);
							covers.Add (cover);
						}
					}
				}

				if (GetHeavyRotationCoversCompleted != null) {
					GetHeavyRotationCoversCompleted(covers);
				}
			};
		}

		public static RdioHelper Instance {
			get {
				return instance; 
			}
		}

		public bool IsAuthorized {
			get { return rdioDelegate.authState == AuthorizeState.Authorized; }
		}

		public string AuthToken {
			get { return NSUserDefaults.StandardUserDefaults.StringForKey (RDIO_TOKEN_KEY); }
		}

		public void AuthorizeWithToken (string token)
		{
			rdio.AuthorizeUsingAccessToken (token);
		}

		public void AuthorizeFromController (UIViewController controller)
		{
			rdio.AuthorizeFromController (controller);
		}

		public void PlaySource (string src)
		{
			rdio.RdioPlayer.PlaySource (src);
			//rdio.RdioPlayer.PlaySource ("t2742133");
		}

		public void GetHeavyRotationCovers()
		{
			var values = new NSObject[]
			{
				NSObject.FromObject(rdio.User[NSObject.FromObject("key")]),
				NSObject.FromObject("albums"),
				NSObject.FromObject("false"),
				NSObject.FromObject("12"),
				NSObject.FromObject("1"),
				NSObject.FromObject("12"),
				NSObject.FromObject("bigIcon")
			};

			var keys = new NSObject[]
			{
				NSObject.FromObject("user"),
				NSObject.FromObject("type"),
				NSObject.FromObject("friends"),
				NSObject.FromObject("limit"),
				NSObject.FromObject("start"),
				NSObject.FromObject("count"),
				NSObject.FromObject("extras")
			};

			var args = NSDictionary.FromObjectsAndKeys (values, keys);

			rdio.CallApiMethod ("getHeavyRotation", args, getHeavyRotationResult);
		}

		class MyRequestDelegate : RequestDelegateHandlers
		{
			public delegate void DidLoadDataDelegate(NSObject data);
			public event DidLoadDataDelegate LoadedData;

			public override void DidLoadData (Request request, NSObject data)
			{
				if (LoadedData != null)
					LoadedData (data);
				//new UIAlertView("DidLoadData", "data" , null, "ok", null).Show(); 
			}

			public override void DidFail (Request request, NSError error)
			{
				new UIAlertView("DidFail", "Failed" , null, "ok", null).Show(); 
			}
		}


		class MyRdioDelegate : RdioDelegate
		{
			public AuthorizeState authState = AuthorizeState.Unknown;

			public override void DidAuthorizeUser (NSDictionary user, string accessToken)
			{
				NSUserDefaults.StandardUserDefaults.SetString(accessToken, RDIO_TOKEN_KEY);
				NSUserDefaults.StandardUserDefaults.Synchronize ();
				authState = AuthorizeState.Authorized;
				NotifyRequestCompleted ();
			}

			public override void AuthorizationFailed (string error)
			{
				authState = AuthorizeState.AuthorizeFailure;
				NotifyRequestCompleted ();
			}

			public override void AuthorizationCancelled ()
			{
				authState = AuthorizeState.AuthorizeCancelled;
				NotifyRequestCompleted ();
			}

			public override void DidLogout ()
			{
				authState = AuthorizeState.LoggedOut;
				NotifyRequestCompleted ();
			}

			void NotifyRequestCompleted()
			{
				if (AuthorizeRequestCompleted != null) {
					AuthorizeRequestCompleted (authState);
				}
			}
		}
	}


}