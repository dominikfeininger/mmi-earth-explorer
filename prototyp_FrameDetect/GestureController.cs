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

        //attributes for frame detection
        private int CurrentSkeletonFrame = 0;
        //30frames per second
        private static int totalFrames = 120;//4sec
        //saves the skeletons
        private Skeleton[] SkeletonFrames = new Skeleton[totalFrames + 1];
        //frame interval
        private int frameInterval = 3;
        private float minMovementTotal = 0.03F;
        //calc from screensize and distance
        /*ungenutzt*/private float proportion = (distanceZ_user_display / screensize) * screen_dpi;
        private static float screen_dpi = 10;
        //in cm
        private static float distanceZ_user_display = 310;
        private static float screensize = 330;
        //just for calc.
        private int frameCounter;

        //Entfernung zur Leinwand (diesen Parameter können Sie natürlich nicht in der SW einstellbar machen, die nun folgenden allerdings  schon...)
        public float distanceX; //to screen

        //params for input 
        public float minMovementFrame = 0.01F; //minMoveFrame  //Verzögerung, mit der der Effekt der Geste einsetzt
        public float zoomspeed = 3; //zoomSpeed //die Geschwindigkeit der Geste im Verhältnis zum entsprechenden Verhalten (Geschwindigkeit) mit der sich das Objekt vergrößert/verkleinert)
        /*ungenutzt*/public float tolleranceHandsSDiff; //tollerance
        public float proportionParam = 1; //proportionParam //das Verhältnis zwischen Gestengröße und Effektgröße (also Vergrößerung/Verkleinerung des Objektes)
        public float handMovingZTollerance = 3; //handZTollerance //Aktivitätsradius/-feld der Geste (wo beginnt und endet die Geste, in welchem Bereich steuert die Geste die Darstellung)
        

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
            //System.Console.WriteLine("this.CurrentSkeletonFrame: " + this.CurrentSkeletonFrame);
            this.SkeletonFrames[this.CurrentSkeletonFrame] = skeleton;

            //System.Console.WriteLine("CurrentSkeletonFrame:" + CurrentSkeletonFrame);
            if (this.CurrentSkeletonFrame == totalFrames)
            {
                //System.Console.WriteLine("totalFrames reached");
                this.CurrentSkeletonFrame = 0;
            }
            else
            {
                //System.Console.WriteLine("totalFrames NOT reached");
                this.CurrentSkeletonFrame++;
            }
        }

        //aktiviere die Gesten falls die Hände weit genug vor dem Kopf sind
        private bool handsZAxle(Joint current_wristHandR, Joint current_wristHandL, Joint current_head)
        {
            float zAxlehands = 0.2F;//0.45F;
            /*System.Console.WriteLine("current_wristHandR.Position.Z: " + current_wristHandR.Position.Z);
            System.Console.WriteLine("current_wristHandL.Position.Z: " + current_wristHandL.Position.Z);
            System.Console.WriteLine("current_head.Position.Z: " + current_head.Position.Z);
            */
            if (((current_wristHandL.Position.Z + zAxlehands) < current_head.Position.Z) && ((current_wristHandL.Position.Z + zAxlehands) < current_head.Position.Z))
            {
                // System.Console.WriteLine("Hand Poisiton Ok for ZOOM");
                this.zoomActive = true;
                return true;
            }
            else
            {
                this.zoomActive = false;
                return false;
            }


        }

        private float calcProportion(Joint current_wristHandR, Joint current_wristHandL, Joint current_head)
        {
           
            float tmpPropParam = 1;//0;
           float tmpProp = (current_wristHandL.Position.X * (-1)) + current_wristHandR.Position.X;
           //System.Console.WriteLine("tmpProp" + tmpProp);
           tmpPropParam = tmpProp / 10 + 1;
            return tmpPropParam * proportionParam;
        }

        private bool handsZAxleMove(Joint current_wristHandR, Joint current_wristHandL, Joint skeleton1_wristHandR, Joint skeleton1_wristHandL, Joint skeleton2_wristHandR, Joint skeleton2_wristHandL)
        {
            bool rightMoveZForw = false;
            bool leftMoveZForw = false;

            bool rightMoveZBack = false;
            bool leftMoveZBack = false;

            //rechte Hand
            if (((skeleton1_wristHandR.Position.Z + handMovingZTollerance) < skeleton2_wristHandR.Position.Z) && ((current_wristHandR.Position.Z + handMovingZTollerance) < skeleton1_wristHandR.Position.Z))
            {
                rightMoveZForw = true;
                //System.Console.WriteLine("rechte Hand bewegt sich nach Aussen");

            }

            //linke hand

            if (((skeleton1_wristHandL.Position.Z + handMovingZTollerance) < skeleton2_wristHandL.Position.Z) && ((current_wristHandL.Position.Z + handMovingZTollerance) < skeleton1_wristHandL.Position.Z))
            {
                leftMoveZForw = true;
                //System.Console.WriteLine("linke Hand bewegt sich nach Aussen");

            }
            //rechte Hand
            if (((skeleton1_wristHandR.Position.Z - handMovingZTollerance) < skeleton2_wristHandR.Position.Z) && ((current_wristHandR.Position.Z - handMovingZTollerance) < skeleton1_wristHandR.Position.Z))
            {
                rightMoveZBack = true;
                //System.Console.WriteLine("rechte Hand bewegt sich nach Aussen");

            }

            //linke hand

            if (((skeleton1_wristHandL.Position.Z - handMovingZTollerance) < skeleton2_wristHandL.Position.Z) && ((current_wristHandL.Position.Z - handMovingZTollerance) < skeleton1_wristHandL.Position.Z))
            {
                leftMoveZBack = true;
                //System.Console.WriteLine("linke Hand bewegt sich nach Aussen");

            }

            if (leftMoveZForw && rightMoveZForw && rightMoveZBack && leftMoveZBack)
            {
                return false;
            }
            else
            {
                return true;
            }


        }

        //TODO:doppelt aufgerudfen, da zoomin und zoom out gleiche methode callt!!!!!!!!!!!!!11
        private bool[] detectHandMovementZoom(Joint current_wristHandR, Joint current_wristHandL, Joint current_head)
        {
            bool handLeftFast = false;
            bool handRightFast = false;

            bool[] handMovement = new bool[5];
            handMovement[0] = false;
            handMovement[1] = false;
            handMovement[2] = false;
            handMovement[3] = false;

            //System.Console.WriteLine("minMovementFrame: " + minMovementFrame); 

            //for accesleration
            handMovement[4] = false;

            //2 und 3 , da current auf -1 gespeichert ist
            Skeleton skeleton1 = this.SkeletonFrames[this.CurrentSkeletonFrame - 2];
            Skeleton skeleton2 = this.SkeletonFrames[this.CurrentSkeletonFrame - 3];

            Joint skeleton1_wristHandR = skeleton1.Joints[JointType.WristRight];
            Joint skeleton1_wristHandL = skeleton1.Joints[JointType.WristLeft];

            Joint skeleton2_wristHandR = skeleton2.Joints[JointType.WristRight];
            Joint skeleton2_wristHandL = skeleton2.Joints[JointType.WristLeft];

            //check for active
            if (handsZAxle(current_wristHandR, current_wristHandL, current_head))
            {
                //
                if (handsZAxleMove(current_wristHandR, current_wristHandL, skeleton1_wristHandR, skeleton1_wristHandL, skeleton2_wristHandR, skeleton2_wristHandL))
                {
                    //check for active (double) ???
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
                    /*
                    System.Console.WriteLine("  current_wristHandR.Position.X: " + current_wristHandR.Position.X);
                    System.Console.WriteLine("skeleton1_wristHandR.Position.X: " + skeleton1_wristHandR.Position.X);
                    System.Console.WriteLine("skeleton2_wristHandR.Position.X: " + skeleton2_wristHandR.Position.X);
                    */

                    //rechte Hand
                    if (((skeleton1_wristHandR.Position.X - minMovementFrame) > skeleton2_wristHandR.Position.X) && ((current_wristHandR.Position.X - minMovementFrame) > skeleton1_wristHandR.Position.X))
                    {
                        handMovement[1] = true;
                        //System.Console.WriteLine("rechte Hand bewegt sich nach Aussen");

                        //acceleration
                        if ((current_wristHandR.Position.X - minMovementTotal) > skeleton2_wristHandR.Position.X)
                        {
                            handRightFast = true;
                        }
                    }
                    else
                    {
                        handMovement[1] = false;
                    }
                    //linke hand

                    if (((skeleton1_wristHandL.Position.X + minMovementFrame) < skeleton2_wristHandL.Position.X) && ((current_wristHandL.Position.X + minMovementFrame) < skeleton1_wristHandL.Position.X))
                    {
                        handMovement[0] = true;
                        //System.Console.WriteLine("linke Hand bewegt sich nach Aussen");

                        //acceleration
                        if ((current_wristHandL.Position.X + minMovementTotal) < skeleton2_wristHandL.Position.X)
                        {
                            handLeftFast = true;
                        }

                    }
                    else
                    {
                        handMovement[0] = false;
                    }

                    //rechte Hand
                    if (((skeleton1_wristHandR.Position.X + minMovementFrame) < skeleton2_wristHandR.Position.X) && ((current_wristHandR.Position.X + minMovementFrame) < skeleton1_wristHandR.Position.X))
                    {
                        handMovement[3] = true;
                        //System.Console.WriteLine("rechte Hand bewegt sich nach INNEN");

                        //acceleration
                        if ((current_wristHandR.Position.X + minMovementTotal) < skeleton2_wristHandR.Position.X)
                        {
                            handRightFast = true;
                        }
                    }
                    else
                    {
                        handMovement[3] = false;
                    }
                    //linke hand

                    if (((skeleton1_wristHandL.Position.X - minMovementFrame) > skeleton2_wristHandL.Position.X) && ((current_wristHandL.Position.X - minMovementFrame) > skeleton1_wristHandL.Position.X))
                    {
                        handMovement[2] = true;
                        //System.Console.WriteLine("linke Hand bewegt sich nach INNEN");

                        //acceleration
                        if ((current_wristHandL.Position.X - minMovementTotal) > skeleton2_wristHandL.Position.X)
                        {
                            handLeftFast = true;
                        }
                    }
                    else
                    {
                        handMovement[2] = false;
                    }
                }
            }
            if (handLeftFast && handRightFast)
            {
                handMovement[4] = true;
            }

            //[links nach aussen, rechts nach aussen, links nach innen, rechts nach innen]
            return handMovement;
        }


        //TODO: 2fach zoom
        private void detectZoomINByFrame(Joint current_wristHandR, Joint current_wristHandL, Joint current_head)
        {

            //TODO: erzeugt abweichungen am beginn des arrays --> ring
            if (this.CurrentSkeletonFrame >= 3)
            {

                bool[] handMovement = detectHandMovementZoom(current_wristHandR, current_wristHandL, current_head);

                if (handMovement[1] && handMovement[0])
                {

                    if (true)//handMovement[4])//zoom double
                    {
                        zoomspeed *= (float)calcProportion(current_wristHandR, current_wristHandL, current_head);
                        //zoomspeed = (float)4;


                        // System.Console.WriteLine("Zoom - OUT TWICE");
                        //this.window.gestureZoom.Text = this.window.gestureZoom.Text + " IN with speed: ...";
                        //window.Browser.InvokeScript("zoomInByValue",zoomspeed.ToString());

                        //means zoomsped
                        String zs = zoomspeed.ToString();
                        window.Browser.InvokeScript("zoomInByValue", zs);
                        //window.Browser.InvokeScript("zoomIn2");
                    }
                    else if (!handMovement[4])//zoom single
                    {
                        //System.Console.WriteLine("Zoom - OUT");
                        this.window.gestureZoom.Text = this.window.gestureZoom.Text + " IN ";
                        InputSimulator.SimulateKeyDown(VirtualKeyCode.ADD);
                    }
                    else
                    {
                        //System.Console.WriteLine("NO ZOOM recognized");
                        this.window.gestureZoom.Text = this.window.gestureZoom.Text + " NO";
                        stopGestures();
                    }

                }
            }
        }

        private void detectZoomOUTByFrame(Joint current_wristHandR, Joint current_wristHandL, Joint current_head)
        {

            //TODO: erzeugt abweichungen am beginn des arrays --> ring
            if (this.CurrentSkeletonFrame >= 3)
            {
                bool[] handMovement = detectHandMovementZoom(current_wristHandR, current_wristHandL, current_head);
                if (handMovement[3] && handMovement[2])
                {

                    if (true)//handMovement[4])//zoom double
                    {
                        zoomspeed *= calcProportion(current_wristHandR, current_wristHandL, current_head);
                        // System.Console.WriteLine("Zoom - OUT TWICE");
                        //this.window.gestureZoom.Text = this.window.gestureZoom.Text + " OUT with speed: ...";
                        //window.Browser.InvokeScript("zoomOutByValue", new string[] { zoomspeed.ToString() });

                        //window.Browser.InvokeScript("zoomOut2");
                        //zoomspeed = (float)4;
                        String zs = zoomspeed.ToString();
                        window.Browser.InvokeScript("zoomOutByValue", zs);// "4");//
                        
                        //window.Browser.InvokeScript("zoomOut2");
                        //System.Console.WriteLine("window.Browser.InvokeScript(zoomOut2);");
                    }
                    else if (!handMovement[4])//zoom single
                    {
                        //System.Console.WriteLine("Zoom - OUT");
                        this.window.gestureZoom.Text = this.window.gestureZoom.Text + " OUT ";
                        InputSimulator.SimulateKeyDown(VirtualKeyCode.SUBTRACT);
                    }
                    else
                    {
                        //System.Console.WriteLine("NO ZOOM recognized");
                        this.window.gestureZoom.Text = this.window.gestureZoom.Text + " NO";
                        stopGestures();
                    }

                }

                //x = rechts/ links
                //y = hoch/ runter
                //z = vor/ zurück

            }
        }

        //gets called once a frame
        internal void processSkeletonFrame(Skeleton skeleton)
        {
            //System.Console.WriteLine("window.moveSpeed.Text" + window.moveSpeed.Text);


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
            //System.Console.WriteLine("frameCounter:" + frameCounter);
            // if (gesturesActive && (zoomActive || moveActive || perspectiveActive))

            if (positionCorrect)
            {
                //TODO:
                saveToSkeletonFrames(skeleton);

                //saves in interval
                if (this.frameCounter % frameInterval == 1)
                {
                    detectZoomOUTByFrame(wristHandR, wristHandL, head);
                }
                if (this.frameCounter % frameInterval == 0)
                {
                    detectZoomINByFrame(wristHandR, wristHandL, head);
                }
                //TODO:
                //checkContinuousState();

                if (zoomActive)
                {
                    //zoomInOut2(wristHandL, wristHandR, spine);
                }
                if (moveActive)
                {
                    //saveToSkeletonFrames(skeleton.Joints);

                    //TODO:
                    //moveTheMap(head, shoulderLeft, shoulderRight, spine, wristHandL, wristHandR);
                    //detectStepForward(FootLeft, FootRight, KneeLeft, KneeRight);
                    //detectSupermanGeture(head, wristHandR, wristHandL);
                }

                if (perspectiveActive)
                {
                    //tiltMapPerspective(spine, head, wristHandL, wristHandR, KneeLeft, KneeRight);
                    //rotateMap(wristHandL, wristHandR, KneeLeft, KneeRight);
                }
            }
            this.frameCounter++;
        }

        private void checkContinuousState()
        {

            if (!earthMode)
            {
                //this.window.supermanTag.Text = "StreetView";
                this.window.superman.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                //this.window.supermanTag.Text = "";
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


        private void zoomInOut2(Joint wristHandL, Joint wristHandR, Joint spine)
        {

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
