using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using WindowsInput;
using System.Windows;




namespace Microsoft.mmi.Kinect.Explorer
{
    //contains all gestrues
    class SuperController
    {
        private MainWindow window;

        /// <summary>
        /// local variable to de/activate gestures
        /// </summary>
        private bool gesturesActive = true;

        /// <summary>
        /// local variable to de/activate the zoom
        /// </summary>
        private bool zoomActive = false;

        /// <summary>
        /// local variable to de/activate the move
        /// </summary>
        private bool moveActive = false;

        /// <summary>
        /// local variable to de/activate the perspectiveMode
        /// </summary>
        private bool perspectiveActive = false;
        private bool positionCorrect = false;

        private int CurrentSkeletonFrame = 0;
        //30frames per second
        private static int totalFrames = 120;//4sec
        //saves the skeletons
        private Skeleton[] SkeletonFrames = new Skeleton[totalFrames+1];
        //frame interval
        private int frameInterval = 2;


        private bool earthMode = true;
        private int frameCounter;

        public SuperController(MainWindow win)
        {
            window = win;
        }

        internal void gestureRecognition(bool mode)
        {
            this.gesturesActive = mode;

            // de/activate all with once but also be able to de/activate one by one
            this.zoomActive = mode;
            this.moveActive = mode;
            //this.perspectiveActive = !mode;
        }

        
        internal void zoomRecognition(bool mode)
        {
            this.zoomActive = mode;
            //this.perspectiveActive = !mode;

        }

        internal void moveRecognition(bool mode)
        {
            this.moveActive = mode;
            //this.perspectiveActive = !mode;

        }

        internal void perspectiveRecognition(bool mode)
        {
            this.perspectiveActive = mode;
           
        }


        // flyover to the destination city
        internal void goTo(string p)
        {
            if (p.Equals("frankfurt"))
            {
                window.Browser.InvokeScript("lookAtFrankfurt");
            }
            if (p.Equals("madrid"))
            {
                window.Browser.InvokeScript("lookAtMadrid");
            }
            if (p.Equals("mannheim"))
            {
                window.Browser.InvokeScript("lookAtMannheim");
            }
            if (p.Equals("new york"))
            {
                window.Browser.InvokeScript("lookAtNewYork");
            }
            if (p.Equals("earth"))
            {                
                window.Browser.InvokeScript("superman");            
            }
            if (p.Equals("street"))
            {
                window.Browser.InvokeScript("street");
            } if (p.Equals("horizont"))
            {
                window.Browser.InvokeScript("horizont");
            } if (p.Equals("neutral"))
            {
               
                window.Browser.InvokeScript("neutral");
               // InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_N);
            }
            //if (p.Equals("hoch"))
            //{
            //    window.Browser.InvokeScript("hoch");
            //} if (p.Equals("runter"))
            //{
            //    window.Browser.InvokeScript("runter");
            //}
            if (p.Equals("norden"))
            {
                InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_N);
            }
            //if (p.Equals("rechts"))
            //{
            //    window.Browser.InvokeScript("rechts");

            //} if (p.Equals("links"))
            //{
            //    window.Browser.InvokeScript("links");

            //}
            if (p.Equals("test"))
            {
                window.Browser.InvokeScript("test");
            }
        }

        //TODO:
        private void saveToSkeletonFrames(Skeleton skeleton)
        {
            //kSystem.Console.WriteLine("skeleton.Joints.Count: " + skeleton.Joints.Count);
            if (this.frameCounter % frameInterval == 0)
            {
                System.Console.WriteLine("this.CurrentSkeletonFrame: " + this.CurrentSkeletonFrame);
                this.SkeletonFrames[this.CurrentSkeletonFrame] = skeleton;

                if (this.CurrentSkeletonFrame == totalFrames)
                {
                    System.Console.WriteLine("totalFrames reached");
                    this.CurrentSkeletonFrame = 0;
                }
                else
                {
                    System.Console.WriteLine("totalFrames NOT reached");
                    this.CurrentSkeletonFrame++;
                }
            }
            this.frameCounter++;
        }

        //TODO:wird doppelt aufgerufen
        private void detectZoomByFrame(Joint current_wristHandR, Joint current_wristHandL, Joint current_head)
        {
            if (this.CurrentSkeletonFrame >= 2)
            {
                Skeleton skeleton1 = this.SkeletonFrames[this.CurrentSkeletonFrame - 1];
                Skeleton skeleton2 = this.SkeletonFrames[this.CurrentSkeletonFrame - 2];

                Joint skeleton1_wristHandR = skeleton1.Joints[JointType.WristRight];
                Joint skeleton1_wristHandL = skeleton1.Joints[JointType.WristLeft];

                Joint skeleton2_wristHandR = skeleton2.Joints[JointType.WristRight];
                Joint skeleton2_wristHandL = skeleton2.Joints[JointType.WristLeft];
                
                //System.Console.WriteLine("current_wristHandL.Position.X: " + current_wristHandL.Position.X);

                if (current_wristHandR.Position.X > skeleton2_wristHandR.Position.X)
                //if((skeleton1_wristHandR.Position.X > skeleton2_wristHandR.Position.X) && (current_wristHandR.Position.X > skeleton1_wristHandR.Position.X))
                //if((skeleton1_wristHandL.Position.X > skeleton2_wristHandL.Position.X) && (current_wristHandL.Position.X > skeleton1_wristHandL.Position.X))
                {
                    System.Console.WriteLine("rechte Hand bewegt sich nach Aussen");
                }


                //x = rechts/ links
                //y = hoch/ runter
                //z = vor/ zurück

                /*
                System.Console.WriteLine("skeleton1_wristHandL.Position.X: " + skeleton1_wristHandL.Position.X);
                System.Console.WriteLine("skeleton2_wristHandL.Position.X: " + skeleton2_wristHandL.Position.X);
                System.Console.WriteLine("current_wristHandL.Position.X: " + current_wristHandL.Position.X);
                 */
            }
        }
        
        internal void processSkeletonFrame(Skeleton skeleton)
        {

            Joint wristHandR = skeleton.Joints[JointType.WristRight];
            Joint wristHandL = skeleton.Joints[JointType.WristLeft];
            Joint head = skeleton.Joints[JointType.Head];
            Joint AnkleRight = skeleton.Joints[JointType.AnkleRight];
            Joint AnkleLeft = skeleton.Joints[JointType.AnkleLeft];
            Joint KneeRight = skeleton.Joints[JointType.KneeRight];
            Joint KneeLeft = skeleton.Joints[JointType.KneeLeft];
            Joint shoulderLeft = skeleton.Joints[JointType.ShoulderLeft];
            Joint shoulderRight = skeleton.Joints[JointType.ShoulderRight];
            Joint spine = skeleton.Joints[JointType.Spine];
            Joint FootLeft = skeleton.Joints[JointType.FootLeft];
            Joint FootRight = skeleton.Joints[JointType.FootRight];


            if (moveActive)
            {
                this.window.gestureMove.Text = "Move: ON";
                this.window.gestureMove.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                this.window.gestureMove.Text = "Move: OFF";
                this.window.gestureMove.Foreground = System.Windows.Media.Brushes.Red;

            }
            if (zoomActive)
            {
                this.window.gestureZoom.Text = "Zoom: ON";
                this.window.gestureZoom.Foreground = System.Windows.Media.Brushes.Green;

            }
            else
            {
                this.window.gestureZoom.Text = "Zoom: OFF";
                this.window.gestureZoom.Foreground = System.Windows.Media.Brushes.Red;

            }
            if (perspectiveActive)
            {
                this.window.gesturePerspective.Text = "Perspective: ON";
                this.window.gesturePerspective.Foreground = System.Windows.Media.Brushes.Green;

            }
            else
            {
                this.window.gesturePerspective.Text = "Perspective: OFF";
                this.window.gesturePerspective.Foreground = System.Windows.Media.Brushes.Red;

            }

            // System.Console.WriteLine("spine" + spine.Position.X);

            //positionCorrect = footDistanceCorrect(FootLeft, FootRight) && positionCenter(shoulderLeft, shoulderRight);
            positionCorrect = positionCenter(shoulderLeft, shoulderRight);
            //gestureRecognition(footDistanceCorrect(FootLeft, FootRight) && positionCenter(shoulderLeft, shoulderRight));

            /// if (gesturesActive && (zoomActive || moveActive || perspectiveActive))
            if (positionCorrect)
            {
                saveToSkeletonFrames(skeleton);
                detectZoomByFrame(wristHandR, wristHandL, head);
                //TODO:
                //checkContinuousState();

                if (zoomActive)
                {
                    zoomInOut2(wristHandL, wristHandR, spine);
                }
                if (moveActive)
                {
                    //saveToSkeletonFrames(skeleton.Joints);

                    moveTheMap(head, shoulderLeft, shoulderRight, spine, wristHandL, wristHandR);
                    
                    //TODO:
                    //detectStepForward(FootLeft, FootRight, KneeLeft, KneeRight);
                    //detectSupermanGeture(head, wristHandR, wristHandL);
                }

                if (perspectiveActive)
                {
                    tiltMapPerspective(spine, head, wristHandL, wristHandR, KneeLeft, KneeRight);
                    rotateMap(wristHandL, wristHandR, KneeLeft, KneeRight);
                }
            }

        }

        private void checkContinuousState()
        {

            if (!earthMode)
            {
                this.window.supermanTag.Text = "StreetView";
                this.window.superman.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.window.supermanTag.Text = "";
                this.window.superman.Visibility = System.Windows.Visibility.Hidden;
            }

            double altitude = (double)window.Browser.InvokeScript("getAltitude");


            if (altitude < 10)
            {
                earthMode = false;

            }
            else
            {
                earthMode = true;
            }

            if (earthMode && altitude < 50)
            {
                window.Browser.InvokeScript("streetAutomatic");
            }
        }

        private bool positionCenter(Joint shoulderLeft, Joint shoulderRight)
        {


            if (shoulderLeft.Position.X < 0 && shoulderRight.Position.X > 0)
            {
                this.window.PositionBody.Foreground = System.Windows.Media.Brushes.Green;
                this.window.PositionBody.Text = "Position: correct!";
                //gestureRecognition(true);

                return true;
            }
            this.window.PositionBody.Foreground = System.Windows.Media.Brushes.Red;
            this.window.PositionBody.Text = "Position: NOT correct";
            // gestureRecognition(false);
            stopGestures();
            return false;
        }

        //detects superman gesture of one hand from skeleton
        private void detectSupermanGeture(Joint head, Joint rightHand, Joint leftHand)
        {
            if ((leftHand.Position.Y < head.Position.Y) && (rightHand.Position.Y > head.Position.Y))
            {
                System.Console.WriteLine("SUPERMANN");
                this.window.gestureMove.Text = this.window.gestureMove.Text + " SUPERMANN";

                if (!earthMode)
                {
                    window.Browser.InvokeScript("superman");
                    earthMode = true;
                }
                else
                {
                    window.Browser.InvokeScript("superman2x");
                }
            }
        }


        private void moveTheMap(Joint head, Joint shoulderLeft, Joint shoulderRight, Joint spine, Joint leftHand, Joint rightHand)
        {
            if ((leftHand.Position.Y - rightHand.Position.Y) < 0.2 && (leftHand.Position.Y - rightHand.Position.Y) > -0.2)
            {
                zoomActive = false;

                double handCenterY = leftHand.Position.Y + rightHand.Position.Y;
                if (handCenterY > (head.Position.Y + 0.5) && earthMode)   //move the map down
                {
                    //System.Console.WriteLine("Key - Down");
                    showMoveImg("up");
                    this.window.gestureMove.Text = this.window.gestureMove.Text + " UP";
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.UP);

                }
                else if ((handCenterY < spine.Position.Y) && earthMode)        //move the map up
                {
                    showMoveImg("down");
                    //System.Console.WriteLine("Key - UP");
                    this.window.gestureMove.Text = this.window.gestureMove.Text + " DOWN";
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.DOWN);

                }
                else if (leftHand.Position.X < shoulderLeft.Position.X && rightHand.Position.X < spine.Position.X) //move the map the left
                {
                    showMoveImg("left");
                    //System.Console.WriteLine("Key - Left");
                    this.window.gestureMove.Text = this.window.gestureMove.Text + " LEFT";
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.LEFT);
                }
                else if (rightHand.Position.X > shoulderRight.Position.X && leftHand.Position.X > spine.Position.X) // move the map to the left
                {
                    showMoveImg("right");
                    //System.Console.WriteLine("Key - Right");
                    this.window.gestureMove.Text = this.window.gestureMove.Text + " RIGHT";
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.RIGHT);
                }
                else // no special gesture for a movement recognized
                {
                    showMoveImg("");
                    //System.Console.WriteLine("NO gesture!!");
                    this.window.gestureMove.Text = this.window.gestureMove.Text + " NO";
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.DOWN);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.UP);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.LEFT);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.RIGHT);
                    zoomActive = true;
                }
            }
            else // its not allowed to recognize a gesture because the hands are not in the same height
            {
                showMoveImg("");
                //System.Console.WriteLine("NO gesture - not the same height!!");
                this.window.gestureMove.Text = this.window.gestureMove.Text + " NO!!";
                InputSimulator.SimulateKeyUp(VirtualKeyCode.DOWN);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.UP);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.LEFT);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.RIGHT);

            }
        }

        private void tiltMapPerspective(Joint spine, Joint head, Joint wristHandL, Joint wristHandR, Joint KneeLeft, Joint KneeRight)
        {
            double difference = 0.3;
            double handCenterY = wristHandL.Position.Y + wristHandR.Position.Y;

            //this.window.gestureMove.Text = this.window.gestureMove.Text + " " + (wristHandL.Position.Y - wristHandR.Position.Y);

            bool leftHandWristOutKnee = wristHandL.Position.X < KneeLeft.Position.X;
            bool rightHandWristOutKnee = wristHandR.Position.X > KneeRight.Position.X;
            bool handsZoomIn = leftHandWristOutKnee && rightHandWristOutKnee;
            double differenceBetweenHandsY = wristHandL.Position.Y - wristHandR.Position.Y;

            if (handCenterY > head.Position.Y && differenceBetweenHandsY < difference)   //move the map down
            {
                //System.Console.WriteLine("Key - Down");
                this.window.gesturePerspective.Text = this.window.gesturePerspective.Text + " DOWN";
                window.Browser.InvokeScript("hoch");
            }
            else if (handCenterY < spine.Position.Y && differenceBetweenHandsY > -difference)       //move the map up
            {
                //System.Console.WriteLine("Key - UP");
                this.window.gesturePerspective.Text = this.window.gesturePerspective.Text + " UP";
                window.Browser.InvokeScript("runter");
            }
        }

        private void rotateMap(Joint wristHandL, Joint wristHandR, Joint KneeLeft, Joint KneeRight)
        {

            double difference = 0.3;

            //this.window.gestureMove.Text = this.window.gestureMove.Text + " " + (wristHandL.Position.Y - wristHandR.Position.Y);

            bool leftHandWristOutKnee = wristHandL.Position.X < KneeLeft.Position.X;
            bool rightHandWristOutKnee = wristHandR.Position.X > KneeRight.Position.X;
            bool handsZoomIn = leftHandWristOutKnee && rightHandWristOutKnee;
            double differenceBetweenHandsY = wristHandL.Position.Y - wristHandR.Position.Y;


            // if ((differenceBetweenHandsY> difference || differenceBetweenHandsY < -difference) && handsZoomIn)
            if (handsZoomIn)
            {
                if (differenceBetweenHandsY > difference)
                {
                    this.window.gesturePerspective.Text = this.window.gesturePerspective.Text + " RIGHT";
                    window.Browser.InvokeScript("links");

                }
                else if (differenceBetweenHandsY < -difference)
                {
                    this.window.gesturePerspective.Text = this.window.gesturePerspective.Text + " LEFT";
                    window.Browser.InvokeScript("rechts");
                }
            }
        }


        //pinch or spread with both hands
        private void zoomInOut(Joint wristHandL, Joint wristHandR, Joint KneeLeft, Joint KneeRight)
        {

            //System.Console.WriteLine("Linkes Handgelenk:"+ wristHandL.Position.X+ "Rechtes Handgelenk:" +wristHandR.Position.X);
            //System.Console.WriteLine("zoom");
            double altitude = (double)window.Browser.InvokeScript("getAltitude");
            String mode = "";
            if (altitude < 20)
            {
                earthMode = false;
                mode = "";
            }

            double difference = 0.2;

            bool leftHandWristOutKnee = wristHandL.Position.X < KneeLeft.Position.X;
            bool rightHandWristOutKnee = wristHandR.Position.X > KneeRight.Position.X;
            bool handsZoomIn = leftHandWristOutKnee && rightHandWristOutKnee;
            bool leftHandWristInKnee = wristHandL.Position.X > KneeLeft.Position.X;
            bool rightHandWristInKnee = wristHandR.Position.X < KneeRight.Position.X;
            bool handsZoomOut = leftHandWristInKnee && rightHandWristInKnee;

            double differenceBetweenHandsY = wristHandL.Position.Y - wristHandR.Position.Y;

            if (((handsZoomIn || handsZoomOut) && (differenceBetweenHandsY < difference && differenceBetweenHandsY > -difference)))
            {
                // double handCenter = wristHandL.Position.X + wristHandR.Position.X;
                //double kneeCenter = KneeLeft.Position.X + KneeRight.Position.X;

                double handDistance = wristHandR.Position.X - wristHandL.Position.X;
                double kneeDistance = KneeRight.Position.X - KneeLeft.Position.X;

                if (earthMode)
                {
                    window.Browser.InvokeScript("streetAutomatic");
                    mode = "";
                }
                else
                {
                    mode = "StreetView";
                }

                this.window.speech.Text = this.window.speech.Text;

                if (handDistance >= (kneeDistance * 3.5) && earthMode)
                {
                    this.window.gestureZoom.Text = this.window.gestureZoom.Text + " INx2";
                    window.Browser.InvokeScript("zoomIn2");
                }
                else if (handDistance >= (kneeDistance * 3.0))
                {
                    //System.Console.WriteLine("Zoom - IN");
                    this.window.gestureZoom.Text = this.window.gestureZoom.Text + " IN " + mode;
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.ADD);
                }
                else if (handDistance < kneeDistance * 0.3 && earthMode)
                {
                    // System.Console.WriteLine("Zoom - OUT TWICE");
                    this.window.gestureZoom.Text = this.window.gestureZoom.Text + " OUTx2";
                    window.Browser.InvokeScript("zoomOut2");
                }
                else if (handDistance <= kneeDistance * 0.75)
                {
                    //System.Console.WriteLine("Zoom - OUT");
                    this.window.gestureZoom.Text = this.window.gestureZoom.Text + " OUT " + mode;
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.SUBTRACT);
                }
                else
                {
                    //System.Console.WriteLine("NO ZOOM recognized");
                    this.window.gestureZoom.Text = this.window.gestureZoom.Text + " NO";
                    stopGestures();
                }
            }
            else
            {
                //System.Console.WriteLine("NO gesture - not the same height!!");
                this.window.gestureZoom.Text = this.window.gestureZoom.Text + " NO!!";
            }
        }

        private void zoomInOut2(Joint wristHandL, Joint wristHandR, Joint spine)
        {
            // System.Console.WriteLine("X TestWert: " + ((wristHandR.Position.X + spine.Position.X) - (wristHandL.Position.X - spine.Position.X)));
            //System.Console.WriteLine("Linkes Handgelenk:" + wristHandL.Position.X + "Rechtes Handgelenk:" + wristHandR.Position.X);
            //System.Console.WriteLine("zoom");



            double difference = 0.2;

            double differenceBetweenHandsY = wristHandL.Position.Y - wristHandR.Position.Y;
            // double differenceHandSpine = ((wristHandR.Position.X + spine.Position.X) - (wristHandL.Position.X - spine.Position.X));

            double differenceHandSpine = (wristHandR.Position.X - wristHandL.Position.X);

            if (spine.Position.X > 0)
            {
                differenceHandSpine = differenceHandSpine - spine.Position.X;
                //System.Console.WriteLine("X differenceHandSpine2: " + differenceHandSpine);
            }
            else if (spine.Position.X < 0)
            {
                differenceHandSpine = differenceHandSpine + spine.Position.X;
                // System.Console.WriteLine("X differenceHandSpine2: " + differenceHandSpine);
            }


            if (differenceBetweenHandsY < difference && differenceBetweenHandsY > -difference && differenceHandSpine > -0.1)
            {
                moveActive = false;

                if (differenceHandSpine >= 0.8 && earthMode)
                {
                    showZoomImg("zoomIn2x");
                    this.window.gestureZoom.Text = this.window.gestureZoom.Text + " INx2";
                    window.Browser.InvokeScript("zoomIn2");
                }
                else if (differenceHandSpine > 0.65)
                {
                    showZoomImg("zoomIn");
                    //System.Console.WriteLine("Zoom - IN");
                    this.window.gestureZoom.Text = this.window.gestureZoom.Text + " IN ";
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.ADD);
                }
                else if (differenceHandSpine < 0.15 && earthMode)
                {
                    showZoomImg("zoomOut2x");
                    // System.Console.WriteLine("Zoom - OUT TWICE");
                    this.window.gestureZoom.Text = this.window.gestureZoom.Text + " OUTx2";
                    window.Browser.InvokeScript("zoomOut2");
                }
                else if (differenceHandSpine < 0.25)
                {
                    showZoomImg("zoomOut");
                    //System.Console.WriteLine("Zoom - OUT");
                    this.window.gestureZoom.Text = this.window.gestureZoom.Text + " OUT ";
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.SUBTRACT);
                }
                else
                {
                    showZoomImg("");
                    //System.Console.WriteLine("NO ZOOM recognized");
                    this.window.gestureZoom.Text = this.window.gestureZoom.Text + " NO";
                    stopGestures();
                    moveActive = true;
                }
            }
            else
            {
                showZoomImg("");
                //System.Console.WriteLine("NO gesture - not the same height!!");
                this.window.gestureZoom.Text = this.window.gestureZoom.Text + " NO!!";
            }
        }

        private void showMoveImg(String description)
        {
            switch (description)
            {
                case "left":
                    this.window.left.Visibility = System.Windows.Visibility.Visible;
                    this.window.right.Visibility = System.Windows.Visibility.Hidden;
                    this.window.up.Visibility = System.Windows.Visibility.Hidden;
                    this.window.down.Visibility = System.Windows.Visibility.Hidden;
                    break;

                case "right":
                    this.window.left.Visibility = System.Windows.Visibility.Hidden;
                    this.window.right.Visibility = System.Windows.Visibility.Visible;
                    this.window.up.Visibility = System.Windows.Visibility.Hidden;
                    this.window.down.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case "up":
                    this.window.left.Visibility = System.Windows.Visibility.Hidden;
                    this.window.right.Visibility = System.Windows.Visibility.Hidden;
                    this.window.up.Visibility = System.Windows.Visibility.Visible;
                    this.window.down.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case "down":
                    this.window.left.Visibility = System.Windows.Visibility.Hidden;
                    this.window.right.Visibility = System.Windows.Visibility.Hidden;
                    this.window.up.Visibility = System.Windows.Visibility.Hidden;
                    this.window.down.Visibility = System.Windows.Visibility.Visible;
                    break;
                default:
                    this.window.left.Visibility = System.Windows.Visibility.Hidden;
                    this.window.right.Visibility = System.Windows.Visibility.Hidden;
                    this.window.up.Visibility = System.Windows.Visibility.Hidden;
                    this.window.down.Visibility = System.Windows.Visibility.Hidden;
                    break;
            }
        }

        private void showZoomImg(String description)
        {
            switch (description)
            {
                case "zoomIn":
                    this.window.zoomIn.Visibility = System.Windows.Visibility.Visible;
                    this.window.zoomIn2x.Visibility = System.Windows.Visibility.Hidden;
                    this.window.zoomOut.Visibility = System.Windows.Visibility.Hidden;
                    this.window.zoomOut2x.Visibility = System.Windows.Visibility.Hidden;
                    break;

                case "zoomIn2x":
                    this.window.zoomIn.Visibility = System.Windows.Visibility.Hidden;
                    this.window.zoomIn2x.Visibility = System.Windows.Visibility.Visible;
                    this.window.zoomOut.Visibility = System.Windows.Visibility.Hidden;
                    this.window.zoomOut2x.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case "zoomOut":
                    this.window.zoomIn.Visibility = System.Windows.Visibility.Hidden;
                    this.window.zoomIn2x.Visibility = System.Windows.Visibility.Hidden;
                    this.window.zoomOut.Visibility = System.Windows.Visibility.Visible;
                    this.window.zoomOut2x.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case "zoomOut2x":
                    this.window.zoomIn.Visibility = System.Windows.Visibility.Hidden;
                    this.window.zoomIn2x.Visibility = System.Windows.Visibility.Hidden;
                    this.window.zoomOut.Visibility = System.Windows.Visibility.Hidden;
                    this.window.zoomOut2x.Visibility = System.Windows.Visibility.Visible;
                    break;
                default:
                    this.window.zoomIn.Visibility = System.Windows.Visibility.Hidden;
                    this.window.zoomIn2x.Visibility = System.Windows.Visibility.Hidden;
                    this.window.zoomOut.Visibility = System.Windows.Visibility.Hidden;
                    this.window.zoomOut2x.Visibility = System.Windows.Visibility.Hidden;
                    break;
            }

        }


        private void detectStepForward(Joint footLeft, Joint footRight, Joint kneeLeft, Joint kneeRight)
        {
            bool feetTracked = (footRight.TrackingState == kneeLeft.TrackingState);
            bool kneeTracked = ((kneeLeft.TrackingState == JointTrackingState.Tracked) && kneeRight.TrackingState == JointTrackingState.Tracked);
            if (!earthMode)
            {
                if (feetTracked)
                {
                    // 2.0:Vor  2.4: Mitte   2.7: Hinten
                    //System.Console.WriteLine(footRight.Position.Z - kneeLeft.Position.Z);
                    if ((footRight.Position.Z - kneeLeft.Position.Z) > 0.1)
                    {
                        showMoveImg("down");
                        InputSimulator.SimulateKeyDown(VirtualKeyCode.DOWN);
                    }
                    else if ((footRight.Position.Z - kneeLeft.Position.Z) < -0.28)
                    {
                        showMoveImg("up");
                        InputSimulator.SimulateKeyDown(VirtualKeyCode.UP);
                    }
                }
                else if (kneeTracked)
                {
                    // 2.0:Vor  2.4: Mitte   2.7: Hinten
                    //   System.Console.WriteLine(kneeRight.Position.Z - kneeLeft.Position.Z);
                    if ((kneeRight.Position.Z - kneeLeft.Position.Z) > 0.1)
                    {
                        showMoveImg("down");
                        InputSimulator.SimulateKeyDown(VirtualKeyCode.DOWN);
                    }
                    else if ((kneeRight.Position.Z - kneeLeft.Position.Z) < -0.15)
                    {
                        showMoveImg("up");
                        InputSimulator.SimulateKeyDown(VirtualKeyCode.UP);
                    }
                }
                else
                {
                    //stopGestures();
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.UP);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.DOWN);
                }
            }
        }

        private void stopGestures()
        {

            InputSimulator.SimulateKeyUp(VirtualKeyCode.ADD);
            InputSimulator.SimulateKeyUp(VirtualKeyCode.SUBTRACT);
            InputSimulator.SimulateKeyUp(VirtualKeyCode.UP);
            InputSimulator.SimulateKeyUp(VirtualKeyCode.DOWN);
        }
    }
}
