﻿using System;
using System.Buffers.Binary;
using System.Text;
using System.IO;

namespace BayoCosTool;

/*
 * Original Author: Skyth
 * https://github.com/blueskythlikesclouds/BayoMotTool/blob/master/Source/BayoMotTool/EndianBinaryReader.cs
 * Thanks again for letting me use it!
 */

public sealed class EndianBinaryWriter : BinaryWriter
{
    public EndianBinaryWriter(Stream output) : base(output)
    {
    }

    public EndianBinaryWriter(Stream output, Encoding encoding) : base(output, encoding)
    {
    }

    public EndianBinaryWriter(Stream output, Encoding encoding, bool leaveOpen) : base(output, encoding, leaveOpen)
    {
    }

    public bool IsBigEndian { get; set; }

    public override void Write(short value)
    {
        base.Write(IsBigEndian ? BinaryPrimitives.ReverseEndianness(value) : value);
    }

    public override void Write(int value)
    {
        base.Write(IsBigEndian ? BinaryPrimitives.ReverseEndianness(value) : value);
    }

    public override void Write(float value)
    {
        if (IsBigEndian)
            base.Write(BinaryPrimitives.ReverseEndianness(BitConverter.SingleToInt32Bits(value)));
        else
            base.Write(value);
    }

    public override void Write(ushort value)
    {
        base.Write(IsBigEndian ? BinaryPrimitives.ReverseEndianness(value) : value);
    }

    public override void Write(uint value)
    {
        base.Write(IsBigEndian ? BinaryPrimitives.ReverseEndianness(value) : value);
    }
}
