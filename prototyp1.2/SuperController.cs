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


        public SuperController(MainWindow win)
        {
            window = win;
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



            detectStepForward(AnkleLeft, AnkleRight);
            detectPinch(wristHandL, wristHandR, KneeLeft, KneeRight);

            detectMove(head, shoulderLeft, shoulderRight, spine, wristHandL, wristHandR);
           // detectSupermanGeture(head, wristHandR, wristHandL);
            
        }

        //detects superman gesture of one hand from skeleton
        //ShoulderCenter, ShoulderLeft, ElbowLeft, WristLeft, HandLeft, ShoulderRight, ElbowRight, WristRight, HandRight
        private void detectSupermanGeture(Joint head, Joint rightHand, Joint leftHand)
        {
            //TODO:
            if ((leftHand.Position.Y < head.Position.Y) && (rightHand.Position.X > head.Position.Y))
            {
                System.Console.WriteLine(" ");
                System.Console.WriteLine(" ");

                System.Console.WriteLine("SUPERMANN");
                System.Console.WriteLine(" ");
                System.Console.WriteLine(" ");

                //window.Browser.InvokeScript("moveUp");
            }
        }


        private void detectMove(Joint head, Joint shoulderLeft, Joint shoulderRight, Joint spine, Joint leftHand, Joint rightHand) {


            if ((leftHand.Position.Y - rightHand.Position.Y) < 1)
            {

                //wenn mitte der hände 
                double handCenterX = leftHand.Position.X + rightHand.Position.X;

                double handCenterY = leftHand.Position.Y + rightHand.Position.Y;


               // System.Console.WriteLine("Pos: Handcenter X: " + handCenterX);


                System.Console.WriteLine(" ");

                //System.Console.WriteLine("Pos: shoulderLeft X:" + shoulderLeft.Position.X);
                //System.Console.WriteLine("Pos: shoulderRight X:" + shoulderRight.Position.X);

                //System.Console.WriteLine(" ");

                //System.Console.WriteLine("Pos: Handcenter Y: " + handCenterY);

                //System.Console.WriteLine("Pos: head Y:" + head.Position.Y);
                //System.Console.WriteLine("Pos: spine.Position.Y " + spine.Position.Y);
               // System.Console.WriteLine(" ");

                //in der mitte nichts tun


                //über shoulder center
                //nach oben
                if (handCenterY > head.Position.Y)
                {

                    // System.Console.WriteLine("Pos: " + handCenterY + " > " + shoulderCenter.Position.Y);
                    System.Console.WriteLine("Key - UP");

                    InputSimulator.SimulateKeyDown(VirtualKeyCode.UP);
                }
                else

                    //unter spine
                    // nach unten
                    if (handCenterY < spine.Position.Y)
                    {
                        //  System.Console.WriteLine("Pos: " + handCenterY + " > " + spine.Position.Y);
                        System.Console.WriteLine("Key - Down");
                        InputSimulator.SimulateKeyDown(VirtualKeyCode.DOWN);
                    }
                    else


                        //außerhalb linke shoulder
                        // nach links
                        if (handCenterX < shoulderLeft.Position.X)
                        {
                            //System.Console.WriteLine("Pos: shoulder left " + shoulderLeft.Position.X);

                            //7System.Console.WriteLine("Pos: " + handCenterX + " > " + shoulderLeft.Position.X);
                            System.Console.WriteLine("Key - Right");
                            System.Console.WriteLine("Pos:Feini");


                            window.Browser.InvokeScript("lookRigthOnEarth");

//                            InputSimulator.SimulateKeyDown(VirtualKeyCode.RIGHT);
                        }
                        else

                            //außerhalb rechte shoulderCenter 
                            //nach rechts
                            if (handCenterX > shoulderRight.Position.X)
                            {
                                // System.Console.WriteLine("Pos: shoulder right " + shoulderRight.Position.X);


                                //System.Console.WriteLine("Pos: " + handCenterX + " > " + shoulderRight.Position.X);
                                System.Console.WriteLine("Key - Left");
                                InputSimulator.SimulateKeyDown(VirtualKeyCode.LEFT);
                            }
                            else
                            {

                                System.Console.WriteLine("Kein MOVE");

                                InputSimulator.SimulateKeyUp(VirtualKeyCode.DOWN);
                                InputSimulator.SimulateKeyUp(VirtualKeyCode.UP);
                                InputSimulator.SimulateKeyUp(VirtualKeyCode.LEFT);
                                InputSimulator.SimulateKeyUp(VirtualKeyCode.RIGHT);

                            }

            }
         }
        
        //detects pinch of both hands
        private void detectPinch(Joint leftHand, Joint rightHand, Joint KneeLeft, Joint KneeRight)
        {
            double handCenter = leftHand.Position.X + rightHand.Position.X;
            double kneeCenter = KneeLeft.Position.X + KneeRight.Position.X;

            double handDistance = rightHand.Position.X - leftHand.Position.X;
            double kneeDistance = KneeRight.Position.X - KneeLeft.Position.X;

           // System.Console.WriteLine("Handdistance" + handDistance);

            //System.Console.WriteLine("---");
            //System.Console.WriteLine("kneeDistance" + kneeDistance);



            if (handDistance >= (kneeDistance * 2))
            {
                System.Console.WriteLine("Zoom - VERGRÖßERUNG");
                InputSimulator.SimulateKeyDown(VirtualKeyCode.ADD);
            
            }else

                if (handDistance <= kneeDistance)
            {
                System.Console.WriteLine("Zoom - KLEINER");
                InputSimulator.SimulateKeyDown(VirtualKeyCode.SUBTRACT);

            }
            else {
                System.Console.WriteLine("kein ZOOM");
                InputSimulator.SimulateKeyUp(VirtualKeyCode.ADD);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.SUBTRACT);

 
            }



            /*
            if (leftHand.Position.X < KneeLeft.Position.X && rightHand.Position.X > KneeRight.Position.X)
            {
                System.Console.WriteLine("Zoom - VERGRÖßERUNG");
                InputSimulator.SimulateKeyDown(VirtualKeyCode.ADD);
            }
            else if (leftHand.Position.X > KneeLeft.Position.X && rightHand.Position.X < KneeRight.Position.X)
            {
                System.Console.WriteLine("Zoom - KLEINER");
                InputSimulator.SimulateKeyDown(VirtualKeyCode.SUBTRACT);
            }
            else {
                System.Console.WriteLine("nix");
            }

            System.Console.WriteLine("leftHand.PositionX: " + leftHand.Position.X);
            System.Console.WriteLine("KneeLeft.PositionX: " + KneeLeft.Position.X);
            System.Console.WriteLine("rightHand.PositionX: " + rightHand.Position.X);
            System.Console.WriteLine("KneeRight.PositionX: " + KneeRight.Position.X);

            //System.Console.WriteLine("handDifferential: " + handDifferential);


            //print method
           // this.printHandPosition(leftHand, rightHand);
             */

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
            InputSimulator.SimulateKeyUp(VirtualKeyCode.UP);
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

        public void setSeatedMode(bool mode)
        {
            this.seatedModeOn = mode;
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
