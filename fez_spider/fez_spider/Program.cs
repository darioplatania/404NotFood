using Gadgeteer.Modules.GHIElectronics;
using System;
using System.Collections;
using System.Threading;
using System.Net;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using System.IO;
using System.Net.Sockets;


namespace fez_spider
{
    public partial class Program
    {
        
        private static GHI.Glide.Display.Window _mainwindow;             
        private static GHI.Glide.Display.Window _menu;
        private static GHI.Glide.Display.Window _ordina;
        private GHI.Glide.UI.Button _startbtn;
        private GHI.Glide.UI.Button _deleteBtn;
        private GHI.Glide.UI.Button _ingBtn;
        private GHI.Glide.UI.Button _ordBtn;
        private GHI.Glide.UI.Button _payBtn;
        private GHI.Glide.UI.Button _annullaBtn;
        private GHI.Glide.UI.Button _mdfBtn;
        private GHI.Glide.UI.DataGrid _dataGrid;
        private GHI.Glide.UI.TextBlock _pCounter;
        private GHI.Glide.UI.TextBlock _qntCounter;
        private GHI.Glide.UI.TextBlock _errMsg;
        private GHI.Glide.UI.TextBox _textorder;        
        private  int qnt; 
        private  Double price;
        private  static Font font = Resources.GetFont(Resources.FontResources.NinaB);
        private  Double getid;     
        private  string getpizza;
        private  Double getprice;
        private  int getqnt;
        private  int row = -1;
        private  int count = 0;
        private  int exist = 0;
        private  int aux = 0;
        private  int flagmdf = 0;    
        byte[] result = new byte[65536];
        ArrayList payment = new ArrayList();
        String url = "http://192.168.100.1:8080/food/webapi/food";
        



        /*This method is run when the mainboard is powered up or reset*/
        void ProgramStarted()
        {
            /*Use Debug.Print to show messages in Visual Studio's "Output" window during debugging*/
            Debug.Print("Program Started");           

            /*Ethernet Configuration*/
            ethernetJ11D.UseThisNetworkInterface();
            //ethernetJ11D.UseStaticIP("")
            ethernetJ11D.NetworkUp += ethernetJ11D_NetworkUp;
            ethernetJ11D.NetworkDown += ethernetJ11D_NetworkDown;                                     
            new Thread(RunWebServer).Start();           

            /*welcome into display*/
            first_step();
            /*button plus(input 4)*/
            plus.ButtonPressed += Plus_ButtonPressed;
            plus.TurnLedOff();
            /*button minus(input 5)*/
            minus.ButtonPressed += Minus_ButtonPressed;
            minus.TurnLedOff();
        }      

        /****************
         * FUNCTION 
         * *************/
        void first_step()
        {
            flagmdf = 0;
            Glide.FitToScreen = true;
            _mainwindow = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.Window));          

            GlideTouch.Initialize();           
            Glide.MainWindow = _mainwindow;

            /*create button to start*/
           _startbtn = (GHI.Glide.UI.Button)_mainwindow.GetChildByName("startbtn");            
            /*press button event*/            
           _startbtn.PressEvent += Button_PressEvent;

            //Bitmap prova = new Bitmap(Resources.GetBytes(Resources.BinaryResources.start), Bitmap.BitmapImageType.Jpeg);

            //displayTE35.SimpleGraphics.DisplayImage(prova, 30, 20);
            //displayTE35.BacklightEnabled = true;
        }

        void initMenu()
        {
            
            Debug.Print("Init Menu!");

            /*load menu*/
            _menu = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.Menu));
            Glide.MainWindow = _menu;

            _dataGrid = (GHI.Glide.UI.DataGrid)_menu.GetChildByName("dataGrid");
            _pCounter = (GHI.Glide.UI.TextBlock)_menu.GetChildByName("pCounter");
            _qntCounter = (GHI.Glide.UI.TextBlock)_menu.GetChildByName("qntCounter");
            _errMsg   = (GHI.Glide.UI.TextBlock)_menu.GetChildByName("errMsg");            

            _ordBtn = (GHI.Glide.UI.Button)_menu.GetChildByName("ordBtn");
            if (flagmdf == 0)
                _ordBtn.Enabled = false;
            else
                _ordBtn.Enabled = true;
            _menu.Invalidate();
            _ordBtn.PressEvent += _ordBtn_PressEvent;

            _deleteBtn = (GHI.Glide.UI.Button)_menu.GetChildByName("deleteBtn");
            if (flagmdf == 0)
                _deleteBtn.Enabled = false;
            else
                _deleteBtn.Enabled = true;
            _menu.Invalidate();
            _deleteBtn.PressEvent += deleteBtn_PressEvent;

            //_ingBtn = (GHI.Glide.UI.Button)_menu.GetChildByName("ingBtn");
            //_ingBtn.Visible = false;
            //_menu.Invalidate();
            //_ingBtn.PressEvent += ingBtn_PressEvent;      
            
            /*Setup the dataGrid reference*/
            _dataGrid = (DataGrid)_menu.GetChildByName("dataGrid");
                        
            // Listen for tap cell events.
            _dataGrid.TapCellEvent += new OnTapCell(dataGrid_TapCellEvent);

            /*Create our four columns*/
            _dataGrid.AddColumn(new DataGridColumn("ID", 0));
            _dataGrid.AddColumn(new DataGridColumn("PIZZA", 125));
            _dataGrid.AddColumn(new DataGridColumn("PREZZO", 80));
            _dataGrid.AddColumn(new DataGridColumn("QNT", 50));
            
            /*Populate the data grid with random data*/
            Populate();

            /*Add the data grid to the window before rendering it*/
            _menu.AddChild(_dataGrid);            
            _dataGrid.Render();            

            /*Create a timer & run method timer_trick when thr timer ticks (for joystick)*/
            GT.Timer timer = new GT.Timer(200);
            timer.Tick += Timer_Tick; 
            timer.Start(); 
            
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
        void Populate()
        {
            Debug.Print("Populating...");
                        
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            Stream stream = res.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string json = sr.ReadToEnd();
            ArrayList al = Json.NETMF.JsonSerializer.DeserializeString(json) as ArrayList;

            /*populating iniziale*/
            for (int i = 0; i < al.Count; i++)
            {
                Hashtable ht = al[i] as Hashtable;
                _dataGrid.AddItem(new DataGridItem(new object[4] { ht["id"], ht["name"], ht["price"], qnt }));
            }
            _dataGrid.Invalidate();

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
        void _ordBtn_PressEvent(object sender) {

            var random = new Random(System.DateTime.Now.Millisecond);
            uint randomNumber = (uint)random.Next();            
            string id_ordine = randomNumber.ToString();
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

            string order_as_json = Json.NETMF.JsonSerializer.SerializeObject(order);

            // TODO: MANDARE order_as_json al Desktop tramite Socket
            Debug.Print(order_as_json);

            /*load menu*/            
            _ordina = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.Ordina));
            Glide.MainWindow = _ordina;
            _annullaBtn = (GHI.Glide.UI.Button)_ordina.GetChildByName("annullaBtn");
            _payBtn = (GHI.Glide.UI.Button)_ordina.GetChildByName("payBtn");
            _mdfBtn = (GHI.Glide.UI.Button)_ordina.GetChildByName("mdfBtn");

            _annullaBtn.TapEvent += _annullaBtn_TapEvent;
            _mdfBtn.TapEvent += _mdfBtn_TapEvent;
            _payBtn.TapEvent += _payBtn_TapEvent;

            //_textorder = (GHI.Glide.UI.TextBox)_ordina.GetChildByName("textorder");
            //string prova = "aaa";
            // _textorder.Text = prova.ToString();
            
            
            //displayTE35.SimpleGraphics.DisplayText(prova,font,GT.Color.Black,20,10);
            //displayTE35.BacklightEnabled = true;

            /*
             <TextBox Name="textorder" X="20" Y="10" Width="250" Height="150" Alpha="255" TextAlign="Left" Font="4" FontColor="000000"/>
	         <Button Name="paytBtn" X="110" Y="202" Width="100" Height="32" Alpha="255" Text="Paga" Font="4" FontColor="000000" DisabledFontColor="808080" TintColor="000000" TintAmount="0"/>
             */
        }

        /*void mdfMenu()
        {
            Debug.Print("Modified...");

            _pCounter.Text = price.ToString();
            _qntCounter.Text = qnt.ToString();
            _menu.Invalidate();

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            Stream stream = res.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string json = sr.ReadToEnd();
            ArrayList al = Json.NETMF.JsonSerializer.DeserializeString(json) as ArrayList;

            for (int i = 0; i < al.Count; i++)
            {
                /*DataGridItems must contain an object array whose length matches the number of columns.
                Hashtable ht = al[i] as Hashtable;               
                
                    foreach (Product p in payment)
                        if (p.id == Double.Parse(ht["id"].ToString()))
                            qnt = p.quantita;
                _dataGrid.AddItem(new DataGridItem(new object[4] { ht["id"], ht["name"], ht["price"], qnt }));               
            }
            _dataGrid.Invalidate();
        }*/

        /*apre pagina per il pagamento*/
        private void _payBtn_TapEvent(object sender)
        {           
            
        }

        /*modifica ordine prima di pagare*/
        private void _mdfBtn_TapEvent(object sender)
        {
            flagmdf = 1;
            initMenu();
        }

        /*chiamata quando annullo tutto l'ordine prima di pagare e torna all'inizio*/
        private void _annullaBtn_TapEvent(object sender)
        {
            getqnt = 0;//set qnt to 0            
            price = 0;//set total price to 0     
            qnt = 0;//set total qnt to 0            
            payment.Clear();
            first_step();
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
        
        /*Ingredients_btn TapEvent*/
        void ingBtn_PressEvent(object sender)
        {
            ingredients();
        }        

        /*Ingredients Function display*/
        void ingredients()
        {
            switch(getpizza)
            {
                case "Margherita0":
                    Debug.Print("m0");                    
                    break;

                case "Margherita1":
                    Debug.Print("m1");
                    break;

                case "Margherita2":
                    Debug.Print("m2");
                    break;

                case "Margherita3":
                    Debug.Print("m3");
                    break;

                default:
                    Debug.Print("default case");
                    break;
            }
        }

        /*Ethernet Network_Down Function*/
        void ethernetJ11D_NetworkDown(GTM.Module.NetworkModule sender,GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network is down!");
        }

        /*Ethernet Network_Up Function*/
        void ethernetJ11D_NetworkUp(GTM.Module.NetworkModule sender,GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network is up!");
            Debug.Print("My IP is: " + ethernetJ11D.NetworkSettings.IPAddress);
           
        }

        /*Ethernet Run Web_Server Function*/
        void RunWebServer()
        {
            /*Wait for the network...*/
            while (ethernetJ11D.IsNetworkUp == false)
            {
                Debug.Print("Waiting...");
                Thread.Sleep(1000);
            }
            /*Start the server*/           
            WebServer.StartLocalServer(ethernetJ11D.NetworkSettings.IPAddress, 80);
            //WebServer.DefaultEvent.WebEventReceived += DefaultEvent_WebEventReceived;

            /*inizio socket*/
            SocketClient.StartClient();           
            /*fine socket*/

            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        
        /*void DefaultEvent_WebEventReceived(string path, WebServer.HttpMethod method, Responder responder)
        {
            if (method == WebServer.HttpMethod.GET)
            {
                Debug.Print("Get Riuscita");

                string content = "<html><body><h1>Hello World!!</h1></body></html>";
                byte[] bytes = new System.Text.UTF8Encoding().GetBytes(content);
                responder.Respond(bytes, "text/html");                       
            }
            else
                Debug.Print("Get Non Riuscita");
        }*/

        /****************
         * CALLBACK 
         * *************/
        private void Button_PressEvent(object sender)
        {           
            initMenu(); 
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
}
