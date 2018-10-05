using PlutoRover.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlutoRover.Core
{
    public class Pluto : IPlanet
    {
        public Pluto()
        {
            Surface = new object[100, 100];
        }

        public object[,] Surface
        {
            get;
        }
    }
}
