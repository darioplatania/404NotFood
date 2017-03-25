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
        private static DataGrid datagrid;     
                   

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
            var button = (Button)window.GetChildByName("button");
            /*press button event*/            
            button.PressEvent += Button_PressEvent;            
        }

        static void initMenu()
        {
            
            Debug.Print("Init Menu!");
            /*load menu*/
            menu = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.Menu));
            Glide.MainWindow = menu;
            
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
