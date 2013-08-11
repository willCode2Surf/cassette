using System;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit; 

using RdioSdk.iOS;

namespace Cassette
{
	public enum AuthorizeState
	{ 			
		Unknown,
		Authorized,
		Failed,
		Cancelled,
		LoggedOut
	}

	public sealed class RdioClient
	{
		public event Action<AuthorizeState> AuthorizeRequestCompleted = delegate{};
		public event Action<List<Cover>> GetHeavyRotationCoversCompleted = delegate{};

		RequestHandler heavyRotationHandler;

		static readonly RdioClient sharedClient = new RdioClient ();
		const string AccessTokenDefaultName = "rdioAccessToken";

		Rdio rdio;
		RdioDelegate rdioDelegate;

		AuthorizeState authorizeState = AuthorizeState.Unknown;

		public AuthorizeState AuthorizeState { 
			get {
				return authorizeState;
			}
			private set {
				authorizeState = value;
			}
		}

		RdioClient ()
		{
			rdioDelegate = new RdioDelegate ();
			rdioDelegate.RequestComplete += state => {
				AuthorizeState = state;
				AuthorizeRequestCompleted (state);
			};

			rdio = new Rdio ("45uk6pq7vbxdb2zunug9r7mu", "Vbe54ghj2N", rdioDelegate);

			heavyRotationHandler = new RequestHandler (ProcessHeavyRotationData);
		}

		void ProcessHeavyRotationData (NSObject data)
		{
			var covers = new List<Cover> ();
			var results = data as NSMutableArray;

			if (results != null) {
				for (uint i = 0; i < results.Count; i++) {
					using (var album = new NSDictionary(results.ValueAt (i))) {

						string artist = album.ObjectForKey ((NSString)"artist").ToString ();
						string bigIcon = album.ObjectForKey ((NSString)"bigIcon").ToString ();
						var tracks = new List<string> ();
						var tracksArray = album.ObjectForKey ((NSString)"trackKeys") as NSMutableArray;
						if (tracksArray != null) {
							foreach (var track in NSArray.FromArray <NSString> (tracksArray)) {
								tracks.Add (track);
							}
						}
						var cover = new Cover (bigIcon, artist, tracks);
						covers.Add (cover);
					}
				}
			}
			GetHeavyRotationCoversCompleted (covers);
		}

		public static RdioClient SharedClient {
			get { return sharedClient; }
		}

		public bool IsAuthorized {
			get { return AuthorizeState == AuthorizeState.Authorized; }
		}

		public string AuthToken {
			get { 
				return NSUserDefaults.StandardUserDefaults.StringForKey (AccessTokenDefaultName); 
			}
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
		}

		public void GetHeavyRotationCovers (int start = 0, int count = 12)
		{
			var values = new [] {
				rdio.User [(NSString) "key"],
				(NSString) "albums",
				(NSString) "false",
				(NSString) count.ToString (),
				(NSString) start.ToString (),
				(NSString) count.ToString (),
				(NSString) "bigIcon"
			};

			var keys = new [] {
				(NSString) "user",
				(NSString) "type",
				(NSString) "friends",
				(NSString) "limit",
				(NSString) "start",
				(NSString) "count",
				(NSString) "extras"
			};

			var args = NSDictionary.FromObjectsAndKeys (values, keys);

			rdio.CallApiMethod ("getHeavyRotation", args, heavyRotationHandler);
		}

		class RequestHandler : RequestDelegateHandlers
		{
			readonly Action<NSObject> LoadedData;
			readonly Action<NSError> Failed;

			public RequestHandler (Action<NSObject> loadedData, Action<NSError> failed = null) 
			{
				LoadedData = loadedData;
				Failed = failed ?? Console.Error.WriteLine;
			}

			public override void DidLoadData (Request request, NSObject data)
			{
				LoadedData (data);
			}

			public override void DidFail (Request request, NSError error)
			{
				Failed (error);
			}
		}

		class RdioDelegate : RdioSdk.iOS.RdioDelegate
		{
			public event Action<AuthorizeState> RequestComplete = delegate {};

			public override void DidAuthorizeUser (NSDictionary user, string accessToken)
			{
				NSUserDefaults.StandardUserDefaults.SetString (accessToken, AccessTokenDefaultName);
				NSUserDefaults.StandardUserDefaults.Synchronize ();
				RequestComplete (AuthorizeState.Authorized);
			}

			public override void AuthorizationFailed (string error)
			{
				RequestComplete (AuthorizeState.Failed);
			}

			public override void AuthorizationCancelled ()
			{
				RequestComplete (AuthorizeState.Cancelled);
			}

			public override void DidLogout ()
			{
				RequestComplete (AuthorizeState.LoggedOut);
			}
		}
	}
}