using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using AiWPF.Args;
using AiWPF.Enums;

namespace AiWPF
{
    /// <summary>
    /// Interaction logic for ControlKit.xaml
    /// </summary>
    public partial class ControlKit : Window
    {
        public static Action ClearTable;
        public static Action<int, double, BackPathType> RequestRouteCalculation;
        public static event EventHandler<UpdateTableEventArgs> UpdateTable;
        public static event EventHandler<ModeEventArgs> ModeChanged;

        private static List<string> MapNames = new List<string>();      
        private static TableObject tableObj;
        public static BackPathType BackPathType { get; set; } = BackPathType.Shortest;
        

        public ControlKit()
        {
            InitializeComponent();        
            LoadSavedMaps();
        }

        private void LoadSavedMaps()
        {
            try
            {
                Open.Items.Clear();
                FileInfo[] maps = new DirectoryInfo("Maps").GetFiles("*.xml");
                maps.ToList().ForEach(map => {
                    MenuItem mapItem = new MenuItem()
                    {
                        Header = map.Name.Split('.')[0],
                        Name = map.Name.Split('.')[0],
                    };
                    mapItem.Click += MapOpen_Click;
                    Open.Items.Add(mapItem);                                        
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            int Rows, Cols, ASpeed;
            if (!String.IsNullOrEmpty(txtName.Text))
            {
                if (int.TryParse(txtRows.Text, out Rows))
                {
                    if (int.TryParse(txtCols.Text, out Cols))
                    {
                        if (int.TryParse(txtAnimationSpeed.Text, out ASpeed))
                        {
                            Table main = new Table() { rowCount = Rows, colCount = Cols, AnimationSpeed = ASpeed, UncertaintyLevel = slider.Value, BackPathType = (Shortest.IsChecked == true) ? BackPathType.Shortest : BackPathType.Reversed };
                            main.NameTable((tableObj != null) ? tableObj.TableTitle : txtName.Text);
                            main.InitDrawGrid((tableObj != null) ? tableObj : null);

                            main.StartPointAdded += () => EndPoint.IsChecked = true;

                            MapNames.Add((tableObj != null) ? tableObj.TableTitle : txtName.Text);

                            tableObj = null;

                            btnUpdate.IsEnabled = true;
                            StartPoint.IsChecked = true;
                        }
                    }
                }
            }                            
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            int Rows, Cols, ASpeed;
            if (!String.IsNullOrEmpty(txtName.Text))
            {
                if (int.TryParse(txtRows.Text, out Rows))
                {
                    if (int.TryParse(txtCols.Text, out Cols))
                    {
                        if (int.TryParse(txtAnimationSpeed.Text, out ASpeed))
                        {
                            StartPoint.IsChecked = true;
                            UpdateTable?.Invoke(null, new UpdateTableEventArgs() { RowCount = Rows, ColCount = Cols, AnimationSpeed = ASpeed, UncertaintyLevel = slider.Value, BackPathType = (Shortest.IsChecked == true) ? BackPathType.Shortest : BackPathType.Reversed, Title = txtName.Text });
                        }
                    }
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {                        
            ClearTable?.Invoke();
        }

        private void btnFindBeacon_Click(object sender, RoutedEventArgs e)
        {
            int ASpeed;
            if (int.TryParse(txtAnimationSpeed.Text, out ASpeed))                                        
                RequestRouteCalculation?.Invoke(ASpeed, slider.Value, (Shortest.IsChecked == true) ? BackPathType.Shortest : BackPathType.Reversed);
        }


        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {            
            switch ((sender as RadioButton).Name)
            {
                case "StartPoint": { ModeChanged?.Invoke(null, new ModeEventArgs() { Mode = ModePoint.StartPoint }); }
                    break;
                case "BlockPoint": { ModeChanged?.Invoke(null, new ModeEventArgs() { Mode = ModePoint.BlockPoint }); }
                    break;
                case "EndPoint": { ModeChanged?.Invoke(null, new ModeEventArgs() { Mode = ModePoint.EndPoint }); }
                    break;
                case "Shortest": { BackPathType = BackPathType.Shortest; }
                    break;
                case "Reversed": { BackPathType = BackPathType.Reversed; }
                    break;
            }
        }

        private void MapOpen_Click(object sender, RoutedEventArgs e)
        {
            tableObj = MapExporterImporter.ImportFromXML((sender as MenuItem).Name);
            if (tableObj != null)
            {
                txtName.Text = tableObj.TableTitle;
                txtRows.Text = tableObj.RowCount.ToString();
                txtCols.Text = tableObj.ColCount.ToString();
                txtAnimationSpeed.Text = tableObj.AnimationSpeed.ToString();
                slider.Value = tableObj.UncertaintyLevel;
                Shortest.IsChecked = (tableObj.BackPathType == BackPathType.Shortest) ? true : false;
                Reversed.IsChecked = (tableObj.BackPathType == BackPathType.Reversed) ? true : false;
                BackPathType = tableObj.BackPathType;

                btnCreate_Click(null, null);
            }
        }

        private void MapSave_Click(object sender, RoutedEventArgs e)
        {
            List<TableObject> TableObjs = new List<TableObject>();
            Application.Current.Windows.OfType<Table>().ToList().ForEach(t => TableObjs.Add(t.GetMyMapData()));

            GUIs.SaveDialog saveDialog = new GUIs.SaveDialog();
            saveDialog.CreateMapCheckBoxes(TableObjs);
            saveDialog.MapsSaved += () => LoadSavedMaps();
            saveDialog.ShowDialog();            
        }

        private void debug_Click(object sender, RoutedEventArgs e)
        {
            GUIs.DebugControl debugControl = new GUIs.DebugControl();
            debugControl.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Windows.OfType<Table>().ToList().ForEach(t => t.Close());          
        }      
    }   
}
