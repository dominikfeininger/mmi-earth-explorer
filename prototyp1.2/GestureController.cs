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

        private bool earthMode = true;

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
            this.perspectiveActive = !mode;
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
            this.moveActive = !mode;
            this.zoomActive = !mode;

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
                window.Browser.InvokeScript("earth");
                
            }
            if (p.Equals("street"))
            {
                window.Browser.InvokeScript("street");
            } if (p.Equals("horizont"))
            {
                window.Browser.InvokeScript("horizont");
            } if (p.Equals("vogel"))
            {
                window.Browser.InvokeScript("vogel");
            } if (p.Equals("hoch"))
            {
                window.Browser.InvokeScript("hoch");
            } if (p.Equals("kippen"))
            {
                window.Browser.InvokeScript("kippen");
            }
            if (p.Equals("norden"))
            {
                InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_N);

            }
            if (p.Equals("rechts"))
            {
                window.Browser.InvokeScript("rechts");

            } if (p.Equals("links"))
            {
                window.Browser.InvokeScript("links");

            }
            if (p.Equals("test"))
            {
                //window.Browser.InvokeScript("test");


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


            if (moveActive)
            {
                this.window.gestureMove.Text = "Move: ON";
            }
            else
            {
                this.window.gestureMove.Text = "Move: OFF";
            }
            if (zoomActive)
            {
                this.window.gestureZoom.Text = "Zoom: ON";
            }
            else
            {
                this.window.gestureZoom.Text = "Zoom: OFF";
            }
            if (perspectiveActive)
            {
               // this.window.gesturePerspective.Text = "Perspective: ON";
            }
            else
            {
               // this.window.gesturePerspective.Text = "Perspective: OFF";
            }



            if (gesturesActive && (zoomActive || moveActive || perspectiveActive))
            {
                System.Console.WriteLine("_");


                if (zoomActive)
                {
                    zoomInOut(wristHandL, wristHandR, KneeLeft, KneeRight);
                }
                if (moveActive)
                {
                    moveTheMap(head, shoulderLeft, shoulderRight, spine, wristHandL, wristHandR);
                    //detectStepForward(spine, AnkleRight);
                    detectSupermanGeture(head, wristHandR, wristHandL);
                }
                if (perspectiveActive && earthMode)
                {
                    //changeMapPerspective(head, spine, wristHandL, wristHandR, KneeLeft, KneeRight);
                }

            }
        }



        //detects superman gesture of one hand from skeleton
        //ShoulderCenter, ShoulderLeft, ElbowLeft, WristLeft, HandLeft, ShoulderRight, ElbowRight, WristRight, HandRight
        private void detectSupermanGeture(Joint head, Joint rightHand, Joint leftHand)
        {

            if ((leftHand.Position.Y < head.Position.Y) && (rightHand.Position.Y > head.Position.Y) && !earthMode)
            {
                System.Console.WriteLine(" ");
                System.Console.WriteLine(" ");
                System.Console.WriteLine("SUPERMANN");
                System.Console.WriteLine(" ");
                System.Console.WriteLine(" ");
                this.window.gestureMove.Text = this.window.gestureMove.Text + " Superman";
                window.Browser.InvokeScript("earth");
                earthMode = true;
            }
        }


        private void moveTheMap(Joint head, Joint shoulderLeft, Joint shoulderRight, Joint spine, Joint leftHand, Joint rightHand)
        {

            System.Console.WriteLine("move");
            if ((leftHand.Position.Y - rightHand.Position.Y) < 0.2 && (leftHand.Position.Y - rightHand.Position.Y) > -0.2)
            {
                //wenn mitte der hände 

                double handCenterY = leftHand.Position.Y + rightHand.Position.Y;

                //in der mitte nichts tun

                if (handCenterY > head.Position.Y)   //move the map down
                {
                    //System.Console.WriteLine("Key - Down");

                    this.window.gestureMove.Text = this.window.gestureMove.Text + " Up";
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.DOWN);

                }
                else if (handCenterY < spine.Position.Y)        //move the map up
                {   //System.Console.WriteLine("Key - UP");
                    this.window.gestureMove.Text = this.window.gestureMove.Text + " Down";
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.UP);

                }
                else if (leftHand.Position.X < shoulderLeft.Position.X && rightHand.Position.X < spine.Position.X) //move the map the left
                {
                    //System.Console.WriteLine("Key - Left");
                    this.window.gestureMove.Text = this.window.gestureMove.Text + " Left";
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.RIGHT);
                }
                else if (rightHand.Position.X > shoulderRight.Position.X && leftHand.Position.X > spine.Position.X) // move the map to the left
                {
                    //System.Console.WriteLine("Key - Right");
                    this.window.gestureMove.Text = this.window.gestureMove.Text + " Right";
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.LEFT);
                }
                else // no special gesture for a movement recognized
                {
                    //System.Console.WriteLine("NO gesture!!");
                    this.window.gestureMove.Text = this.window.gestureMove.Text + " NO Gestures";
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.DOWN);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.UP);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.LEFT);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.RIGHT);
                }
            }
            else // its not allowed to recognize a gesture because the hands are not in the same height
            {
                //System.Console.WriteLine("NO gesture - not the same height!!");
                this.window.gestureMove.Text = this.window.gestureMove.Text + " No gesture (hands position!!)";
                InputSimulator.SimulateKeyUp(VirtualKeyCode.DOWN);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.UP);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.LEFT);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.RIGHT);
            }
        }

        private void changeMapPerspective(Joint spine, Joint head, Joint wristHandL, Joint wristHandR, Joint KneeLeft, Joint KneeRight)
        {
            double difference = 0.5;
            double handCenterY = wristHandL.Position.Y + wristHandR.Position.Y;

            System.Console.WriteLine("perspektive");
            //this.window.gestureMove.Text = this.window.gestureMove.Text + " " + (wristHandL.Position.Y - wristHandR.Position.Y);


            if (((wristHandL.Position.Y - wristHandR.Position.Y) > difference || (wristHandL.Position.Y - wristHandR.Position.Y) < -difference) &&
                (wristHandL.Position.X < KneeLeft.Position.X && wristHandR.Position.X > KneeRight.Position.X))
            {

                if ((wristHandR.Position.Y - wristHandL.Position.Y) > difference)
                {
                    System.Console.WriteLine("rotate Left");
                   // this.window.gesturePerspective.Text = this.window.gesturePerspective.Text + " rotate right";
                    InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.RIGHT);
                }
                else if ((wristHandL.Position.Y - wristHandR.Position.Y) > difference)
                {
                    System.Console.WriteLine("rotate Right");
                   // this.window.gesturePerspective.Text = this.window.gesturePerspective.Text + " rotate left";
                    InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.LEFT);
                }
            }
            if (handCenterY > head.Position.Y)   //move the map down
            {
                //System.Console.WriteLine("Key - Down");
                this.window.gestureMove.Text = this.window.gestureMove.Text + " Up";
                InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.DOWN);
            }
            else if (handCenterY < spine.Position.Y)        //move the map up
            {   //System.Console.WriteLine("Key - UP");
                this.window.gestureMove.Text = this.window.gestureMove.Text + " Down";
                InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.UP);
            }
            else
            {
                InputSimulator.SimulateKeyUp(VirtualKeyCode.UP);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.DOWN);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.LEFT);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.RIGHT);
            }
        }

        //pinch or spread with both hands
        private void zoomInOut(Joint wristHandL, Joint wristHandR, Joint KneeLeft, Joint KneeRight)
        {
            System.Console.WriteLine("zoom");
            double altitude = (double)window.Browser.InvokeScript("getAltitude");
            String mode = "";
            if (altitude < 20)
            {
                earthMode = false;
                mode = "";
            }

            double difference = 0.2;

            if ((((wristHandL.Position.X < KneeLeft.Position.X && wristHandR.Position.X > KneeRight.Position.X) ||
                (wristHandL.Position.X > KneeLeft.Position.X && wristHandR.Position.X < KneeRight.Position.X)) &&
                ((wristHandL.Position.Y - wristHandR.Position.Y) < difference && (wristHandL.Position.Y - wristHandR.Position.Y) > -difference)))
            {

                double handCenter = wristHandL.Position.X + wristHandR.Position.X;
                double kneeCenter = KneeLeft.Position.X + KneeRight.Position.X;

                double handDistance = wristHandR.Position.X - wristHandL.Position.X;
                double kneeDistance = KneeRight.Position.X - KneeLeft.Position.X;

                double wert = (handDistance / kneeDistance);
                

                System.Console.WriteLine("hand / knrie: " + wert);

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

                //check every frame a zomm is possible if streetview is possible.
                // window.Browser.InvokeScript("streetAutomatic");

                if (handDistance >= (kneeDistance * 3.5) && earthMode)
                {
                    //System.Console.WriteLine("Zoom - IN TWICE");
                    this.window.gestureZoom.Text = this.window.gestureZoom.Text + " IN x2";
                    window.Browser.InvokeScript("zoomIn2");
                    //InputSimulator.SimulateKeyDown(VirtualKeyCode.ADD);

                }
                else if (handDistance >= (kneeDistance * 2.5))
                {
                    //System.Console.WriteLine("Zoom - IN");
                    this.window.gestureZoom.Text = this.window.gestureZoom.Text + " IN " + mode;

                    InputSimulator.SimulateKeyDown(VirtualKeyCode.ADD);

                }
                else if (handDistance < kneeDistance * 0.3 && earthMode)
                {
                    // System.Console.WriteLine("Zoom - OUT TWICE");
                    this.window.gestureZoom.Text = this.window.gestureZoom.Text + " OUT x2";

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
                    this.window.gestureZoom.Text = this.window.gestureZoom.Text + " NO Zoom";

                    InputSimulator.SimulateKeyUp(VirtualKeyCode.ADD);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.SUBTRACT);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.UP);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.DOWN);
                }
            }
            else
            {
                //System.Console.WriteLine("NO gesture - not the same height!!");
                this.window.gestureZoom.Text = this.window.gestureZoom.Text + " NO Zoom (hands position!!)";

                InputSimulator.SimulateKeyUp(VirtualKeyCode.ADD);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.SUBTRACT);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.UP);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.DOWN);
            }
        }

        //gets skeleton, to detect walk from
        //KneeLeft, AnkleLeft, FootLeft, HipRight, KneeRight, AnkleRight, FootRight
        private void detectStepForward(Joint spine, Joint ankleRight)
        {


            if (ankleRight.TrackingState == JointTrackingState.Tracked && spine.TrackingState == JointTrackingState.Tracked)
            {

                double difference = 2; // von einbauen und werte ausgeben...test...

                if (ankleRight.Position.Z < spine.Position.Z) //forwards
                {
                    System.Console.WriteLine("Schritt Vor");
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.UP);
                }
                else if (ankleRight.Position.Z > spine.Position.Z) //backwards
                {
                    System.Console.WriteLine("Schritt Zurück");
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.DOWN);
                }
                else
                {
                    System.Console.WriteLine(" kein Schritt");
                }
            }
        }

        //gets skeleton, to detect walk from
        //KneeLeft, AnkleLeft, FootLeft, HipRight, KneeRight, AnkleRight, FootRight
        /*             private void detectStepBackwards(Skeleton skeleton)
                     {
                         //TODO:
                         //check seated Mode
                     }

                     private void detectHandMoveUp(Skeleton skeleton)
                     {
                         //TODO:
                         //InputSimulator.SimulateKeyUp(VirtualKeyCode.UP);
                     }

                     //for test purpose
                     private void detectHead(Joint head)
                     {
                         double xAngle = System.Math.Atan2(head.Position.X, head.Position.Z);
                         double yAngle = System.Math.Atan2(head.Position.Y, head.Position.Z);
                         double zAngle = System.Math.Atan2(head.Position.Z, head.Position.X);

                         //print method
                         // this.printHeadAngel(xAngle, yAngle, zAngle);

                     }

                     //print method
                     private void printHeadAngel(double xAngle, double yAngle, double zAngle)
                     {
                         //TODO
                     }
              * */
    }
}
