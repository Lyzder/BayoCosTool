using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BayoCosTool
{
    enum Globals: UInt16
    {
        HEADER_SIZE = 16,
        STRUCT_SIZE = 148
    }

    public class CosHeader
    {
        uint size;
        uint version;
        uint offsetEntries;
        uint numEntries;

        /// <summary>
        /// Cos Header constructor
        /// </summary>
        /// <param name="size"></param>
        /// <param name="version"></param>
        /// <param name="offsetEntries"></param>
        /// <param name="numEntries"></param>
        public CosHeader(uint size, uint version, uint offsetEntries, uint numEntries)
        {
            this.size = size;
            this.version = version;
            this.offsetEntries = offsetEntries;
            this.numEntries = numEntries;
        }

        public uint GetSize()
        {
            return size;
        }

        public uint GetVersion()
        {
            return version;
        }

        public uint GetOffset()
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

        /// <summary>
        /// Cos color struct constructor
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
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
        sbyte[] name;

        /// <summary>
        /// Cos entry constructor
        /// </summary>
        public CosEntry()
        {
            colors = new CosColor[7];
            name = new sbyte[64];
        }

        public CosColor[] GetColors()
        {
            return colors;
        }

        public sbyte[] GetName()
        {
            return name;
        }
    }

    public class Cos
    {
        CosHeader header;
        CosHeader? header2;
        List<CosEntry> entries;

        /// <summary>
        /// Cos constructor
        /// </summary>
        /// <param name="header"></param>
        public Cos(CosHeader header)
        {
            this.header = header;
            entries = new List<CosEntry>();
        }

        /// <summary>
        /// Cos constructor
        /// </summary>
        /// <param name="size"></param>
        /// <param name="version"></param>
        /// <param name="offsetEntries"></param>
        /// <param name="numEntries"></param>
        public Cos(uint size, uint version, uint offsetEntries, uint numEntries)
        {
            header = new CosHeader(size, version, offsetEntries, numEntries);
            entries = new List<CosEntry>();
        }

        /// <summary>
        /// Set second header
        /// </summary>
        /// <param name="header2"></param>
        public void SetHeader2(CosHeader header2)
        {
            this.header2 = header2;
        }

        /// <summary>
        /// Gets one of the cos file headers. 0 for the first header. 1 for the second header.
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public CosHeader? GetHeader(short index)
        {
            if (index == 0)
                return header;
            else
                return header2;
        }

        public List<CosEntry> GetEntries()
        {
            return entries;
        }
    }
}
