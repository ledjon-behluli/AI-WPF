using System;
using System.Collections.Generic;
using System.IO;
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

namespace AiWPF.GUIs
{
    /// <summary>
    /// Interaction logic for SaveDialog.xaml
    /// </summary>
    public partial class SaveDialog : Window
    {
        public Action MapsSaved;
         
        public SaveDialog()
        {
            InitializeComponent();         
        }
        
        public void CreateMapCheckBoxes(List<TableObject> TableObjs)
        {
            TableObjs.ForEach(tObj =>
            {
                CheckBox checkbox = new CheckBox();
                checkbox.Content = tObj.TableTitle;
                checkbox.Name = tObj.TableTitle;
                checkbox.Margin = new Thickness(10, 0, 0, 0);
                mapStackPanel.Children.Add(checkbox);
            });

            btnSaveMaps.Click += (s, e) =>
            {                
                try
                {
                    bool failed = false;
                    FileInfo[] maps = new DirectoryInfo("Maps").GetFiles("*.xml");
                    List<string> cancelMapNames = new List<string>();                    
                    mapStackPanel.Children.OfType<CheckBox>().Where(c => c.IsChecked == true).ToList().ForEach(map =>
                    {                        
                        maps.ToList().ForEach(m => {
                            if (m.Name.Split('.')[0] == map.Name)
                                if(MessageBox.Show($"Map with same name already exists '{map.Name}', do you want to override it!", "Info", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                                    m.Delete();
                                else
                                    cancelMapNames.Add(m.Name.Split('.')[0]);
                        });
                                                                                                 
                        if (!MapExporterImporter.ExportToXML(TableObjs.Where(tObj => tObj.TableTitle == map.Name && (!cancelMapNames.Contains(tObj.TableTitle))).FirstOrDefault()))
                            failed = true;
                    });

                    if (mapStackPanel.Children.OfType<CheckBox>().Where(c => c.IsChecked == true).ToList().Count != 0)
                    {
                        MapsSaved?.Invoke();
                        MessageBox.Show((!failed) ? "Process finished successfuly." : "An error occured during process!", "Info", MessageBoxButton.OK, (!failed) ? MessageBoxImage.Information : MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }                
                finally
                {
                    this.Close();
                }
            };
        }        
    }      
}
