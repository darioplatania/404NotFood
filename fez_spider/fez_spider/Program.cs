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
using System.Threading;



namespace fez_spider
{

    
    public partial class Program
    {
        private static System.Threading.Thread tHeartBeat;

        #region Ghi.Glide Definitions 

        private static GHI.Glide.Display.Window _mainwindow;
        private static GHI.Glide.Display.Window _errorWindow;
        private static GHI.Glide.Display.Window _menu;
        private static GHI.Glide.Display.Window _cancel;
        private static GHI.Glide.Display.Window _ordina;
        private static GHI.Glide.Display.Window _scegliPagamento;
        private static GHI.Glide.Display.Window _credit_card_payment;
        private static GHI.Glide.Display.Window _paypal_payment;
        private static GHI.Glide.Display.Window _processingPaymentWindow;
        private static GHI.Glide.Display.Window _paymentSuccess;
        private static GHI.Glide.Display.Window _paymentError;
        private static GHI.Glide.Display.Window _serverError;


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
        private GHI.Glide.UI.TextBox _input_creditCard_owner;
        private GHI.Glide.UI.TextBox _input_creditCard_number;
        private GHI.Glide.UI.TextBox _input_creditCard_cvv;
        private GHI.Glide.UI.Dropdown _input_expiration_month;
        private GHI.Glide.UI.Dropdown _input_expiration_year;
        private GHI.Glide.UI.Button _paypal_doneBtn;
        private GHI.Glide.UI.Button _ccBackBtn;
        private GHI.Glide.UI.Button _ccConfirmBtn;
        private GHI.Glide.UI.TextBlock _ccErrMsg;
        private GHI.Glide.UI.Button _backToOrderBtn;
        private GHI.Glide.UI.Button _backToPaymentBtn;
        private Image _qrCodeSample;
        private Image waitQRCode;
        
        #endregion

        private static Font font = Resources.GetFont(Resources.FontResources.NinaB);
        private string selected_id;
        private string selected_name;
        private Double selected_price;
        private int selected_qnt;
        private int selected_row = 0;
        private static double timestamp;
        private static bool startup = false;
        private static GHI.Glide.Display.Window previousWindow;
        byte[] result = new byte[65536];


        private static Image _visaThumb;
        private static Image _mastercardThumb;
        private static Image _americanexpressThumb;
        private List months_list;
        private List years_list;

        private static string orderId = null;
        private const  string key = "1234567890123456";

        /* Socket Variables */
        private Socket s = null;
        private Socket _socket = null;
//        private const String HOST = "172.20.10.3";
        private const String HOST = "192.168.1.9";

        private const int PORT = 4096;
        

        private const String NEW_ORDER      = "NEW_ORDER\r\n";
        private const String PAYMENT_CARD   = "PAYMENT_CARD\r\n";
        private const String PAYMENT_PAYPAL   = "PAYMENT_PAYPAL\r\n";
        private const String PAYMENT_CONFIRM = "PAYMENT_CONFIRM\r\n";
        private const String CLOSE          = "CLOSE\r\n";
        private const String CANCEL_ORDER   = "CANCEL_ORDER\r\n";
        private const String UPDATE_ORDER   = "UPDATE_ORDER\r\n";


        private static String ORDER_CMD = null;
        /* get current year */
        private static int current_year = 2017;

        private ArrayList payment = new ArrayList();
        private String url = "http://95.85.47.151:8080/food/webapi/food";
        private String payment_url = "http://95.85.47.151:8080/food/webapi/order/";
        private StreamReader sr;
        
        
        private static ArrayList menu;
        private static Orders orders;
        

        private static bool qrCodeFlag = false;
        private static bool processingPayment = false;
        
        #region Fez Network Configuration

        //private static String ip_address = "192.168.2.2";
        //private static String subnet     = "255.255.255.0";
        //private static String gateway    = "192.168.2.1";

        private static String ip_address = "192.168.1.253";
        private static String subnet = "255.255.255.0";
        private static String gateway = "192.168.1.1";


        private static String[] dns      = { "8.8.8.8", "8.8.4.4" };


        private static int led_position;


        private static ManualResetEvent HeartBeatEvent = new ManualResetEvent(false);
        private static ManualResetEvent DownEvent = new ManualResetEvent(true);



        #endregion

        #region Fez Initialization on Startup



        private void initFezSettings()
        {
            /*Use Debug.Print to show messages in Visual Studio's "Output" window during debugging*/
            Debug.Print("Program Started");

        
            /* Init Led Position */
            led_position = 0;

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
            Image _Servicelogo = new Image("service-logo", 255, 96, 10, 128, 128);
            _mainwindow.AddChild(_Servicelogo);
            _Servicelogo.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.logo), Bitmap.BitmapImageType.Jpeg);


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


            /* Choose Payment */
            _scegliPagamento = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.ScegliPagamento));
            _backToOrderBtn = (GHI.Glide.UI.Button)_scegliPagamento.GetChildByName("BackToOrderBtn");
            Image _creditCard = new Image("credit-card", 255, 22, 66, 128, 128);
            _creditCard.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.creditcard), Bitmap.BitmapImageType.Jpeg);
            
            Image _paypal = new Image("paypal", 255, 170, 66, 128, 128);
            _paypal.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.paypal), Bitmap.BitmapImageType.Jpeg);
            _scegliPagamento.AddChild(_creditCard);
            _scegliPagamento.AddChild(_paypal);



            /* Payment Via Credit Card*/
            _credit_card_payment = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.PaymentCreditCard));
            _input_creditCard_owner = (GHI.Glide.UI.TextBox)_credit_card_payment.GetChildByName("card_owner");
            _input_creditCard_number = (GHI.Glide.UI.TextBox)_credit_card_payment.GetChildByName("card_number");
            _input_creditCard_cvv = (GHI.Glide.UI.TextBox)_credit_card_payment.GetChildByName("cvv");
            _input_expiration_month = (GHI.Glide.UI.Dropdown)_credit_card_payment.GetChildByName("expiration_date_month");
            _input_expiration_year = (GHI.Glide.UI.Dropdown)_credit_card_payment.GetChildByName("expiration_date_year");
            _input_creditCard_cvv = (GHI.Glide.UI.TextBox)_credit_card_payment.GetChildByName("cvv");
            _ccBackBtn = (GHI.Glide.UI.Button)_credit_card_payment.GetChildByName("ccBackBtn");
            _ccConfirmBtn = (GHI.Glide.UI.Button)_credit_card_payment.GetChildByName("ccConfirmBtn");
            _ccErrMsg = (GHI.Glide.UI.TextBlock)_credit_card_payment.GetChildByName("ccErrMsg");
            _visaThumb = new Image("visa-thumb", 128,20, 25, 48, 30);
            _visaThumb.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.visa), Bitmap.BitmapImageType.Jpeg);
            _mastercardThumb = new Image("mastercard-thumb", 128, 20, 60, 48, 30);
            _mastercardThumb.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.mastercard), Bitmap.BitmapImageType.Jpeg);
            _americanexpressThumb = new Image("americanexpress-thumb", 128, 20, 95, 48, 30);
            _americanexpressThumb.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.americanexpress), Bitmap.BitmapImageType.Jpeg);

            _credit_card_payment.AddChild(_visaThumb);
            _credit_card_payment.AddChild(_mastercardThumb);
            _credit_card_payment.AddChild(_americanexpressThumb);

            /* server Error */
            _serverError = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.serverError));
            Image _serverErrImg = new Image("paypal", 255, 96, 56, 128, 128);
            _serverErrImg.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.server_error), Bitmap.BitmapImageType.Jpeg);
            _serverError.AddChild(_serverErrImg);


            ArrayList months = new ArrayList()
            {
                new object[2] {"January",1},
                new object[2] {"February",2},
                new object[2] {"March",3},
                new object[2] {"April",4},
                new object[2] {"May",5},
                new object[2] {"June",6},
                new object[2] {"July",7},
                new object[2] {"August",8},
                new object[2] {"September",9},
                new object[2] {"October",10},
                new object[2] {"November",11},
                new object[2] {"December",12}
            };

            months_list = new List(months,150);

            ArrayList years = new ArrayList();

          
            //TODO get current year
            for(int i = current_year; i < 2027; i++)
                years.Add(new object[2] { i.ToString(), i });
            
            years_list = new List(years, 150);


            /* Processing Payment Window */
            _processingPaymentWindow = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.processingPaymentWindow));
            Image _processingThumb = new Image("processing-thumb", 255, 96, 30, 128, 128);
            _processingThumb.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.processingPayment), Bitmap.BitmapImageType.Jpeg);
            _processingPaymentWindow.AddChild(_processingThumb);


            /* Payment Via Paypal */
            _paypal_payment = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.paypalPaymentWindow));
            _backToPaymentBtn = (GHI.Glide.UI.Button)_paypal_payment.GetChildByName("BackToPaymentBtn");
            _paypal_doneBtn = (GHI.Glide.UI.Button)_paypal_payment.GetChildByName("doneBtn");
            _paypal_doneBtn.Visible = true;
            _paypal_doneBtn.Enabled = false;
            _qrCodeSample = new Image("qrcode", 255, 10, 47, 160, 160);
            _qrCodeSample.Visible = false;
            _paypal_payment.AddChild(_qrCodeSample);

            waitQRCode = new Image("clessidra", 128, 10, 47, 160, 160);
            waitQRCode.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.waitingQR), Bitmap.BitmapImageType.Jpeg);
            waitQRCode.Visible = true;
            _paypal_payment.AddChild(waitQRCode);
            
            

            /* Payment Success */
            _paymentSuccess = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.paymentSuccessfull));
            Image _pSuccesslogo = new Image("psuccess-logo", 255, 96, 30, 128, 128);
            _pSuccesslogo.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.success), Bitmap.BitmapImageType.Jpeg);
            _paymentSuccess.AddChild(_pSuccesslogo);


            /* Payment Error */
            _paymentError = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.paymentError));
            Image _pErrorlogo = new Image("perror-logo", 255, 96, 30, 128, 128);
            _pErrorlogo.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.error), Bitmap.BitmapImageType.Jpeg);
            _paymentError.AddChild(_pErrorlogo);



            /* NetworkErrorWindow */
            _errorWindow = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.ErrorWindow));
            Image _Errorlogo = new Image("error-logo", 255, 0, 0, displayTE35.Width, displayTE35.Height);
            _Errorlogo.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.Connection_error), Bitmap.BitmapImageType.Jpeg);
            _errorWindow.AddChild(_Errorlogo);

            
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
            _payBtn.TapEvent += _payBtn_TapEvent;
            _creditCard.TapEvent += _creditcard_TapEvent;
            _paypal.TapEvent += _paypal_TapEvent;
            _input_creditCard_owner.TapEvent += new OnTap(Glide.OpenKeyboard);
            _input_creditCard_number.TapEvent += new OnTap(Glide.OpenKeyboard);
            _input_creditCard_number.ValueChangedEvent += _input_creditCard_number_ValueChangedEvent;
            _input_creditCard_cvv.TapEvent += new OnTap(Glide.OpenKeyboard);
            _input_expiration_month.TapEvent += monthsTapEvent;
            _input_expiration_year.TapEvent += yearsTapEvent;
            _paypal_doneBtn.PressEvent += _paypal_doneBtn_PressEvent;
            _ccBackBtn.TapEvent += _ccBackBtnTapEvent;
            _ccConfirmBtn.TapEvent += _ccConfirmBtnTapEvent;
            _backToOrderBtn.TapEvent += _backToOrderBtnTapEvent;
            _backToPaymentBtn.TapEvent += _backToPaymentBtn_TapEvent;



            /* Init Datagrids */

            /* _menu -> _dataGrid */
            _dataGrid.AddColumn(new DataGridColumn("ID", 0));
            _dataGrid.AddColumn(new DataGridColumn("MEAL", 175));
            _dataGrid.AddColumn(new DataGridColumn("PRICE", 50));
            _dataGrid.AddColumn(new DataGridColumn("QTY", 30));
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

        private void _backToPaymentBtn_TapEvent(object sender)
        {

            try
            {
                _socket.Send(Encoding.UTF8.GetBytes("BACK\r\n"));
            }catch(SocketException se)
            {
                Debug.Print(se.Message);
                return;
            }

            loadGUI(_scegliPagamento);
        }

        private void _paypal_doneBtn_PressEvent(object sender)
        {

            if (!qrCodeFlag)
                return;

            _paypal_payment.Invalidate();

            loadGUI(_processingPaymentWindow);
            System.Threading.Thread.Sleep(1500);

            try
            {


                _socket.Send(Encoding.UTF8.GetBytes(PAYMENT_CONFIRM));

                byte[] response = new byte[3];
                _socket.Receive(response, 3, SocketFlags.None);

                var stream = new MemoryStream(response);
                StreamReader sr = new StreamReader(stream);
                string response_as_str = sr.ReadToEnd();


                if (response_as_str == "+OK")
                {
                    // GO TO SUCCESS AND THEN START
                    loadGUI(_paymentSuccess);
                    System.Threading.Thread.Sleep(10000);
                    loadGUI(_mainwindow);

                }
                else if (response_as_str == "ERR")
                {
                    //GO TO ERR AND THEN CHOOSE PAYMENT
                    loadGUI(_paymentError);
                    System.Threading.Thread.Sleep(5000);
                    loadGUI(_scegliPagamento);
                }


            }
            catch(SocketException ex)
            {
                Debug.Print(ex.Message);
            }


        }

        private void _input_creditCard_number_ValueChangedEvent(object sender)
        {
            string number = _input_creditCard_number.Text;
            
            string type = GetCardTypeFromNumber(number);
            if (type != null)
            {
                switch (type)
                {
                    case "visa":
                        _visaThumb.Alpha = 255;
                        break;
                    case "mastercard":
                        _mastercardThumb.Alpha = 255;
                        break;
                    case "amex":
                        _americanexpressThumb.Alpha = 255;
                        break;
                }
            }
            
            
            
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

            ArrayList al = null;


            try
            {
                Debug.Print("Making Request to WebService");
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Timeout = 10000; // timeout to 10 seconds
                Debug.Print("Waiting for response from WebService");
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();

                if (res.StatusCode.ToString().Equals("200"))
                {

                    Debug.Print("Response: " + res.StatusCode);

                    Stream stream = res.GetResponseStream();
                    sr = new StreamReader(stream);
                    string json = sr.ReadToEnd();

                    Debug.Print("Menu: " + json);


                    al = Json.NETMF.JsonSerializer.DeserializeString(json) as ArrayList;

                }

                return al;

            }
            catch (System.Net.WebException se)
            {
                Debug.Print(se.Message);
                return null;
            }



           

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

        private void progressLed(GHI.Glide.Display.Window window)
        {

            if (window != _cancel)
            {
                if (window == _paymentError)
                {
                    led_position = 6;
                }
                else if (window == _menu)
                {
                    led_position = 5;
                }
                else if (window == _ordina)
                {
                    led_position = 4;
                }
                else if (window == _scegliPagamento)
                {
                    led_position = 3;
                }
                else if (window == _credit_card_payment || window == _paypal_payment)
                {
                    led_position = 2;
                }
                else if (window == _processingPaymentWindow)
                {
                    led_position = 1;
                }else if(window == _paymentSuccess || window == _mainwindow)
                {
                    led_position = 0;
                }
                
                
                ledStrip.TurnAllLedsOff();
                ledStrip.TurnLedOn(led_position);
            }
            
        }

        private void loadGUI(GHI.Glide.Display.Window window,bool b)
        {
            if (window == _mainwindow)
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;

                    if (b) {
                        Debug.Print("Main thread aspetta morte heartbeat");
                        HeartBeatEvent.WaitOne();
                        HeartBeatEvent.Reset();

                        Debug.Print("Main thread scopre morte heartbeat");
                        
                    }
                        
                   
                }
                
            }


            Glide.MainWindow = window;
            progressLed(window);
        }

        private void loadGUI(GHI.Glide.Display.Window window)
        {

            loadGUI(window, true);

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

            if (orders.Size() > 3)
                _dataGrid.RowCount = 4;
            else
                _dataGrid.RowCount = 3;

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
            _menu.Invalidate();

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
            led_position = 0;
            qrCodeFlag = false;
            processingPayment = false;

            _socket = null;

            _ordBtn.Enabled = false;

            orderId = "";
            ORDER_CMD = NEW_ORDER;

            orders.Clear();
            orders.Total = 0;
            orders.Price = 0;

            _ingredients.Text = "";
            _pCounter.Text = "";


            _dataGrid.Clear();


            _qrCodeSample.Visible = false;
            _qrCodeSample.Bitmap = null;
            waitQRCode.Visible = true;


            _startbtn.Enabled = true;
            _loadingLbl.Visible = false;

            ledStrip.TurnLedOn(led_position);

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
        private string GetTimeStamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssfff");
           
        }

        private byte[] encryptXTEA(String target,String key)
        {
            // Aggiungo Padding per allinearmi a 16
            while ((target.Length % Xtea.Align) != 0)
                target += "\r\n";


            //Cifro con la chiave passata
            byte[] msg_body = Encoding.UTF8.GetBytes(target);
            Xtea xt = new Xtea(Encoding.UTF8.GetBytes(key));
            xt.encrypt(msg_body, 0, msg_body.Length);

            
            //Genero l'esadecimale
            String output = "";
            for (int i = 0; i < msg_body.Length; i++)
                output += msg_body[i].ToString("X2");

            
            //Ritorno l'esadecimale del byte[] cifrato
            return Encoding.UTF8.GetBytes(output);

        }

        private void processPayment(string creditCard) {

            
            Debug.Print(creditCard);
            loadGUI(_processingPaymentWindow);

            if (recoveryPayment(false))
                return;

            
            processingPayment = true;

            if (qrCodeFlag)
            {
                qrCodeFlag = false;
                _qrCodeSample.Bitmap = null;
                _qrCodeSample.Visible = false;
                waitQRCode.Visible = true;
                
            }


            byte[] msg_header  = Encoding.UTF8.GetBytes(PAYMENT_CARD);

            /* cifro dati carta di credito con Xtea */
            byte[] hex_encrypted_body = encryptXTEA(creditCard, key);

            try
            {
                _socket.Send(msg_header);
                _socket.Send(hex_encrypted_body);
                _socket.Send(Encoding.UTF8.GetBytes("\r\n"));
                

                byte[] response = new byte[3];
                _socket.Receive(response, 3, SocketFlags.None);

                var stream = new MemoryStream(response);
                StreamReader sr = new StreamReader(stream);
                string response_as_str = sr.ReadToEnd();




                if (response_as_str == "ERR")
                {
                    loadGUI(_paymentError);
                    System.Threading.Thread.Sleep(5000);
                    processingPayment = false;
                    loadGUI(_scegliPagamento);

                }else
                {
                    Debug.Print("Payment Success");

                    loadGUI(_paymentSuccess);
                    System.Threading.Thread.Sleep(2000);
                    _socket.Send(Encoding.UTF8.GetBytes(PAYMENT_CONFIRM));
                    System.Threading.Thread.Sleep(2000);
                    loadGUI(_mainwindow);

                }



            }catch(SocketException ex)
            {
                Debug.Print(ex.ToString());




            }

            
        }
        /*
             
              TYPE             PREFIX      LENGTH
              
              Visa             4           13,16
              Master Card      51 to 55    16
              American Expr.   34, 37      15
              
        */
        private String GetCardTypeFromNumber(string number)
        {

            if (number.Length < 13)
                return null;

            string retval = null;

            string prefix = number.Substring(0, 2);

            if ((prefix.Equals("34") || prefix.Equals("37")) && number.Length == 15)
                retval = "amex";
            else if ((prefix.Equals("51") || prefix.Equals("52") || prefix.Equals("53") || prefix.Equals("54") || prefix.Equals("55")) && number.Length==16)
                retval = "mastercard";
            else
            {
                prefix = number.Substring(0, 1);
                if (prefix.Equals("4") && (number.Length >= 13 && number.Length <= 16))
                    retval = "visa";
            }
            

            return retval;
        }
        #endregion

        #region Events Implementation

        /*ordBtn TapEvent*/
        private void _ordBtn_PressEvent(object sender)
        {

            if (qrCodeFlag)
            {
                qrCodeFlag = false;
                _qrCodeSample.Bitmap = null;
                _qrCodeSample.Visible = false;
                waitQRCode.Visible = true;
                _paypal_doneBtn.Enabled = false;
                   
            }

            String order_as_json = GetOrderAsJson();

            Debug.Print(ORDER_CMD);
            Debug.Print(order_as_json);

            /* Send Socket */

            byte[] msg_header = Encoding.UTF8.GetBytes(ORDER_CMD);
            byte[] msg_body = Encoding.UTF8.GetBytes(order_as_json + "\r\n");


            try
            {
                _socket.Send(msg_header);
                _socket.Send(msg_body);

            }catch(SocketException ex)
            {
                Debug.Print("Error Sending " + ORDER_CMD);
                Debug.Print("SocketException: " + ex.ToString());
                return;
            }



            /* Update Command */
            ORDER_CMD = UPDATE_ORDER;

            /* Update GUI */

            _gridOrdine.Clear();
            
            foreach (Order o in orders.List)
                if (o.Quantity > 0)
                    _gridOrdine.AddItem(new DataGridItem(new object[4] { o.Product.id, o.Product.nome,o.Product.prezzo, o.Quantity }));

            _finalPrice.Text = orders.Price + " EURO";

            previousWindow = Glide.MainWindow;
            loadGUI(_ordina);



            

        }
        private void _payBtn_TapEvent(object sender)
        {
            loadGUI(_scegliPagamento);       
        }
        private void _creditcard_TapEvent(object sender)
        {
            loadGUI(_credit_card_payment);
        }
        private void resetPaymentInputs()
        {
            _input_creditCard_owner.Text = "";
            _input_creditCard_number.Text = "";
            _input_creditCard_cvv.Text = "";
            _input_expiration_month.Text = "Month";
            _input_expiration_year.Text = "Year";
            _visaThumb.Alpha = 128;
            _mastercardThumb.Alpha = 128;
            _americanexpressThumb.Alpha = 128;
            _credit_card_payment.Invalidate();
        }
        private void _ccBackBtnTapEvent(object sender)
        {
            resetPaymentInputs();
            loadGUI(_scegliPagamento);
        }
        
        private void _ccConfirmBtnTapEvent(object sender)
        {
            

            string message = "";


            string owner    = _input_creditCard_owner.Text;
            string number   = _input_creditCard_number.Text;
            string cvv      = _input_creditCard_cvv.Text;
            string month    = _input_expiration_month.Text;
            string year     = _input_expiration_year.Text;
            string type     = null;

            int month_as_number = 0;

            number  = number.Trim();
            cvv     = cvv.Trim();

            bool valid_owner = true;
            bool valid_cvv = true;
            bool valid_number = true;


            

            if (!owner.Equals("") && !number.Equals("") && !cvv.Equals(""))
            {


                owner = owner.TrimStart();
                owner = owner.TrimEnd();
                foreach (char c in owner.ToCharArray())
                {
                    if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == ' ' || c=='à' || c=='ò' || c=='è' || c=='é' || c=='ù')
                        continue;
                    else
                    {
                        valid_owner = false;
                        break;
                    }
                    
                }

                if (!valid_owner) {
                    message = "Wrong Name";
                    _ccErrMsg.Text = message;
                    _credit_card_payment.Invalidate();
                    return;
                }


                number.Trim(); //Removing spaces
                foreach (char c in number.ToCharArray())
                {
                    if (c < '0' || c > '9') { 
                        valid_number = false;
                        break;
                    }

                }

                if (!valid_number)
                {
                    message = "Wrong Card Number";
                    _ccErrMsg.Text = message;
                    _credit_card_payment.Invalidate();
                    return;
                }
                

                type = GetCardTypeFromNumber(number);

                if (type != null)
                {

                    foreach (char c in cvv.ToCharArray())
                    {
                        if (c < '0' || c > '9')
                        {
                            valid_cvv = false;
                            break;
                        }
                            
                    }

                    if (cvv.Length != 3 || !valid_cvv)
                    {
                        message = "Wrong CVV";
                        _ccErrMsg.Text = message;
                        _credit_card_payment.Invalidate();
                    }
                    else if (month.Equals("Month") || year.Equals("Year"))
                    {
                        message = "Wrong Expiration Date";
                        _ccErrMsg.Text = message;
                        _credit_card_payment.Invalidate();
                    }
                    else
                    {
                        //Process Payment
                        month_as_number = int.Parse(_input_expiration_month.Value.ToString());
                        Debug.Print(owner + " " + number + " " + cvv + " " + month_as_number + " " + year);


                        
                        string[] splitted = owner.Split(' ');
                        string firstname = "";

                        // Supporting multiple names
                        for (int i = 0; i < splitted.Length-1; i++)
                            if (i == 0)
                                firstname = splitted[i];
                            else
                                firstname += " " + splitted[i];

                        string lastname = splitted[splitted.Length-1];


                        Hashtable card = new Hashtable();
                        card.Add("firstname", firstname);
                        card.Add("lastname", lastname);
                        card.Add("cvv2", cvv);
                        card.Add("number", number);
                        card.Add("expireMonth", month_as_number);
                        card.Add("expireYear", year);
                        card.Add("type", type);

                        string card_as_json = Json.NETMF.JsonSerializer.SerializeObject(card);

                        resetPaymentInputs();
                        processPayment(card_as_json);

                    }
                }
                else {

                    message = "Wrong Card Number";
                    _ccErrMsg.Text = message;
                    _credit_card_payment.Invalidate();
                }
                
                

            }
            else { 
                message = "Please fill in all fields";
                _ccErrMsg.Text = message;
                _credit_card_payment.Invalidate();

            }

        }
        private void monthsTapEvent(object sender)
        {
            Glide.OpenList(sender, months_list);
        }
        private void yearsTapEvent(object sender)
        {
            Glide.OpenList(sender, years_list);
        }
        private void _paypal_TapEvent(object sender)
        {
            loadGUI(_paypal_payment);

            // Waiting for Response
            // 0. Sending PAYMENT_PAYPAL
            // 1. PAYMENT_OK
            // 1.1 Waiting for byte stream
            // 1.2 Render QR Code
            // 1.3 Wait for user tap Done Button
            //1.4 Send Payment_Confirm to Server
            // 1.4.1 OK 
            //1.4.1.1 Go to END PAGE
            // 1.4.2 ERR
            // 1.4.2.1 Go TO ERR PAGE
            // 1.4.2.2 Redirect to Choose Payment after 5 seconds

            // 2. PAYMENT_ERR
            // 2.1 Go To ERR PAGE
            // 2.2 Redirect to Choose Payment after 5 seconds
            

            if(!qrCodeFlag){

                _paypal_payment.Invalidate();

                try
                {
                    Debug.Print("Requesting QRCode");
                    _socket.Send(Encoding.UTF8.GetBytes(PAYMENT_PAYPAL));

                    byte[] response = new byte[3];
                    _socket.Receive(response, 3, SocketFlags.None);

                    var stream = new MemoryStream(response);
                    StreamReader sr = new StreamReader(stream);
                    string response_as_str = sr.ReadToEnd();

                    if (response_as_str == "+OK")
                    {


                        _paypal_doneBtn.Enabled = true;
                        _paypal_payment.Invalidate();


                        Debug.Print("Aspetto immagine");

                        byte[] size = new byte[5];

                        _socket.Receive(size, 5, SocketFlags.None);

                        stream = new MemoryStream(size);
                        sr = new StreamReader(stream);
                        string size_as_str = sr.ReadToEnd();



                        int len = int.Parse(size_as_str);



                        byte[] img = new byte[len];
                        Array.Clear(img, 0, len);


                        int tmp = 0;
                        int count = 0;
                        while (count < len)
                        {
                            if (count + 1024 > len)
                                tmp = len - count;
                            else
                                tmp = 1024;

                            count += _socket.Receive(img, count, tmp, SocketFlags.None);

                        }

                        Debug.Print("count: " + count);

                        byte[] end = new byte[3];

                        _socket.Receive(end, 3, SocketFlags.None);

                        stream = new MemoryStream(end);
                        sr = new StreamReader(stream);
                        Debug.Print("Ho ricevuto --> " + sr.ReadToEnd());

                        // Render QR CODE + DONE button
                        waitQRCode.Visible = false;
                        _qrCodeSample.Bitmap = new Bitmap(img, Bitmap.BitmapImageType.Jpeg);
                        _qrCodeSample.Visible = true;
                        qrCodeFlag = true;
                        _paypal_payment.Invalidate();



                    }
                    else if (response_as_str == "ERR")
                    {

                        loadGUI(_paymentError);
                        System.Threading.Thread.Sleep(5000);
                        loadGUI(_scegliPagamento);

                    }




                }
                catch (SocketException ex)
                {
                    Debug.Print(ex.Message);
                  //  loadGUI(_errorWindow);
                    loadGUI(_mainwindow);
                }

            }









        }
        private void _backToOrderBtnTapEvent(object sender)
        {
            loadGUI(_ordina);
        }
        private void _noBtn_TapEvent(object sender)
        {
            loadGUI(_menu);
        }
        private void _backBtn_TapEvent(object sender)
        {
            loadGUI(_menu);
        }
        private void _siBtn_TapEvent(object sender)
        {
            String target = "";
            if (ORDER_CMD == NEW_ORDER)
                target = CLOSE;
            else
                target = CANCEL_ORDER;

            byte[] msg_cancel = Encoding.UTF8.GetBytes(target);

            try
            {
                _socket.Send(msg_cancel);

                loadGUI(_mainwindow);
            }
            catch(SocketException ex)
            {
                Debug.Print(ex.Message);
            }
            
        }
        private void deleteBtn_PressEvent(object sender)
        {
            loadGUI(_cancel);
        }
        /*Ethernet Network_Down Function*/
        private void ethernetJ11D_NetworkDown(GTM.Module.NetworkModule sender,GTM.Module.NetworkModule.NetworkState state)
        {
            DownEvent.Reset();
            previousWindow = Glide.MainWindow;
            

            if (startup) {
                Debug.Print("Network Down");
                loadGUI(_errorWindow);
            }

        }
        ///*Ethernet Network_Up Function*/
        private void ethernetJ11D_NetworkUp(GTM.Module.NetworkModule sender,GTM.Module.NetworkModule.NetworkState state)
        {


            Debug.Print("Network is UP");

            DownEvent.Set();

            if (!startup) { 
                startup = true;
                loadGUI(_mainwindow);
            }else {
                if (previousWindow == _processingPaymentWindow)
                {
                    if (qrCodeFlag)
                        loadGUI(_paypal_payment);
                    else 
                        recoveryPayment(true);
                     
                }
                else
                    loadGUI(previousWindow);

            }

            HeartBeatEvent.Set();
        }


        private bool recoveryPayment(bool flag)
        {

            Debug.Print("Retrieving payment Info");

            loadGUI(_processingPaymentWindow);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(payment_url + orderId);
            req.Timeout = 10000; // timeout to 10 seconds
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            if (res.StatusCode.ToString().Equals("200"))
            {


                Stream stream = res.GetResponseStream();
                sr = new StreamReader(stream);
                string str = sr.ReadToEnd();

                switch (str)
                {
                    case "OK":

                        _socket.Send(Encoding.UTF8.GetBytes(PAYMENT_CONFIRM));
                        loadGUI(_paymentSuccess);
                        System.Threading.Thread.Sleep(5000);
                       

                        loadGUI(_mainwindow);
                        return true;
                    default:
                        if (flag) { 
                            loadGUI(_paymentError);
                            System.Threading.Thread.Sleep(5000);
                            processingPayment = false;
                            loadGUI(_credit_card_payment);
                        }
                        return false;

                }

            }

            return false;

        }

        private void Start_PressEvent(object sender)
        {

            /* Start 2nd Thread */
            Debug.Print("Starting Session");
            tHeartBeat = new System.Threading.Thread(() => {

                int counter = 0;
                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                
                IPHostEntry hostInfo = Dns.GetHostEntry(HOST);
                IPAddress ipAddr = hostInfo.AddressList[0];
                IPEndPoint remEP = new IPEndPoint(ipAddr, 4097);

                Debug.Print("HEARTBEAT: Connecting to "+remEP.Address.ToString());

                s.Connect(remEP);
               
                if (s.RemoteEndPoint.ToString() == (HOST + ":4097"))
                {
                    Debug.Print("Heartbeat connected");
                    
                    string response_as_str = "";
                    byte[] response = new byte[6];

                    HeartBeatEvent.Set();
                    s.ReceiveTimeout = 3000;

                    while (true)
                    {
                        DownEvent.WaitOne();
                        try
                        {
                            if (counter == 10)
                            {

                                // non riesco a contattare il client, termino e faccio terminare il main thread
                                s.Close();
                                s = null;
                                loadGUI(_serverError);
                                System.Threading.Thread.Sleep(2000);
                                loadGUI(_mainwindow, false);
                                break;

                            }else if (_socket == null)
                            {
                                // Il main thread ha deciso di terminare la sessione
                                s.Close();
                                s = null;
                                break;
                            }


                            s.Send(Encoding.UTF8.GetBytes("PING\r\n"));
                            s.Receive(response, 6, SocketFlags.None);

                            s.ReceiveTimeout = 3000;

                            StreamReader sr = new StreamReader(new MemoryStream(response));
                            response_as_str = sr.ReadLine();
                            Debug.Print(response_as_str);

                            counter = 0;
                            
                            System.Threading.Thread.Sleep(500);
                        }
                        catch (SocketException se)
                        {

                            Debug.Print("Heartbeat get exception");
                            counter++;
                            s.ReceiveTimeout = 3000;
                        }

                    }

                    Debug.Print("Heartbeat is going to die...");
                    HeartBeatEvent.Set();
                    


                }
                else {

                    Debug.Print("HEARTBEAT: Failed connecting socket");
                    s = null;
                    HeartBeatEvent.Set();
                  

                }

                            
            });



            ResetStatus();

            _loadingLbl.Text = "Loading, Please Wait..";
            _startbtn.Enabled = false;
            _loadingLbl.Visible = true;
            _mainwindow.Invalidate();

            
            orderId = RandomString(32);
            Debug.Print("orderId: " + orderId);


            // Socket connection
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.ReceiveTimeout = 15000;


            IPHostEntry ipHostInfo = Dns.GetHostEntry(HOST);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, PORT);

            Debug.Print("Connecting to: "+remoteEP.Address.ToString());
            _socket.Connect(remoteEP);

            if (_socket.RemoteEndPoint.ToString() == (HOST + ":" + PORT)) {


                Debug.Print("Socket connected to " + _socket.RemoteEndPoint.ToString());
                Debug.Print("Starting thread");
                tHeartBeat.Start();

                Debug.Print("Aspetto thread");
                HeartBeatEvent.WaitOne();
                Debug.Print("Notifica arrivata");
                HeartBeatEvent.Reset();
                if (s == null) { 
                    _socket.Close();
                    _socket = null;

                    _startbtn.Enabled = true;
                    _loadingLbl.Visible = false;
                    _mainwindow.Invalidate();

                    return;
                }
                

                Debug.Print("Secondo thread creato correttamente");
               

            }
            else {

                Debug.Print("Failed connecting socket");
                _socket = null;
                _startbtn.Enabled = true;
                _loadingLbl.Visible = false;
                _mainwindow.Invalidate();
                return;

            }




            menu = downloadMenu(url);
            if (menu != null)
            {


                _startbtn.Enabled = true;
                _loadingLbl.Visible = false;

                initOrders();

                loadGUI(_menu);

                printDatagrid();


                return; // Sucessfull exit!
            }else
            {
                //chiudo socket
                _socket.Close();
            }

           
            
            
            // something wrong, may not arrive here!
            _startbtn.Enabled = true;
            _loadingLbl.Visible = false;
            loadGUI(_mainwindow);
                

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
