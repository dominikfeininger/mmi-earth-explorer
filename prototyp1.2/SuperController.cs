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
            Joint rightHand = skeleton.Joints[JointType.WristRight];
            Joint leftHand = skeleton.Joints[JointType.WristLeft];
            Joint head = skeleton.Joints[JointType.Head];
            detectPinch(leftHand, rightHand);
        }

        //detects superman gesture of one hand from skeleton
        //ShoulderCenter, ShoulderLeft, ElbowLeft, WristLeft, HandLeft, ShoulderRight, ElbowRight, WristRight, HandRight
        private void detectSupermanGeture(Skeleton skeleton)
        {
            //TODO:
        }

        //detects pinch of both hands
        private void detectPinch(Joint leftHand, Joint rightHand)
        {
            double armDifferential = leftHand.Position.X - rightHand.Position.X;

            double diffX = leftHand.Position.X - rightHand.Position.X;
            double diffY = leftHand.Position.Y - rightHand.Position.Y;

            //print method
            this.printHandPosition(leftHand, rightHand);

        }

        //gets skeleton, to detect walk from
        //KneeLeft, AnkleLeft, FootLeft, HipRight, KneeRight, AnkleRight, FootRight
        private void detectStepForward(Skeleton skeleton)
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
            this.printHeadAngel(xAngle, yAngle, zAngle);

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
            System.Console.WriteLine("leftHand.PositionY: " + leftHand.Position.Y);
            System.Console.WriteLine("leftHand.PositionZ: " + leftHand.Position.Z);

            System.Console.WriteLine("rightHand.PositionX: " + rightHand.Position.X);
            System.Console.WriteLine("rightHand.PositionY: " + rightHand.Position.Y);
            System.Console.WriteLine("rightHand.PositionZ: " + rightHand.Position.Z);
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
