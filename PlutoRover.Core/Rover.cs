using PlutoRover.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlutoRover.Core
{
    public class Rover : IVehicle
    {
        public Rover()
        {
            Orientation = Orientation.None;
            _ValidCommands = new HashSet<string>(new[] { "F", "B", "L", "R" }, StringComparer.InvariantCultureIgnoreCase);
            _Commands = new Dictionary<(string, Orientation), (int X, int Y, int Dir)>()
            {
                { ("F", Orientation.North), (0,1,0)},
                { ("F", Orientation.South), (0,-1,0)},
                { ("F", Orientation.East), (1,0,0)},
                { ("F", Orientation.West), (-1,0,0)},

                { ("B", Orientation.South), (0,1,0)},
                { ("B", Orientation.North), (0,-1,0)},
                { ("B", Orientation.West), (1,0,0)},
                { ("B", Orientation.East), (-1,0,0)},

                { ("L", Orientation.North), (0,0,-1)},
                { ("L", Orientation.East), (0,0,-1)},
                { ("L", Orientation.South), (0,0,-1)},
                { ("L", Orientation.West), (0,0,-1)},

                { ("R", Orientation.North), (0,0,1)},
                { ("R", Orientation.East), (0,0,1)},
                { ("R", Orientation.South), (0,0,1)},
                { ("R", Orientation.West), (0,0,1)},
            };
        }

        public (int X, int Y) Location { get; private set; }
        public Orientation Orientation {
            get { return _Orientation; }
            set {
                if (_Planet == null && value != Orientation.None)
                    throw new InvalidOperationException();
                else if (_Planet != null && value == Orientation.None)
                    throw new InvalidOperationException();
                _Orientation = value;
            }
        }

        private readonly HashSet<string> _ValidCommands;
        private Orientation _Orientation;

        public void Land(IPlanet planet, int? x = null, int? y = null, Orientation initialDirection = Orientation.None)
        {
            if (_Planet != null)
                throw new InvalidOperationException("The rover has already landed on a planet");
            var point = planet.GetRandomPoint();
            point = (x ?? point.X, y ?? point.Y);
            planet.Surface[point.X, point.Y] = this;
            this.Location = point;
            _Planet = planet;

            Land_SetOrientation(initialDirection);

        }

        private void Land_SetOrientation(Orientation initialDirection)
        {
            if (initialDirection != Orientation.None)
                Orientation = initialDirection;
            else
                Orientation = (Orientation)new Random().Next(1, 5);
        }

        private IPlanet _Planet;

        private readonly Dictionary<(string, Orientation), (int X, int Y, int Dir)> _Commands;
            

        public bool Travel(string fullCommand)
        {
            var commands = fullCommand.Select(c => c.ToString()).ToArray();
            if (commands.Any(c => !_ValidCommands.Contains(c)))
                throw new InvalidOperationException();

            foreach (var c in commands)
            {
                var change = _Commands[(c, this.Orientation)];
                if (!ApplyXY(change))
                    return false;
                ApplyDirection(change);
            }
            return true;
        }

        private void ApplyDirection((int X, int Y, int Dir) change)
        {
            int toShift = change.Dir;
            int iterBy = toShift >= 0 ? -1 : 1;
            int shift = toShift >= 0 ? 1 : -1;
            while (toShift != 0)
            {
                var currentDir = Orientation;
                var newDir = ShiftDirection(shift, currentDir);
                Orientation = newDir;
                toShift += iterBy;
            }
        }

        private static Orientation ShiftDirection(int shift, Orientation currentDir)
        {
            if (shift == -1)
            {
                switch (currentDir)
                {
                    case Orientation.North:
                        currentDir = Orientation.West;
                        break;
                    case Orientation.West:
                        currentDir = Orientation.South;
                        break;
                    case Orientation.South:
                        currentDir = Orientation.East;
                        break;
                    case Orientation.East:
                        currentDir = Orientation.North;
                        break;
                }
            }
            else
            {
                switch (currentDir)
                {
                    case Orientation.North:
                        currentDir = Orientation.East;
                        break;
                    case Orientation.East:
                        currentDir = Orientation.South;
                        break;
                    case Orientation.South:
                        currentDir = Orientation.West;
                        break;
                    case Orientation.West:
                        currentDir = Orientation.North;
                        break;
                }
            }

            return currentDir;
        }

        private bool ApplyXY((int X, int Y, int Dir) change)
        {
            var pos = this.Location;
            (int X, int Y) newPos = (pos.X + change.X, pos.Y + change.Y);

            newPos = WrapXIfRequired(newPos);
            newPos = WrapYIfRequired(newPos);

            var obstacle = _Planet.Surface[newPos.X, newPos.Y];
            if (obstacle != null && obstacle != this)
                return false;

            _Planet.Surface[pos.X, pos.Y] = null;
            _Planet.Surface[newPos.X, newPos.Y] = this;
            this.Location = newPos;
            return true;
            
        }

        private (int X, int Y) WrapXIfRequired((int X, int Y) newPos)
        {
            if (newPos.X > _Planet.Surface.GetUpperBound(0))
                newPos.X = 0;
            else if (newPos.X < 0)
                newPos.X = _Planet.Surface.GetUpperBound(0);
            return newPos;
        }

        private (int X, int Y) WrapYIfRequired((int X, int Y) newPos)
        {
            if (newPos.Y > _Planet.Surface.GetUpperBound(1))
                newPos.Y = 0;
            else if (newPos.Y < 0)
                newPos.Y = _Planet.Surface.GetUpperBound(1);
            return newPos;
        }
    }
}
