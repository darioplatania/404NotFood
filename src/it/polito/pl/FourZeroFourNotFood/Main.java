package it.polito.pl.FourZeroFourNotFood;

import java.io.IOException;
import java.util.logging.Level;


public class Main {

	
	// SET APP NAME
	
	public static final String 	APP_NAME	= "404 Not Food";
	public static final int		PORT_NUMBER = 4096;
	
	// SETUP LOGGER
	
	
	public static void main(String[] args){

		// Variables
		
		Server server;
		
		
		// Starting App...
		
		// Starting Logger Engine...
		LoggerWrapper logger = LoggerWrapper.getInstance();

		
		
		// System Starting...
		logger.DEBUG_INFO(Level.INFO,APP_NAME+" Starting...");
		
		// TODO: 1. Start Connection on IPv4 Socket
		logger.DEBUG_INFO(Level.INFO,"Starting connection...");
		try{
			
			
			server = new Server(PORT_NUMBER);
			
			// Waiting for new Connections...
			logger.DEBUG_INFO(Level.INFO,"Server Started at "+server.getSocket().getLocalSocketAddress()); 
			logger.DEBUG_INFO(Level.INFO,"Waiting for new connections...");
			
			// Server Starting
			server.start();
			
			// Server Quitting
			server.stop();
			
			
			
			
		}catch(IOException ex){
			logger.DEBUG_INFO(Level.SEVERE, "Failed starting server: "+ex.getMessage());
			return;
		}
		
		
		
		
		
	}

	
}
