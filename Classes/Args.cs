using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiWPF.Args
{
    public class UpdateTableEventArgs : EventArgs
    {
        public string Title { get; set; }
        public int RowCount { get; set; }
        public int ColCount { get; set; }
        public int AnimationSpeed { get; set; }
        public double UncertaintyLevel { get; set; }
        public AiWPF.Enums.BackPathType BackPathType { get; set; }
    }

    public class ModeEventArgs : EventArgs
    {
        public AiWPF.Enums.ModePoint Mode { get; set; }
    }

    public class SavedMapEventArgs : EventArgs
    {
        public string RowCount { get; set; }
        public string ColCount { get; set; }
        public string AnimationSpeed { get; set; }
        public double UncertaintyLevel { get; set; }
        public AiWPF.Enums.BackPathType BackPathType { get; set; }
    }
}
