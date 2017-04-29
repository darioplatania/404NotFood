package it.polito.pl.FourZeroFourNotFood;

import java.util.logging.Level;
import java.util.logging.Logger;

public class Main {

	
	// SET APP NAME
	private static final String APP_NAME = "404 Not Food";
	
	// SETUP LOGGER
	private static final Logger LOGGER = Logger.getLogger(Main.class.getName());
	
	
	public static void main(String[] args){
		
		
		// System Starting...
		DEBUG_INFO(Level.INFO,APP_NAME+" Starting...");
		
		// TODO: 1. Start Connection on IPv4 Socket
		
		
		
		// TODO: 2. Wait for oders
		// TODO: 2.1 Follow order workflow
		// TODO: 2.2 Close Order
		
		
		
		
	}
	
	private static void DEBUG_INFO(Level lvl, String msg){
		LOGGER.log(lvl, msg);
	}
	
}
