using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlutoRover.Core;
using PlutoRover.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlutoRover.Tests
{
    [TestClass]
    public partial class RoverTests
    {

        [TestMethod]
        public void RoverCanBeCreated()
        {
            Rover rover = new Rover();
        }

        [TestMethod]
        public void RoverIsAVehicle()
        {
            IVehicle rover = new Rover();
        }

        [TestMethod]
        public void ARoverCanBeLandedOnAPlanet()
        {
            IPlanet planet = GetPlanet();
            var rover = new Rover();
            rover.Land(planet);
            (int X, int Y) coords = rover.Location;
            var objectAtCoords = planet.Surface[coords.X, coords.Y];
            Assert.AreEqual(rover, objectAtCoords);
        }

        [TestMethod]
        public void ARoverCanBeLandedOnAPlanetAtASpecificPlace()
        {
            IPlanet planet = GetPlanet();
            var rover = new Rover();
            rover.Land(planet, 7, 9);
            (int X, int Y) coords = rover.Location;
            Assert.AreEqual((7, 9), coords);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ARoverCannotLandTwice()
        {
            IPlanet planet = GetPlanet();
            var rover = new Rover();
            rover.Land(planet);
            rover.Land(planet);
        }

        [TestMethod]
        public void ANonLandedRoverHasNoOrientation()
        {
            var rover = new Rover();
            Orientation o = rover.Orientation;
            Assert.AreEqual(Orientation.None, o);
        }

        [TestMethod]
        public void ALandedRoverHasAnOrientation()
        {
            Rover rover = GetLandedRover();

            Orientation o = rover.Orientation;
            Assert.AreNotEqual(o, Orientation.None);
        }

        

        [TestMethod]
        public void AVehicleCanTravel()
        {
            IVehicle rover = GetLandedRover();
            rover.Travel("F");
        }

        [DataTestMethod]
        [DataRow(Orientation.North, "F", 0,1, Orientation.North)]
        [DataRow(Orientation.North, "B", 0,-1, Orientation.North)]

        [DataRow(Orientation.South, "F", 0,-1, Orientation.South)]
        [DataRow(Orientation.South, "B", 0,1, Orientation.South)]

        [DataRow(Orientation.East, "F", 1,0, Orientation.East)]
        [DataRow(Orientation.East, "B", -1,0, Orientation.East)]

        [DataRow(Orientation.West, "F", -1,0, Orientation.West)]
        [DataRow(Orientation.West, "B", 1,0, Orientation.West)]

        [DataRow(Orientation.North, "L", 0,0, Orientation.West)]
        [DataRow(Orientation.West, "L", 0, 0, Orientation.South)]
        [DataRow(Orientation.South, "L", 0, 0, Orientation.East)]
        [DataRow(Orientation.East, "L", 0, 0, Orientation.North)]

        [DataRow(Orientation.North, "R", 0, 0, Orientation.East)]
        [DataRow(Orientation.East, "R", 0, 0, Orientation.South)]
        [DataRow(Orientation.South, "R", 0, 0, Orientation.West)]
        [DataRow(Orientation.West, "R", 0, 0, Orientation.North)]
        public void TravellingIncrementsInDirectionOfTravel(Orientation setTo, string command, int incrementX, int incrementY, Orientation newDirection)
        {
            Rover rover = GetLandedRover();
            var pos = rover.Location;
            rover.Orientation = setTo;
            var expectedPos = (pos.X + incrementX, pos.Y + incrementY);
            rover.Travel(command);
            Assert.AreEqual(expectedPos, rover.Location);
            Assert.AreEqual(newDirection, rover.Orientation);
        }

        [DataTestMethod]
        [DataRow("F", 0, 0, Orientation.South, 0, DEFAULT_PLANET_SIZE - 1)]
        [DataRow("F", 0, 0, Orientation.West, DEFAULT_PLANET_SIZE - 1, 0)]
        [DataRow("B", 0, 0, Orientation.North, 0, DEFAULT_PLANET_SIZE - 1)]
        [DataRow("B", 0, 0, Orientation.East, DEFAULT_PLANET_SIZE - 1, 0)]

        [DataRow("F", DEFAULT_PLANET_SIZE - 1, DEFAULT_PLANET_SIZE - 1, Orientation.North, DEFAULT_PLANET_SIZE - 1, 0)]
        [DataRow("F", DEFAULT_PLANET_SIZE - 1, DEFAULT_PLANET_SIZE - 1, Orientation.East, 0, DEFAULT_PLANET_SIZE - 1)]
        [DataRow("B", DEFAULT_PLANET_SIZE - 1, DEFAULT_PLANET_SIZE - 1, Orientation.South, DEFAULT_PLANET_SIZE - 1, 0)]
        [DataRow("B", DEFAULT_PLANET_SIZE - 1, DEFAULT_PLANET_SIZE - 1, Orientation.West, 0, DEFAULT_PLANET_SIZE - 1)]
        public void WhenARoverIsAtAnEdgeAndMovesItWraps(string command, int startX, int startY, Orientation startDir, int endX, int endY)
        {
            Rover rover = GetLandedRover(startX, startY);
            var startPos = rover.Location;
            Assert.AreEqual((startX, startY), startPos);
            rover.Orientation = startDir;
            rover.Travel(command);

            var endPos = rover.Location;
            Assert.AreEqual((endX, endY), endPos);
        }

        [DataTestMethod]
        [DataRow("FFBFRFF", 0,0, Orientation.North, 2,2,Orientation.East)]
        public void TheRoverCanTakeMultipleSimultaneousCommands(string command, int startX, int startY, Orientation startDir, int endX, int endY, Orientation endDir)
        {
            Rover rover = GetLandedRover(startX, startY);
            var pos = rover.Location;
            rover.Orientation = startDir;
            var expectedPos = (endX, endY);
            rover.Travel(command);
            Assert.AreEqual(expectedPos, rover.Location);
            Assert.AreEqual(endDir, rover.Orientation);
        }

        [TestMethod]
        public void IfThereIsAnObstacleTheRoverStopsAndReportsItsPosition()
        {
            IPlanet planet = GetPlanet();
            var rover = new Rover();
            rover.Land(planet, 1,1, Orientation.North);

            // Insert an obstacle at (2,2)
            planet.Surface[2, 2] = new object();
            var travelSuccess = rover.Travel("FRF");
            Assert.AreEqual((1,2), rover.Location);
            Assert.AreEqual(false, travelSuccess);
        }
        
    }
}
