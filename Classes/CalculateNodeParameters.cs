using System;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AiWPF.RectangleParameters;


namespace AiWPF
{
    public static class CalculateNodeParameters 
    {
        private static readonly ImageBrush GraySpace = new ImageBrush(new BitmapImage(new Uri("../../Resources/Gray-Space.png", UriKind.Relative)));       
        private static bool Is_CurrentRectangle_Horizontal_or_Vertical_On_Its_ParentNode(Rectangle currentRectangle) => (((currentRectangle.Tag as RectangleParameter).GridPosition.X == (((currentRectangle.Tag as RectangleParameter).NodeParameters.Parent.Tag) as RectangleParameter).GridPosition.X) || ((currentRectangle.Tag as RectangleParameter).GridPosition.Y == (((currentRectangle.Tag as RectangleParameter).NodeParameters.Parent.Tag) as RectangleParameter).GridPosition.Y));

        public static int CalculateReversedHValue(this Rectangle StartPoint, Rectangle EndPoint)
        {
            int ReversedHValue = 0;
            
            int StartPoint_X = ((RectangleParameter)StartPoint.Tag).GridPosition.X;
            int StartPoint_Y = ((RectangleParameter)StartPoint.Tag).GridPosition.Y;

            int EndPoint_X = ((RectangleParameter)EndPoint.Tag).GridPosition.X;
            int EndPoint_Y = ((RectangleParameter)EndPoint.Tag).GridPosition.Y;              
            
            if (StartPoint_X <= EndPoint_X && StartPoint_Y <= EndPoint_Y)
            {
                ReversedHValue = ((EndPoint_X - StartPoint_X) + (EndPoint_Y - StartPoint_Y));
            }
            else if (StartPoint_X >= EndPoint_X && StartPoint_Y <= EndPoint_Y)
            {
                ReversedHValue = ((StartPoint_X - EndPoint_X) + (EndPoint_Y - StartPoint_Y));
            }
            else if (StartPoint_X <= EndPoint_X && StartPoint_Y >= EndPoint_Y)
            {
                ReversedHValue = ((EndPoint_X - StartPoint_X) + (StartPoint_Y - EndPoint_Y));
            }
            else if (StartPoint_X >= EndPoint_X && StartPoint_Y >= EndPoint_Y)
            {
                ReversedHValue = ((StartPoint_X - EndPoint_X) + (StartPoint_Y - EndPoint_Y));
            }
            
            return ReversedHValue;
        }

        public static IEnumerable<Rectangle> CalculateHValue(this IEnumerable<Rectangle> Rectangles, Rectangle EndPoint)
        {
            int EndPoint_X = ((RectangleParameter)EndPoint.Tag).GridPosition.X;
            int EndPoint_Y = ((RectangleParameter)EndPoint.Tag).GridPosition.Y;

            foreach (Rectangle rect in Rectangles)
            {
                int Rect_X = ((RectangleParameter)rect.Tag).GridPosition.X;
                int Rect_Y = ((RectangleParameter)rect.Tag).GridPosition.Y;

                if (((ImageBrush)rect.Fill).ImageSource != GraySpace.ImageSource)
                {
                    if (Rect_X <= EndPoint_X && Rect_Y <= EndPoint_Y)
                    {
                        ((RectangleParameter)rect.Tag).NodeParameters.H_Value = ((EndPoint_X - Rect_X) + (EndPoint_Y - Rect_Y));                                                
                    }
                    else if (Rect_X >= EndPoint_X && Rect_Y <= EndPoint_Y)
                    {
                        ((RectangleParameter)rect.Tag).NodeParameters.H_Value = ((Rect_X - EndPoint_X) + (EndPoint_Y - Rect_Y));
                    }
                    else if (Rect_X <= EndPoint_X && Rect_Y >= EndPoint_Y)
                    {
                        ((RectangleParameter)rect.Tag).NodeParameters.H_Value = ((EndPoint_X - Rect_X) + (Rect_Y - EndPoint_Y));
                    }
                    else if (Rect_X >= EndPoint_X && Rect_Y >= EndPoint_Y)
                    {
                        ((RectangleParameter)rect.Tag).NodeParameters.H_Value = ((Rect_X - EndPoint_X) + (Rect_Y - EndPoint_Y));
                    }
                }               
            }

            return Rectangles;
        }

        public static Rectangle CalculateGValue(this Rectangle currentRectangle, Rectangle ParentNode = null)
        {
            if(Is_CurrentRectangle_Horizontal_or_Vertical_On_Its_ParentNode(currentRectangle))
            {
                (currentRectangle.Tag as RectangleParameter).NodeParameters.G_Value = ((currentRectangle.Tag as RectangleParameter).NodeParameters.Parent.Tag as RectangleParameter).NodeParameters.G_Value + 10;
            }
            else
            {
                (currentRectangle.Tag as RectangleParameter).NodeParameters.G_Value = ((currentRectangle.Tag as RectangleParameter).NodeParameters.Parent.Tag as RectangleParameter).NodeParameters.G_Value + 14;
            }
            
            return currentRectangle;
        }
        
        public static Rectangle CalculateFValue(this Rectangle currentRectangle)
        {
            ((RectangleParameter)currentRectangle.Tag).NodeParameters.F_Value = ((RectangleParameter)currentRectangle.Tag).NodeParameters.H_Value + ((RectangleParameter)currentRectangle.Tag).NodeParameters.G_Value;

            return currentRectangle;
        }        
    }       
}
