package it.polito.pl.FourZeroFourNotFood;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.ArrayList;
import java.util.logging.Level;

import com.google.gson.Gson;
import com.mysql.jdbc.log.Log;




public class Main {

	
	// SET APP NAME
	
	public static final String 	APP_NAME		  = "404 Not Food";
	public static final int		PORT_NUMBER 	  = 4096;
	public static final int 	PORT_HEART_NUMBER = 4097;
	
	
	
	public static void main(String[] args){

		// HEART BEAT THREAD
		new Thread(new Runnable(){

			@Override
			public void run() {
				heartService();
			}
			
		}).start();
		
		startServerThread();
		
		//START NEW THREAD TO HADLE GUI		
		try {
			MainWindow window = new MainWindow();
			window.open();
			
		} catch (Exception e) {
			e.printStackTrace();
		}
		
		
		
		
	}


	public static void startServerThread(){
		
		// START NEW THREAD WORKING AS SERVER

		new Thread(
				new Runnable(){

					@Override
					public void run() {

						startService();
						
					}
					
				}
		).start();
		
		
		
	}
	
	protected static void heartService(){
		ServerSocket socket;
		try {
			socket = new ServerSocket(PORT_HEART_NUMBER);
		
			while(true){
			
				System.out.println("Hearth beat +++++++++++++++ BORN "+Thread.currentThread().getId());
				try {
					
					
					// WAIT FOR NEW CONNECTION
					Socket new_client = socket.accept();
					System.out.println("Hearth beat ************* ACCEPT "+Thread.currentThread().getId());
					//String host = new_client.getInetAddress().getHostAddress();
					PrintWriter out = new PrintWriter(new_client.getOutputStream(), true);
					BufferedReader in = new BufferedReader(new InputStreamReader(new_client.getInputStream()));
				
					while(true){
						
						if(in.readLine()==null)
							break;
		
						Thread.sleep(500);
						out.println("PONG");
					}
					
					System.out.println("Hearth beat -------------- DEAD "+Thread.currentThread().getId());
				
				} catch (IOException|InterruptedException e) {
					System.out.println("Hearth beat -------------- DEAD "+Thread.currentThread().getId());
				}
			}
		}catch(IOException e){
			System.out.println("Hearth beat XXXXXXXXXXXXXXXXXXXXXX DEAD "+Thread.currentThread().getId());
		}
	}


	private static void startService(){
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
					System.exit(0);
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
