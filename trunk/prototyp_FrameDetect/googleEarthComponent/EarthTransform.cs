using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Microsoft.mmi.Kinect.Explorer
{
    /// <summary>
    /// Trasnformation Helper Class
    /// </summary>
    public class EarthTransform
    {
        private ScaleTransform3D _scale;
        private RotateTransform3D _rotate;
        private RotateTransform3D _self;
        private double _selfAngle;
        private Vector3D _axis;
        private double _angle;
        private TranslateTransform3D _translate;

        public double Scale
        {
            get
            {
                return _scale.ScaleX;
            }
            set
            {
                _scale = new ScaleTransform3D(value, value, value);
            }
        }

        public double Angle
        {
            get
            {
                return _angle;
            }
            set
            {
                _angle = value;
                _rotate = new RotateTransform3D(new AxisAngleRotation3D(_axis, value));
            }
        }

        public Vector3D Axis
        {
            get
            {
                return _axis;
            }
            set
            {
                _axis = value;
                _rotate = new RotateTransform3D(new AxisAngleRotation3D(value, _angle));
            }
        }
        public Vector3D Translate
        {
            get
            {
                return new Vector3D(_translate.OffsetX, _translate.OffsetY, _translate.OffsetZ);
            }
            set
            {
                _translate = new TranslateTransform3D(value);
            }
        }

        public double SelfRotation
        {
            get
            {
                return _selfAngle;
            }
            set
            {
                _selfAngle = value;
                _self = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), value));
            }
        }

        public EarthTransform()
        {
            this.Scale = 1.0;
            this.Angle = 0.0;
            this.Axis = new Vector3D(0.0, 1.0, 0.0);
            this.Translate = new Vector3D(0.0, 0.0, 0.0);
            this.SelfRotation = 0.0;
        }

        public Transform3D GetTransform3D()
        {
            Transform3DGroup t3dg = new Transform3DGroup();
            t3dg.Children.Add(_scale);
            t3dg.Children.Add(_self);
            t3dg.Children.Add(_rotate);
            t3dg.Children.Add(_translate);
            return t3dg;
        }

    }

}