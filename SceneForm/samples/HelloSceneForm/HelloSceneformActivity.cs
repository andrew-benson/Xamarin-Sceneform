
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Widget;
using Google.AR.Core;
using Google.AR.Sceneform;
using Google.AR.Sceneform.Rendering;
using Google.AR.Sceneform.UX;
using System;


namespace HelloSceneForm
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Locked, Exported = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class HelloSceneformActivity : AppCompatActivity
    {
        ShapeFactory shapeFactory;
        private static double MIN_OPENGL_VERSION = 3.0;
        private ArFragment arFragment;
        private ModelRenderable andyRenderable;

        Material material;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //check to see if the device supports SceneForm
            if (!CheckIsSupportedDeviceOrFinish(this))
                return;

            //set the content
            SetContentView(Resource.Layout.activity_ux);

            //set the fragment
            arFragment = (ArFragment)SupportFragmentManager.FindFragmentById(Resource.Id.ux_fragment_test);

            /*
            //load and build the model
            ModelRenderable.InvokeBuilder().SetSource(this, Resource.Raw.andy ).Build((renderable) =>
            {
                andyRenderable = renderable;
            });
            */

            //MaterialFactory.MakeOpaqueWithColor(this, new Color(Android.Graphics.Color.Red)).ThenAccept(mat => { material = mat;  } );


            //add the event handler
            arFragment.TapArPlane += OnTapArPlane;


        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
            {
                Toast.MakeText(this, "Camera permission is needed to run this application", ToastLength.Long).Show();
                Finish();
            }
        }
        private void OnTapArPlane(object sender, BaseArFragment.TapArPlaneEventArgs e)
        {
            if (andyRenderable == null)
                return;

            // Create the Anchor.
            Anchor anchor = e.HitResult.CreateAnchor();
            AnchorNode anchorNode = new AnchorNode(anchor);
            anchorNode.SetParent(arFragment.ArSceneView.Scene);

            // Create the transformable andy and add it to the anchor.
            TransformableNode andy = new TransformableNode(arFragment.TransformationSystem);
            andy.SetParent(anchorNode);
            andy.Renderable = andyRenderable;
            andy.Select();

        }

        public static bool CheckIsSupportedDeviceOrFinish(Activity activity)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.N)
            {
                Toast.MakeText(activity, "Sceneform requires Android N or later", ToastLength.Long).Show();

                activity.Finish();

                return false;
            }

            var openglString = ((ActivityManager)activity.GetSystemService(Context.ActivityService)).DeviceConfigurationInfo.GlEsVersion;

            if (Double.Parse(openglString) < MIN_OPENGL_VERSION)
            {
                Toast.MakeText(activity, "Sceneform requires OpenGL ES 3.0 or later", ToastLength.Long).Show();

                return false;
            }

            return true;

        }


    }
}


