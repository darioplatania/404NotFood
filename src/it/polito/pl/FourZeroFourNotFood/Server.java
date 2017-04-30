package it.polito.pl.FourZeroFourNotFood;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.logging.Level;

import com.google.gson.Gson;
import com.google.gson.JsonSyntaxException;



class ClientRunnable implements Runnable{

	
	private static final String MSG_ORDER	= "ORDER";
	private static final String MSG_PAYMENT = "PAYMENT";
	private static final String MSG_CLOSE	= "CLOSE";
	private OrderDB db;
	
	
	private String hostname;
	private Socket socket;
	
	public ClientRunnable(String hostname, Socket socket){
		this.hostname = hostname;
		this.socket = socket;
		this.db = OrderDB.getInstance();
	}
	
	@Override
	public void run() {
		
		
		try {
			
			String message = "";
			
			PrintWriter out =
				        new PrintWriter(socket.getOutputStream(), true);
			BufferedReader in = new BufferedReader(
			        new InputStreamReader(socket.getInputStream()));
			
			
			
			
			// Handle Order Workflow
			do{
				System.out.println("Waiting for commands from: "+hostname);
			}while(dispatcher(in.readLine()));
			
			
			// TODO: 2.2 Close Order Updating DB
			try {
				closeConnection();
				LoggerWrapper.getInstance().DEBUG_INFO(Level.INFO, "Connection closed with host: "+hostname);
			} catch (IOException e) {
				LoggerWrapper.getInstance().DEBUG_INFO(Level.INFO, e.getMessage());
			}
			
			
		} catch (IOException e) {
		
			LoggerWrapper.getInstance().DEBUG_INFO(Level.SEVERE, e.getMessage());
		}
		

		
		
		
	}
	
	private boolean dispatcher(String message){
		switch(message)
		{
		
			case MSG_ORDER:
				handleOrder();
				return true;
			
			case MSG_PAYMENT:
				//handlePayment();
				return true;

			case MSG_CLOSE:
				return false;
				
			default:
				return false;
		}
		
	}
	
	private void handleOrder(){
		
		// TODO: 2.1 Follow order workflow
		BufferedReader in;
		try {
			in = new BufferedReader(
			        new InputStreamReader(socket.getInputStream()));
			
			System.out.println("Waiting for Order from: "+hostname);
			String orderAsJSON = in.readLine();
			if(orderAsJSON!=null){
			
				Gson gson = new Gson();
				Order order = gson.fromJson(orderAsJSON, Order.class);
				this.db.add(order);
				LoggerWrapper.getInstance().DEBUG_INFO(Level.INFO, "ORDER "+order.getid()+" received from "+hostname);
				
		
			}
			
			
			
		} catch (IOException|JsonSyntaxException e) {
			LoggerWrapper.getInstance().DEBUG_INFO(Level.SEVERE,e.getMessage()+" from: "+hostname);
		}
		
		
		
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
			
			// START NEW THREAD TO HANDLE NEW CONNECTION
			new Thread(
					new ClientRunnable(host, new_client)
			).start();
	
			
						
		
		}
	}
	
	
	public void stop() throws IOException{
		this.socket.close();
	}
	
	
}
