package it.polito.pl.FourZeroFourNotFood;


import org.eclipse.swt.SWT;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.Shell;
import org.eclipse.swt.widgets.MenuItem;
import org.eclipse.wb.swt.SWTResourceManager;
import org.eclipse.swt.custom.CLabel;
import org.eclipse.swt.widgets.Label;

public class MainWindow {

	protected Shell shell;
	
	
	protected MenuItem fileMenuHeader;

	

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
		shell.setBackground(SWTResourceManager.getColor(SWT.COLOR_DARK_RED));
		shell.setSize(800, 600);
		shell.setText("404NotFood");
		
		CLabel lblNewLabel = new CLabel(shell, SWT.NONE);
		lblNewLabel.setBackground(SWTResourceManager.getColor(SWT.COLOR_DARK_RED));
		lblNewLabel.setImage(SWTResourceManager.getImage(MainWindow.class, "/it/polito/pl/FourZeroFourNotFood/resources/logo.png"));
		lblNewLabel.setBounds(327, 216, 146, 151);
		lblNewLabel.setText("");
		
		Label lblnotfood = new Label(shell, SWT.NONE);
		lblnotfood.setFont(SWTResourceManager.getFont(".SF NS Text", 18, SWT.NORMAL));
		lblnotfood.setForeground(SWTResourceManager.getColor(SWT.COLOR_WHITE));
		lblnotfood.setAlignment(SWT.CENTER);
		lblnotfood.setBounds(325, 364, 136, 48);
		lblnotfood.setText("404NotFood");

		
		
	    
	}
}
