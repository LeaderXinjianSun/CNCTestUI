using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNCTestUI.Models
{
    public class Param
    {
        public string SerialCOM;

        public double X1Abs;
        public double Y1Abs;
        public double Z1Abs;
        public double R1Abs;

        public double X1JogSpeed;
        public double Y1JogSpeed;
        public double Z1JogSpeed;
        public double R1JogSpeed;

        public double X1RunSpeed;
        public double Y1RunSpeed;
        public double Z1RunSpeed;
        public double R1RunSpeed;

        public MPoint InitPos;
        public double Z1SafePos;
        public double Z1CarvePos;

        public MPoint ToolPoint;
    }
    public class MPoint
    {
        public double X;
        public double Y;
        public double Z;
        public double R;
    }
    public class M1Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Speed { get; set; }
        public ushort Type { get; set; }
    }
}
