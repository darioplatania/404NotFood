using Gadgeteer.Modules.GHIElectronics;
using System;
using System.Collections;
using System.Net;
using Microsoft.SPOT;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using GHI.Glide;
using GHI.Glide.UI;
using System.IO;
using System.Text;
using System.Net.Sockets;

namespace fez_spider
{
    public partial class Program
    {

        #region Ghi.Glide Definitions 

        private static GHI.Glide.Display.Window _mainwindow;
        private static GHI.Glide.Display.Window _errorWindow;
        private static GHI.Glide.Display.Window _menu;
        private static GHI.Glide.Display.Window _cancel;
        private static GHI.Glide.Display.Window _ordina;
        private static GHI.Glide.Display.Window _pagamento;
        private GHI.Glide.UI.Button _startbtn;
        private GHI.Glide.UI.Button _deleteBtn;
        private GHI.Glide.UI.Button _ordBtn;
        private GHI.Glide.UI.Button _payBtn;
        private GHI.Glide.UI.Button _backBtn;
        private GHI.Glide.UI.Button _siBtn;
        private GHI.Glide.UI.Button _noBtn;
        private GHI.Glide.UI.DataGrid _dataGrid;
        private GHI.Glide.UI.DataGrid _gridOrdine;
        private GHI.Glide.UI.TextBlock _loadingLbl;
        private GHI.Glide.UI.TextBlock _pCounter;
        private GHI.Glide.UI.TextBlock _finalPrice;
        private GHI.Glide.UI.TextBlock _errMsg;
        private GHI.Glide.UI.TextBlock _ingredients;
        private GHI.Glide.UI.TextBlock _paypal;

        #endregion

        private static Font font = Resources.GetFont(Resources.FontResources.NinaB);
        private string selected_id;
        private string selected_name;
        private Double selected_price;
        private int selected_qnt;
        private int selected_row = 0;
        byte[] result = new byte[65536];


        private static string orderId = null;

        /* Socket Variables */
        private const String HOST = "192.168.1.70";
        private const int PORT = 4096;
        

        private const String NEW_ORDER      = "NEW_ORDER\r\n";
        private const String PAYMENT        = "PAYMENT\r\n";
        private const String CLOSE          = "CLOSE\r\n";
        private const String CANCEL_ORDER   = "CANCEL_ORDER\r\n";
        private const String UPDATE_ORDER   = "UPDATE_ORDER\r\n";


        private static String ORDER_CMD = null;

        private ArrayList payment = new ArrayList();
        //private String url = "http://192.168.100.1:8080/food/webapi/food";
        private String url = "http://404notfood.sloppy.zone/food/webapi/food";
        HttpWebRequest req;
        private HttpWebResponse res;
        private Stream stream;
        private StreamReader sr;


        
        private static ArrayList menu;
        private static Orders orders;
        private SocketClient sockWrap;


        #region Fez Network Configuration

        private static String ip_address = "192.168.2.2";
        private static String subnet     = "255.255.255.0";
        private static String gateway    = "192.168.2.1";
        private static String[] dns      = { "8.8.8.8", "8.8.4.4" };

        #endregion

        #region Fez Initialization on Startup
        private void initFezSettings()
        {
            /*Use Debug.Print to show messages in Visual Studio's "Output" window during debugging*/
            Debug.Print("Program Started");


            /*
             * Configure Joypad
             * Create a timer & run method timer_trick when thr timer ticks (for joystick)
             */
            GT.Timer timer = new GT.Timer(200);
            timer.Tick += Timer_Tick;
            timer.Start();

            /* MainWindow */
            _mainwindow = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.Window));
            _startbtn = (GHI.Glide.UI.Button)_mainwindow.GetChildByName("startbtn");
            _loadingLbl = (GHI.Glide.UI.TextBlock)_mainwindow.GetChildByName("loading_lbl");

            // MainWindow.Adding Service Logo
            Image _Servicelogo = new Image("service-logo", 255, 0, 0, 320, 100);
            _mainwindow.AddChild(_Servicelogo);
            _Servicelogo.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.logo_food), Bitmap.BitmapImageType.Jpeg);


            /* Menu */
            _menu = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.Menu));
            _dataGrid = (GHI.Glide.UI.DataGrid)_menu.GetChildByName("dataGrid");
            _dataGrid.SelectedIndex = selected_row;
            _pCounter = (GHI.Glide.UI.TextBlock)_menu.GetChildByName("pCounter");
            _errMsg = (GHI.Glide.UI.TextBlock)_menu.GetChildByName("errMsg");
            _ingredients = (GHI.Glide.UI.TextBlock)_menu.GetChildByName("ingredientsLbl");
            _ordBtn = (GHI.Glide.UI.Button)_menu.GetChildByName("ordBtn");
            _deleteBtn = (GHI.Glide.UI.Button)_menu.GetChildByName("deleteBtn");

            /* Cancel */
            _cancel = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.Annulla));
            _siBtn = (GHI.Glide.UI.Button)_cancel.GetChildByName("siBtn");
            _noBtn = (GHI.Glide.UI.Button)_cancel.GetChildByName("noBtn");

            /* Order */
            _ordina = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.Ordina));
            _backBtn = (GHI.Glide.UI.Button)_ordina.GetChildByName("backBtn");
            _payBtn = (GHI.Glide.UI.Button)_ordina.GetChildByName("payBtn");
            _finalPrice = (GHI.Glide.UI.TextBlock)_ordina.GetChildByName("finalPrice");
            _gridOrdine = (DataGrid)_ordina.GetChildByName("gridOrdine");

            /* NetworkErrorWindow */
            _errorWindow = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.ErrorWindow));
            // NetworkErrorWindow.Adding Error Logo
            Image _Errorlogo = new Image("error-logo", 255, 0, 0, displayTE35.Width, displayTE35.Height);
            _errorWindow.AddChild(_Errorlogo);
            _Errorlogo.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.Connection_error), Bitmap.BitmapImageType.Jpeg);

            
            /* Register Events to Buttons */

            _startbtn.PressEvent += Start_PressEvent;
            plus.ButtonPressed += Plus_ButtonPressed;
            minus.ButtonPressed += Minus_ButtonPressed;
            _ordBtn.Enabled = false;
            _ordBtn.PressEvent += _ordBtn_PressEvent;
            _deleteBtn.PressEvent += deleteBtn_PressEvent;
            _dataGrid.TapCellEvent += new OnTapCell(dataGrid_TapCellEvent);
            _siBtn.TapEvent += _siBtn_TapEvent;
            _noBtn.TapEvent += _noBtn_TapEvent;
            _backBtn.TapEvent += _backBtn_TapEvent;


            
            /* Init Datagrids */
            
            /* _menu -> _dataGrid */
            _dataGrid.AddColumn(new DataGridColumn("ID", 0));
            _dataGrid.AddColumn(new DataGridColumn("MEAL", 175));
            _dataGrid.AddColumn(new DataGridColumn("PRICE", 50));
            _dataGrid.AddColumn(new DataGridColumn("QTY", 30));
            _dataGrid.RowCount = 4;
            _dataGrid.Render();

            
            /* _ordina -> _gridOrdine */
            _gridOrdine.AddColumn(new DataGridColumn("ID", 0));
            _gridOrdine.AddColumn(new DataGridColumn("MEAL", 175));
            _gridOrdine.AddColumn(new DataGridColumn("PRICE", 50));
            _gridOrdine.AddColumn(new DataGridColumn("QTY", 30));
            _gridOrdine.Render();


            /* End Init Datagrids */

            /* Shut down Light on Buttons */
            plus.TurnLedOff();
            minus.TurnLedOff();
            

            /* Initialize Glide */
            GlideTouch.Initialize();
            Glide.FitToScreen = true;

            /*Ethernet Configuration*/

            ethernetJ11D.DebugPrintEnabled = true;
            ethernetJ11D.UseStaticIP(ip_address, subnet, gateway, dns);
            ethernetJ11D.UseThisNetworkInterface();
            ethernetJ11D.NetworkUp += ethernetJ11D_NetworkUp;
            ethernetJ11D.NetworkDown += ethernetJ11D_NetworkDown;
            



        }
        #endregion

        /*This method is run when the mainboard is powered up or reset*/
        void ProgramStarted()
        {

            menu = new ArrayList();
            orders = new Orders();
            initFezSettings();
                

        }

        #region Custom Function Implementation 

        private Socket connectToDesktop(String hostname, int port)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse(hostname);
            IPEndPoint serverEndPoint = new IPEndPoint(ipAddress, port);

            socket.Connect(serverEndPoint);

            Debug.Print("avail: " + socket.Available.ToString());

            return socket;




        }
        private ArrayList downloadMenu(String url)
        {
            /*inizio get menu*/
            Debug.Print("GET MENU");
            ArrayList al = new ArrayList();

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            Debug.Print("Response: " + res.StatusCode);
            Stream stream = res.GetResponseStream();
            sr = new StreamReader(stream);
            string json = sr.ReadToEnd();


            //Debug.Print("json: " + json);

            al = Json.NETMF.JsonSerializer.DeserializeString(json) as ArrayList;


            return al;

        }
        private string RandomString(int Size)
        {
            Random random = new Random();
            string input = "abcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < Size; i++)
            {
                ch = input[random.Next(input.Length)];
                builder.Append(ch);
            }
            return builder.ToString();
        }
        private void loadGUI(GHI.Glide.Display.Window window)
        {
            Glide.MainWindow = window;
            window.Invalidate();
        }
        private void initOrders()
        {
            if (orders.Size() > 0)
                orders.Clear();

            for (int i = 0; i < menu.Count; i++)
            {
                Hashtable ht = menu[i] as Hashtable;
                orders.Add(new Product(ht["id"].ToString(),ht["name"].ToString(),double.Parse(ht["price"].ToString()),ht["ingredients"].ToString()), 0);
            }
               
        }
        private void printDatagrid()
        {
            _dataGrid.Clear();
            
            Product p = null;
            foreach (Order order in orders.List)
            {
                p = order.Product;
                _dataGrid.AddItem(new DataGridItem(new object[4] { p.id, p.nome, p.prezzo, order.Quantity }));
            }

            selected_row = 0;
            updateSelectedValues(selected_row);


            updateIngredientsLabel();

            _pCounter.Text = orders.Total.ToString();
            _dataGrid.SelectedIndex = selected_row;
            _dataGrid.Invalidate();



        }
        /*Timer_Tick function for joystick*/
        private void Timer_Tick(GT.Timer timer)
        {
            Joystick.Position pos = joystick.GetPosition();
            if (pos.X > 0.50)            
                Joystick_Up();            
            else if(pos.X < -0.50)
                Joystick_Down();
        }
        void dataGrid_TapCellEvent(object sender, TapCellEventArgs args)
        {           
            // Get the data from the row we tapped.            
            object[] data = _dataGrid.GetRowData(args.RowIndex);                  
            if (data != null)
            {
                /*enable ingredients button*/
                //_ingBtn.Visible = true;               
                //_menu.Invalidate();                                

                GlideUtils.Debug.Print("GetRowData[" + args.RowIndex + "] = ", data);
                /*mem row index*/
                selected_row = args.RowIndex;
                /*select id row*/
                selected_id = _dataGrid.GetRowData(args.RowIndex).GetValue(0).ToString();
                /*select name row*/
                selected_name = _dataGrid.GetRowData(args.RowIndex).GetValue(1).ToString();
                /*select price row*/
                selected_price = Double.Parse(_dataGrid.GetRowData(args.RowIndex).GetValue(2).ToString());
                /*select qnt row*/
                selected_qnt = (int)_dataGrid.GetRowData(args.RowIndex).GetValue(3);


                updateIngredientsLabel();
                
           
            }                   

        }
        void updateIngredientsLabel()
        {
            _errMsg.Text = "";

            Product p = orders.GetProduct(selected_id);

            if (p != null)
                _ingredients.Text = "Ingredients: " + p.ingredients;
            else
                Debug.Print("Product at row " + selected_row + " not found");

            _menu.Invalidate();
        }
        void updateSelectedValues(int selected_row)
        {

            /*select id row*/
            selected_id = _dataGrid.GetRowData(selected_row).GetValue(0).ToString();
            /*select name row*/
            selected_name = _dataGrid.GetRowData(selected_row).GetValue(1).ToString();
            /*select price row*/
            selected_price = Double.Parse(_dataGrid.GetRowData(selected_row).GetValue(2).ToString());
            /*select qnt row*/
            selected_qnt = (int)_dataGrid.GetRowData(selected_row).GetValue(3);

        }
        /*Joystick Up function*/
        void Joystick_Up()
        {
            if (Glide.MainWindow == _menu)
            {
                _dataGrid.ScrollUp(1);
                selected_row--;

                if (selected_row < 0)
                    selected_row = 0;

                updateSelectedValues(selected_row);

                _dataGrid.SelectedIndex = selected_row;
                _dataGrid.Invalidate();

                updateIngredientsLabel();
            }else if(Glide.MainWindow == _ordina)
            {
                _gridOrdine.ScrollUp(1);
                _gridOrdine.Invalidate();
                
            }
                           
        }
        /*Joystick Down function*/
        void Joystick_Down()
        {
            if (Glide.MainWindow == _menu)
            {
                _dataGrid.ScrollDown(1);
                selected_row++;

                if (selected_row == orders.Size())
                    selected_row = orders.Size() - 1;


                updateSelectedValues(selected_row);

                _dataGrid.SelectedIndex = selected_row;
                _dataGrid.Invalidate();


                updateIngredientsLabel();
            }else if(Glide.MainWindow == _ordina)
            {
                _gridOrdine.ScrollDown(1);
                _gridOrdine.Invalidate();
            }
            

        }
        private void ResetStatus()
        {
            orderId = "";
            ORDER_CMD = NEW_ORDER;

            orders.Clear();
            orders.Total = 0;
            orders.Price = 0;

            _ingredients.Text = "";
            _pCounter.Text = "";

            _dataGrid.Clear();

        }
        private string GetOrderAsJson()
        {
            Hashtable order = new Hashtable();
            order.Add("id", orderId); //Scegliere id_ordine random
            order.Add("price", orders.Price.ToString());
            
            ArrayList foods = new ArrayList();
            Product p;
            foreach (Order o in orders.List)
            {
                if (o.Quantity == 0)
                    continue;

                p = o.Product;
                Hashtable new_food = new Hashtable();
                new_food.Add("name", p.nome);
                new_food.Add("price", p.prezzo);


                Hashtable food = new Hashtable();
                food.Add("food", new_food);
                food.Add("ingredients", p.ingredients);
                food.Add("quantity", o.Quantity);

                foods.Add(food);
            }

            order.Add("foods", foods);
            
            return Json.NETMF.JsonSerializer.SerializeObject(order);
            
            
        }

        #endregion

        #region Events Implementation

        /*ordBtn TapEvent*/
        private void _ordBtn_PressEvent(object sender)
        {

            
            String order_as_json = GetOrderAsJson();

            Debug.Print(ORDER_CMD);
            Debug.Print(order_as_json);

            /* Send Socket */


            /* Update Command */
            ORDER_CMD = UPDATE_ORDER;

            /* Update GUI */

            _gridOrdine.Clear();
            
            foreach (Order o in orders.List)
                if (o.Quantity > 0)
                    _gridOrdine.AddItem(new DataGridItem(new object[4] { o.Product.id, o.Product.nome,o.Product.prezzo, o.Quantity }));

            _finalPrice.Text = orders.Price + " EURO";

            
            loadGUI(_ordina);



            

        }
        private void _payBtn_TapEvent(object sender)
        {
            if (sockWrap != null)
            {
                byte[] msg = Encoding.UTF8.GetBytes(PAYMENT);
                sockWrap.Socket.Send(msg);
            }

            /*load pagamento*/
            _pagamento = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.Pagamento));
            Glide.MainWindow = _pagamento;                      
            
            _paypal = (GHI.Glide.UI.TextBlock)_pagamento.GetChildByName("paypal");            
            _pagamento.Invalidate();           
        }
        private void _noBtn_TapEvent(object sender)
        {
            Glide.MainWindow = _menu;
        }
        private void _backBtn_TapEvent(object sender)
        {
            loadGUI(_menu);
        }
        private void _siBtn_TapEvent(object sender)
        {
            ResetStatus();
            loadGUI(_mainwindow);
        }
        private void deleteBtn_PressEvent(object sender)
        {
            ORDER_CMD = CANCEL_ORDER;

            //TODO SEND CANCEL_ORDER IN SOCKET

            loadGUI(_cancel);
        }
        /*Ethernet Network_Down Function*/
        private void ethernetJ11D_NetworkDown(GTM.Module.NetworkModule sender,GTM.Module.NetworkModule.NetworkState state)
        {
           loadGUI(_errorWindow);

        }
        /*Ethernet Network_Up Function*/
        private void ethernetJ11D_NetworkUp(GTM.Module.NetworkModule sender,GTM.Module.NetworkModule.NetworkState state)
        {
            loadGUI(_mainwindow);
        }
        private void Start_PressEvent(object sender)
        {

            
            _startbtn.Enabled = false;
            _loadingLbl.Visible = true;
            _mainwindow.Invalidate();

            ResetStatus();

            //TODO Generate New Order Random Id
            orderId = RandomString(32);
            Debug.Print("orderId: " + orderId);

            // TODO Service Discovery

            menu = downloadMenu(url);
            if (menu != null) {

                
                _startbtn.Enabled = true;
                _loadingLbl.Visible = false;


                Glide.MainWindow = _menu;

                initOrders();
                printDatagrid();


                _menu.Invalidate();
                    
            }

            

        }
        private void Minus_ButtonPressed(GTM.GHIElectronics.Button sender, GTM.GHIElectronics.Button.ButtonState state) {

            if (Glide.MainWindow == _menu)
            {
                if (selected_row >= 0)
                {
                    minus.TurnLedOn();
                    minus.ButtonPressed -= Minus_ButtonPressed;

                    int new_qty = orders.Decrement(selected_id);
                    //orders.printStatus();

                    if (orders.Total == 0)
                        _ordBtn.Enabled = false;

                    _dataGrid.SetCellData(3, selected_row, new_qty);
                    _dataGrid.Invalidate();

                    _pCounter.Text = orders.Price.ToString();


                    _menu.Invalidate();

                    minus.ButtonPressed += Minus_ButtonPressed;
                    minus.TurnLedOff();
                }

            }else

                Debug.Print("Minus Button Not Available in this Activity");

        }
        private void Plus_ButtonPressed(GTM.GHIElectronics.Button sender, GTM.GHIElectronics.Button.ButtonState state)
        {

            if (Glide.MainWindow == _menu)
            {

                if (selected_row >= 0)
                {
                    plus.TurnLedOn();
                    plus.ButtonPressed -= Plus_ButtonPressed;

                    _errMsg.Visible = false;

                    int new_qty = orders.Increment(selected_id);
                    //orders.printStatus();

                    _ordBtn.Enabled = true;

                    _pCounter.Text   = orders.Price.ToString();


                    _dataGrid.SetCellData(3, selected_row, new_qty);
                    _dataGrid.Invalidate();

                    _menu.Invalidate();


                    plus.ButtonPressed += Plus_ButtonPressed;
                    plus.TurnLedOff();
                }
                else
                {

                    _errMsg.Text = "Select a Row!";
                    _errMsg.Visible = true;

                }

                _menu.Invalidate();

            }
            else
                Debug.Print("Plus Button Not Available in this Activity");

            
        }
        #endregion

    }
}
