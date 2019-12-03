using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Windows.Input;
using System.Drawing;

namespace Cuba
{
    public class Camera
    {
        public const float SPEED = 1.0f;

        public float Fovy { get; private set; }
        public Vector3 Eye { get; private set; }
        public Vector3 Target { get; private set; }
        public Vector3 Up { get; private set; }
        public Vector3 Right { get; private set; }
        public Vector3 Forward { get { return Vector3.Cross(Right, Up); } }

        private Point m_prevPoint;

        public Matrix4 ViewMatrix
        {
            get { return Matrix4.LookAt(Eye, Target, Up); }
        }

        public Camera()
        {
            Eye = new Vector3(0, 0, -10);
            Target = new Vector3(0.0f, 0.0f, 1.0f);

            Up = Vector3.UnitY;
            Right = Vector3.UnitX;

            Fovy = 45.0f;
        }

        public void HandleInput(Key key)
        {
            Vector3 direction = Vector3.Zero;

            switch (key)
            {
                case Key.W:
                    direction += SPEED * Forward;
                    break;
                case Key.S:
                    direction -= SPEED * Forward;
                    break;
                case Key.A:
                    direction += SPEED * Right;
                    break;
                case Key.D:
                    direction -= SPEED * Right;
                    break;
                case Key.Q:
                    direction += SPEED * Up;
                    break;
                case Key.E:
                    direction -= SPEED * Up;
                    break;
            }

            Eye += direction;
            Target += direction;
        }

        public void HandleScroll(int delta)
        {
            Vector3 direction = (delta / 150.0f * SPEED) * Forward;

            Eye += direction;
            Target += direction;
        }

        public void HandleRotate(System.Windows.Forms.MouseEventArgs e)
        {
            float delta_x = e.X - m_prevPoint.X;
            float delta_y = e.Y - m_prevPoint.Y;

            delta_y /= 220.0f;
            delta_x /= 220.0f;

            Matrix3 test;
            Matrix3.CreateRotationX(delta_y, out test);

            Matrix3 test2;
            Matrix3.CreateRotationY(delta_x, out test2);

            Up = Vector3.Transform(Up, test).Normalized();
            Right = Vector3.Transform(Right, test2).Normalized();

            Target = Vector3.Cross(Up, Right);

            m_prevPoint = e.Location;
        }
    }
}
