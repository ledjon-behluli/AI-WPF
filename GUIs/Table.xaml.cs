using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using AiWPF.RectangleParameters;
using AiWPF.Args;
using AiWPF.Enums;


namespace AiWPF
{
    /// <summary>
    /// Interaction logic for Table.xaml
    /// </summary>        
    public partial class Table : Window
    {
        /// <summary>
        /// Node Parameters:
        /// H_Value (Heuristc)
        /// G_Value (Movement Cost)
        /// F_Value (H + G)
        /// OpenList => Lista e Nodeve qe duhet me u bo check
        /// ClosedLiest => Lista e Nodeve qe jane bere check
        /// 
        /// 
        /// 1) Add the starting square (or node) to the open list. 
        /// 2) Repeat the following:
        ///     a) Look for the lowest F cost square on the open list.We refer to this as the current square.
        ///     b) Switch it to the closed list. 
        ///     c) For each of the 8 squares adjacent to this current square:
        ///         If it is not walkable or if it is on the closed list, ignore it. Otherwise do the following.            
        ///         If it isn’t on the open list, add it to the open list. Make the current square the parent of this square. Record the F, G, and H costs of the square.  
        ///         If it is on the open list already, check to see if this path to that square is better, using G cost as the measure.A lower G cost means that this is a better path.If so, change the parent of the square to the current square, and recalculate the G and F scores of the square. If you are keeping your open list sorted by F score, you may need to resort the list to account for the change. 
        /// d) Stop when you:
        ///     Add the target square to the closed list, in which case the path has been found(see note below), or
        ///     Fail to find the target square, and the open list is empty.In this case, there is no path.    
        /// 3) Save the path. Working backwards from the target square, go from each square to its parent square until you reach the starting square. That is your path.                                  
        /// </summary>
        private readonly double WindowHeight = 600;
        private readonly double WindowWidth = 600;

        private bool ApplyingAStarAlgorithm = false;
        private bool CancelProcessing = false;
        private bool IsGoingBack = false;
       
        public int rowCount { get; set; }
        public int colCount { get; set; }           
        private int _AnimationSpeed;
        public int AnimationSpeed { get { return _AnimationSpeed; } set { _AnimationSpeed = (value != 0) ? value : 1; } }
        public double UncertaintyLevel { get; set; }
        public BackPathType BackPathType { get; set; } = BackPathType.Shortest;
        public ModePoint CurrentMode { get; set; } = ModePoint.StartPoint;

        #region "Brushes & Bitmaps"

        private readonly ImageBrush GreenFlag = new ImageBrush(new BitmapImage(new Uri("../../Resources/green-flag.png", UriKind.Relative)));
        private readonly ImageBrush RedFlag = new ImageBrush(new BitmapImage(new Uri("../../Resources/red-flag.png", UriKind.Relative)));
        private readonly ImageBrush RedFlagChecked = new ImageBrush(new BitmapImage(new Uri("../../Resources/red-flag-checked.png", UriKind.Relative)));
        private readonly ImageBrush WhiteSpace = new ImageBrush(new BitmapImage(new Uri("../../Resources/White-Space.png", UriKind.Relative)));
        private readonly ImageBrush GraySpace = new ImageBrush(new BitmapImage(new Uri("../../Resources/Gray-Space.png", UriKind.Relative)));
        private readonly ImageBrush YellowSpace = new ImageBrush(new BitmapImage(new Uri("../../Resources/Yellow-Space.png", UriKind.Relative)));
        private readonly ImageBrush CyanSpace = new ImageBrush(new BitmapImage(new Uri("../../Resources/Cyan-Space.png", UriKind.Relative)));
        private readonly ImageBrush VioletSpace = new ImageBrush(new BitmapImage(new Uri("../../Resources/Violet-Space.png", UriKind.Relative)));

        private readonly BitmapImage car_h_r = new BitmapImage(new Uri("../../Resources/car_h_r.png", UriKind.Relative));
        private readonly BitmapImage car_h_l = new BitmapImage(new Uri("../../Resources/car_h_l.png", UriKind.Relative));
        private readonly BitmapImage car_v_d = new BitmapImage(new Uri("../../Resources/car_v_d.png", UriKind.Relative));
        private readonly BitmapImage car_v_u = new BitmapImage(new Uri("../../Resources/car_v_u.png", UriKind.Relative));

        #endregion

        public Action StartPointAdded;
    
        private Action ClearTableHandler;
        private Action<int, double, BackPathType> RequestRouteCalculationHandler;
        private EventHandler<ModeEventArgs> ModeChangedHandler;
        private EventHandler<UpdateTableEventArgs> UpdateTableHandler;


        public void NameTable(string Title) => this.Title = Title;         
        private bool AnyGreenReactangles() => (myGrid.Children.OfType<Rectangle>()).Any(r => (r.Fill as ImageBrush).ImageSource == GreenFlag.ImageSource);              
        private bool AnyRedReactangles() => (myGrid.Children.OfType<Rectangle>()).Any(r => (r.Fill as ImageBrush).ImageSource == RedFlag.ImageSource);
        private bool IsNodeWalkable(Rectangle r) => ((r.Fill as ImageBrush).ImageSource != GraySpace.ImageSource);
        private bool IsNodeAdjacent(Rectangle r, Rectangle ParentNode) => ((((RectangleParameter)r.Tag).GridPosition.X >= ((RectangleParameter)ParentNode.Tag).GridPosition.X - 1 && ((RectangleParameter)r.Tag).GridPosition.X <= ((RectangleParameter)ParentNode.Tag).GridPosition.X + 1)             //   X - 1 <= X <= X + 1
                                                                       && (((RectangleParameter)r.Tag).GridPosition.Y >= ((RectangleParameter)ParentNode.Tag).GridPosition.Y - 1 && ((RectangleParameter)r.Tag).GridPosition.Y <= ((RectangleParameter)ParentNode.Tag).GridPosition.Y + 1));            //   Y - 1 <= Y <= Y + 1
        private bool IsNodeDiagonal(Rectangle r, Rectangle Current) => (((r.Tag as RectangleParameter).GridPosition.X == (Current.Tag as RectangleParameter).GridPosition.X - 1 || (r.Tag as RectangleParameter).GridPosition.X == (Current.Tag as RectangleParameter).GridPosition.X + 1)
                                                                       && (r.Tag as RectangleParameter).GridPosition.Y != (Current.Tag as RectangleParameter).GridPosition.Y);
        private List<Rectangle> CreateEndPointList(Rectangle CurrentPoint, List<Rectangle> CheckedEndPoints) => ((myGrid.Children.OfType<Rectangle>()).
                                                        Where(r => (r.Fill as ImageBrush).ImageSource == RedFlag.ImageSource).
                                                        Select(r => { (r.Tag as RectangleParameter).ReversedHValue = CurrentPoint.CalculateReversedHValue(r); return r; }).
                                                        OrderBy(r => (r.Tag as RectangleParameter).ReversedHValue).
                                                        Except(CheckedEndPoints).ToList());
        private void ClearYellowAndCarTracks() => (myGrid.Children.OfType<Rectangle>()).
                                                                Where(r => (r.Fill as ImageBrush).ImageSource == YellowSpace.ImageSource
                                                                        || (r.Fill as ImageBrush).ImageSource == new ImageBrush(car_h_l).ImageSource
                                                                        || (r.Fill as ImageBrush).ImageSource == new ImageBrush(car_h_r).ImageSource
                                                                        || (r.Fill as ImageBrush).ImageSource == new ImageBrush(car_v_u).ImageSource
                                                                        || (r.Fill as ImageBrush).ImageSource == new ImageBrush(car_v_d).ImageSource).
                                                                Select(r => r.Fill = WhiteSpace).ToList();         // Heki katrorat 'yellow' edhe 'car' nese ka ne 'initiate. P.s Vyen qajo ToList() ne fund, sdi pse pa to spo ban veq shnosh      
        private void ClearSearchPaths() => (myGrid.Children.OfType<Rectangle>()).
                                                                Where(r => (r.Fill as ImageBrush).ImageSource == CyanSpace.ImageSource
                                                                        || (r.Fill as ImageBrush).ImageSource == VioletSpace.ImageSource).
                                                                Select(r => r.Fill = WhiteSpace).ToList();


        public Table()
        {                        
            InitializeComponent();
            ClearTableHandler = () => { DrawGrid(); };
            RequestRouteCalculationHandler = (s, u, p) => { AnimationSpeed = s; UncertaintyLevel = u; BackPathType = p; CalculateRoute(); };
            ModeChangedHandler = (s, args) => { CurrentMode = args.Mode; };      
            UpdateTableHandler = (s, args) =>
            {
                this.Title = args.Title;
                rowCount = args.RowCount;
                colCount = args.ColCount;
                AnimationSpeed = args.AnimationSpeed;
                UncertaintyLevel = args.UncertaintyLevel;
                InitDrawGrid();
            };

            ControlKit.ClearTable += ClearTableHandler;
            ControlKit.RequestRouteCalculation += RequestRouteCalculationHandler;
            ControlKit.ModeChanged += ModeChangedHandler;
            ControlKit.UpdateTable += UpdateTableHandler;                        
        }
       
        public void InitDrawGrid(TableObject tableObj = null)
        {            
            try
            {
                ThreadedInfoBox TinfoBox = new ThreadedInfoBox();
                TinfoBox.Canceled += (o) => {
                    if (o == Operation.Creating)
                        this.Dispatcher.BeginInvoke(new Action(() => this.Close()));                   
                };
                TinfoBox.StartNewThreadInfoBox(Operation.Creating, "Creating table structure ...", this.Title);

                DrawGrid();
                if(tableObj != null)
                    FillSavedMapFields(tableObj);
                this.Show();

                TinfoBox.EndNewThreadInfoBox();                
            }
            catch { }            
        }     

        private void WindowDimensions()    
        {            
            this.ResizeMode = ResizeMode.CanResize;

            this.Height = WindowHeight;
            this.Width = WindowWidth;

            if (rowCount < colCount)
                this.Height = this.Height / (colCount / (float)rowCount);
            else if (rowCount > colCount)
                this.Width = this.Width / (rowCount / (float)colCount);

            this.ResizeMode = ResizeMode.NoResize;           
        }

        private void ClearGrid()
        {
            myGrid.Children.Clear();
            myGrid.RowDefinitions.Clear();
            myGrid.ColumnDefinitions.Clear();
        }
        
        private void DrawGrid()
        {            
            ClearGrid();
            WindowDimensions();

            for (int i = 0; i < rowCount; i++)
                myGrid.RowDefinitions.Add(new RowDefinition());

            for (int i = 0; i < colCount; i++)
                myGrid.ColumnDefinitions.Add(new ColumnDefinition());

            int rowTrack = 0;
            int colTrack = 0;
                                    
            foreach (RowDefinition rd in myGrid.RowDefinitions)
            {           
                foreach (ColumnDefinition cd in myGrid.ColumnDefinitions)
                {
                    Rectangle rect = new Rectangle()
                    {
                        Name = "rect" + rowTrack.ToString() + colTrack.ToString(),
                        Stretch = Stretch.UniformToFill,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Stretch,                                           
                        Fill = WhiteSpace,
                        Cursor = Cursors.Hand,
                        Stroke = new SolidColorBrush(Colors.Black),
                        Tag = new RectangleParameter() { GridPosition = new GridPosition(rowTrack, colTrack), NodeParameters = new NodeParameters() }
                    };

                    Grid.SetRow(rect, rowTrack); 
                    Grid.SetColumn(rect, colTrack);
                    myGrid.Children.Add(rect);

                    rect.MouseEnter += (sender, args) => { FillRectangleColor(sender, args); };
                    rect.MouseDown += (sender, args) => { FillRectangleColor(sender, args); };

                    colTrack++;                    
                }
                colTrack = 0;
                rowTrack++;
            }
        }

        private void FillSavedMapFields(TableObject tableObj)
        {            
            tableObj.RectangleParamsList.ForEach(rObj => {
                switch (rObj.Type)
                {
                    case RectangleType.GreenFlag: myGrid.Children.OfType<Rectangle>().Where(r => (r.Tag as RectangleParameter).GridPosition.Location == rObj.Location).FirstOrDefault().Fill = GreenFlag;
                        break;
                    case RectangleType.RedFlag: myGrid.Children.OfType<Rectangle>().Where(r => (r.Tag as RectangleParameter).GridPosition.Location == rObj.Location).FirstOrDefault().Fill = RedFlag;
                        break;
                    case RectangleType.GraySpace: myGrid.Children.OfType<Rectangle>().Where(r => (r.Tag as RectangleParameter).GridPosition.Location == rObj.Location).FirstOrDefault().Fill = GraySpace;
                        break;
                    case RectangleType.WhiteSpace:                        
                        break;
                    default:                        
                        break;
                };
            });                          
        }

        public TableObject GetMyMapData()
        {            
            List<ExportImport.RectangleParams> RectangleParamsList = new List<ExportImport.RectangleParams>();

            myGrid.Children.OfType<Rectangle>().ToList().
                ForEach(r => RectangleParamsList.Add(new ExportImport.RectangleParams() {
                    X = (r.Tag as RectangleParameter).GridPosition.X,
                    Y = (r.Tag as RectangleParameter).GridPosition.Y,
                    Type = MapExporterImporter.FindType(r)
                }));

            return new TableObject(this.Title, rowCount, colCount, AnimationSpeed, UncertaintyLevel, BackPathType, RectangleParamsList);
        }

        private void FillRectangleColor(object sender, MouseEventArgs args)
        {
            if (!ApplyingAStarAlgorithm)
            {
                if (args.LeftButton == MouseButtonState.Pressed)
                {
                    switch (CurrentMode)
                    {
                        case ModePoint.StartPoint:
                            {
                                if (!AnyGreenReactangles())
                                {
                                    ((Rectangle)sender).Fill = GreenFlag;                                   
                                    CurrentMode = ModePoint.EndPoint;
                                    StartPointAdded?.Invoke();                                    
                                }
                            }
                            break;
                        case ModePoint.BlockPoint:
                            {
                                ((Rectangle)sender).Fill = GraySpace;
                            }
                            break;
                        case ModePoint.EndPoint:
                            {
                                ((Rectangle)sender).Fill = RedFlag;
                            }
                            break;
                    }
                }
                else if (args.RightButton == MouseButtonState.Pressed)
                {
                    ((Rectangle)sender).Fill = WhiteSpace;
                }
            }           
        }

        private List<Rectangle> ReconstructPath (Rectangle StartPoint, Rectangle CurrentPoint)
        {            
            List<Rectangle> path = new List<Rectangle>();
            path.Add(CurrentPoint);
                    
            while ((CurrentPoint.Tag as RectangleParameter).NodeParameters.Parent != null && CurrentPoint != StartPoint)
            {                
                path.Add((CurrentPoint.Tag as RectangleParameter).NodeParameters.Parent);
                CurrentPoint = (CurrentPoint.Tag as RectangleParameter).NodeParameters.Parent;                        
            }

            return path.Except(new List<Rectangle> { StartPoint }).ToList();            
        }

        private void NewCircle(Rectangle rect)
        {
            Ellipse elip = new Ellipse()
            {
                Name = "elip",
                Height = 5,
                Width = 5,
                Cursor = Cursors.Hand,
                Fill = new SolidColorBrush(Colors.Red),
            };
            Grid.SetRow(elip, (rect.Tag as RectangleParameter).GridPosition.X);
            Grid.SetColumn(elip, (rect.Tag as RectangleParameter).GridPosition.Y);
            myGrid.Children.Add(elip);
        }
        
        private BitmapImage CarOrientaion(Rectangle OldRect, Rectangle NewRect)
        {
            BitmapImage car = car_h_r;

            if (((OldRect.Tag as RectangleParameter).GridPosition.X == (NewRect.Tag as RectangleParameter).GridPosition.X) && ((OldRect.Tag as RectangleParameter).GridPosition.Y == (NewRect.Tag as RectangleParameter).GridPosition.Y - 1))
                car = car_h_r;
            else if (((OldRect.Tag as RectangleParameter).GridPosition.X == (NewRect.Tag as RectangleParameter).GridPosition.X) && ((OldRect.Tag as RectangleParameter).GridPosition.Y == (NewRect.Tag as RectangleParameter).GridPosition.Y + 1))
                car = car_h_l;
            else if (((OldRect.Tag as RectangleParameter).GridPosition.Y == (NewRect.Tag as RectangleParameter).GridPosition.Y) && ((OldRect.Tag as RectangleParameter).GridPosition.X == (NewRect.Tag as RectangleParameter).GridPosition.X - 1))
                car = car_v_d;
            else if (((OldRect.Tag as RectangleParameter).GridPosition.Y == (NewRect.Tag as RectangleParameter).GridPosition.Y) && ((OldRect.Tag as RectangleParameter).GridPosition.X == (NewRect.Tag as RectangleParameter).GridPosition.X + 1))
                car = car_v_u;

            return car;
        }
        
        void AllowUIToUpdate()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);
            Dispatcher.PushFrame(frame);
        }
        
        private void AStarAlgorithm()
        {
            #region "Initiate" 

            CancelProcessing = false;
            ApplyingAStarAlgorithm = true;            
            bool AtLeastOnePathOpen = false;
            List<Rectangle> OpenList = new List<Rectangle>();
            List<Rectangle> ClosedList = new List<Rectangle>();
            ClearYellowAndCarTracks();
            ClearSearchPaths();
            List<List<Rectangle>> MasterPath = new List<List<Rectangle>>();

            Rectangle MasterStartPoint = (myGrid.Children.OfType<Rectangle>()).Where(r => (r.Fill as ImageBrush).ImageSource == GreenFlag.ImageSource).FirstOrDefault();            
            Rectangle StartPoint = MasterStartPoint;                        
            Rectangle Current = StartPoint;

            List<Rectangle> AccumulatedEndPointList = new List<Rectangle>();
            List<Rectangle> CheckedEndPoints = new List<Rectangle>();
            List<Rectangle> EndPointList = CreateEndPointList(StartPoint, CheckedEndPoints);
           
            int NumberOfEndPoints = EndPointList.Count;            
            Rectangle EndPoint = EndPointList.FirstOrDefault();

            #endregion

            #region "Starto nje ThreadedInfoBox me tregue procesimin"

            ThreadedInfoBox TinfoBox = new ThreadedInfoBox(WindowStartupLocation.Manual, 50, 250);
            TinfoBox.Canceled += (o) => 
            {
                if (o == Operation.Processing)
                    CancelProcessing = true;
            };
            TinfoBox.StartNewThreadInfoBox(Operation.Processing, "Processing path ...", this.Title);

            #endregion

            #region "Algoritmi"

            MoveNext:

            OpenList.Add(Current);            

            while (OpenList.Count != 0)
            {
                if (CancelProcessing) { TinfoBox.EndNewThreadInfoBox(); return; }

                Current = OpenList.ApplyUncertainty(UncertaintyLevel);           // Merre Current rectangle amo me nji uncertainty

                if (NumberOfEndPoints != 0)
                {
                    EndPointList = CreateEndPointList(Current, CheckedEndPoints);     // Checkirati distancat me endpoints qe jane me afer Current
                    EndPoint = EndPointList.FirstOrDefault();
                }

                if ((Current.Tag as RectangleParameter).GridPosition.Location == (EndPoint.Tag as RectangleParameter).GridPosition.Location)
                {
                    #region "EndPoint Found"                     

                    AtLeastOnePathOpen = true;
                    List<Rectangle> Path = new List<Rectangle>();
                    Path = ReconstructPath(StartPoint, EndPoint);
                    
                    if (!MasterPath.Contains(Path))
                        MasterPath.Add(Path);

                    if (!AccumulatedEndPointList.Contains(EndPoint))
                        AccumulatedEndPointList.Add(EndPoint);

                    NumberOfEndPoints--;
                    if (NumberOfEndPoints > 0)
                    {                       
                        if (!CheckedEndPoints.Contains(EndPoint))
                            CheckedEndPoints.Add(EndPoint);
                                                
                        StartPoint = Current;
                        
                        ClosedList.Clear();                      
                        OpenList.Clear();
                        goto MoveNext;
                    }
                    else if(NumberOfEndPoints == 0 && BackPathType == BackPathType.Shortest)
                    {
                        StartPoint = Current;
                        EndPoint = MasterStartPoint;
                        IsGoingBack = true;

                        ClosedList.Clear();
                        OpenList.Clear();
                        goto MoveNext;
                    }

                    IsGoingBack = false;
                    DrawPath(MasterPath, AccumulatedEndPointList, MasterStartPoint, TinfoBox, PathType.StartToStop);
                    return;                                   

                    #endregion
                }

                OpenList.Remove(Current);
                ClosedList.Add(Current);

                List<Rectangle> NeighborRectangles = ((myGrid.Children.OfType<Rectangle>()).CalculateHValue(EndPoint)).
                                                        Where(r => IsNodeAdjacent(r, Current)
                                                                && IsNodeWalkable(r)).ToList();

                foreach (Rectangle rect in NeighborRectangles.Except(ClosedList))
                {
                    if (IsNodeDiagonal(rect, Current))
                        continue;           // Mos i merr parasysh levizjen ne diagonale

                    if ((rect.Fill as ImageBrush).ImageSource != GreenFlag.ImageSource && (rect.Fill as ImageBrush).ImageSource != RedFlag.ImageSource && (rect.Fill as ImageBrush).ImageSource != RedFlagChecked.ImageSource)
                    {
                        rect.Fill = IsGoingBack ? VioletSpace : CyanSpace;
                        AllowUIToUpdate();                       
                    }                    

                    if (!OpenList.Contains(rect))
                    {
                        OpenList.Add(rect);
                        (rect.Tag as RectangleParameter).NodeParameters.Parent = Current;
                        rect.CalculateGValue().CalculateFValue();                        
                    }
                    else
                    {
                        Rectangle temp = new Rectangle();
                        temp.Tag = new RectangleParameter() { GridPosition = new GridPosition((rect.Tag as RectangleParameter).GridPosition), NodeParameters = new NodeParameters(Current) };
                        temp.CalculateGValue();

                        if ((temp.Tag as RectangleParameter).NodeParameters.G_Value < (rect.Tag as RectangleParameter).NodeParameters.G_Value)
                        {
                            (rect.Tag as RectangleParameter).NodeParameters.Parent = Current;
                            rect.CalculateFValue();
                        }
                    }
                }
            }

            #endregion

            #region "Handel 'EndPoint not found'"

            NumberOfEndPoints--;
            if (NumberOfEndPoints > 0)
            {
                if (!CheckedEndPoints.Contains(EndPoint))
                    CheckedEndPoints.Add(EndPoint);
                
                Current = StartPoint;                
                ClosedList.Clear();
                ClosedList.Add(MasterStartPoint);
                OpenList.Clear();
                goto MoveNext;
            }
            else if(NumberOfEndPoints == 0 && AtLeastOnePathOpen)
            {
                if(BackPathType == BackPathType.Shortest)
                {
                    StartPoint = AccumulatedEndPointList.LastOrDefault();
                    Current = StartPoint;
                    EndPoint = MasterStartPoint;

                    ClosedList.Clear();
                    OpenList.Clear();
                    goto MoveNext;
                }
                else if(BackPathType == BackPathType.Reversed)
                {
                    DrawPath(MasterPath, AccumulatedEndPointList, MasterStartPoint, TinfoBox, PathType.StartToStop);
                    return;
                }
            }           

            ApplyingAStarAlgorithm = false;
            TinfoBox.EndNewThreadInfoBox();
            ShowMessage(false, PathType.StartToStop);

            #endregion
        }

        private void DrawPath(List<List<Rectangle>> MasterPath, List<Rectangle> AccumulatedEndPointList, Rectangle MasterStartPoint, ThreadedInfoBox TinfoBox, PathType PathType, int Steps = 0)
        {
            this.Dispatcher.BeginInvoke(new Action(() => ClearYellowAndCarTracks()));
            
            TinfoBox.DisplayTextChanged?.Invoke((PathType == PathType.StartToStop) ? "Reconstructing path ..." : "Going back the path ...");

            MasterPath.ForEach(subPath => subPath.Reverse());      // Reversi items mbrenda listave 

            Rectangle OldRect = MasterPath.FirstOrDefault().FirstOrDefault();
            Task.Factory.StartNew(() =>
            {                                     
                MasterPath.ForEach(subPath =>
                {
                    if (CancelProcessing) return;
                    subPath.ForEach(r =>
                    {                        
                        if (CancelProcessing) return;
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (AccumulatedEndPointList.Any(rList => (rList.Tag as RectangleParameter).GridPosition.Location == (OldRect.Tag as RectangleParameter).GridPosition.Location &&
                                                                     (rList.Tag as RectangleParameter).GridPosition.Location != (MasterStartPoint.Tag as RectangleParameter).GridPosition.Location))
                                OldRect.Fill = RedFlagChecked;                          
                            else if ((MasterStartPoint.Tag as RectangleParameter).GridPosition.Location == (OldRect.Tag as RectangleParameter).GridPosition.Location)
                                OldRect.Fill = GreenFlag;
                            else
                                OldRect.Fill = YellowSpace;
                            r.Fill = new ImageBrush(CarOrientaion(OldRect, r));
                            OldRect = r;
                        }));
                        Steps++;
                        AllowUIToUpdate();          // Qiky funksion e mundeson te behet Update UI, ende pa u krye kjo method                                                                                                                                                        
                        Thread.Sleep(_AnimationSpeed);                        
                    });
                });
            }).ContinueWith(task => {
            if (!CancelProcessing)
            {
                this.Dispatcher.BeginInvoke(new Action(() => {
                    OldRect.Fill = (BackPathType == BackPathType.Reversed) ? 
                                   ((PathType == PathType.StartToStop) ? RedFlagChecked : YellowSpace) : GreenFlag;                                                
                }));
                    
                    TinfoBox.DisplayTextChanged?.Invoke("Waiting for confirmation ...");
                    //ShowMessage(true, PathType);                    
                    if (PathType == PathType.StartToStop && BackPathType == BackPathType.Reversed)
                    {
                        MasterPath.Reverse();       // Reversi listat mbrenda MasterPath e pastaj ne fillim te methodes bohen ashtu kshtu reverse items mbrenda seciles liste
                        DrawPath(MasterPath, AccumulatedEndPointList, MasterStartPoint, TinfoBox, PathType.StopToStart, Steps);
                    }
                }
                CancelProcessing = false;
                ApplyingAStarAlgorithm = false;
                if (PathType == PathType.StopToStart || BackPathType == BackPathType.Shortest)
                {                    
                    TinfoBox.DisplayTextChanged?.Invoke($"Number of steps traveled: {Steps}");
                    //TinfoBox.EndNewThreadInfoBox();
                }

            }, TaskScheduler.Current);            
        }
       
        private void CalculateRoute()
        {
            (myGrid.Children.OfType<Rectangle>()).Where(r => (r.Fill as ImageBrush).ImageSource == RedFlagChecked.ImageSource).Select(r => r.Fill = RedFlag).ToList();          // Boni RedFlagChecked to RedFlag
            if (AnyGreenReactangles() && AnyRedReactangles())
            {                                             
                AStarAlgorithm();                
            }
            else if (!AnyGreenReactangles() && !AnyRedReactangles())
                MessageBox.Show("A StartPoint and at least one EndPoint is missing!", $"Notice from {this.Title}", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (!AnyGreenReactangles())
                MessageBox.Show("StartPoint missing!", $"Notice from {this.Title}", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (!AnyRedReactangles())
                MessageBox.Show("At least one EndPoint is missing!", $"Notice from {this.Title}", MessageBoxButton.OK, MessageBoxImage.Error);            
        }

        private void ShowMessage(bool TargetFound, PathType PathType)
        {
            if (PathType == PathType.StartToStop)
                MessageBox.Show((TargetFound) ? "Last EndPoint is found successfuly!" : "It is not possible to find any Endpoint!", $"Notice from {this.Title}", MessageBoxButton.OK, (TargetFound) ? MessageBoxImage.Information : MessageBoxImage.Error);
            else
                MessageBox.Show((TargetFound) ? "Startpoint is found successfuly!" : "It is not possible to find Startpoint!", $"Notice from {this.Title}", MessageBoxButton.OK, (TargetFound) ? MessageBoxImage.Information : MessageBoxImage.Error);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ControlKit.ClearTable -= ClearTableHandler;
            ControlKit.RequestRouteCalculation -= RequestRouteCalculationHandler;
            ControlKit.ModeChanged -= ModeChangedHandler;
            ControlKit.UpdateTable -= UpdateTableHandler;
        }        
    }    
}
