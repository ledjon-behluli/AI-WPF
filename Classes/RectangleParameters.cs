using System;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AiWPF.RectangleParameters
{
    public class RectangleParameter
    {
        public GridPosition GridPosition { get; set; }
        public NodeParameters NodeParameters { get; set; }
        public int ReversedHValue { get; set; } = 0;
    }

    public class GridPosition
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public Point Location { get { return new Point(this.X, this.Y); } }

        public GridPosition(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public GridPosition(GridPosition GP)
        {
            this.X = GP.X;
            this.Y = GP.Y;
        }
    }

    public class NodeParameters
    {
        public int G_Value { get; set; } = 0;
        public int H_Value { get; set; } = 0;
        public int F_Value { get; set; } = 0;
        public Rectangle Parent { get; set; }

        public NodeParameters() : this(null) { }
        public NodeParameters(Rectangle Parent)
        {
            this.Parent = Parent;
        }
    }
}
