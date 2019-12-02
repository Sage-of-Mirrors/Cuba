using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Windows.Input;

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
        public Vector3 Forward { get; private set; }

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
            Forward = Vector3.UnitZ;

            Fovy = 45.0f;
        }

        public void HandleInput(Key key)
        {
            switch (key)
            {
                case Key.W:
                    Eye = new Vector3(Eye.X, Eye.Y, Eye.Z + SPEED);
                    Target = new Vector3(Target.X, Target.Y, Target.Z + SPEED);
                    break;
                case Key.A:
                    Eye = new Vector3(Eye.X + SPEED, Eye.Y, Eye.Z);
                    Target = new Vector3(Target.X + SPEED, Target.Y, Target.Z);
                    break;
                case Key.D:
                    Eye = new Vector3(Eye.X - SPEED, Eye.Y, Eye.Z);
                    Target = new Vector3(Target.X - SPEED, Target.Y, Target.Z);
                    break;
                case Key.S:
                    Eye = new Vector3(Eye.X, Eye.Y, Eye.Z - SPEED);
                    Target = new Vector3(Target.X, Target.Y, Target.Z - SPEED);
                    break;
                case Key.Q:
                    Eye = new Vector3(Eye.X, Eye.Y + SPEED, Eye.Z);
                    Target = new Vector3(Target.X, Target.Y + SPEED, Target.Z);
                    break;
                case Key.E:
                    Eye = new Vector3(Eye.X, Eye.Y - SPEED, Eye.Z);
                    Target = new Vector3(Target.X, Target.Y - SPEED, Target.Z);
                    break;
            }
        }

        public void HandleScroll(int delta)
        {
            float change = delta / 5 * SPEED;

            Fovy += (-change) * (float)(Math.PI / 180.0f);

            if (Fovy >= 180.0f)
            {
                Fovy = 179.99f;
            }

            if (Fovy <= 0)
            {
                Fovy = 0.01f;
            }
        }
    }
}
