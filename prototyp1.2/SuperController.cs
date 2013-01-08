using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using WindowsInput;

namespace Microsoft.mmi.Kinect.Explorer
{
    //contains all gestrues
    class SuperController
    {
        private MainWindow window;

        /// <summary>
        /// local variable to check seated mode
        /// </summary>
        private bool seatedModeOn = false;

        /// <summary>
        /// local variable to de/activate gestures
        /// </summary>
        private bool gesturesActive = false;

        /// <summary>
        /// local variable to de/activate the zoom
        /// </summary>
        private bool zoomActive = true;
       
        /// <summary>
        /// local variable to de/activate the move
        /// </summary>
        private bool moveActive = true;


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
        }

        internal void zoomRecognition(bool mode)
        {
            this.zoomActive = mode;
        }

        internal void moveRecognition(bool mode)
        {
            this.moveActive = mode;
        }

        public void setSeatedMode(bool mode)
        {
            this.seatedModeOn = mode;
        }


        // flyover to the destination city
        internal void goTo(string p)
        {
            if (p.Equals("frankfurt")) {
                    window.Browser.InvokeScript("lookAtFrankfurt");
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
            }
            if (p.Equals("test"))
            {
                window.Browser.InvokeScript("test");
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

            window.Browser.InvokeScript("streetAutomatic");


            if (gesturesActive)
            {
                System.Console.WriteLine("_");

               // detectStepForward(AnkleLeft, AnkleRight);
                if (zoomActive)
                {
                    zoomInOut(wristHandL, wristHandR, KneeLeft, KneeRight);
                }
                if (moveActive)
                {
                    moveTheMap(head, shoulderLeft, shoulderRight, spine, wristHandL, wristHandR);
                }
               // detectSupermanGeture(head, wristHandR, wristHandL);
            }
        }

        //detects superman gesture of one hand from skeleton
        //ShoulderCenter, ShoulderLeft, ElbowLeft, WristLeft, HandLeft, ShoulderRight, ElbowRight, WristRight, HandRight
        private void detectSupermanGeture(Joint head, Joint rightHand, Joint leftHand)
        {
            if ((leftHand.Position.Y < head.Position.Y) && (rightHand.Position.Y > head.Position.Y))
            {
                System.Console.WriteLine(" ");
                System.Console.WriteLine(" ");
                System.Console.WriteLine("SUPERMANN");
                System.Console.WriteLine(" ");
                System.Console.WriteLine(" ");
                window.Browser.InvokeScript("lookAt");

            }
        }


        private void moveTheMap(Joint head, Joint shoulderLeft, Joint shoulderRight, Joint spine, Joint leftHand, Joint rightHand)
        {
            if ((leftHand.Position.Y - rightHand.Position.Y) < 0.2 && (leftHand.Position.Y - rightHand.Position.Y) > -0.2)
            {               
                //wenn mitte der hände 
                
                double handCenterY = leftHand.Position.Y + rightHand.Position.Y;

                //in der mitte nichts tun


                if (handCenterY > head.Position.Y)                 //move the map down
                {     
                     double altitude = (double)window.Browser.InvokeScript("getAltitude");
                     //if (altitude < 10)
                     //{ }
                     //else
                     //{
                         System.Console.WriteLine("Key - Down");
                         InputSimulator.SimulateKeyDown(VirtualKeyCode.DOWN);
                     //}
                }
                else if (handCenterY < spine.Position.Y)        //move the map up
                {
                    
                     double altitude = (double)window.Browser.InvokeScript("getAltitude");
                    // if (altitude < 10)
                     //{ }
                     //else
                     //{
                         System.Console.WriteLine("Key - UP");
                         InputSimulator.SimulateKeyDown(VirtualKeyCode.UP);
                     //}
                }
                else if (leftHand.Position.X < shoulderLeft.Position.X && rightHand.Position.X < spine.Position.X) //move the map the left
                {                   
                    System.Console.WriteLine("Key - Left");
                    // window.Browser.InvokeScript("lookRigthOnEarth");
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.RIGHT);
                }
                else if (rightHand.Position.X > shoulderRight.Position.X && leftHand.Position.X > spine.Position.X) // move the map to the left
                {
                    System.Console.WriteLine("Key - Right");
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.LEFT);
                }
                else // no special gesture for a movement recognized
                {
                    System.Console.WriteLine("NO gesture!!");
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.DOWN);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.UP);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.LEFT);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.RIGHT);

                }
            }
            else // its not allowed to recognize a gesture because the hands are not in the same height
            {
                System.Console.WriteLine("NO gesture - not the same height!!");
                InputSimulator.SimulateKeyUp(VirtualKeyCode.DOWN);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.UP);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.LEFT);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.RIGHT);

            }
        }



        //pinch or spread with both hands
        private void zoomInOut(Joint leftHand, Joint rightHand, Joint KneeLeft, Joint KneeRight)
        {

            if ((((leftHand.Position.X < KneeLeft.Position.X && rightHand.Position.X > KneeRight.Position.X) ||
                (leftHand.Position.X > KneeLeft.Position.X && rightHand.Position.X < KneeRight.Position.X)) &&
                ((leftHand.Position.Y - rightHand.Position.Y) < 0.2 && (leftHand.Position.Y - rightHand.Position.Y) > -0.2)))
            {

                double handCenter = leftHand.Position.X + rightHand.Position.X;
                double kneeCenter = KneeLeft.Position.X + KneeRight.Position.X;

                double handDistance = rightHand.Position.X - leftHand.Position.X;
                double kneeDistance = KneeRight.Position.X - KneeLeft.Position.X;

                //check every frame a zomm is possible if streetview is possible.
                window.Browser.InvokeScript("streetAutomatic");
                double altitude = (double)window.Browser.InvokeScript("getAltitude");


                if (handDistance >= (kneeDistance * 2.5))
                {
                    System.Console.WriteLine("Zoom - IN TWICE");

                    if (altitude < 10)
                    {
                        //InputSimulator.SimulateKeyDown(VirtualKeyCode.UP);
                    }
                    else {
                        window.Browser.InvokeScript("zoomIn2");
                        //InputSimulator.SimulateKeyDown(VirtualKeyCode.ADD);
                    }                    
                }
                else if (handDistance >= (kneeDistance * 1.5))
                {
                    System.Console.WriteLine("Zoom - IN");
                   // window.Browser.InvokeScript("zoomIn1");

                   //System.Console.WriteLine("Höhe: "+height2);
                   if (altitude < 10)
                   {
                       InputSimulator.SimulateKeyDown(VirtualKeyCode.ADD);
                   }
                   else
                   {
                       InputSimulator.SimulateKeyDown(VirtualKeyCode.ADD);
                   }
                }
                else if (handDistance < kneeDistance * 0.05)
                {
                    System.Console.WriteLine("Zoom - OUT TWICE");

                    if (altitude < 10)
                    {
                        InputSimulator.SimulateKeyDown(VirtualKeyCode.SUBTRACT);
                    }
                    else
                    {
                        window.Browser.InvokeScript("zoomOut2");
                        //InputSimulator.SimulateKeyDown(VirtualKeyCode.SUBTRACT);
                    }
                }
                else if (handDistance <= kneeDistance * 0.25)
                {
                    System.Console.WriteLine("Zoom - OUT");

                    if (altitude < 10)
                    {
                        //InputSimulator.SimulateKeyDown(VirtualKeyCode.DOWN);
                    }
                    else
                    {
                        //window.Browser.InvokeScript("zoomOut1");
                        InputSimulator.SimulateKeyDown(VirtualKeyCode.SUBTRACT);
                    }
                }
                else
                {
                    System.Console.WriteLine("NO ZOOM recognized");
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.ADD);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.SUBTRACT);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.UP);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.DOWN);
                }
            }
            else
            {
                System.Console.WriteLine("NO gesture - not the same height!!");
                InputSimulator.SimulateKeyUp(VirtualKeyCode.ADD);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.SUBTRACT);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.UP);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.DOWN);
            }
        }

        //gets skeleton, to detect walk from
        //KneeLeft, AnkleLeft, FootLeft, HipRight, KneeRight, AnkleRight, FootRight
        private void detectStepForward(Joint ankleLeft, Joint ankleRight)
        {
            //TODO:
            //check seated Mode
        }

        //gets skeleton, to detect walk from
        //KneeLeft, AnkleLeft, FootLeft, HipRight, KneeRight, AnkleRight, FootRight
        private void detectStepBackwards(Skeleton skeleton)
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
            System.Console.WriteLine("head.angleX: " + xAngle);
            System.Console.WriteLine("head.angleY: " + yAngle);
            System.Console.WriteLine("head.angleZ: " + zAngle);

            if (xAngle > -0.0139 && yAngle > -0.042)
            {
                System.Console.WriteLine("right ");
            }

            else if (xAngle < -0.439 && yAngle < -0.62)
            {
                System.Console.WriteLine("left ");
            }
            else
            {
                System.Console.WriteLine("center ");
            }
        }

        //print method
        private void printHandPosition(Joint leftHand, Joint rightHand)
        {
            System.Console.WriteLine("leftHand.PositionX: " + leftHand.Position.X);
            //System.Console.WriteLine("leftHand.PositionY: " + leftHand.Position.Y);
            //System.Console.WriteLine("leftHand.PositionZ: " + leftHand.Position.Z);

            System.Console.WriteLine("rightHand.PositionX: " + rightHand.Position.X);
            //System.Console.WriteLine("rightHand.PositionY: " + rightHand.Position.Y);
            //System.Console.WriteLine("rightHand.PositionZ: " + rightHand.Position.Z);
        }

       
        private void translatePosition()
        {
            //TODO:
            /*
             Vector shoulderC = new Vector();
            Vector handR = new Vector();
            Vector handL = new Vector();
            Vector shoulderR = new Vector();
            Vector shoulderL = new Vector();

            // find positions of shoulders and hands
            foreach (SkeletonData data in skeletonFrame.Skeletons)
            {
                if (SkeletonTrackingState.Tracked == data.TrackingState)
                {
                    TODO.
                }
            }

            // translate
            // Find the center of both hands
            Vector3D pos = new Vector3D((handR.X + handL.X) / 2.0, (handR.Y + handL.Y) / 2.0, (handR.Z + handL.Z) / 2.0);
            // move to the center of both hand
            earthTransform.Translate = new Vector3D(pos.X*tfactor, pos.Y*tfactor, pos.Z);
            // scale
            // find the vector from left hand to right hand
            Vector3D hand = new Vector3D(handR.X - handL.X, handR.Y - handL.Y, handR.Z - handL.Z);
            // find the vector from left shoulder to right shoulder
            Vector3D shoulder = new Vector3D(shoulderR.X - shoulderL.X, shoulderR.Y - shoulderL.Y, shoulderR.Z - shoulderL.Z);
            // scale the earth from the difference of lengths(squared) of inter-shoulders and inter-hands
            // if same length scale to 0.8. longer inter-hand , bigger scale
            earthTransform.Scale= hand.LengthSquared - shoulder.LengthSquared + 0.8;
            // rotataion
            // get the angle and axis of inter-hands vector to rotate the earth
            hand.Normalize();
            earthTransform.Angle = Vector3D.AngleBetween(new Vector3D(1, 0, 0), hand);
            earthTransform.Axis = Vector3D.CrossProduct(new Vector3D(1, 0, 0), hand);
             */
        }

        
    }
}
