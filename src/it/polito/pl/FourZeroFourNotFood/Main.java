package it.polito.pl.FourZeroFourNotFood;

import java.util.logging.Level;


public class Main {

	
	// SET APP NAME
	
	public static final String APP_NAME	= "404 Not Food";
	
	
	// SETUP LOGGER
	
	
	public static void main(String[] args){

		// Starting App...
		
		// Starting Logger Engine...
		LoggerWrapper logger = LoggerWrapper.getInstance();
		
		// System Starting...
		logger.DEBUG_INFO(Level.INFO,APP_NAME+" Starting...");
		
		// TODO: 1. Start Connection on IPv4 Socket
		logger.DEBUG_INFO(Level.INFO,"Starting connection...");
		
		
		// TODO: 2. Wait for oders
		// TODO: 2.1 Follow order workflow
		// TODO: 2.2 Close Order
		
		
		
		
	}

	
}
