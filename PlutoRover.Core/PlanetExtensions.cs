using PlutoRover.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlutoRover.Core
{
    public static class PlanetExtensions
    {
        public static (int X, int Y) GetRandomPoint(this IPlanet planet, Random random = null)
        {
            var lbX = planet.Surface.GetLowerBound(0);
            var ubX = planet.Surface.GetLength(0);
            var lbY = planet.Surface.GetLowerBound(1);
            var ubY = planet.Surface.GetUpperBound(1);
            random = random ?? new Random();
            return (random.Next(lbX, ubX), random.Next(lbY, ubY));
        }
    }
}
