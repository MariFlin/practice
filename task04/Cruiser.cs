using System;

namespace task04
{
    public class Cruiser : ISpaceship
    {
        public int Speed => 50;
        public int FirePower => 100;
        public Rocket LastRocket { get; private set; } = null!;

        private double _x = 0;
        private double _y = 0;
        private int _angle = 0;

        public double X => _x;
        public double Y => _y;
        public int Angle => _angle;

        public void MoveForward()
        {
            double radians = _angle * Math.PI / 180;
            _x += Speed * Math.Cos(radians);
            _y += Speed * Math.Sin(radians);
        }

        public void Rotate(int angle)
        {
            _angle += angle;
            _angle = _angle % 360;
            if (_angle < 0)
            {
                _angle += 360;
            }
        }

        public void Fire()
        {
            LastRocket = new Rocket(_x, _y, _angle, FirePower, "Cruiser");
        }
    }
}