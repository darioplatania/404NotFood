using Gadgeteer.Modules.GHIElectronics;
using System;
using System.Collections;
using System.Threading;
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

namespace fez_spider
{
    public partial class Program
    {
        
        private static GHI.Glide.Display.Window _mainwindow;             
        private static GHI.Glide.Display.Window _menu;
        private GHI.Glide.UI.Button _startbtn;
        private GHI.Glide.UI.Button _deleteBtn;
        private GHI.Glide.UI.Button _ingBtn;
        private GHI.Glide.UI.DataGrid _dataGrid;
        private GHI.Glide.UI.TextBlock _pCounter;
        private GHI.Glide.UI.TextBlock _qntCounter;
        private GHI.Glide.UI.TextBlock _errMsg;  
        private static int qnt; 
        private static int price;
        private static Font font = Resources.GetFont(Resources.FontResources.NinaB);       
        private static string getpizza;
        private static int getprice;
        private static int getqnt;
        private static int row = -1;       

        /*This method is run when the mainboard is powered up or reset*/
        void ProgramStarted()
        {
            /*Use Debug.Print to show messages in Visual Studio's "Output" window during debugging*/
            Debug.Print("Program Started");

            /*Ethernet Configuration
            ethernetJ11D.UseThisNetworkInterface();
            //ethernetJ11D.UseStaticIP("")
            ethernetJ11D.NetworkUp += ethernetJ11D_NetworkUp;
            ethernetJ11D.NetworkDown += ethernetJ11D_NetworkDown;                                     
            new Thread(RunWebServer).Start();*/          

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
                /*enable ingredients & delete button*/
                _ingBtn.Visible = true;
                _menu.Invalidate();                                

                GlideUtils.Debug.Print("GetRowData[" + args.RowIndex + "] = ", data);
                /*mem row index*/
                row = args.RowIndex;                
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

        /*Delete_btn TapEvent*/
        void deleteBtn_PressEvent(object sender)
        {            
            getqnt = 0;//set qnt to 0
            getprice = 0;//set getprice to selected row to 0 
            price = 0;//set total price to 0     
            qnt = 0;//set total qnt to 0
                  
            _pCounter.Text = price.ToString();
            _qntCounter.Text = qnt.ToString();
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

        void priceadd(int prezzo)
        {           
            price = price + prezzo;
            _pCounter.Text = price.ToString();
            _menu.Invalidate();            
            
        }

        void priceremove(int prezzo)
        {
            price = price - prezzo;
            _pCounter.Text = price.ToString();
            _menu.Invalidate();
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
        void ethernetJ11D_NetworkUp(GTM.Module.NetworkModule sender,
        GTM.Module.NetworkModule.NetworkState state)
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
            WebServer.DefaultEvent.WebEventReceived += DefaultEvent_WebEventReceived;                    

            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        
        void DefaultEvent_WebEventReceived(string path, WebServer.HttpMethod method, Responder responder)
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
        }

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
                _errMsg.Visible = false;

                /*calculate price function*/
                priceadd(getprice);
                getqnt = getqnt + 1; //quantità della pizza selezionata
                qnt = qnt + 1; //quantità totale
                _qntCounter.Text = qnt.ToString();
                _dataGrid.SetCellData(3, row, getqnt);               
                _menu.Invalidate();

                Debug.Print("Hai Aggiunto: " + getpizza + " Qnt: " + getqnt);
                Debug.Print("Prezzo Totale: " + price.ToString());
            }
            plus.TurnLedOff();

        }

        private void Minus_ButtonPressed(GTM.GHIElectronics.Button sender, GTM.GHIElectronics.Button.ButtonState state)
        {
            minus.TurnLedOn();
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
                priceremove(getprice);
                getqnt = getqnt - 1; //quantità della pizza selezionata
                qnt = qnt - 1; //quantità totale    
                _qntCounter.Text = qnt.ToString();
                _dataGrid.SetCellData(3, row, getqnt);               
                _menu.Invalidate();

                Debug.Print("Hai eliminato: " + getpizza + " Qnt: " + getqnt);
                Debug.Print("Prezzo Totale: " + price.ToString());
                Debug.Print("QNT dopo rimozione: " + getqnt);
            }
            minus.TurnLedOff();

        }

    }
}
