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

        private static GHI.Glide.Display.Window _mainwindow;
        private static GHI.Glide.Display.Window _errorWindow;
        private static GHI.Glide.Display.Window _menu;

        private static GHI.Glide.Display.Window _cancel;
        private static GHI.Glide.Display.Window _ordina;
        private static GHI.Glide.Display.Window _pagamento;
        private GHI.Glide.UI.Button _startbtn;
        private GHI.Glide.UI.Button _deleteBtn;
        private GHI.Glide.UI.Button _ingBtn;
        private GHI.Glide.UI.Button _ordBtn;
        private GHI.Glide.UI.Button _payBtn;
        private GHI.Glide.UI.Button _annullaBtn;
        private GHI.Glide.UI.Button _mdfBtn;
        private GHI.Glide.UI.Button _siBtn;
        private GHI.Glide.UI.Button _noBtn;
        private GHI.Glide.UI.DataGrid _dataGrid;
        private GHI.Glide.UI.DataGrid _gridOrdine;
        private GHI.Glide.UI.TextBlock _loadingLbl;
        private GHI.Glide.UI.TextBlock _pCounter;
        private GHI.Glide.UI.TextBlock _msgord1;
        private GHI.Glide.UI.TextBlock _msgord2;
        private GHI.Glide.UI.TextBlock _pfinal;
        private GHI.Glide.UI.TextBlock _errMsg;
        private GHI.Glide.UI.TextBlock _ingredients;
        private GHI.Glide.UI.TextBlock _paypal;   
             
        private int qnt;
        private Double price;
        private static Font font = Resources.GetFont(Resources.FontResources.NinaB);
        private string selected_id;
        private string selected_name;
        private Double selected_price;
        private int selected_qnt;
        private int selected_row = 0;
        private int count = 0;
        private int exist = 0;
        private int aux = 0;
        private int flagmdf = 0;
        private int flagstart = 0;        
        private string json;
        byte[] result = new byte[65536];


        private static string pendingOrderId = null;

        /* Socket Variables */
        private const String HOST = "192.168.1.70";
        private const int PORT = 4096;
        

        private const String NEW_ORDER      = "NEW_ORDER\r\n";
        private const String PAYMENT        = "PAYMENT\r\n";
        private const String CLOSE          = "CLOSE\r\n";
        private const String CANCEL_ORDER   = "CANCEL_ORDER\r\n";
        private const String UPDATE_ORDER   = "UPDATE_ORDER\r\n";



        private ArrayList payment = new ArrayList();
        //private String url = "http://192.168.100.1:8080/food/webapi/food";
        private String url = "http://404notfood.sloppy.zone/food/webapi/food";
        HttpWebRequest req;
        private HttpWebResponse res;
        private Stream stream;
        private StreamReader sr;



        /* Added by Melo */
        private static ArrayList menu;
        private static Orders orders;
        private SocketClient sockWrap;


        private static String ip_address = "192.168.2.2";
        private static String subnet     = "255.255.255.0";
        private static String gateway    = "192.168.2.1";
        private static String[] dns      = { "8.8.8.8", "8.8.4.4" };


        private void initFezSettings()
        {
            /*Use Debug.Print to show messages in Visual Studio's "Output" window during debugging*/
            Debug.Print("Program Started");


            /* References */
            _mainwindow = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.Window));
            _errorWindow = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.ErrorWindow));
            _menu = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.Menu));
            _cancel = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.Annulla));

            _dataGrid = (DataGrid)_menu.GetChildByName("dataGrid");

            _startbtn = (GHI.Glide.UI.Button)_mainwindow.GetChildByName("startbtn");

            _siBtn = (GHI.Glide.UI.Button)_cancel.GetChildByName("siBtn");
            _noBtn = (GHI.Glide.UI.Button)_cancel.GetChildByName("noBtn");


            _loadingLbl = (GHI.Glide.UI.TextBlock)_mainwindow.GetChildByName("loading_lbl");

            _dataGrid = (GHI.Glide.UI.DataGrid)_menu.GetChildByName("dataGrid");
            _dataGrid.SelectedIndex = selected_row;
            _dataGrid.Invalidate();

            _pCounter = (GHI.Glide.UI.TextBlock)_menu.GetChildByName("pCounter");
            _pCounter.Text = "0.00 EURO";

            _errMsg = (GHI.Glide.UI.TextBlock)_menu.GetChildByName("errMsg");

            _ingredients = (GHI.Glide.UI.TextBlock)_menu.GetChildByName("ingredientsLbl");

            _ordBtn = (GHI.Glide.UI.Button)_menu.GetChildByName("ordBtn");
            _deleteBtn = (GHI.Glide.UI.Button)_menu.GetChildByName("deleteBtn");

            /* Adding Service Logo */
            Image _Servicelogo = new Image("service-logo", 255, 0, 0, 320, 100);
            _mainwindow.AddChild(_Servicelogo);
            _Servicelogo.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.logo_food), Bitmap.BitmapImageType.Jpeg);


            /* Adding Error Logo */
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


            /* Initializing Menu Page */

            /*Setup the dataGrid reference*/


            // Listen for tap cell events.


            /*Create our four columns*/
            _dataGrid.AddColumn(new DataGridColumn("ID", 0));
            
            _dataGrid.AddColumn(new DataGridColumn("MEAL", 175));
            _dataGrid.AddColumn(new DataGridColumn("PRICE", 50));
            _dataGrid.AddColumn(new DataGridColumn("QTY", 30));
            _menu.AddChild(_dataGrid);
            _dataGrid.Render();

            /* Shut down Light on Buttons */
            plus.TurnLedOff();
            minus.TurnLedOff();

            /*
             * Configure Joypad
             * Create a timer & run method timer_trick when thr timer ticks (for joystick)
             */
            GT.Timer timer = new GT.Timer(200);
            timer.Tick += Timer_Tick;
            timer.Start();


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
        
        private void loadGUI(GHI.Glide.Display.Window window)
        {
            Glide.MainWindow = window;
        }


        /*This method is run when the mainboard is powered up or reset*/
        void ProgramStarted()
        {

            menu = new ArrayList();
            orders = new Orders();

            
            initFezSettings();
                

        }

        #region Functions

        void initOrders()
        {
            if (orders.Size() > 0)
                orders.Clear();

            for (int i = 0; i < menu.Count; i++)
            {
                Hashtable ht = menu[i] as Hashtable;
                orders.Add(new Product(ht["id"].ToString(),ht["name"].ToString(),double.Parse(ht["price"].ToString()),ht["ingredients"].ToString()), 0);
            }
               
        }

        void printDatagrid()
        {
            _dataGrid.Clear();

            //  _dataGrid.RowCount = orders.Size();
            _dataGrid.RowCount = 4;

            Product p = null;
            foreach (Order order in orders.List)
            {
                p = order.Product;
                _dataGrid.AddItem(new DataGridItem(new object[4] { p.id, p.nome, p.prezzo, order.Quantity }));
            }

            selected_row = 0;
            updateSelectedValues(selected_row);
            updateIngredientsLabel();
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



        /*Populate Grid function*/
        //void Populate(ArrayList al)
        //{
        //    Debug.Print("Populating...");          
            
        //    /*se l'utente deve modificare l'ordine'*/
        //    if (flagmdf == 1)
        //    {               
        //        _pCounter.Text = price.ToString();
        //        _qntCounter.Text = qnt.ToString();
        //        int qnt_appoggio = qnt;
                
        //        _dataGrid.Clear();
        //        _menu.Invalidate();
        //        _dataGrid.Invalidate();

        //        for (int i = 0; i < al.Count; i++)
        //        { 
        //        Hashtable ht = al[i] as Hashtable;
        //        qnt = 0;
        //        foreach (Product p in payment)
        //            if (p.id == Double.Parse(ht["id"].ToString()))
        //                qnt = p.quantita;                   
        //        _dataGrid.AddItem(new DataGridItem(new object[4] { ht["id"], ht["name"], ht["price"], qnt }));
        //        }               
        //        _dataGrid.Invalidate();                
        //        qnt = qnt_appoggio;
        //    }
        //    else
        //    {               
        //        /*populating iniziale*/
        //        for (int i = 0; i < al.Count; i++)
        //        {
        //            Hashtable ht = al[i] as Hashtable;
        //            _dataGrid.AddItem(new DataGridItem(new object[4] { ht["id"], ht["name"], ht["price"], qnt }));
        //        }
        //        _dataGrid.Invalidate();
        //    }
        //}

        /*DataGrid TapCellEvent*/
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
            _dataGrid.ScrollUp(1);
            selected_row--;

            if (selected_row < 0)
                selected_row = 0;

            updateSelectedValues(selected_row);

            _dataGrid.SelectedIndex = selected_row;
            _dataGrid.Invalidate();

            updateIngredientsLabel();
                           
        }

        /*Joystick Down function*/
        void Joystick_Down()
        {
            _dataGrid.ScrollDown(1);
            selected_row++;

            if (selected_row == orders.Size())
                selected_row = orders.Size() - 1;


            updateSelectedValues(selected_row);

            _dataGrid.SelectedIndex = selected_row;
            _dataGrid.Invalidate();


            updateIngredientsLabel();

        }
        
       

        /*ordBtn TapEvent*/
        void _ordBtn_PressEvent(object sender)
        {

            string id_ordine;            

            if (pendingOrderId != null)
                id_ordine = pendingOrderId;
            else
            {
                var random = new Random(System.DateTime.Now.Millisecond);
                uint randomNumber = (uint)random.Next();
                id_ordine = randomNumber.ToString();
            }          
            
            string tot = price.ToString();
            Hashtable order = new Hashtable();
            order.Add("id", id_ordine);
            order.Add("price", tot);
            
            // Preparing order array list
            ArrayList foods = new ArrayList();
            
            foreach (Product p in payment)
            {
                // Preparing food array list
                Hashtable new_food = new Hashtable();
                new_food.Add("name",p.nome);
                new_food.Add("price",p.prezzo);
                

                Hashtable food = new Hashtable();
                food.Add("food", new_food);
                //food.Add("quantity", p.quantita);

                foods.Add(food);
            }

            order.Add("foods", foods);

            pendingOrderId = id_ordine;

            string order_as_json = Json.NETMF.JsonSerializer.SerializeObject(order);

            // TODO: MANDARE order_as_json al Desktop tramite Socket
            Debug.Print(order_as_json);

            if (sockWrap != null)
            {

                byte[] msg;
                if(flagmdf==0)
                    msg = Encoding.UTF8.GetBytes(NEW_ORDER);
                else
                    msg = Encoding.UTF8.GetBytes(UPDATE_ORDER);

                sockWrap.Socket.Send(msg);
                msg = Encoding.UTF8.GetBytes(order_as_json + "\r\n");
                sockWrap.Socket.Send(msg);

            }

            /*load ordina*/
            _ordina = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.Ordina));
            Glide.MainWindow = _ordina;
            _gridOrdine = (GHI.Glide.UI.DataGrid)_menu.GetChildByName("gridOrdine");
            _pfinal = (GHI.Glide.UI.TextBlock)_ordina.GetChildByName("pFinal");

            /*Setup the dataGrid reference*/
            _gridOrdine = (DataGrid)_ordina.GetChildByName("gridOrdine");

            /*Create our four columns*/
            _gridOrdine.AddColumn(new DataGridColumn("MEAL", 175));
            _gridOrdine.AddColumn(new DataGridColumn("PRICE", 50));
            _gridOrdine.AddColumn(new DataGridColumn("QTY", 30));

            //foreach (Product p in payment)
            //    _gridOrdine.AddItem(new DataGridItem(new object[3] { p.nome, p.prezzo, p.quantita }));

            _pfinal.Text = price.ToString();
                                    
            _gridOrdine.Invalidate();
                 

            _annullaBtn = (GHI.Glide.UI.Button)_ordina.GetChildByName("annullaBtn");
            _payBtn = (GHI.Glide.UI.Button)_ordina.GetChildByName("payBtn");
            _mdfBtn = (GHI.Glide.UI.Button)_ordina.GetChildByName("mdfBtn");
            _msgord1 = (GHI.Glide.UI.TextBlock)_ordina.GetChildByName("msgord1");
            
            
            _msgord2.Visible = false;
            _ordina.Invalidate();

            _annullaBtn.TapEvent += _annullaBtn_TapEvent;
            _mdfBtn.TapEvent += _mdfBtn_TapEvent;
            _payBtn.TapEvent += _payBtn_TapEvent;
            _siBtn.TapEvent += _siBtn_TapEvent;
            _noBtn.TapEvent += _noBtn_TapEvent;           
        }        

        /*apre pagina per il pagamento*/
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

        /*modifica ordine prima di pagare*/
        private void _mdfBtn_TapEvent(object sender)
        {
            flagmdf = 1;
            //printDatagrid();
        }

        /*chiamata quando annullo tutto l'ordine prima di pagare e torna all'inizio*/
        private void _annullaBtn_TapEvent(object sender)
        {
            Glide.MainWindow = _cancel;
            _cancel.Invalidate();
        }

        /*pulsante no annulla ordine*/
        private void _noBtn_TapEvent(object sender)
        {
            _gridOrdine.Visible = true;
            _msgord1.Visible = true;
            _pfinal.Visible = true;
            _annullaBtn.Visible = true;
            _payBtn.Visible = true;
            _mdfBtn.Visible = true;

            _msgord2.Visible = false;
            _siBtn.Visible = false;
            _noBtn.Visible = false;

            _ordina.Invalidate();
        }

        /*pulsante si annulla ordine*/
        private void _siBtn_TapEvent(object sender)
        {
            selected_id = "";//set selected_id to 0
            selected_name = null;//set selected_name null
            selected_price = 0;//set selected_price to 0
            selected_qnt = 0;//set qnt to 0            
            price = 0;//set total price to 0     
            qnt = 0;//set total qnt to 0 
            selected_row = -1;                
            payment.Clear();

            byte[] msg = Encoding.UTF8.GetBytes(CANCEL_ORDER);
            sockWrap.Socket.Send(msg);
            sockWrap.Socket.Close();
            sockWrap = null;

            // TODO FIX HERE
            //first_step();
        }

        /*Delete_btn TapEvent*/
        void deleteBtn_PressEvent(object sender)
        {
            Glide.MainWindow = _cancel;
            _cancel.Invalidate();
        }
        
        // TODO Handle THIS SITUATION
        /*Ethernet Network_Down Function*/
        void ethernetJ11D_NetworkDown(GTM.Module.NetworkModule sender,GTM.Module.NetworkModule.NetworkState state)
        {



            loadGUI(_errorWindow);

        }

        /*Ethernet Network_Up Function*/
        void ethernetJ11D_NetworkUp(GTM.Module.NetworkModule sender,GTM.Module.NetworkModule.NetworkState state)
        {
            loadGUI(_mainwindow);
        }

        /****************
         * CALLBACK 
         * *************/


        private Socket connectToDesktop(String hostname,int port)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse(hostname);
            IPEndPoint serverEndPoint = new IPEndPoint(ipAddress, port);

            socket.Connect(serverEndPoint);

            Debug.Print("avail: "+socket.Available.ToString());

            return socket;

            

           
        }


        private ArrayList downloadMenu(String url)
        {
            /*inizio get menu*/
            Debug.Print("GET MENU");
            ArrayList al = new ArrayList();

            HttpWebRequest  req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            Debug.Print("Response: " + res.StatusCode);
            Stream stream = res.GetResponseStream();
            sr = new StreamReader(stream);
            string json = sr.ReadToEnd();
            

            Debug.Print("json: " + json);

            al = Json.NETMF.JsonSerializer.DeserializeString(json) as ArrayList;

            
            return al;

        }

        private void Start_PressEvent(object sender)
        {
            

            _startbtn.Enabled = false;
            _loadingLbl.Visible = true;
            _mainwindow.Invalidate();

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

                    int new_qty = orders.Decrement(selected_id);
                    orders.printStatus();

                    if (orders.Total == 0)
                        _ordBtn.Enabled = false;

                    _dataGrid.SetCellData(3, selected_row, new_qty);
                    _dataGrid.Invalidate();

                    _pCounter.Text = orders.Price.ToString()+" EURO";


                    _menu.Invalidate();

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

                    _errMsg.Visible = false;

                    int new_qty = orders.Increment(selected_id);
                    orders.printStatus();

                    _ordBtn.Enabled = true;

                    _pCounter.Text   = orders.Price.ToString()+" EURO";


                    _dataGrid.SetCellData(3, selected_row, new_qty);
                    _dataGrid.Invalidate();

                    _menu.Invalidate();

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


    }
    #endregion Functions
}
