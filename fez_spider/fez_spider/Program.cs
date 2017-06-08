using Gadgeteer.Modules.GHIElectronics;
using System;
using System.Collections;
using System.Threading;
using System.Net;
using Microsoft.SPOT;
using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using GHI.Glide;
using GHI.Glide.UI;
using System.IO;
using System.Text;



namespace fez_spider
{
    public partial class Program
    {

        private static GHI.Glide.Display.Window _mainwindow;
        private static GHI.Glide.Display.Window _errorWindow;
        private static GHI.Glide.Display.Window _menu;
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
        private GHI.Glide.UI.TextBlock _qntCounter;
        private GHI.Glide.UI.TextBlock _errMsg;
        private GHI.Glide.UI.TextBlock _paypal;        
        private int qnt;
        private Double price;
        private static Font font = Resources.GetFont(Resources.FontResources.NinaB);
        private Double getid;
        private string getpizza;
        private Double getprice;
        private int getqnt;
        private int row = -1;
        private int count = 0;
        private int exist = 0;
        private int aux = 0;
        private int flagmdf = 0;
        private int flagstart = 0;        
        private string json;
        byte[] result = new byte[65536];


        private static string pendingOrderId = null;

        /* Socket Variables */
        private const String HOST = "192.168.1.23";
        //private const String HOST = "192.168.100.1";
        private const int PORT = 4096;
        private SocketClient sockWrap = null;


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



        private static String ip_address = "192.168.1.253";
        private static String subnet     = "255.255.255.0";
        private static String gateway    = "192.168.1.1";
        private static String[] dns      = { "8.8.8.8", "8.8.4.4" };


        private void initFezSettings()
        {
            /*Use Debug.Print to show messages in Visual Studio's "Output" window during debugging*/
            Debug.Print("Program Started");


            /* References */
            _mainwindow = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.Window));
            _errorWindow = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.ErrorWindow));
            _menu = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.Menu));
            _dataGrid = (DataGrid)_menu.GetChildByName("dataGrid");
            _startbtn = (GHI.Glide.UI.Button)_mainwindow.GetChildByName("startbtn");
            _dataGrid = (GHI.Glide.UI.DataGrid)_menu.GetChildByName("dataGrid");
            _pCounter = (GHI.Glide.UI.TextBlock)_menu.GetChildByName("pCounter");
            _qntCounter = (GHI.Glide.UI.TextBlock)_menu.GetChildByName("qntCounter");
            _errMsg = (GHI.Glide.UI.TextBlock)_menu.GetChildByName("errMsg");
            _ordBtn = (GHI.Glide.UI.Button)_menu.GetChildByName("ordBtn");
            _deleteBtn = (GHI.Glide.UI.Button)_menu.GetChildByName("deleteBtn");
            _loadingLbl = (GHI.Glide.UI.TextBlock)_mainwindow.GetChildByName("loading_lbl");

            /* Adding Error Logo */
            Image _logo = new Image("logo", 1000, 0, 0, displayTE35.Width, displayTE35.Height);
            _errorWindow.AddChild(_logo);
            _logo.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.Connection_error), Bitmap.BitmapImageType.Jpeg);

            /* Register Events to Buttons */

            _startbtn.PressEvent += Start_PressEvent;
            plus.ButtonPressed += Plus_ButtonPressed;
            minus.ButtonPressed += Minus_ButtonPressed;
            _ordBtn.PressEvent += _ordBtn_PressEvent;
            _deleteBtn.PressEvent += deleteBtn_PressEvent;
            _dataGrid.TapCellEvent += new OnTapCell(dataGrid_TapCellEvent);


            /* Initializing Menu Page */

            /*Setup the dataGrid reference*/


            // Listen for tap cell events.


            /*Create our four columns*/
            _dataGrid.AddColumn(new DataGridColumn("ID", 0));
            _dataGrid.AddColumn(new DataGridColumn("PIZZA", 125));
            _dataGrid.AddColumn(new DataGridColumn("PREZZO", 80));
            _dataGrid.AddColumn(new DataGridColumn("QNT", 50));
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
            ethernetJ11D.UseThisNetworkInterface();
            ethernetJ11D.UseStaticIP(ip_address, subnet, gateway, dns);
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


            initFezSettings();
                

        }

        #region Functions
     

        void initMenu(ArrayList menu)
        {
            
            Debug.Print("Init Menu!");

          _dataGrid.Clear();

            for (int i = 0; i < menu.Count; i++)
            {
                Hashtable ht = menu[i] as Hashtable;
                _dataGrid.AddItem(new DataGridItem(new object[4] { ht["id"], ht["name"], ht["price"], qnt }));
            }

            _dataGrid.Invalidate();
            
            Glide.MainWindow = _menu;


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
        void Populate(ArrayList al)
        {
            Debug.Print("Populating...");          
            
            /*se l'utente deve modificare l'ordine'*/
            if (flagmdf == 1)
            {               
                _pCounter.Text = price.ToString();
                _qntCounter.Text = qnt.ToString();
                int qnt_appoggio = qnt;
                
                _dataGrid.Clear();
                _menu.Invalidate();
                _dataGrid.Invalidate();

                for (int i = 0; i < al.Count; i++)
                { 
                Hashtable ht = al[i] as Hashtable;
                qnt = 0;
                foreach (Product p in payment)
                    if (p.id == Double.Parse(ht["id"].ToString()))
                        qnt = p.quantita;                   
                _dataGrid.AddItem(new DataGridItem(new object[4] { ht["id"], ht["name"], ht["price"], qnt }));
                }               
                _dataGrid.Invalidate();                
                qnt = qnt_appoggio;
            }
            else
            {               
                /*populating iniziale*/
                for (int i = 0; i < al.Count; i++)
                {
                    Hashtable ht = al[i] as Hashtable;
                    _dataGrid.AddItem(new DataGridItem(new object[4] { ht["id"], ht["name"], ht["price"], qnt }));
                }
                _dataGrid.Invalidate();
            }
        }

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
                row = args.RowIndex;
                /*select id row*/
                getid = Double.Parse(_dataGrid.GetRowData(args.RowIndex).GetValue(0).ToString());
                /*select name row*/
                getpizza = (string)_dataGrid.GetRowData(args.RowIndex).GetValue(1);
                /*select price row*/
                getprice = Double.Parse(_dataGrid.GetRowData(args.RowIndex).GetValue(2).ToString());
                /*select qnt row*/
                getqnt = (int)_dataGrid.GetRowData(args.RowIndex).GetValue(3);
                Debug.Print("QNT tapcell: " + getqnt);                
            }                   

        }

        /*Joystick Up function*/
        void Joystick_Up()
        {
            _dataGrid.ScrollUp(1);
            _dataGrid.Invalidate();                 
        }

        /*Joystick Down function*/
        void Joystick_Down()
        {
            _dataGrid.ScrollDown(1);
            _dataGrid.Invalidate();
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
                food.Add("quantity", p.quantita);

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
            _gridOrdine.AddColumn(new DataGridColumn("PIZZA", 125));
            _gridOrdine.AddColumn(new DataGridColumn("PREZZO", 80));
            _gridOrdine.AddColumn(new DataGridColumn("QNT", 50));

            foreach (Product p in payment)
                _gridOrdine.AddItem(new DataGridItem(new object[3] { p.nome, p.prezzo, p.quantita }));

            _pfinal.Text = price.ToString();
                                    
            _gridOrdine.Invalidate();
                 

            _annullaBtn = (GHI.Glide.UI.Button)_ordina.GetChildByName("annullaBtn");
            _payBtn = (GHI.Glide.UI.Button)_ordina.GetChildByName("payBtn");
            _mdfBtn = (GHI.Glide.UI.Button)_ordina.GetChildByName("mdfBtn");
            _siBtn = (GHI.Glide.UI.Button)_ordina.GetChildByName("siBtn");
            _noBtn = (GHI.Glide.UI.Button)_ordina.GetChildByName("noBtn");
            _msgord1 = (GHI.Glide.UI.TextBlock)_ordina.GetChildByName("msgord1");
            _msgord2 = (GHI.Glide.UI.TextBlock)_ordina.GetChildByName("msgord2");

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
            //initMenu();
        }

        /*chiamata quando annullo tutto l'ordine prima di pagare e torna all'inizio*/
        private void _annullaBtn_TapEvent(object sender)
        {
            _gridOrdine.Visible = false;
            _annullaBtn.Visible = false;
            _payBtn.Visible = false;
            _mdfBtn.Visible = false;
            _msgord1.Visible = false;
            _pfinal.Visible = false;

            _msgord2.Visible = true;
            _siBtn.Visible = true;
            _noBtn.Visible = true;

            _ordina.Invalidate();
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
            getid = 0;//set getid to 0
            getpizza = null;//set getpizza null
            getprice = 0;//set getprice to 0
            getqnt = 0;//set qnt to 0            
            price = 0;//set total price to 0     
            qnt = 0;//set total qnt to 0 
            row = -1;                
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
            getqnt = 0;//set qnt to 0            
            price = 0;//set total price to 0     
            qnt = 0;//set total qnt to 0            
            payment.Clear();
                  
            _pCounter.Text = price.ToString();
            _qntCounter.Text = qnt.ToString();
            _deleteBtn.Enabled = false;
            _ordBtn.Enabled = false;
            _menu.Invalidate();
            
            /*vedere se questo for va bene o c'è un altro modo??*/
            for (int i = 0; i < 7; i++)
            {
                _dataGrid.SetCellData(3, i, qnt);
                _dataGrid.Invalidate();                
            }
            Debug.Print("Annullato tutto! Qnt: " + qnt + " Prezzo: " + price);
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


        private SocketClient connectToDesktop(String hostname,int port)
        {
            SocketClient sockWrap = new SocketClient();

            if (sockWrap == null)
                return null;

            try
            {
                sockWrap.Connect(hostname, port);
                return sockWrap;
            }catch(Exception ex){
                return null;
            }

           
        }


        private ArrayList downloadMenu(String url)
        {
            /*inizio get menu*/
            Debug.Print("GET MENU");
            ArrayList al = new ArrayList();

            HttpWebRequest  req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            Stream stream = res.GetResponseStream();
            sr = new StreamReader(stream);
            string json = sr.ReadToEnd();
            
            al = Json.NETMF.JsonSerializer.DeserializeString(json) as ArrayList;



            return al;

        }

        private void Start_PressEvent(object sender)
        {

            ArrayList menu = new ArrayList();

            _loadingLbl.Visible = true;
            _mainwindow.Invalidate();

            // TODO Service Discovery

            sockWrap = connectToDesktop(HOST, PORT);
            if (sockWrap != null)
            {
                menu = downloadMenu(url);
                if (menu != null)
                {
                    initMenu(menu);
                }
            }

            _loadingLbl.Visible = false;

            

        }

        private void Plus_ButtonPressed(GTM.GHIElectronics.Button sender, GTM.GHIElectronics.Button.ButtonState state)
        {
            plus.TurnLedOn();
            if (row == -1)
            {
                Debug.Print("Seleziona una pizza!!");
                _errMsg.Text = "Select pizza!";
                _errMsg.Visible = true;
                _menu.Invalidate();
            }           
            else
            {
                _deleteBtn.Enabled = true;
                _ordBtn.Enabled = true;
                _errMsg.Visible = false;                
                /*calculate price function*/               
                price = price + getprice;
                _pCounter.Text = price.ToString();

                getqnt = getqnt + 1; //quantità della pizza selezionata
                qnt = qnt + 1; //quantità totale
                _qntCounter.Text = qnt.ToString();
                _dataGrid.SetCellData(3, row, getqnt);               
                _menu.Invalidate();

                /*inizio parte array*/
                 
                if(payment.Count == 0)
                {
                    Debug.Print("First insert into array");
                    payment.Add(new Product(getid, getpizza, getprice, getqnt));                    
                    //Debug.Print("Elementi presenti array(plus):  " + payment.Count);
                }                
                   
                else
                {
                    Debug.Print("Second or plus insert into array");
                    exist = 0;
                    foreach ( Product i in payment)
                    {
                      int indice = payment.IndexOf(i);
                      //Debug.Print("Indice: " + indice);
                    
                      if(getid == i.id)
                        {
                            Debug.Print("Esiste già uno -- setto qnt+1");
                            payment.RemoveAt(indice);
                            payment.Insert(indice, new Product(getid, getpizza, getprice, getqnt));
                            //Debug.Print("Elementi presenti array(plus):  " + payment.Count);
                            exist = 1;
                            break;
                        }                                         
                    }

                    if (exist == 0)
                    {
                        Debug.Print("Non esiste -- aggiungo");
                        payment.Add(new Product(getid, getpizza, getprice, getqnt));
                        //Debug.Print("Elementi presenti array(plus):  " + payment.Count);
                    }                

                }              
                /*fine parte array*/

                Debug.Print("Hai Aggiunto: " + getpizza + " Qnt: " + getqnt);
                Debug.Print("Prezzo Totale: " + price.ToString());
            }
            plus.TurnLedOff();

        }

        private void Minus_ButtonPressed(GTM.GHIElectronics.Button sender, GTM.GHIElectronics.Button.ButtonState state)
        {
            minus.TurnLedOn();
            count = 1;
            
            if (getqnt == 0)
            {                
                Debug.Print("Aggiungi pizza!");
                _errMsg.Text = "Add pizza!";
                _errMsg.Visible = true;                
                _menu.Invalidate();
            }
            else
            {
                _errMsg.Visible = false;

                /*calculate price function*/
                aux = getqnt;
                price = price - getprice;
                _pCounter.Text = price.ToString();
                getqnt = getqnt - 1; //quantità della pizza selezionata
                qnt = qnt - 1; //quantità totale    
                _qntCounter.Text = qnt.ToString();
                _dataGrid.SetCellData(3, row, getqnt);               
                _menu.Invalidate();

                if (qnt == 0)
                {
                    _ordBtn.Enabled = false;
                    _deleteBtn.Enabled = false;
                    _menu.Invalidate();                    
                }

                /*inizio parte array*/                
                foreach (Product i in payment)
                {
                    int indice = payment.IndexOf(i);
                    //Debug.Print("Indice: " + indice);

                    if (getid == i.id && count == 1)
                    {
                        Debug.Print("IF");
                        //Debug.Print("QNT: " + aux);
                        if (aux >= 2)
                        {
                            Debug.Print("IF MAGGIORE DI UNO --- ELIMINO");
                            payment.RemoveAt(indice);
                            payment.Insert(indice, new Product(getid, getpizza, getprice, getqnt));
                            count = 0;
                            break;
                        }
                        else
                        {
                            Debug.Print("ELSE UNO --- ELIMINO");
                            payment.RemoveAt(indice);
                            count = 0;
                            break;
                        }                  
                    }
                      
                }

                //Debug.Print("Elementi presenti array(minus):  " + payment.Count);               
                /*fine parte array*/

                Debug.Print("Hai eliminato: " + getpizza + " Qnt: " + getqnt);
                Debug.Print("Prezzo Totale: " + price.ToString());
                Debug.Print("QNT dopo rimozione: " + getqnt);               
            }
            minus.TurnLedOff();

        }

    }
    #endregion Functions
}
