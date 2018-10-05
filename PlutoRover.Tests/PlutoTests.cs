using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlutoRover.Core;
using PlutoRover.Interfaces;

namespace PlutoRover.Tests
{
    [TestClass]
    public class PlutoTests
    {
        [TestMethod]
        public void CanCreatePluto()
        {
            Pluto pluto = new Pluto();
        }

        [TestMethod]
        public void PlutoIsAPlanet()
        {
            IPlanet pluto = new Pluto();
        }

        [TestMethod]
        public void PlutoHasASurface()
        {
            IPlanet pluto = new Pluto();
            object[,] surface = pluto.Surface;
            Assert.IsNotNull(surface);
        }

        [TestMethod]
        public void PlutoIs100By100()
        {
            IPlanet pluto = new Pluto();
            object[,] surface = pluto.Surface;
            var minX = 0;
            var minY = 0;
            var lenX = 100;
            var lenY = 100;

            Assert.AreEqual(minX, surface.GetLowerBound(0));
            Assert.AreEqual(minY, surface.GetLowerBound(1));
            Assert.AreEqual(lenX, surface.GetLength(0));
            Assert.AreEqual(lenY, surface.GetLength(1));
        }

        
    }
}
