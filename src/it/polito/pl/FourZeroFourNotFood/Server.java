package it.polito.pl.FourZeroFourNotFood;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.logging.Level;

import org.eclipse.swt.SWT;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.TableItem;

import com.google.gson.Gson;
import com.google.gson.JsonSyntaxException;



class ClientRunnable implements Runnable{

	
	private static final String MSG_ORDER		 = "NEW_ORDER";
	private static final String MSG_PAYMENT		 = "PAYMENT";
	private static final String MSG_CLOSE		 = "CLOSE";
	private static final String MSG_UPDATE_ORDER = "UPDATE_ORDER";
	private static final String MSG_CANCEL_ORDER = "CANCEL_ORDER";
	
	
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

			case MSG_CLOSE:
				return false;
				
			default:
				return false;
		}
		
	}
	
	private void handleOrder(){
		
		// TODO: 2.1 Follow order workflow
		
		Order 			order;
		BufferedReader	in;
		boolean			isEnded = false;
		
		
		try {
			in = new BufferedReader(
			        new InputStreamReader(socket.getInputStream()));
			
			System.out.println("Waiting for Order from: "+hostname);
			order = getOrderFromJson(in.readLine());
			System.out.println(order.getId());
			if(this.db.get(order.getId())!=null){
				System.out.println("DUPLICATO");
				throw new JsonSyntaxException("Duplicated Order ID");
			}
			
			// UPDATE RAM DB
			this.db.add(order);
			System.out.println(this.db.size());
			
			LoggerWrapper.getInstance().DEBUG_INFO(Level.INFO, "ORDER "+order.getid()+" received from "+hostname);
			
			//UPDATE GUI
			updateGUIWithNewOrder(order);
			
			while(!isEnded){
				
				System.out.println("Waiting for Payment from: "+hostname);
				
				switch(in.readLine()){
				
					case MSG_PAYMENT:
						if(PaymentWrapper.handlePayment(in.readLine(),order))
							isEnded=true;
						
						break;
					case MSG_UPDATE_ORDER:
						Order current_order = getOrderFromJson(in.readLine());
						
						if(current_order.getid().equals(order.getid()))
							this.db.add(current_order);
						else
							LoggerWrapper.getInstance().DEBUG_INFO(Level.SEVERE, "Cannot update different order ids received from "+hostname);
						break;
				
					case MSG_CANCEL_ORDER:
						final String id_to_remove = order.getId();
						this.db.remove(id_to_remove);
						Display.getDefault().asyncExec(new Runnable(){
								
							@Override
							public void run() {
								TableItem item_to_remove = MainWindow.getOrderItems().get(id_to_remove);
								MainWindow.getTable().remove(MainWindow.getTable().indexOf(item_to_remove));
								MainWindow.getOrderItems().remove(id_to_remove);
								LoggerWrapper.getInstance().DEBUG_INFO(Level.INFO, "Order "+id_to_remove+" canceled from "+hostname);
							
								if(MainWindow.selected_id.equals(id_to_remove))
									MainWindow.getMenuTable().removeAll();
							
							}
							
						});
						
						
						isEnded=true;
						break;
						
					default:
						break;
				
			
				}
			}
			
	
		} catch (IOException|JsonSyntaxException e) {
			LoggerWrapper.getInstance().DEBUG_INFO(Level.SEVERE,e.getMessage()+" from: "+hostname);
		}
		
		
		
	}
	
	private Order getOrderFromJson(String json){
		return new Gson().fromJson(json, Order.class);
	}
	
	private void closeConnection() throws IOException{
		this.socket.close();
	}
	
	private void updateGUIWithNewOrder(final Order order){
		Display.getDefault().asyncExec(new Runnable() {
			 public void run() {
				 
				 String id = order.getid();
				 
				 String paid;
				 
				 if(order.isPaid())
					 paid = "Already Paid";
				 else
					 paid = "Waiting for Payment";
				 
				 String price = String.valueOf(order.getPrice())+" €";
				 
				 String menu = "";
				 for(OrderedFood of:order.getFoods()){
					 if(menu.equals(""))
							menu += of.getFood().getName()+" Qty: "+of.getQuantity();
					 else
						 	menu += " + "+of.getFood().getName()+" Qty: "+of.getQuantity();
				 }
				 
				 TableItem new_item = new TableItem(MainWindow.getTable(), SWT.NONE);
				 new_item.setText(new String[] { id,menu,price,paid,"Received" });
				 MainWindow.getOrderItems().putIfAbsent(id, new_item);
				 
			 }
			});
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
