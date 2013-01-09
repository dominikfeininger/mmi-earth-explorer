
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
        private readonly Pen trackedBonePen = new Pen(Brushes.Yellow, 6);

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

        // Create a new SpeechRecognitionEngine instance.
        SpeechRecognitionEngine sre = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("de-DE"));


        //gesture controller
        SuperController gestureController;

        //Sprachkontrolle
        Boolean speechEnabled;


        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            //Make sure to change this path to your index2.html file
            //Dominik
            //Browser.Navigate(new Uri("F:/VisualStudio12/KinectDev/EarthExplorer/googleEarthComponent/index2.html"));
            //Roy
            Browser.Navigate(new Uri("E:/Kinect/EarthExplorer/googleEarthComponent/index2.html"));
            //CN
            //Browser.Navigate(new Uri("C:/Users/n00b/Downloads/cs247-prototype/index2.html"));

            //from Stanford Project
            //Browser is the container name
            Keyboard.Focus(Browser);
        }

        //Speech!!
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
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "de-DE".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
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
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            //new gesture controller for google earth gestures
            gestureController = new SuperController(this);

            if (null == this.sensor)
            {

                // this.statusBarText.Text = Properties.Resources.NoKinectReady;
            }
            RecognizerInfo ri = GetKinectRecognizer();

            if (null != ri)
            {
                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                //Use this code to create grammar programmatically rather than from a grammar file.                
                var directions = new Choices();
                directions.Add(new SemanticResultValue("los", "START"));
                directions.Add(new SemanticResultValue("stop", "STOP"));
                directions.Add(new SemanticResultValue("zoom an", "ZOOMAN"));
                directions.Add(new SemanticResultValue("zoom aus", "ZOOMAUS"));
                directions.Add(new SemanticResultValue("bewegung an", "BEWEGUNG AN"));
                directions.Add(new SemanticResultValue("bewegungs aus", "BEWEGUNG AUS"));
                directions.Add(new SemanticResultValue("mannheim", "MANNHEIM"));
                directions.Add(new SemanticResultValue("new york", "NEW YORK"));
                directions.Add(new SemanticResultValue("frankfurt", "FRANKFURT"));
                directions.Add(new SemanticResultValue("madrid", "MADRID"));

                directions.Add(new SemanticResultValue("earth", "EARTH"));
                directions.Add(new SemanticResultValue("street", "STREET"));
                directions.Add(new SemanticResultValue("horizon", "HORIZONT"));
                directions.Add(new SemanticResultValue("vogel", "VOGEL"));
                directions.Add(new SemanticResultValue("hoch", "HOCH"));
                directions.Add(new SemanticResultValue("kippen", "KIPPEN"));
                directions.Add(new SemanticResultValue("sprache an", "SPRACHE AN"));
                directions.Add(new SemanticResultValue("sprache aus", "SPRACHE AUS"));
                directions.Add(new SemanticResultValue("test", "TEST"));

                var gb = new GrammarBuilder { Culture = ri.Culture };
                gb.Append(directions);
                var g = new Grammar(gb);

                speechEngine.LoadGrammar(g);

                speechEngine.SpeechRecognized += SpeechRecognized;
                speechEngine.SpeechRecognitionRejected += SpeechRejected;

                speechEngine.SetInputToAudioStream(
                    sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);

                speechEnabled = false;

                this.gestureMove.Text = "Move: No Skeleton found";
                this.gestureZoom.Text = "Zoom: No Skeleton found";
                this.speech.Text = "Speech: OFF";

                //this.kinectRuntime.NuiCamera.ElevationAngle = Convert.ToInt32(this.textAngel.Text);
                this.sensor.ElevationAngle = 11;

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


        //speech       

        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;

            //ClearRecognitionHighlights();
            if (speechEnabled)
            {
                this.speech.Text = "Speech: ON";
            }
            else
            {
                this.speech.Text = "Speech: OFF";
            }
            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "SPRACHE AN":
                        System.Console.WriteLine("Sprache AN");
                        speechEnabled = true;
                        break;
                    case "SPRACHE AUS":
                        System.Console.WriteLine("Sprache AUS");
                        speechEnabled = false;
                        break;
                }
            }

            if (e.Result.Confidence >= ConfidenceThreshold && speechEnabled)
            {
                
                this.speech.Text = this.speech.Text + " - " + e.Result.Semantics.Value.ToString();

                switch (e.Result.Semantics.Value.ToString())
                {
                    case "START":
                        System.Console.WriteLine("START");
                        System.Console.WriteLine("START");
                        System.Console.WriteLine("START");
                        gestureController.gestureRecognition(true);
                        break;

                    case "STOP":
                        System.Console.WriteLine("STOP");
                        System.Console.WriteLine("STOP");
                        System.Console.WriteLine("STOP");
                        gestureController.gestureRecognition(false);
                        break;
                    case "ZOOMAN":
                        System.Console.WriteLine("ZOOM AN");
                        gestureController.zoomRecognition(true);
                        break;
                    case "ZOOMAUS":
                        System.Console.WriteLine("ZOOM AUS");
                        gestureController.zoomRecognition(false);
                        break;
                    case "BEWEGUNG AN":
                        System.Console.WriteLine("BEWEGUNG AN");
                        gestureController.moveRecognition(true);
                        break;
                    case "BEWEGUNG AUS":
                        System.Console.WriteLine("BEWEGUNG AUS");
                        gestureController.moveRecognition(false);
                        break;
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
                    case "EARTH":
                        System.Console.WriteLine("Earth");
                        gestureController.goTo("earth");
                        break;
                    case "STREET":
                        System.Console.WriteLine("street");
                        gestureController.goTo("street");
                        break;
                    case "HORIZONT":
                        System.Console.WriteLine("horizont");
                        gestureController.goTo("horizont");
                        break;
                    case "VOGEL":
                        System.Console.WriteLine("vogel");
                        gestureController.goTo("vogel");
                        break;
                    case "HOCH":
                        System.Console.WriteLine("hoch");
                        gestureController.goTo("hoch");
                        break;
                    case "KIPPEN":
                        System.Console.WriteLine("kippen");
                        gestureController.goTo("kippen");
                        break;

                    case "TEST":
                        System.Console.WriteLine("test");
                        gestureController.goTo("test");
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
            //ClearRecognitionHighlights();
        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
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
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

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

        //Skeletzeichnung
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

        /// <summary>
        /// Handles the checking or unchecking of the seated mode combo box
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        /*private void CheckBoxSeatedModeChanged(object sender, RoutedEventArgs e)
        {
            /*
            if (null != this.sensor)
            {
                if (this.checkBoxSeatedMode.IsChecked.GetValueOrDefault())
                {
                    this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                    gestureController.setSeatedMode(true);
                }
                else
                {
                    this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                    gestureController.setSeatedMode(false);
                }
            }
            */
        //}
    }
}