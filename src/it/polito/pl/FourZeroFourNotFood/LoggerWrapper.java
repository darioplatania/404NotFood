package it.polito.pl.FourZeroFourNotFood;

import java.io.BufferedWriter;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.io.Writer;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.logging.FileHandler;
import java.util.logging.Level;
import java.util.logging.Logger;
import java.util.logging.SimpleFormatter;


// SINGLETON: LoggerWrapper

public class LoggerWrapper {

	
	private static final String SEPARATOR	= "/";
	private static final String LOG_DIR		= "log"; //TODO: Change Next	
	private static Logger LOGGER;
	private static FileHandler fh;
	private static Writer writer = null;
	

	private static LoggerWrapper wrapper = null;
	
	private LoggerWrapper(){
		try {
			if(checkIfLogDirExists())
				initLogger();
		} catch (SecurityException | IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	public static LoggerWrapper getInstance(){
		if(wrapper==null){
			wrapper = new LoggerWrapper();
		}
		return wrapper;
	}
	
	private static String getCurrentTimestamp(){
		return new SimpleDateFormat("yyyy_MM_dd").format(new Date()).toString();
	}
	
	private static void initLogger() throws SecurityException, IOException{
		
		
		String target = LOG_DIR+SEPARATOR+getCurrentTimestamp()+".log";
		
		
		try {
			writer = new BufferedWriter(
				new OutputStreamWriter( new FileOutputStream(target), "utf-8")
				);

			LOGGER = Logger.getLogger(Main.APP_NAME);
			fh = new FileHandler(target,true); //TODO: append true does not work!!
			fh.setFormatter(new SimpleFormatter());
			LOGGER.addHandler(fh);
			
		
		}catch (IOException ex) {
			 ex.printStackTrace();
		} finally {
		   try{
			   writer.close();
		   } catch (Exception ex) {/*ignore*/}
		}
		
				
			
		
		
		

	}
	
	public void DEBUG_INFO(Level lvl, String msg){
		LOGGER.log(lvl, msg);
	}
	
	private boolean checkIfLogDirExists(){
		
		File temp = new File(LOG_DIR);
		if(!temp.exists())
			return temp.mkdir();
		return true;
			
	}
	
}
