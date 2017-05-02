package it.polito.pl.FourZeroFourNotFood;


import java.io.InputStream;
import java.util.ArrayList;
import java.util.concurrent.ConcurrentHashMap;

import org.eclipse.swt.SWT;
import org.eclipse.swt.widgets.Button;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.Event;
import org.eclipse.swt.widgets.Shell;
import org.eclipse.swt.widgets.MenuItem;
import org.eclipse.wb.swt.SWTResourceManager;
import org.eclipse.swt.custom.CLabel;
import org.eclipse.swt.graphics.Image;
import org.eclipse.swt.layout.FillLayout;
import org.eclipse.swt.layout.GridData;
import org.eclipse.swt.layout.GridLayout;
import org.eclipse.swt.layout.RowData;
import org.eclipse.swt.layout.RowLayout;
import org.eclipse.swt.widgets.Label;
import org.eclipse.swt.widgets.Listener;
import org.eclipse.swt.widgets.Table;
import org.eclipse.swt.widgets.TableColumn;
import org.eclipse.swt.widgets.TableItem;
import org.eclipse.swt.widgets.Text;

public class MainWindow {

	protected Shell shell;
	protected static Table table;
	
	protected MenuItem fileMenuHeader;

	private static ConcurrentHashMap<String,TableItem> orderItems = new ConcurrentHashMap<String,TableItem>();
	

	public static ConcurrentHashMap<String,TableItem> getOrderItems() {
		return orderItems;
	}


	public static Table getTable() {
		return table;
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
		
		shell = new Shell();
		shell.setImage(SWTResourceManager.getImage(MainWindow.class, "/it/polito/pl/FourZeroFourNotFood/resources/logo.png"));
		
		
		
		shell.setLayout(new FillLayout());
		shell.setText("404NotFood");
		
		table = new Table(shell, SWT.BORDER | SWT.FULL_SELECTION);
	    table.setHeaderVisible(true);
	    table.setLinesVisible(true);

		
	    TableColumn tc1 = new TableColumn(table, SWT.CENTER);
	    TableColumn tc2 = new TableColumn(table, SWT.CENTER);
	    TableColumn tc3 = new TableColumn(table, SWT.CENTER);
	    TableColumn tc4 = new TableColumn(table, SWT.CENTER);
	    TableColumn tc5 = new TableColumn(table, SWT.CENTER);
	    
	    tc1.setText("Id");
	    tc2.setText("Menu");
	    tc3.setText("Price");
	    tc4.setText("Payment");
	    tc5.setText("Status");
	    
	    


	    table.addListener(SWT.Resize, new Listener(){

	    	// Adjust table columns width on resize
			@Override
			public void handleEvent(Event e) {

				int w = shell.getBounds().width;
				int n = table.getColumnCount();
				
				
				table.getColumn(0).setWidth((w/n)/2);
				table.getColumn(1).setWidth(2*(w/n));
				table.getColumn(2).setWidth((w/n)/2);
				table.getColumn(3).setWidth((w/n));
				table.getColumn(4).setWidth((w/n));
				
					
			}
	    	
	    });
	    
	}
}
