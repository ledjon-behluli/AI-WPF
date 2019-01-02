using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Windows.Threading;
using System.IO;
using System.Data;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Input;
using AiWPF.Enums;
using AiWPF.RectangleParameters;
using AiWPF.ExportImport;

namespace AiWPF
{
    public static class MapExporterImporter
    {
        private static readonly ImageBrush GreenFlag = new ImageBrush(new BitmapImage(new Uri("../../Resources/green-flag.png", UriKind.Relative)));
        private static readonly ImageBrush RedFlag = new ImageBrush(new BitmapImage(new Uri("../../Resources/red-flag.png", UriKind.Relative)));
        private static readonly ImageBrush RedFlagChecked = new ImageBrush(new BitmapImage(new Uri("../../Resources/red-flag-checked.png", UriKind.Relative)));
        private static readonly ImageBrush WhiteSpace = new ImageBrush(new BitmapImage(new Uri("../../Resources/White-Space.png", UriKind.Relative)));
        private static readonly ImageBrush GraySpace = new ImageBrush(new BitmapImage(new Uri("../../Resources/Gray-Space.png", UriKind.Relative)));
        private static readonly ImageBrush YellowSpace = new ImageBrush(new BitmapImage(new Uri("../../Resources/Yellow-Space.png", UriKind.Relative)));
        private static readonly ImageBrush CyanSpace = new ImageBrush(new BitmapImage(new Uri("../../Resources/Cyan-Space.png", UriKind.Relative)));
        private static readonly ImageBrush VioletSpace = new ImageBrush(new BitmapImage(new Uri("../../Resources/Violet-Space.png", UriKind.Relative)));

        public static TableObject ImportFromXML(string DocumentTitle)
        {
            try
            {             
                XDocument doc = XDocument.Load($@".\Maps\{DocumentTitle}.xml");
            
                string _tableTitle = doc.Root.Element("Title").Attribute("name").Value;
                int _rowCount = int.Parse(doc.Root.Element("TableDimensions").Attribute("rowCount").Value);
                int _colCount = int.Parse(doc.Root.Element("TableDimensions").Attribute("colCount").Value);
                int _animationSpeed = int.Parse(doc.Root.Element("Variables").Attribute("animation_speed").Value);
                double _uncertaintyLevel = double.Parse(doc.Root.Element("Variables").Attribute("uncertainty_level").Value);
                BackPathType _back_path_type = (doc.Root.Element("Variables").Attribute("back_path_type").Value == "Shortest") ? BackPathType.Shortest : BackPathType.Reversed;

                List<RectangleParams> _rectangleParamsList = doc.Descendants("Rectangle")
                                      .Select(r => new RectangleParams()
                                      {
                                          X = int.Parse(r.Attribute("x").Value),
                                          Y = int.Parse(r.Attribute("y").Value),
                                          Type = FindType(r.Attribute("type").Value)
                                      }).ToList();

                return new TableObject(_tableTitle, _rowCount, _colCount, _animationSpeed, _uncertaintyLevel, _back_path_type, _rectangleParamsList);
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); return null; }
        }

        /// <summary>
        ///
        /// <?xml version = "1.0" encoding="utf-8"?>
        ///  <Table>
        ///     <Title name = "Map1" />
        ///     <TableDimensions rowCount="2" colCount="2" />
        ///     <Rectangles>
        ///       <Rectangle x = "0" y="0" type="GreenFlag" />
        ///       <Rectangle x = "0" y="1" type="RedFlag" />
        ///       <Rectangle x = "1" y="0" type="GraySpace" />
        ///       <Rectangle x = "1" y="1" type="WhiteSpace" />
        ///     </Rectangles>
        ///  </Table>
        ///          
        /// </summary>       
        
        public static bool ExportToXML(TableObject tableObj)
        {            
            try
            {
                if (tableObj != null)
                {
                    Directory.CreateDirectory("Maps");

                    XDocument doc = new XDocument(
                                new XElement("Table",
                                    new XElement("Title", new XAttribute("name", tableObj.TableTitle)),
                                    new XElement("TableDimensions", new XAttribute("rowCount", tableObj.RowCount),
                                                                    new XAttribute("colCount", tableObj.ColCount)),
                                    new XElement("Variables", new XAttribute("animation_speed", tableObj.AnimationSpeed),
                                                              new XAttribute("uncertainty_level", tableObj.UncertaintyLevel),
                                                              new XAttribute("back_path_type", tableObj.BackPathType)),

                                    new XElement("Rectangles", tableObj.RectangleParamsList.Select(p => new XElement("Rectangle",
                                                                                                            new XAttribute("x", p.X),
                                                                                                            new XAttribute("y", p.Y),
                                                                                                            new XAttribute("type", p.Type)))))
                            );

                    doc.Save($@".\Maps\{tableObj.TableTitle}.xml");
                }
                return true;
            }
            catch { return false; }                  
        }

        public static RectangleType FindType(string type)
        {
            if (type == "GreenFlag")
                return RectangleType.GreenFlag;
            else if (type == "RedFlag")
                return RectangleType.RedFlag;
            else if (type == "RedFlagChecked")
                return RectangleType.RedFlag;
            else if (type == "GraySpace")
                return RectangleType.GraySpace;
            else if (type == "YellowSpace")
                return RectangleType.WhiteSpace;
            else if (type == "CyanSpace")
                return RectangleType.WhiteSpace;
            else if (type == "VioletSpace")
                return RectangleType.WhiteSpace;
            else
                return RectangleType.WhiteSpace;
        }

        public static RectangleType FindType(Rectangle r)
        {           
            if ((r.Fill as ImageBrush).ImageSource.ToString() == GreenFlag.ImageSource.ToString())
                return RectangleType.GreenFlag;
            else if ((r.Fill as ImageBrush).ImageSource.ToString() == RedFlag.ImageSource.ToString())
                return RectangleType.RedFlag;
            else if ((r.Fill as ImageBrush).ImageSource.ToString() == RedFlagChecked.ImageSource.ToString())
                return RectangleType.RedFlag;
            else if ((r.Fill as ImageBrush).ImageSource.ToString() == GraySpace.ImageSource.ToString())
                return RectangleType.GraySpace;
            else if ((r.Fill as ImageBrush).ImageSource.ToString() == YellowSpace.ImageSource.ToString())
                return RectangleType.WhiteSpace;
            else if ((r.Fill as ImageBrush).ImageSource.ToString() == CyanSpace.ImageSource.ToString())
                return RectangleType.WhiteSpace;
            else if ((r.Fill as ImageBrush).ImageSource.ToString() == VioletSpace.ImageSource.ToString())
                return RectangleType.WhiteSpace;
            else
                return RectangleType.WhiteSpace;
        }        
    }
}
