# FCMXamarinExample

I struggled to find out how to use Firebase Cloud Messaging for Push Notifications with Xamarin, both iOS and Android projects. 

With a lot of research and discussions in Xamarin Forums, I finally got it working and I would like to share my final implementation.


## Configuration Tips

Don’t forget to configure correctly your projects on in Firebase Console. 
You must put you package name (Android) and bundle ID (iOS) exactly the same everywhere you need to use it.  

### Android

Add the Firebase configuration file to your project, right click on it and enable the option Build Action > GoogleServicesJson , if this option doen’t appear, you should close and open your solution.

Firebase last version packages are not compatible with Xamarin.Forms last version. Till now, the only compatible version is 32.094.0-beta3.

In the AndroidManifext.xml file, put you package name where you find this example package name.

<permission android:name=“**com.mysamples.fcmxamarinexample**.permission.C2D_MESSAGE" android:protectionLevel="signature" />
	<uses-permission android:name=“**com.mysamples.fcmxamarinexample**.permission.C2D_MESSAGE" />

…

			<intent-filter>
				<action android:name="com.google.android.c2dm.intent.RECEIVE" />
				<action android:name="com.google.android.c2dm.intent.REGISTRATION" />
				<category android:name=“**com.mysamples.fcmxamarinexample**” />
			</intent-filter>

### iOS

Add the Firebase configuration file to your project, right click on it and enable the option Build Action > BudleResource.

Disable Incremental Builds in your project build settings. Otherwise, you’ll get some errors.

In info.plist file, enable Background Modes > Remote notifications.




