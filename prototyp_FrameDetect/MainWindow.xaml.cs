
namespace Microsoft.mmi.Kinect.Explorer
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using Microsoft.Speech.Recognition;
    using Microsoft.Speech.AudioFormat;
    using System.Windows.Documents;
    using System.Collections.Generic;
    //using System.Windows.Controls;
    //using System.Windows.Forms;
    //using mshtml;
    //using System.Drawing;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Green;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Green;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.White, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        /// <summary>
        /// Speech recognition engine using audio data from Kinect.
        /// </summary>
        private SpeechRecognitionEngine speechEngine;

        //private static String debugInputMessage;

        // Create a new SpeechRecognitionEngine instance.
        SpeechRecognitionEngine sre = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));


        //gesture controller
        SuperController gestureController;

        //Sprachkontrolle
        Boolean speechEnabled;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
       
        //TODO:
        public MainWindow()
        {
            InitializeComponent();

            //for local file usage
            //Make sure to change this path to YOUR index2.html file
            //Browser.Navigate(new Uri("F:/VisualStudio12/KinectDev/EarthExplorer_FrameDetect/sphere/keycanvas_2.html"));
            
            Browser.Navigate(new Uri("https://mmi-earth-explorer.googlecode.com/svn/trunk/webComponents/index.html"));
            //Browser.Navigate(new Uri("https://mmi-earth-explorer.googlecode.com/svn/trunk/webComponents/keycanvas_2.html"));
           
            /*
            WebBrowser wb = new WebBrowser();
            wb = Browser;
            var jobValue = wb.Document.GetElementById("");
            */
            mshtml.IHTMLDocument2 htmlDoc = Browser.Document as mshtml.IHTMLDocument2;
            // do something like find button and click
            //htmlDoc.all.item("zoomValue").SetAttribute("value", 4);
            //System.Console.WriteLine("htmlDoc.all.item: "+htmlDoc.all.item("zoomValue"));
            //System.Console.WriteLine("html fiel value: " + Browser.FindResource("zoomValue"));

            //Browser is the container name
            //Keyboard.Focus(Browser);
        }
        

        /// <title>
        /// Speech!!
        /// </title>
        /// <summary>
        /// Gets the metadata for the speech recognizer (acoustic model) most suitable to
        /// process audio from Kinect device.
        /// </summary>
        /// <returns>
        /// RecognizerInfo if found, <code>null</code> otherwise.
        /// </returns>
        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }
            return null;
        }



        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {

            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Image.Source = this.imageSource;

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();
                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;
                // Start the sensor!
                try
                {
                    this.sensor.Start();
                    speechconfig(sender, e);
                    //speechconfigMod(sender, e);

                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            //new gesture controller for google earth gestures
            gestureController = new SuperController(this);

            gestureController.gestureRecognition(false);

            initUI(sender, e);

            if (null == this.sensor)
            {
                this.noKinect.Text = "! NO KINECT FOUND !";
                this.zoomIn.Visibility = System.Windows.Visibility.Visible;
                this.up.Visibility = System.Windows.Visibility.Visible;
                // this.statusBarText.Text = Properties.Resources.NoKinectReady;
            }
            //System.Console.WriteLine("html fiel value: " + Browser);

            
            //.FindName("zoomValue")); //FindResource("zoomValue"));
        }

        private void kinectUp_Click(object sender, RoutedEventArgs e)
        {

            System.Console.WriteLine("angle "+ this.sensor.ElevationAngle);

            if (this.sensor != null && this.sensor.ElevationAngle < 26)
            {

                this.sensor.ElevationAngle +=2;
            }
            System.Console.WriteLine("kinect up");

        }

        private void kinectDown_Click(object sender, RoutedEventArgs e)
        {
           System.Console.WriteLine("angle "+ this.sensor.ElevationAngle);
            if (this.sensor != null && this.sensor.ElevationAngle > -26)
            {
                this.sensor.ElevationAngle -= 2;

            }
            System.Console.WriteLine("kinect down");
  
        }

        void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    if (sender.Equals(zoomSpeed))
                    {
                        //System.Console.WriteLine("zoomSpeed");
                        gestureController.zoomspeed = Convert.ToSingle(zoomSpeed.Text);
                    }
                    else if (sender.Equals(handMovingZTollerance))
                    {
                        gestureController.handMovingZTollerance = Convert.ToSingle(handMovingZTollerance.Text);
                    }
                    else if (sender.Equals(distanceX))
                    {
                        gestureController.distanceX = Convert.ToSingle(distanceX.Text);
                    }
                    else if (sender.Equals(tolleranceHandsSDiff))
                    {
                        gestureController.tolleranceHandsSDiff = Convert.ToSingle(tolleranceHandsSDiff.Text);
                    }
                    else if (sender.Equals(proportionParam))
                    {
                        gestureController.proportionParam = Convert.ToSingle(proportionParam.Text);
                    }
                    else if (sender.Equals(minMovementFrame))
                    {
                        gestureController.minMovementFrame = float.Parse(minMovementFrame.Text);
                    }
                    else
                    {
                        System.Console.WriteLine("");
                        /*
                        gestureController.handMovingZTollerance = float.Parse(handMovingZTollerance.Text);
                        gestureController.zoomspeed = float.Parse(zoomSpeed.Text);
                        gestureController.distanceX = float.Parse(distanceX.Text);
                        gestureController.tolleranceHandsSDiff = int.Parse(tolleranceHandsSDiff.Text);
                        gestureController.proportionParam = int.Parse(proportionParam.Text);
                        */
                    }
                }
                catch (Exception exc)
                {
                    System.Console.WriteLine("e" + exc);
                }
            
                //System.Console.WriteLine("moveSpeed Textfield");
                //debugInputMessage = moveSpeed.Text;
                //System.Console.WriteLine("tb_KeyDown moveSpeed debugInputMessage: " + debugInputMessage.ToString());
                //gestureController.handleDebugInput(debugInputMessage);
            }
        }

        /// <Title>
        /// Labels and Pictures
        /// </Title>
        /// <summary>
        /// initialize UI 
        /// </summary>
        private void initUI(object sender, RoutedEventArgs e)
        {
            this.PositionBody.Text = "Position: NOT correct";
            this.PositionBody.Foreground = System.Windows.Media.Brushes.Red;

            zoomSpeed.KeyDown += new KeyEventHandler(tb_KeyDown);
            minMovementFrame.KeyDown += new KeyEventHandler(tb_KeyDown);
            proportionParam.KeyDown += new KeyEventHandler(tb_KeyDown);
            //distanceX.KeyDown += new KeyEventHandler(tb_KeyDown);
            tolleranceHandsSDiff.KeyDown += new KeyEventHandler(tb_KeyDown);
            handMovingZTollerance.KeyDown += new KeyEventHandler(tb_KeyDown);
            
            //System.Console.WriteLine("moveSpeed debugInputMessage: " + debugInputMessage);

            this.gestureMove.Text = "Move: No Skeleton found";
            this.gestureMove.Foreground = System.Windows.Media.Brushes.Red;

            this.gestureZoom.Text = "Zoom: No Skeleton found";
            this.gestureZoom.Foreground = System.Windows.Media.Brushes.Red;

            this.gesturePerspective.Text = "Perspective: OFF";
            this.gesturePerspective.Foreground = System.Windows.Media.Brushes.Red;

            if (speechEnabled)
            {
                this.speech.Text = "Speech: ON";
                this.speech.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                this.speech.Text = "Speech: OFF";
                this.speech.Foreground = System.Windows.Media.Brushes.Red;
            }
            this.superman.Visibility = System.Windows.Visibility.Hidden;

            this.left.Visibility = System.Windows.Visibility.Hidden;
            this.right.Visibility = System.Windows.Visibility.Hidden;
            this.up.Visibility = System.Windows.Visibility.Hidden;
            this.down.Visibility = System.Windows.Visibility.Hidden;

            this.zoomIn.Visibility = System.Windows.Visibility.Hidden;
            this.zoomIn2x.Visibility = System.Windows.Visibility.Hidden;
            this.zoomOut.Visibility = System.Windows.Visibility.Hidden;
            this.zoomOut2x.Visibility = System.Windows.Visibility.Hidden;

            //Icon
            System.Drawing.Bitmap backgroundBitmap = Properties.Resources.icon;
            IntPtr hbitmap = backgroundBitmap.GetHbitmap();
            ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(backgroundBitmap.Width, backgroundBitmap.Height));
            this.icon.Background = new ImageBrush(imageSource);

            //Superman           
            backgroundBitmap = Properties.Resources.superman;
            hbitmap = backgroundBitmap.GetHbitmap();
            imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(backgroundBitmap.Width, backgroundBitmap.Height));
            this.superman.Background = new ImageBrush(imageSource);

            //Direction Arrows            
            backgroundBitmap = Properties.Resources.up;
            hbitmap = backgroundBitmap.GetHbitmap();
            imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(backgroundBitmap.Width, backgroundBitmap.Height));
            this.up.Background = new ImageBrush(imageSource);

            //Direction Arrows            
            backgroundBitmap = Properties.Resources.down;
            hbitmap = backgroundBitmap.GetHbitmap();
            imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(backgroundBitmap.Width, backgroundBitmap.Height));
            this.down.Background = new ImageBrush(imageSource);

            //Direction Arrows            
            backgroundBitmap = Properties.Resources.left;
            hbitmap = backgroundBitmap.GetHbitmap();
            imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(backgroundBitmap.Width, backgroundBitmap.Height));
            this.left.Background = new ImageBrush(imageSource);

            //Direction Arrows            
            backgroundBitmap = Properties.Resources.right;
            hbitmap = backgroundBitmap.GetHbitmap();
            imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(backgroundBitmap.Width, backgroundBitmap.Height));
            this.right.Background = new ImageBrush(imageSource);

            //Zoom Arrows            
            backgroundBitmap = Properties.Resources.zoomIn;
            hbitmap = backgroundBitmap.GetHbitmap();
            imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(backgroundBitmap.Width, backgroundBitmap.Height));
            this.zoomIn.Background = new ImageBrush(imageSource);

            //Zoom Arrows            
            backgroundBitmap = Properties.Resources.zoomIn2x;
            hbitmap = backgroundBitmap.GetHbitmap();
            imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(backgroundBitmap.Width, backgroundBitmap.Height));
            this.zoomIn2x.Background = new ImageBrush(imageSource);

            //Zoom Arrows            
            backgroundBitmap = Properties.Resources.zoomOut;
            hbitmap = backgroundBitmap.GetHbitmap();
            imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(backgroundBitmap.Width, backgroundBitmap.Height));
            this.zoomOut.Background = new ImageBrush(imageSource);

            //Zoom Arrows            
            backgroundBitmap = Properties.Resources.zoomOut2x;
            hbitmap = backgroundBitmap.GetHbitmap();
            imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(backgroundBitmap.Width, backgroundBitmap.Height));
            this.zoomOut2x.Background = new ImageBrush(imageSource);

        }

        private void OnKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            throw new NotImplementedException();
        }


       
        // </speechconfig TEST>
        private void speechconfig(object sender, RoutedEventArgs e)
        {
            //Speechrecognizer
            RecognizerInfo ri = GetKinectRecognizer();

            if (null != ri)
            {
                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                speechEnabled = false;

                //Use this code to create grammar programmatically rather than from a grammar file.                
                var directions = new Choices();

                //basic commands                
                directions.Add(new SemanticResultValue("start", "START"));
                directions.Add(new SemanticResultValue("stop", "STOP"));
                directions.Add(new SemanticResultValue("speech on", "SPEECH ON"));
                directions.Add(new SemanticResultValue("speech off", "SPEECH OFF"));

                directions.Add(new SemanticResultValue("perspective on", "PERSPECTIVE ON"));
                directions.Add(new SemanticResultValue("perspective off", "PERSPECTIVE OFF"));


                //cities
                directions.Add(new SemanticResultValue("mannheim", "MANNHEIM"));
                directions.Add(new SemanticResultValue("new york", "NEW YORK"));
                directions.Add(new SemanticResultValue("frankfurt", "FRANKFURT"));
                directions.Add(new SemanticResultValue("madrid", "MADRID"));

                // change mode
                directions.Add(new SemanticResultValue("earthview", "EARTH"));
                directions.Add(new SemanticResultValue("streetview", "STREET"));

                //adapt the perspective
                directions.Add(new SemanticResultValue("northern", "NORTHERN"));
                directions.Add(new SemanticResultValue("horizon", "HORIZON"));
                directions.Add(new SemanticResultValue("neutral", "NEUTRAL"));


                //for development
               // directions.Add(new SemanticResultValue("test", "TEST"));

                var gb = new GrammarBuilder { Culture = ri.Culture };
                gb.Append(directions);
                var g = new Grammar(gb);

                speechEngine.LoadGrammar(g);

                speechEngine.SpeechRecognized += SpeechRecognized;
                speechEngine.SpeechRecognitionRejected += SpeechRejected;

                speechEngine.SetInputToAudioStream(
                    sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);

                if (speechEnabled)
                {
                    this.speech.Text = "Speech: ON";
                    this.speech.Foreground = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    this.speech.Text = "Speech: OFF";
                    this.speech.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.AudioSource.Stop();
                this.sensor.Stop();
                this.sensor = null;
            }
            if (null != this.speechEngine)
            {
                this.speechEngine.SpeechRecognized -= SpeechRecognized;
                this.speechEngine.SpeechRecognitionRejected -= SpeechRejected;
                this.speechEngine.RecognizeAsyncStop();
            }
        }



        


        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;

            if (speechEnabled)
            {
                this.speech.Text = "Speech: ON";
                this.speech.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                this.speech.Text = "Speech: OFF";
                this.speech.Foreground = System.Windows.Media.Brushes.Red;
            }

            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "SPEECH ON":
                        speechEnabled = true;
                        this.speech.Text = "Speech: ON";
                        this.speech.Foreground = System.Windows.Media.Brushes.Green;
                        break;
                    case "SPEECH OFF":
                        speechEnabled = false;
                        this.speech.Text = "Speech: OFF";
                        this.speech.Foreground = System.Windows.Media.Brushes.Red;
                        break;
                    case "START":
                        gestureController.gestureRecognition(true);
                        gestureController.perspectiveRecognition(false);
                        break;
                    case "STOP":
                        gestureController.gestureRecognition(false);
                        gestureController.perspectiveRecognition(false);
                        break;
                }
            }

            if (e.Result.Confidence >= ConfidenceThreshold && speechEnabled)
            {
                this.speech.Text = this.speech.Text + " - " + e.Result.Semantics.Value.ToString();

                switch (e.Result.Semantics.Value.ToString())
                {

                    //Cities

                    case "FRANKFURT":
                        System.Console.WriteLine("FRANKFURT");
                        gestureController.goTo("frankfurt");
                        break;
                    case "MADRID":
                        System.Console.WriteLine("MADRID");
                        gestureController.goTo("madrid");
                        break;
                    case "MANNHEIM":
                        System.Console.WriteLine("MANNHEIM");
                        gestureController.goTo("mannheim");
                        break;
                    case "NEW YORK":
                        System.Console.WriteLine("NEW YORK");
                        gestureController.goTo("new york");
                        break;

                    //Perspektive
                    case "NORTHERN":
                        System.Console.WriteLine("norden");
                        gestureController.goTo("norden");
                        break;
                    case "EARTH":
                        System.Console.WriteLine("Earth");
                        gestureController.goTo("earth");
                        break;
                    case "STREET":
                        System.Console.WriteLine("street");
                        gestureController.goTo("street");
                        break;
                    case "HORIZON":
                        System.Console.WriteLine("horizont");
                        gestureController.goTo("horizont");
                        break;
                    case "NEUTRAL":
                        System.Console.WriteLine("neutral");
                        gestureController.goTo("neutral");
                        break;

                }
            }
        }

        /// <summary>
        /// Handler for rejected speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {

        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            //System.Console.WriteLine("SensorSkeletonFrameReady");
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            using (DrawingContext dc = this.drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    //Skeleton skel = skeletons[0];
                    foreach (Skeleton skel in skeletons)
                    {
                        RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.DrawBonesAndJoints(skel, dc);

                            Skeleton correctSkeleton = GetPrimarySkeleton(skeletons);

                            gestureController.processSkeletonFrame(correctSkeleton);

                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
                    }
                }
                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }

        /// <summary>
        /// for just one, the first skeleton
        /// </summary>
        /// <param name="skeletons"></param>
        /// <returns></returns>
        private static Skeleton GetPrimarySkeleton(IEnumerable<Skeleton> skeletons)
        {
            

            Skeleton primarySkeleton = null;
            foreach (Skeleton skeleton in skeletons)
            {
                if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
                {
                    continue;
                }

                if (primarySkeleton == null)
                    primarySkeleton = skeleton;
                else if (primarySkeleton.Position.Z > skeleton.Position.Z)
                    primarySkeleton = skeleton;
            }
            return primarySkeleton;
        }

        /// <title>
        /// Skeletzeichnung
        /// </title>
        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
            // Render Torso
            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            // Left Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

            // Right Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);


            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }
            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }

        private void tolleranceHandsSDiff_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}