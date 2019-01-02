using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiWPF.Enums
{
    public enum ModePoint
    {
        StartPoint,
        BlockPoint,
        EndPoint
    }

    public enum Operation
    {
        Creating,
        Processing    
    }

    public enum PathType
    {
        StartToStop,
        StopToStart
    }

    public enum BackPathType
    {
        Shortest,
        Reversed
    }

    public enum RectangleType
    {
        GreenFlag,
        RedFlag,
        WhiteSpace,
        GraySpace                
    }
}
