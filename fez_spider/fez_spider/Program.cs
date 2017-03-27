﻿using Gadgeteer.Modules.GHIElectronics;
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
        private static int row;
        private static int column;
        //static DisplayTE35 display = new DisplayTE35(14, 13, 12);
        //private static Bitmap display = new Bitmap(SystemMetrics.ScreenWidth, SystemMetrics.ScreenHeight);  


        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");

            /*welcome into display*/
            first_step();
            /*button plus(input 9)*/
            plus.ButtonPressed += Plus_ButtonPressed;
            /*button minus(input 5)*/
            minus.ButtonPressed += Minus_ButtonPressed;
        }      

        /****************
         * FUNCTION 
         * *************/
        static void first_step()
        {
            window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.Window));

            GlideTouch.Initialize();
            Glide.MainWindow = window;

            /*create button to start*/
            GHI.Glide.UI.Button button = (GHI.Glide.UI.Button)window.GetChildByName("button");            
            /*press button event*/            
            button.PressEvent += Button_PressEvent;            
        }

        static void initMenu()
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
            GHI.Glide.UI.Button scrollUpBtn = (GHI.Glide.UI.Button)menu.GetChildByName("scrollUpBtn");
            scrollUpBtn.TapEvent += new OnTap(scrollUpBtn_TapEvent);

            GHI.Glide.UI.Button scrollDownBtn = (GHI.Glide.UI.Button)menu.GetChildByName("scrollDownBtn");
            scrollDownBtn.TapEvent += new OnTap(scrollDownBtn_TapEvent);

            /*Button selectUpBtn = (Button)menu.GetChildByName("selectUpBtn");
            selectUpBtn.TapEvent += new OnTap(selectUpBtn_TapEvent);

            Button selectDownBtn = (Button)menu.GetChildByName("selectDownBtn");
            selectDownBtn.TapEvent += new OnTap(selectDownBtn_TapEvent);

            Button selectClearBtn = (Button)menu.GetChildByName("selectClearBtn");
            selectClearBtn.TapEvent += new OnTap(selectClearBtn_TapEvent);

            Button selectDeleteBtn = (Button)menu.GetChildByName("selectDeleteBtn");
            selectDeleteBtn.TapEvent += new OnTap(selectDeleteBtn_TapEvent);*/

            /*Button clearBtn = (Button)menu.GetChildByName("clearBtn");
            clearBtn.TapEvent += new OnTap(clearBtn_TapEvent);*/

            GHI.Glide.UI.Button fillBtn = (GHI.Glide.UI.Button)menu.GetChildByName("fillBtn");
            fillBtn.TapEvent += new OnTap(fillBtn_TapEvent);

            GHI.Glide.UI.Button deleteBtn = (GHI.Glide.UI.Button)menu.GetChildByName("deleteBtn");
            GHI.Glide.UI.Button continueBtn = (GHI.Glide.UI.Button)menu.GetChildByName("continueBtn");
        }          

        static void Populate(bool invalidate)
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

        static void dataGrid_TapCellEvent(object sender, TapCellEventArgs args)
        {
            // Get the data from the row we tapped.            
            object[] data = dataGrid.GetRowData(args.RowIndex);                  
            if (data != null)
            {
                GlideUtils.Debug.Print("GetRowData[" + args.RowIndex + "] = ", data);
                /*mem column and row index*/
                row = args.RowIndex;
                //column = args.ColumnIndex;
                /*select name row*/
                getpizza = (string)dataGrid.GetRowData(args.RowIndex).GetValue(1);
                /*select price row*/
                getprice = (int)dataGrid.GetRowData(args.RowIndex).GetValue(2);
                /*select qnt row*/
                getqnt = (int)dataGrid.GetRowData(args.RowIndex).GetValue(3);
                Debug.Print("QNT tapcell: " + getqnt);
            }

        }     

        static void scrollUpBtn_TapEvent(object sender)
        {
            dataGrid.ScrollUp(1);
            dataGrid.Invalidate();
        }

        static void scrollDownBtn_TapEvent(object sender)
        {
            dataGrid.ScrollDown(1);
            dataGrid.Invalidate();
        }

        /*static void selectUpBtn_TapEvent(object sender)
        {
            if (dataGrid.SelectedIndex > 0)
                dataGrid.SelectedIndex--;
        }

        static void selectDownBtn_TapEvent(object sender)
        {
            if (dataGrid.SelectedIndex < dataGrid.NumItems - 1)
                dataGrid.SelectedIndex++;
        }

        static void selectClearBtn_TapEvent(object sender)
        {
            dataGrid.SelectedIndex = -1;
        }

        static void selectDeleteBtn_TapEvent(object sender)
        {
            dataGrid.RemoveItemAt(dataGrid.SelectedIndex);
            dataGrid.Invalidate();
        }*/

        /*static void clearBtn_TapEvent(object sender)
        {
            dataGrid.Clear();
            dataGrid.Invalidate();
        }*/

        static void fillBtn_TapEvent(object sender)
        {
            Populate(true);
        }

        static void priceadd(int prezzo)
        {           
            price = price + prezzo;            
        }

        static void priceremove(int prezzo)
        {
            price = price - prezzo;
        }

        /****************
         * CALLBACK 
         * *************/
        private static void Button_PressEvent(object sender)
        {           
            initMenu(); 
        }


        private void Plus_ButtonPressed(GTM.GHIElectronics.Button sender, GTM.GHIElectronics.Button.ButtonState state)
        {
            /*calculate price function*/
            priceadd(getprice);
            getqnt = getqnt + 1;
            dataGrid.SetCellData(3, row, getqnt);

            Debug.Print("Hai Aggiunto: "+ getpizza + " Qnt: " + getqnt);
            Debug.Print("Prezzo Totale: " + price.ToString());
            
        }

        private void Minus_ButtonPressed(GTM.GHIElectronics.Button sender, GTM.GHIElectronics.Button.ButtonState state)
        {
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
                Debug.Print("Hai eliminato: " + getpizza + " Qnt: " + getqnt);
                Debug.Print("Prezzo Totale: " + price.ToString());
                Debug.Print("QNT dopo rimozione: " + getqnt);

            }
            
        }

    }
}
