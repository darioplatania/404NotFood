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
        private static GHI.Glide.Display.Window _scegliPagamento;
        private static GHI.Glide.Display.Window _credit_card_payment;
        private static GHI.Glide.Display.Window _paypal_payment;
        private static GHI.Glide.Display.Window _processingPaymentWindow;
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
        private GHI.Glide.UI.Button _ccBackBtn;
        private GHI.Glide.UI.Button _ccConfirmBtn;
        private GHI.Glide.UI.TextBlock _ccErrMsg;
        private GHI.Glide.UI.Button _backToOrderBtn;

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

        private List months_list;
        private List years_list;

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
            Image _visaThumb = new Image("visa-thumb", 255,20, 25, 48, 30);
            _visaThumb.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.visa), Bitmap.BitmapImageType.Jpeg);
            Image _mastercardThumb = new Image("mastercard-thumb", 255, 20, 60, 48, 30);
            _mastercardThumb.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.mastercard), Bitmap.BitmapImageType.Jpeg);
            Image _americanexpressThumb = new Image("americanexpress-thumb", 255, 20, 95, 48, 30);
            _americanexpressThumb.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.americanexpress), Bitmap.BitmapImageType.Jpeg);

            _credit_card_payment.AddChild(_visaThumb);
            _credit_card_payment.AddChild(_mastercardThumb);
            _credit_card_payment.AddChild(_americanexpressThumb);


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
            for(int i = 2017; i < 2027; i++)
                years.Add(new object[2] { i.ToString(), i });
            
            years_list = new List(years, 150);



            /* Processing Payment Window */
            _processingPaymentWindow = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.processingPaymentWindow));
            Image _processingThumb = new Image("processing-thumb", 255, 86, 56, 128, 128);
            _processingThumb.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.processingPayment), Bitmap.BitmapImageType.Jpeg);
            _processingPaymentWindow.AddChild(_processingThumb);


            /* Payment Via Paypal */
            _paypal_payment = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.paypalPaymentWindow));
            Image _qrcodeSample = new Image("qrcode", 255, 0, 0, 320, 240);
            _qrcodeSample.Bitmap = new Bitmap(Resources.GetBytes(Resources.BinaryResources.sample_qrcode), Bitmap.BitmapImageType.Jpeg);
            _paypal_payment.AddChild(_qrcodeSample);

            /* NetworkErrorWindow */
            _errorWindow = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.ErrorWindow));
            
            // NetworkErrorWindow.Adding Error Logo
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
            _input_creditCard_cvv.TapEvent += new OnTap(Glide.OpenKeyboard);
            _input_expiration_month.TapEvent += monthsTapEvent;
            _input_expiration_year.TapEvent += yearsTapEvent;
            _ccBackBtn.TapEvent += _ccBackBtnTapEvent;
            _ccConfirmBtn.TapEvent += _ccConfirmBtnTapEvent;
            _backToOrderBtn.TapEvent += _backToOrderBtnTapEvent;



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

        //TODO Commentare sopra ogni signature

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
        private string GetTimeStamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssfff");
           
        }
        private void processPayment(string creditCard) {

            _ccConfirmBtn.Enabled = false;
            _ccConfirmBtn.TapEvent -= _ccConfirmBtnTapEvent;

            Debug.Print(creditCard);
            loadGUI(_processingPaymentWindow);


            //TODO PROCESS PAYMENT

            _ccConfirmBtn.Enabled = true;
            _ccConfirmBtn.TapEvent += _ccConfirmBtnTapEvent;

            System.Threading.Thread.Sleep(3000);
            loadGUI(_mainwindow);

        }
        /*
             
              TYPE             PREFIX      LENGTH
              
              Visa             4           13,16
              Master Card      51 to 55    16
              American Expr.   34, 37      15
              
        */
        private String GetCardTypeFromNumber(string number)
        {
            string retval = null;
            string prefix = number.Substring(0, 2);

            if (prefix.Equals("34") || prefix.Equals("37"))
                retval = "American Express";
            else if (prefix.Equals("51") || prefix.Equals("52") || prefix.Equals("53") || prefix.Equals("54") || prefix.Equals("55"))
                retval = "Master Card";
            else
            {
                prefix = number.Substring(0, 1);
                if (prefix.Equals("4"))
                    retval = "Visa";
            }
            

            return retval;
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
            
            if(!owner.Equals("") && !number.Equals("") && !cvv.Equals(""))
            {
                //TODO REPLACE ALL Special Symbols from number
                type = GetCardTypeFromNumber(number);

                if (type != null)
                {
                    if ((type.Equals("Visa") && (number.Length < 13 || number.Length > 16))
                    || ((type.Equals("Master Card") && number.Length != 16))
                    || ((type.Equals("American Express") && number.Length != 15)))
                    {
                        message = "Wrong Card Number";
                        _ccErrMsg.Text = message;
                        _credit_card_payment.Invalidate();
                    }
                    else if (cvv.Length > 3)
                    {
                        message = "Wrong Expiration Date";
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
                        string firstname = splitted[0];
                        string lastname = splitted[1];


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
        }
        private void _backToOrderBtnTapEvent(object sender)
        {
            loadGUI(_ordina);
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
            previousWindow = Glide.MainWindow;
            timestamp = Double.Parse(GetTimeStamp(DateTime.Now));
            loadGUI(_errorWindow);

        }
        ///*Ethernet Network_Up Function*/
        private void ethernetJ11D_NetworkUp(GTM.Module.NetworkModule sender,GTM.Module.NetworkModule.NetworkState state)
        {
            int timeout = 10000;
            double up_timestamp = Double.Parse(GetTimeStamp(DateTime.Now));
            //Debug.Print(up_timestamp + " - " + timestamp + " = " + (up_timestamp - timestamp));
            if ((up_timestamp - timestamp) < timeout && startup)
                loadGUI(previousWindow);
            else {
                startup = true;
                loadGUI(_mainwindow);
            }


        }
        private void Start_PressEvent(object sender)
        {

            
            _startbtn.Enabled = false;
            _loadingLbl.Visible = true;
            _mainwindow.Invalidate();

            ResetStatus();
            
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
