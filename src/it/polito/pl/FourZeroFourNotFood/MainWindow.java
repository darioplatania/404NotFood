package it.polito.pl.FourZeroFourNotFood;


import java.io.IOException;
import java.io.InputStream;
import java.net.Socket;
import java.util.ArrayList;
import java.util.concurrent.ConcurrentHashMap;

import org.eclipse.swt.SWT;
import org.eclipse.swt.widgets.Button;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.Event;
import org.eclipse.swt.widgets.Group;
import org.eclipse.swt.widgets.Label;
import org.eclipse.swt.widgets.Shell;
import org.eclipse.swt.widgets.TabFolder;
import org.eclipse.swt.widgets.TabItem;
import org.eclipse.swt.widgets.MenuItem;
import org.eclipse.swt.widgets.MessageBox;
import org.eclipse.wb.swt.SWTResourceManager;
import org.eclipse.swt.custom.CLabel;
import org.eclipse.swt.custom.ScrolledComposite;
import org.eclipse.swt.graphics.Color;
import org.eclipse.swt.graphics.GC;
import org.eclipse.swt.graphics.Image;
import org.eclipse.swt.graphics.Point;
import org.eclipse.swt.graphics.Rectangle;
import org.eclipse.swt.layout.GridData;
import org.eclipse.swt.layout.GridLayout;
import org.eclipse.swt.widgets.Listener;
import org.eclipse.swt.widgets.Table;
import org.eclipse.swt.widgets.TableColumn;
import org.eclipse.swt.widgets.TableItem;
import org.eclipse.swt.widgets.Text;


public class MainWindow {

	protected Shell shell;
	protected static Table table;
	protected static Table menu_table;
	
	private final String title = "404NotFood";
	
	protected MenuItem fileMenuHeader;

	private static ConcurrentHashMap<String,TableItem> orderItems = new ConcurrentHashMap<String,TableItem>();
	
	public static String selected_id = "";

	public static ConcurrentHashMap<String,TableItem> getOrderItems() {
		return orderItems;
	}


	public static Table getTable() {
		return table;
	}

	public static Table getMenuTable() {
		return menu_table;
	}







	/**
	 * Open the window.
	 */
	public void open() {
		
		Display display = Display.getDefault();
		createContents();
		
		shell.open();
		shell.layout();
		while (!shell.isDisposed()) {
			if (!display.readAndDispatch()) {
				display.sleep();
			}
		}
	}

	/**
	 * Create contents of the window.
	 * @wbp.parser.entryPoint
	 */
	protected void createContents() {
		
		//Display display = new Display();
		shell = new Shell();
		/*
		Image bg_Image = new Image(display, "background.png"); 
	    shell.setBackgroundImage(bg_Image);
	    shell.setBackgroundMode(SWT.INHERIT_FORCE);  
		*/
		shell.setBackground(SWTResourceManager.getColor(SWT.COLOR_DARK_RED));
		shell.setBackgroundMode(SWT.INHERIT_FORCE);
		
		Image icon = SWTResourceManager.getImage(MainWindow.class, "/it/polito/pl/FourZeroFourNotFood/resources/logo.png");
		shell.setImage(icon);
		
		shell.setLayout(new GridLayout());
		shell.setText(title);
		
		CLabel icon_lbl = new CLabel(shell,SWT.NONE);
		icon_lbl.setBackground(SWTResourceManager.getColor(SWT.COLOR_DARK_RED));
		icon_lbl.setLayoutData(new GridData(SWT.CENTER,SWT.TOP,false,false));
		icon_lbl.setImage(icon);
		icon_lbl.setText(title);
		icon_lbl.setForeground(SWTResourceManager.getColor(SWT.COLOR_WHITE));
	    icon_lbl.setFont(SWTResourceManager.getFont(".SF NS Text", 23, SWT.NORMAL));
		
		Label orders_lbl = new Label(shell,SWT.NONE); 
		orders_lbl.setForeground(SWTResourceManager.getColor(SWT.COLOR_WHITE));
	    orders_lbl.setFont(SWTResourceManager.getFont(".SF NS Text", 23, SWT.NORMAL));
	    orders_lbl.setText("Orders");
	    new Label(shell, SWT.HORIZONTAL | SWT.SEPARATOR).setLayoutData(new GridData(GridData.FILL_HORIZONTAL));

	    final ScrolledComposite orders_composite = new ScrolledComposite(shell, SWT.NONE);
	    orders_composite.setLayout(new GridLayout());
	    orders_composite.setLayoutData(new GridData(SWT.FILL, SWT.CENTER, true, true));

		table = new Table(orders_composite, SWT.NO_SCROLL | SWT.FULL_SELECTION);
		table.setForeground(SWTResourceManager.getColor(SWT.COLOR_WHITE));
		table.setBackground(SWTResourceManager.getColor(SWT.COLOR_DARK_RED));
	    table.setLayoutData(new GridData(SWT.FILL,SWT.TOP,true,false));
		table.setHeaderVisible(false);
		table.setLinesVisible(false);

		
	    orders_composite.setContent(table);
	    orders_composite.setExpandHorizontal(true);
	    orders_composite.setExpandVertical(true);
	    orders_composite.setAlwaysShowScrollBars(false);
	    orders_composite.setMinHeight(shell.getBounds().height/2);
	    
	    orders_composite.addListener(SWT.Resize, new Listener(){

			@Override
			public void handleEvent(Event arg0) {

				orders_composite.setMinHeight(shell.getBounds().height/2);
			}
	    	
	    });
		
		

		
	    TableColumn tc1 = new TableColumn(table, SWT.CENTER);
	    TableColumn tc2 = new TableColumn(table, SWT.CENTER);
	    TableColumn tc3 = new TableColumn(table, SWT.CENTER);
	    TableColumn tc4 = new TableColumn(table, SWT.CENTER);
	    //TableColumn tc5 = new TableColumn(table, SWT.CENTER);
	    
	    tc1.setText("Id");
	    tc2.setText("Menu");
	    tc3.setText("Price");
	    tc4.setText("Payment");
	    //tc5.setText("Status");

	    table.addListener(SWT.Resize, new Listener(){

	    	// Adjust table columns width on resize
			@Override
			public void handleEvent(Event e) {

				int w = shell.getBounds().width;
				int n = table.getColumnCount();
				n--;
				
				table.getColumn(0).setWidth(0);
				table.getColumn(1).setWidth((w/n));
				table.getColumn(2).setWidth((w/n));
				table.getColumn(3).setWidth((w/n));
				//table.getColumn(4).setWidth((w/n));
				
					
			}
	    	
	    });
	    
		shell.addListener(SWT.Close, new Listener() {
		      public void handleEvent(Event event) {
		    	  
		    	// create a dialog with ok and cancel buttons and a question icon
		    	  MessageBox dialog =
		    	      new MessageBox(shell, SWT.ICON | SWT.OK| SWT.CANCEL);
		    	  dialog.setText("Warning");
		    	  dialog.setMessage("All the orders will be lost. Are you sure to close it?");
		    	  

		    	  // open dialog and await user selection
		    	  int returnCode = dialog.open();
		    	  
		    	  if(returnCode == SWT.OK){
		    			for(Socket s:Server.sockets){
				    		try {
								s.close();
							} catch (IOException e) {
								// TODO Auto-generated catch block
								e.printStackTrace();
							}
				    	}
				        System.exit(0);
		    	  }else if(returnCode == SWT.CANCEL){
		    		  event.doit = false;
		    	  }
		    		  
		    
		    	  
		      }
		    });
	    
	    
	    Label items_lbl = new Label(shell,SWT.NONE); 
	    items_lbl.setForeground(SWTResourceManager.getColor(SWT.COLOR_WHITE));
	    items_lbl.setFont(SWTResourceManager.getFont(".SF NS Text", 23, SWT.NORMAL));
	    items_lbl.setText("Items");
	    new Label(shell, SWT.HORIZONTAL | SWT.SEPARATOR).setLayoutData(new GridData(GridData.FILL_HORIZONTAL));
	    
	    
	    final ScrolledComposite menu_composite = new ScrolledComposite(shell, SWT.NONE);
	    menu_composite.setForeground(SWTResourceManager.getColor(SWT.COLOR_WHITE));
	    menu_composite.setBackground(SWTResourceManager.getColor(SWT.COLOR_DARK_RED));
	    menu_composite.setLayout(new GridLayout());
	    menu_composite.setLayoutData(new GridData(SWT.FILL, SWT.CENTER, true, true));
	    
	    menu_table = new Table(menu_composite, SWT.HIDE_SELECTION);
	    menu_table.setBackground(SWTResourceManager.getColor(SWT.COLOR_DARK_RED));
	    menu_table.setFont(SWTResourceManager.getFont(".SF NS Text", 13, SWT.BOLD));
	    menu_table.setForeground(SWTResourceManager.getColor(SWT.COLOR_WHITE));
	    menu_table.setLayoutData(new GridData(SWT.FILL,SWT.TOP,true,true));
	    menu_table.setHeaderVisible(false);
	    menu_table.setLinesVisible(false);
	    

	    
	    TableColumn tableColumn = new TableColumn(menu_table, SWT.LEFT);
	    tableColumn.setImage(null);
	    tableColumn.setText("Item");
	    new TableColumn(menu_table, SWT.CENTER).setText("Qty");
	    new TableColumn(menu_table, SWT.CENTER).setText("Price");

	    
	    menu_table.addListener(SWT.Resize, new Listener(){

	    	// Adjust table columns width on resize
			@Override
			public void handleEvent(Event e) {

				int w = shell.getBounds().width;
				int n = menu_table.getColumnCount();
				
				
				menu_table.getColumn(0).setWidth(2*w/n);
				menu_table.getColumn(1).setWidth((w/n)/2);
				menu_table.getColumn(2).setWidth((w/n)/2);
				
				
					
			}
	    	
	    });
	    
	    menu_table.addListener(SWT.EraseItem, new Listener()
	    {
	        @Override
	        public void handleEvent(Event event)
	        {
	            event.detail &= ~SWT.HOT;
	            if ((event.detail & SWT.SELECTED) == 0) return; /// item not selected

	            GC gc = event.gc;
	            gc.setBackground(SWTResourceManager.getColor(SWT.COLOR_DARK_RED));

	            event.detail &= ~SWT.SELECTED;
	            menu_table.deselectAll();

	        }
	    });
	    
	    menu_composite.setContent(menu_table);
	    menu_composite.setExpandHorizontal(true);
	    menu_composite.setExpandVertical(true);
	    menu_composite.setAlwaysShowScrollBars(false);
	    menu_composite.setMinHeight(shell.getBounds().height/2);
	    
	    menu_composite.addListener(SWT.Resize, new Listener(){

			@Override
			public void handleEvent(Event arg0) {

				menu_composite.setMinHeight(shell.getBounds().height/2);
			}
	    	
	    });
		



	    table.addListener(SWT.Selection, new Listener() {
	        public void handleEvent(Event event) {
	          
	        	
	            TableItem[] selection = table.getSelection();
	            String id = selection[0].getText(0);
	            
	            if(!id.equals(selected_id)){
	            	
	            	selected_id = id;
	            	
	            	menu_table.removeAll();
	            	
	            	Order order = OrderDB.getInstance().get(id);
		            
	            	
	            	
		            for(OrderedFood of:order.getFoods()){
		            
		            	
		            	new TableItem(menu_table,SWT.NONE).setText(new String[]{of.getFood().getName(),of.getQuantity()+"",of.getFood().getPrice()*of.getQuantity()+" €"});
		            
		            }	
		            new TableItem(menu_table,SWT.NONE).setText(new String[]{"Total","",order.getPrice()+" €"});
		        		
	            }
	            
	        	
	        }
	      });

	    
	}
}
