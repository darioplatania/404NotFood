package it.polito.pl.FourZeroFourNotFood;

import java.io.IOException;
import java.util.ArrayList;
import java.util.logging.Level;

import com.google.gson.Gson;


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
		
		// Start Connection on IPv4 Socket
		logger.DEBUG_INFO(Level.INFO,"Starting connection...");
		try{
			
		
			
			
			server = new Server(PORT_NUMBER);
			
			// Waiting for new Connections...
			logger.DEBUG_INFO(Level.INFO,"Server Started at "+server.getSocket().getLocalSocketAddress()); 
			logger.DEBUG_INFO(Level.INFO,"Waiting for new connections...");
			
			// Server Starting
			server.start();
			
			// Server Quitting -> NEVER EXECUTED AT THE MOMENT
			server.stop();
			
			
			
			
		}catch(IOException ex){
			logger.DEBUG_INFO(Level.SEVERE, "Failed starting server: "+ex.getMessage());
			return;
		}
		
		
		
		
		
	}

	@SuppressWarnings("unused")
	//TODO METODO PROVA -> ELIMINARE ALLA FINE
	private String generateRandomFoodJSON(){
		// Trying GSON
		Gson gson = new Gson();
		
		
		
		ArrayList<String> ingredients = new ArrayList<String>();
		ingredients.add("pomodoro");
		ingredients.add("mozzarella");
	
		Food margherita = new Food("Pizza Margherita",5,ingredients);
		
		String id = "ID1";
		Order order = new Order(id);
		order.addFood(margherita,3);
		
		ArrayList<String> ingredients1 = new ArrayList<String>();
		ingredients1.add("pomodoro");
		ingredients1.add("mozzarella");
		ingredients1.add("funghi");
		ingredients1.add("prosciutto cotto");
		
		Food capricciosa = new Food("Pizza Capricciosa",7,ingredients1);
		
		order.addFood(capricciosa,2);
		
		String res = gson.toJson(order);
		
		return res;
	}
	
}
