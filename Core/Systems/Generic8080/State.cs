using System;
using System.IO;
using bEmu.Core.CPUs.Intel8080;
using bEmu.Core;
using bEmu.Core.Util;

namespace bEmu.Core.Systems.Generic8080
{
    public class State : bEmu.Core.CPUs.Intel8080.State
    {
        public Ports Ports;

        public State(ISystem system) : base(system) { }

        public void UpdatePorts(int number, byte value)
        {
            switch (number)
            {
                case 1:
                    Ports.Read1 = value;
                    break;
                case 2:
                    Ports.Read2 = value;
                    break;
                case 3:
                    break;
            }
        }
    }
}