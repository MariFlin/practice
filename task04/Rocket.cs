namespace task04
{
    public class Rocket
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int Angle { get; set; }
        public int Power { get; set; }
        public string OwnerShip { get; set; }

        public Rocket(double x, double y, int angle, int power, string ownerShip)
        {
            X = x;
            Y = y;
            Angle = angle;
            Power = power;
            OwnerShip = ownerShip;
        }
    }
}