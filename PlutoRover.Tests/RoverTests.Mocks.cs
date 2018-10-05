using Moq;
using PlutoRover.Core;
using PlutoRover.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlutoRover.Tests
{
    public partial class RoverTests
    {
        private const int DEFAULT_PLANET_SIZE = 75;
        private static readonly Random _rnd = new Random(0);

        private IPlanet GetPlanet(int size = DEFAULT_PLANET_SIZE)
        {
            var mock = new Mock<IPlanet>();
            mock.Setup(p => p.Surface).Returns(new object[size, size]);
            return mock.Object;
        }

        private Rover GetLandedRover(int? x = null, int? y = null)
        {
            IPlanet planet = GetPlanet();
            var rover = new Rover();
            var minX = planet.Surface.GetLowerBound(0) + 1;
            var minY = planet.Surface.GetLowerBound(1) + 1;
            var maxX = planet.Surface.GetUpperBound(0) - 1;
            var maxY = planet.Surface.GetUpperBound(1) - 1;
            var point = planet.GetRandomPoint(_rnd);
            while (point.X < minX || point.X > maxX || point.Y < minY || point.Y > maxY)
                point = planet.GetRandomPoint();
            rover.Land(planet, x ?? point.X, y ?? point.Y);
            return rover;
        }
    }
}
