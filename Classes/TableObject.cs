using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AiWPF.Enums;
using AiWPF.ExportImport;

namespace AiWPF
{
    /// <summary>
    /// Objecti qe reprezenton te dhenat e tabeles kur ta exportojme/importojme
    /// </summary>
    public class TableObject
    {
        public string TableTitle { get; private set; }
        public int RowCount { get; private set; }
        public int ColCount { get; private set; }
        public int AnimationSpeed { get; set; }
        public double UncertaintyLevel { get; private set; }
        public BackPathType BackPathType { get; private set; }
        public List<RectangleParams> RectangleParamsList { get; private set; }     

        public TableObject(string _TableTitle, int _RowCount, int _ColCount, int _AnimationSpeed, double _UncertaintyLevel, BackPathType _BackPathType, List<RectangleParams> _RectangleParamsList)
        {
            this.TableTitle = _TableTitle;
            this.RowCount = _RowCount;
            this.ColCount = _ColCount;
            this.AnimationSpeed = _AnimationSpeed;
            this.UncertaintyLevel = _UncertaintyLevel;
            this.BackPathType = _BackPathType;
            this.RectangleParamsList = _RectangleParamsList;
        }
    }   
}

/// <summary>
/// Po e vendosi ne namespace te vetin pasi qe mos me mu perzie anej kah 'Window:Table' ku osht klasa 'RectangleParameters'
/// </summary>
namespace AiWPF.ExportImport
{
    public class RectangleParams
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point Location { get { return new Point(this.X, this.Y); } }
        public RectangleType Type { get; set; }
    }
}
