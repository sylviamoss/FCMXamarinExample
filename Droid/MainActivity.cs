using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using Firebase.Messaging;
using Firebase.Iid;
using Android.Util;

namespace FCMXamarinExample.Droid
{
	[Activity(Label = "FCMXamarinExample.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);

			if (Intent.Extras != null)
			{
				foreach (var key in Intent.Extras.KeySet())
				{
					var value = Intent.Extras.GetString(key);
					Log.Debug("MainActivity", "Key: {0} Value: {1}", key, value);
				}
			}

			Log.Debug("MainActivity", "InstanceID token: " + FirebaseInstanceId.Instance.Token);

			FirebaseMessaging.Instance.SubscribeToTopic("topicName");

			LoadApplication(new App());
		}
	}
}
