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

	    TableColumn tc1 = new TableColumn(table, SWT.CENTER);
	    TableColumn tc2 = new TableColumn(table, SWT.CENTER);
	    TableColumn tc3 = new TableColumn(table, SWT.CENTER);
	    TableColumn tc4 = new TableColumn(table, SWT.CENTER);
	    
	    tc1.setText("Order Id");
	    tc2.setText("Menu");
	    tc3.setText("Price");
	    tc4.setText("Payment");
	    
	    int w = shell.getBounds().width;
	    int n = table.getColumnCount();
	    
	    tc1.setWidth((w/n)/2);
	    tc2.setWidth(2*(w/n));
	    tc3.setWidth((w/n)/2);
	    tc4.setWidth(w/n);
	    
	    table.setHeaderVisible(true);
	    table.setLinesVisible(true);

	    
	}
}
