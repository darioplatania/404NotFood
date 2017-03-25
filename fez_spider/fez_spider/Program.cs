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
        static DataGrid dataGrid;     
                   

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");

            /*welcome into display*/
            first_step();        

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
            Button button = (Button)window.GetChildByName("button");            
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

            // Possible configurations...
            //dataGrid.ShowHeaders = false;
            //dataGrid.SortableHeaders = false;
            //dataGrid.TappableCells = false;
            //dataGrid.Draggable = false;
            //dataGrid.ShowScrollbar = false;
            //dataGrid.ScrollbarWidth = 4;

            // Listen for tap cell events.
            dataGrid.TapCellEvent += new OnTapCell(dataGrid_TapCellEvent);

            // Create our three columns.
            dataGrid.AddColumn(new DataGridColumn("ID", 50));
            dataGrid.AddColumn(new DataGridColumn("First Name", 125));
            dataGrid.AddColumn(new DataGridColumn("Last Name", 125));

            // Populate the data grid with random data.
            Populate(true);

            // Add the data grid to the window before rendering it.
            window.AddChild(dataGrid);
            dataGrid.Render();

            // Setup the button controls.

            Button scrollUpBtn = (Button)menu.GetChildByName("scrollUpBtn");
            scrollUpBtn.TapEvent += new OnTap(scrollUpBtn_TapEvent);

            Button scrollDownBtn = (Button)menu.GetChildByName("scrollDownBtn");
            scrollDownBtn.TapEvent += new OnTap(scrollDownBtn_TapEvent);

            Button selectUpBtn = (Button)menu.GetChildByName("selectUpBtn");
            selectUpBtn.TapEvent += new OnTap(selectUpBtn_TapEvent);

            Button selectDownBtn = (Button)menu.GetChildByName("selectDownBtn");
            selectDownBtn.TapEvent += new OnTap(selectDownBtn_TapEvent);

            Button selectClearBtn = (Button)menu.GetChildByName("selectClearBtn");
            selectClearBtn.TapEvent += new OnTap(selectClearBtn_TapEvent);

            Button selectDeleteBtn = (Button)menu.GetChildByName("selectDeleteBtn");
            selectDeleteBtn.TapEvent += new OnTap(selectDeleteBtn_TapEvent);

            Button clearBtn = (Button)menu.GetChildByName("clearBtn");
            clearBtn.TapEvent += new OnTap(clearBtn_TapEvent);

            Button fillBtn = (Button)menu.GetChildByName("fillBtn");
            fillBtn.TapEvent += new OnTap(fillBtn_TapEvent);
        }

        static void Populate(bool invalidate)
        {
            // Add items with random data
            for (int i = 0; i < 20; i++)
            {
                // DataGridItems must contain an object array whose length matches the number of columns.
                dataGrid.AddItem(new DataGridItem(new object[3] { i, RandomString(10), RandomString(10) }));
            }

            if (invalidate)
                dataGrid.Invalidate();
        }

        static void dataGrid_TapCellEvent(object sender, TapCellEventArgs args)
        {
            // Get the data from the row we tapped.
            object[] data = dataGrid.GetRowData(args.RowIndex);
            if (data != null)
                GlideUtils.Debug.Print("GetRowData[" + args.RowIndex + "] = ", data);
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

        static void selectUpBtn_TapEvent(object sender)
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
        }

        static void clearBtn_TapEvent(object sender)
        {
            dataGrid.Clear();
            dataGrid.Invalidate();
        }

        static void fillBtn_TapEvent(object sender)
        {
            Populate(true);
        }

        // -------------------------------------------------------------------
        // This is used to generate random strings.
        private static Random random = new Random((int)DateTime.Now.Ticks);
        private static string RandomString(int size)
        {
            string str = String.Empty;
            double d;
            ushort u;
            char ch;
            for (int i = 0; i < size; i++)
            {
                d = System.Math.Floor(26 * random.NextDouble() + 65);
                u = Convert.ToUInt16(d.ToString());
                ch = Convert.ToChar(u);
                str += ch;
            }

            return str;
        }

    

        /****************
         * CALLBACK 
         * *************/
        private static void Button_PressEvent(object sender)
        {           
            initMenu(); 
        }
                
    }
}
