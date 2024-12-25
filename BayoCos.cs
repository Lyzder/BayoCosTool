using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BayoCosTool
{
    public class CosHeader
    {
        uint size;
        uint version;
        uint offsetEntries;
        uint numEntries;

        public CosHeader(uint size, uint version, uint offsetEntries, uint numEntries)
        {
            this.size = size;
            this.version = version;
            this.offsetEntries = offsetEntries;
            this.numEntries = numEntries;
        }

        public uint GetOffsets()
        {
            return offsetEntries;
        }

        public uint GetNumOfEntries()
        {
            return numEntries;
        }
    }

    public class CosColor
    {
        public float Red { get; set; }
        public float Green { get; set; }
        public float Blue { get; set; }

        public CosColor(float red, float green, float blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }
    }

    public class CosEntry
    {
        CosColor[] colors;
        char[] name;

        public CosEntry()
        {
            colors = new CosColor[8];
            name = new char[64];
        }
    }

    public class Cos
    {
        CosHeader header;
        CosHeader? header2;
        CosEntry[] entries;

        public Cos(CosHeader header)
        {
            this.header = header;
            entries = new CosEntry[8];
        }

        public void SetHeader2(CosHeader header2)
        {
            this.header2 = header2;
        }
    }
}
