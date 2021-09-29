using System;
using System.Collections.Generic;
using System.IO;
using bEmu.Core;
using bEmu.Core.System;
using bEmu.Core.Util;

namespace bEmu.Core.CPU.Z80
{
    public class State : Core.System.State<ushort, ushort>
    {
        public bool AlternativeRegisters { get; set; }
        public RegisterSet Main { get; }
        public RegisterSet Alt { get; }
        public bool EnableInterrupts { get; set; }
        public Ports Ports { get; set; }

        public byte A 
        {
            get => AlternativeRegisters ? Alt.A : Main.A;
            set { if (AlternativeRegisters) Alt.A = value; else Main.A = value; }
        }
        public byte B
        {
            get => AlternativeRegisters ? Alt.B : Main.B;
            set { if (AlternativeRegisters) Alt.B = value; else Main.B = value; }
        }
        public byte C
        {
            get => AlternativeRegisters ? Alt.C : Main.C;
            set { if (AlternativeRegisters) Alt.C = value; else Main.C = value; }
        }
        public byte D
        {
            get => AlternativeRegisters ? Alt.D : Main.D;
            set { if (AlternativeRegisters) Alt.D = value; else Main.D = value; }
        }
        public byte E
        {
            get => AlternativeRegisters ? Alt.E : Main.E;
            set { if (AlternativeRegisters) Alt.E = value; else Main.E = value; }
        }
        public byte F
        {
            get => AlternativeRegisters ? Alt.F : Main.F;
            set { if (AlternativeRegisters) Alt.F = value; else Main.F = value; }
        }
        public byte H
        {
            get => AlternativeRegisters ? Alt.H : Main.H;
            set { if (AlternativeRegisters) Alt.H = value; else Main.H = value; }
        }
        public byte L
        {
            get => AlternativeRegisters ? Alt.L : Main.L;
            set { if (AlternativeRegisters) Alt.L = value; else Main.L = value; }
        }
        public ushort AF
        {
            get => AlternativeRegisters ? Alt.AF : Main.AF;
            set { if (AlternativeRegisters) Alt.AF = value; else Main.AF = value; }
        }
        public ushort BC
        {
            get => AlternativeRegisters ? Alt.BC : Main.BC;
            set { if (AlternativeRegisters) Alt.BC = value; else Main.BC = value; }
        }
        public ushort DE
        {
            get => AlternativeRegisters ? Alt.DE : Main.DE;
            set { if (AlternativeRegisters) Alt.DE = value; else Main.DE = value; }
        }
        public ushort HL
        {
            get => AlternativeRegisters ? Alt.HL : Main.HL;
            set { if (AlternativeRegisters) Alt.HL = value; else Main.HL = value; }
        }
        public Flags Flags
        {
            get => AlternativeRegisters ? Alt.Flags : Main.Flags;
            set { if (AlternativeRegisters) Alt.Flags = value; else Main.Flags = value; }
        }

        public ushort IX { get; set; }
        public ushort IY { get; set; }

        public override byte[] SaveState()
        {
            throw new NotImplementedException();
        }

        public override void LoadState(byte[] value)
        {
            throw new NotImplementedException();
        }

        public State(IRunnableSystem system) : base(system) 
        { 
            Main = new RegisterSet(this);
            Alt = new RegisterSet(this);
            Ports = new Ports(0x100);
        }
    }
}