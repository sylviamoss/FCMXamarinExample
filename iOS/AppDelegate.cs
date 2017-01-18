using System;
using Firebase.CloudMessaging;
using Foundation;
using UIKit;
using UserNotifications;

namespace FCMXamarinExample.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IUNUserNotificationCenterDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();

			// Code for starting up the Xamarin Test Cloud Agent
#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start();
#endif

			LoadApplication(new App());

			//Firebase Cloud Messaging Configuration

			//Get permission for notification
			if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
			{
				// iOS 10
				var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
				UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) =>
				{
					Console.WriteLine(granted);
				});

				// For iOS 10 display notification (sent via APNS)
				UNUserNotificationCenter.Current.Delegate = this;

				Messaging.SharedInstance.RemoteMessageDelegate = this as IMessagingDelegate;

			}
			else {
				// iOS 9 <=
				var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
				var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
				UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
			}

			UIApplication.SharedApplication.RegisterForRemoteNotifications();

			Firebase.Analytics.App.Configure();

			Firebase.InstanceID.InstanceId.Notifications.ObserveTokenRefresh((sender, e) =>
			{
				var newToken = Firebase.InstanceID.InstanceId.SharedInstance.Token;
				System.Diagnostics.Debug.WriteLine(newToken);

				connectFCM();
			});

			return base.FinishedLaunching(app, options);
		}

		public override void DidEnterBackground(UIApplication uiApplication)
		{
			Messaging.SharedInstance.Disconnect();
		}

		public override void OnActivated(UIApplication uiApplication)
		{
			connectFCM();
			base.OnActivated(uiApplication);
		}

		public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
			Firebase.InstanceID.InstanceId.SharedInstance.SetApnsToken(deviceToken, Firebase.InstanceID.ApnsTokenType.Prod);
		}

		//Fire when background received notification is clicked
		public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
		{
			//Messaging.SharedInstance.AppDidReceiveMessage(userInfo);
			System.Diagnostics.Debug.WriteLine(userInfo);

			// Generate custom event
			NSString[] keys = { new NSString("Event_type") };
			NSObject[] values = { new NSString("Recieve_Notification") };
			var parameters = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(keys, values, keys.Length);

			// Send custom event
			Firebase.Analytics.Analytics.LogEvent("CustomEvent", parameters);

			if (application.ApplicationState == UIApplicationState.Active)
			{
				System.Diagnostics.Debug.WriteLine(userInfo);
				var aps_d = userInfo["aps"] as NSDictionary;
				var alert_d = aps_d["alert"] as NSDictionary;
				var body = alert_d["body"] as NSString;
				var title = alert_d["title"] as NSString;
				debugAlert(title, body);
			}
		}

		private void connectFCM()
		{
			Messaging.SharedInstance.Connect((error) =>
			{
				if (error == null)
				{
					Messaging.SharedInstance.Subscribe("/topics/topicName");
				}
				System.Diagnostics.Debug.WriteLine(error != null ? "error occured" : "connect success");
			});
		}

		private void debugAlert(string title, string message)
		{
			var alert = new UIAlertView(title ?? "Title", message ?? "Message", null, "Cancel", "OK");
			alert.Show();
		}
	}
}
