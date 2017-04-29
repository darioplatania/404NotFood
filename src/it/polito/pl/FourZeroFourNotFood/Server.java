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

public class Server {

	private ServerSocket socket;
	private LoggerWrapper logger;
	private static final String CLOSE = "CLOSE";
	
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
		
		HashMap<String,Socket> clients = new HashMap<String,Socket>();
		while(true){
		
			// WAIT FOR NEW CONNECTION
			
			Socket new_client = this.socket.accept();
			
			String host = new_client.getInetAddress().getHostAddress();
			clients.put(host, new_client);
			
			logger.DEBUG_INFO(Level.INFO, "New connection from "+host);
			
			// TODO 2.0 START NEW THREAD TO HANDLE NEW CONNECTION
			
			PrintWriter out =
				        new PrintWriter(clients.get(host).getOutputStream(), true);
			BufferedReader in = new BufferedReader(
				        new InputStreamReader(clients.get(host).getInputStream()));
	
			String message = "";
			do{
				
				// TODO: 2.1 Follow order workflow
				
				
			}while(!message.equals(CLOSE));
			
			
			
			// TODO: 2.2 Close Order Updating DB
						
		
		}
	}
	
	
	public void stop() throws IOException{
		this.socket.close();
	}
	
}
