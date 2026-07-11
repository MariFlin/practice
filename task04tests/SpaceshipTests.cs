using Xunit;
using Moq;
using task04;

namespace task04tests
{
    public class SpaceshipTests
    {
        [Fact]
        public void Cruiser_ShouldHaveCorrectStats()
        {
            ISpaceship cruiser = new Cruiser();
            Assert.Equal(50, cruiser.Speed);
            Assert.Equal(100, cruiser.FirePower);
        }

        [Fact]
        public void Fighter_ShouldBeFasterThanCruiser()
        {
            var fighter = new Fighter();
            var cruiser = new Cruiser();
            Assert.True(fighter.Speed > cruiser.Speed);
        }

        [Fact]
        public void Fighter_ShouldHaveCorrectStats()
        {
            ISpaceship fighter = new Fighter();
            Assert.Equal(100, fighter.Speed);
            Assert.Equal(30, fighter.FirePower);
        }

        [Fact]
        public void Cruiser_MoveForward_ShouldChangePosition()
        {
            var cruiser = new Cruiser();
            cruiser.MoveForward();
            
            Assert.Equal(50, cruiser.X, precision: 5);
            Assert.Equal(0, cruiser.Y, precision: 5);
        }

        [Fact]
        public void Fighter_MoveForward_ShouldChangePosition()
        {
            var fighter = new Fighter();
            fighter.MoveForward();
            
            Assert.Equal(100, fighter.X, precision: 5);
            Assert.Equal(0, fighter.Y, precision: 5);
        }

        [Fact]
        public void Cruiser_Rotate_ShouldChangeAngle()
        {
            var cruiser = new Cruiser();
            cruiser.Rotate(45);
            
            Assert.Equal(45, cruiser.Angle);
        }

        [Fact]
        public void Fighter_Rotate_ShouldChangeAngle()
        {
            var fighter = new Fighter();
            fighter.Rotate(90);
            
            Assert.Equal(90, fighter.Angle);
        }

        [Fact]
        public void MoveForward_WithAngle90_ShouldMoveAlongY()
        {
            var fighter = new Fighter();
            fighter.Rotate(90);
            fighter.MoveForward();
            
            Assert.Equal(0, fighter.X, precision: 5);
            Assert.Equal(100, fighter.Y, precision: 5);
        }

        [Fact]
        public void Rotate_ShouldNormalizeAngle()
        {
            var fighter = new Fighter();
            fighter.Rotate(400);
            
            Assert.Equal(40, fighter.Angle);
        }

        [Fact]
        public void Rotate_WithNegativeAngle_ShouldNormalize()
        {
            var fighter = new Fighter();
            fighter.Rotate(-90);
            
            Assert.Equal(270, fighter.Angle);
        }

        [Fact]
        public void NewShip_ShouldStartAtOrigin()
        {
            var fighter = new Fighter();
            Assert.Equal(0, fighter.X, precision: 5);
            Assert.Equal(0, fighter.Y, precision: 5);
            Assert.Equal(0, fighter.Angle);
        }

        [Fact]
        public void Cruiser_Fire_ShouldCreateRocket()
        {
            var cruiser = new Cruiser();
            cruiser.Fire();
            
            Assert.NotNull(cruiser.LastRocket);
            Assert.Equal(100, cruiser.LastRocket.Power);
        }

        [Fact]
        public void Fighter_Fire_ShouldCreateRocket()
        {
            var fighter = new Fighter();
            fighter.Fire();
            
            Assert.NotNull(fighter.LastRocket);
            Assert.Equal(30, fighter.LastRocket.Power);
        }

        [Fact]
        public void Fire_RocketShouldStartAtShipPosition()
        {
            var fighter = new Fighter();
            fighter.MoveForward(); // X = 100
            fighter.Fire();
            
            Assert.Equal(100, fighter.LastRocket.X, precision: 5);
        }

        [Fact]
        public void MultipleMoves_ShouldAccumulatePosition()
        {
            var fighter = new Fighter();
            fighter.MoveForward();
            fighter.MoveForward();
            
            Assert.Equal(200, fighter.X, precision: 5);
        }
    }
}