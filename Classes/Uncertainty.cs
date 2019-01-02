using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace AiWPF
{
    public static class Uncertainty
    {
        /// <summary>
        /// Apliko ni mase te Uncertainty qysh te vendoset nga ControlKit, dmth thjesht elementet ne OpenList kur ta llogarit 
        /// elementin me F_Value me te vogel mos e merr ate por nese psh UncertaintyLevel = 50%, atehere merre jo elemtin e pare por 1/2 e 
        /// elementeve ne OpenList edhe prej tynve merre njonin Random prej asaj liste te re, qate element bone si Current.
        /// </summary>
        /// <param name="OpenList"></param>
        /// <param name="UncertaintyLevel">Niveli nga Slider</param>
        /// <returns></returns>
        public static Rectangle ApplyUncertainty(this List<Rectangle> OpenList, double UncertaintyLevel)
        {
            int num = (int)Math.Round(OpenList.Count * (UncertaintyLevel / 100.0), MidpointRounding.AwayFromZero);
            num = (num == 0) ? 1 : num;
            List<Rectangle> ModedOpenList = OpenList.OrderBy(r => (r.Tag as AiWPF.RectangleParameters.RectangleParameter).NodeParameters.F_Value).Take(num).ToList();
            Random rand = new Random();
            Rectangle Current = ModedOpenList[rand.Next(0, ModedOpenList.Count - 1)];

            return Current;
        }
    }
}
