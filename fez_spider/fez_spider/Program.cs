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
        private GHI.Glide.UI.DataGrid _dataGrid;
        private GHI.Glide.UI.TextBlock _pCounter;
        private GHI.Glide.UI.TextBlock _qntCounter;
        private GHI.Glide.UI.TextBlock _errMsg;
        private GHI.Glide.UI.TextBox _textorder;
        private  int qnt; 
        private  int price;
        private  static Font font = Resources.GetFont(Resources.FontResources.NinaB);
        private  int getid;     
        private  string getpizza;
        private  int getprice;
        private  int getqnt;
        private  int row = -1;
        private  int count = 0;
        private  int exist = 0;
        private  int aux = 0;
        byte[] result = new byte[65536];
        ArrayList payment = new ArrayList();       
        


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

            /*Setup the button controls*/

            /*
            GHI.Glide.UI.Button fillBtn = (GHI.Glide.UI.Button)_menu.GetChildByName("fillBtn");
            fillBtn.TapEvent += new OnTap(fillBtn_TapEvent);
            */

            /*create button*/

            _ordBtn = (GHI.Glide.UI.Button)_menu.GetChildByName("ordBtn");
            _ordBtn.Enabled = false;
            _menu.Invalidate();
            _ordBtn.PressEvent += _ordBtn_PressEvent;

            _deleteBtn = (GHI.Glide.UI.Button)_menu.GetChildByName("deleteBtn");
            _deleteBtn.Enabled = false;
            _menu.Invalidate();
            _deleteBtn.PressEvent += deleteBtn_PressEvent;

            _ingBtn = (GHI.Glide.UI.Button)_menu.GetChildByName("ingBtn");
            _ingBtn.Visible = false;
            _menu.Invalidate();
            _ingBtn.PressEvent += ingBtn_PressEvent;
            
            //GHI.Glide.UI.Button continueBtn = (GHI.Glide.UI.Button)_menu.GetChildByName("continueBtn");
            
            /*Setup the dataGrid reference*/
            _dataGrid = (DataGrid)_menu.GetChildByName("dataGrid");
                        
            // Listen for tap cell events.
            _dataGrid.TapCellEvent += new OnTapCell(dataGrid_TapCellEvent);

            /*Create our four columns*/
            _dataGrid.AddColumn(new DataGridColumn("ID", 40));
            _dataGrid.AddColumn(new DataGridColumn("PIZZA", 125));
            _dataGrid.AddColumn(new DataGridColumn("EUR", 50));
            _dataGrid.AddColumn(new DataGridColumn("QNT", 50));
            
            /*Populate the data grid with random data*/
            Populate(true);

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
        void Populate(bool invalidate)
        {
            /*Add items with data*/
            for (int i = 0; i < 7; i++)
            {
                /*DataGridItems must contain an object array whose length matches the number of columns.*/
                _dataGrid.AddItem(new DataGridItem(new object[4] { i, "Margherita" + i, i,qnt }));
            }

            if (invalidate)
                _dataGrid.Invalidate();
        }

        /*DataGrid TapCellEvent*/
        void dataGrid_TapCellEvent(object sender, TapCellEventArgs args)
        {           
            // Get the data from the row we tapped.            
            object[] data = _dataGrid.GetRowData(args.RowIndex);                  
            if (data != null)
            {
                /*enable ingredients button*/
                _ingBtn.Visible = true;               
                _menu.Invalidate();                                

                GlideUtils.Debug.Print("GetRowData[" + args.RowIndex + "] = ", data);
                /*mem row index*/
                row = args.RowIndex;
                /*select id row*/
                getid = (int)_dataGrid.GetRowData(args.RowIndex).GetValue(0);
                /*select name row*/
                getpizza = (string)_dataGrid.GetRowData(args.RowIndex).GetValue(1);
                /*select price row*/
                getprice = (int)_dataGrid.GetRowData(args.RowIndex).GetValue(2);
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

        /*Fill_btn TapEvent*/
        void fillBtn_TapEvent(object sender)
        {
            Populate(true);
        }

        /*Pay_btn TapEvent*/
        void _ordBtn_PressEvent(object sender)
        {
            Debug.Print("HAI ORDINATO: ");
            foreach (Product i in payment)
            {
                Debug.Print("Pizza: " + i.nome + " Prezzo: " + i.prezzo + " Qnt: " + i.quantita);
            }

            /*load menu*/
            _ordina = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.Ordina));
            Glide.MainWindow = _ordina;
            _payBtn = (GHI.Glide.UI.Button)_ordina.GetChildByName("payBtn");
            //_textorder = (GHI.Glide.UI.TextBox)_ordina.GetChildByName("textorder");
            //string prova = "aaa";
            //_textorder.Text = prova.ToString();   
            /*
             <TextBox Name="textorder" X="20" Y="10" Width="250" Height="150" Alpha="255" TextAlign="Left" Font="4" FontColor="000000"/>
	         <Button Name="paytBtn" X="110" Y="202" Width="100" Height="32" Alpha="255" Text="Paga" Font="4" FontColor="000000" DisabledFontColor="808080" TintColor="000000" TintAmount="0"/>
             */
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

           /* String url = @"http://192.168.100.1:8080/food/webapi/food";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            Stream stream = res.GetResponseStream();
            StreamReader sr = new StreamReader(stream);

            string json = sr.ReadToEnd();
            string target = "\\";
            Debug.Print(target);
            //for (int i = 0; i < json.Length; i++) {

                //if (json[i] == '"')
               // {
                //    target+= "\"";
              //  }
             //   else target += json[i];
            //}
            ArrayList al = Json.NETMF.JsonSerializer.DeserializeString(json) as ArrayList;

            Debug.Print(json);
             
                        
            //Debug.Print(sr.ReadToEnd());
            //string json = Json.NETMF.JsonSerializer.SerializeObject(sr.ReadToEnd());

            //dynamic json = new Json.NETMF.JsonSerializer().Deserialize(sr.ReadToEnd());

            //var serializer = new Json.NETMF.JsonSerializer();
            //dynamic jsonObject = serializer.Deserialize(sr.ReadToEnd());

            //string prova = ((Menu)json).menu[2].name;         
            //Debug.Print("prova stampa: " + prova);        


            //Debug.Print(json);

            //Menu menu = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Menu>(sr.ReadToEnd());
            */
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
