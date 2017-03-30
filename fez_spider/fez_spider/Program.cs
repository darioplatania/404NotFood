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
        
        private static GHI.Glide.Display.Window window;             
        private static GHI.Glide.Display.Window menu;        
        private static DataGrid dataGrid;        
        private static int qnt; /*use for setting quantity*/
        private static int price;
        private static Font font = Resources.GetFont(Resources.FontResources.NinaB);       
        private static string getpizza;
        private static int getprice;
        private static int getqnt;
        private static int row = -1;        
        //static DisplayTE35 display = new DisplayTE35(14, 13, 12);
        //private static Bitmap display = new Bitmap(SystemMetrics.ScreenWidth, SystemMetrics.ScreenHeight);  


        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");

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
            window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.Window));

            GlideTouch.Initialize();
            Glide.MainWindow = window;

            /*create button to start*/
            GHI.Glide.UI.Button button = (GHI.Glide.UI.Button)window.GetChildByName("button");            
            /*press button event*/            
            button.PressEvent += Button_PressEvent;            
        }

        void initMenu()
        {
            
            Debug.Print("Init Menu!");

            /*load menu*/
            menu = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.Menu));
            Glide.MainWindow = menu;

            // Setup the dataGrid reference.            
            dataGrid = (DataGrid)menu.GetChildByName("dataGrid");
                        
            // Listen for tap cell events.
            dataGrid.TapCellEvent += new OnTapCell(dataGrid_TapCellEvent);

            // Create our four columns.
            dataGrid.AddColumn(new DataGridColumn("ID", 40));
            dataGrid.AddColumn(new DataGridColumn("PIZZA", 125));
            dataGrid.AddColumn(new DataGridColumn("EUR", 50));
            dataGrid.AddColumn(new DataGridColumn("QNT", 50));
            
            // Populate the data grid with random data.
            Populate(true);

            // Add the data grid to the window before rendering it.
            menu.AddChild(dataGrid);            
            dataGrid.Render();           

            // Setup the button controls.
            GHI.Glide.UI.Button fillBtn = (GHI.Glide.UI.Button)menu.GetChildByName("fillBtn");
            fillBtn.TapEvent += new OnTap(fillBtn_TapEvent);

            GHI.Glide.UI.Button deleteBtn = (GHI.Glide.UI.Button)menu.GetChildByName("deleteBtn");
            deleteBtn.TapEvent += new OnTap(deleteBtn_TapEvent);

            GHI.Glide.UI.Button continueBtn = (GHI.Glide.UI.Button)menu.GetChildByName("continueBtn");

            GHI.Glide.UI.Button ingBtn = (GHI.Glide.UI.Button)menu.GetChildByName("ingBtn");
            ingBtn.TapEvent += new OnTap(ingBtn_TapEvent);

            GT.Timer timer = new GT.Timer(200); // Create a timer
            timer.Tick += Timer_Tick; // Run the method timer_tick when the timer ticks
            timer.Start(); // Start the timer

        }

        private void Timer_Tick(GT.Timer timer)
        {
            Joystick.Position pos = joystick.GetPosition();
            if (pos.X > 0.50)            
                Joystick_Up();            
            else if(pos.X < -0.50)
                Joystick_Down();
        }

        void Populate(bool invalidate)
        {            
            // Add items with random data
            for (int i = 0; i < 7; i++)
            {
                // DataGridItems must contain an object array whose length matches the number of columns.
                dataGrid.AddItem(new DataGridItem(new object[4] { i, "Margherita" + i, i,qnt }));
            }

            if (invalidate)
                dataGrid.Invalidate();
        }

        void dataGrid_TapCellEvent(object sender, TapCellEventArgs args)
        {
            // Get the data from the row we tapped.            
            object[] data = dataGrid.GetRowData(args.RowIndex);                  
            if (data != null)
            {
                GlideUtils.Debug.Print("GetRowData[" + args.RowIndex + "] = ", data);
                /*mem row index*/
                row = args.RowIndex;                
                /*select name row*/
                getpizza = (string)dataGrid.GetRowData(args.RowIndex).GetValue(1);
                /*select price row*/
                getprice = (int)dataGrid.GetRowData(args.RowIndex).GetValue(2);
                /*select qnt row*/
                getqnt = (int)dataGrid.GetRowData(args.RowIndex).GetValue(3);
                Debug.Print("QNT tapcell: " + getqnt);
            }

        }

        void Joystick_Up()
        {
            dataGrid.ScrollUp(1);
            dataGrid.GetRowData(row + 1);
            dataGrid.Invalidate();
            
        }

        void Joystick_Down()
        {
            dataGrid.ScrollDown(1);
            dataGrid.GetRowData(row - 1);
            dataGrid.Invalidate();
            
        }

        void fillBtn_TapEvent(object sender)
        {
            Populate(true);
        }

        void deleteBtn_TapEvent(object sender)
        {
            getqnt = 0;
            getprice = 0;
            /*vedere se questo for va bene o c'è un altro modo??*/
            for (int i = 0; i < 7; i++)
            {
                dataGrid.SetCellData(3, i, getqnt);
                dataGrid.Invalidate();
            }
            Debug.Print("Annullato tutto! Qnt: " + getqnt + " Prezzo: " + getprice);
        }
        
        void ingBtn_TapEvent(object sender)
        {
            ingredients();
        }

        void priceadd(int prezzo)
        {           
            price = price + prezzo;            
        }

        void priceremove(int prezzo)
        {
            price = price - prezzo;
        }

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
            }
            else
            {                
                /*calculate price function*/
                priceadd(getprice);
                getqnt = getqnt + 1;
                dataGrid.SetCellData(3, row, getqnt);
                dataGrid.Invalidate();

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
            }
            else
            {                
                /*calculate price function*/
                priceremove(getprice);
                getqnt = getqnt - 1;
                dataGrid.SetCellData(3, row, getqnt);
                dataGrid.Invalidate();
                Debug.Print("Hai eliminato: " + getpizza + " Qnt: " + getqnt);
                Debug.Print("Prezzo Totale: " + price.ToString());
                Debug.Print("QNT dopo rimozione: " + getqnt);
            }
            minus.TurnLedOff();

        }

    }
}
