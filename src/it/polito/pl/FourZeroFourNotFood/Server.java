package it.polito.pl.FourZeroFourNotFood;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.logging.Level;



class ClientRunnable implements Runnable{

	
	private static final String MSG_ORDER	= "ORDER";
	private static final String MSG_PAYMENT = "PAYMENT";
	private static final String MSG_CLOSE	= "CLOSE";
	
	
	private String hostname;
	private Socket socket;
	
	public ClientRunnable(String hostname, Socket socket){
		this.hostname = hostname;
		this.socket = socket;
	}
	
	@Override
	public void run() {
		
		
		try {
			
			PrintWriter out =
				        new PrintWriter(socket.getOutputStream(), true);
			BufferedReader in = new BufferedReader(
			        new InputStreamReader(socket.getInputStream()));
			
			
			String message = "";
			do{
				
				// Handle Order Workflow
				dispatcher(message);
				
				
				
			}while(!message.equals(MSG_CLOSE));
			
			
		} catch (IOException e) {
		
			LoggerWrapper.getInstance().DEBUG_INFO(Level.INFO, e.getMessage());
		}
		

		
		
		
	}
	
	private void dispatcher(String message){
		switch(message)
		{
		
			case MSG_ORDER:
				handleOrder();
				break;
			
			case MSG_PAYMENT:
				//handlePayment();
				break;
				
			case MSG_CLOSE:

				// TODO: 2.2 Close Order Updating DB
				try {
					closeConnection();
				} catch (IOException e) {
					LoggerWrapper.getInstance().DEBUG_INFO(Level.INFO, e.getMessage());
				}
				
				break;
			
			default:
				break;
		}
		
		return;
	}
	
	private void handleOrder(){
		// TODO: 2.1 Follow order workflow
	}
	
	private void closeConnection() throws IOException{
		this.socket.close();
	}
	
}

public class Server {

	private ServerSocket socket;
	private LoggerWrapper logger;
	
	
	public Server(int PORT_NUMBER) throws IOException{
		
		this.logger = LoggerWrapper.getInstance();
		this.socket = new ServerSocket(PORT_NUMBER);
		
		
	}

	public ServerSocket getSocket() {
		return socket;
	}

	public void setSocket(ServerSocket socket) {
		this.socket = socket;
	}
	
	public void start() throws IOException{
		
		
		while(true){
		
			// WAIT FOR NEW CONNECTION
			
			Socket new_client = this.socket.accept();
			String host = new_client.getInetAddress().getHostAddress();
			
			
			logger.DEBUG_INFO(Level.INFO, "New connection from "+host);
			
			// TODO START NEW THREAD TO HANDLE NEW CONNECTION
			new Thread(
					new ClientRunnable(host, new_client)
			).start();
	
			
						
		
		}
	}
	
	
	public void stop() throws IOException{
		this.socket.close();
	}
	
	
}
