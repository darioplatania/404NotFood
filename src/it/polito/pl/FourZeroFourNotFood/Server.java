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

import javax.ws.rs.client.Client;
import javax.ws.rs.client.ClientBuilder;
import javax.ws.rs.client.WebTarget;
import javax.ws.rs.core.Response;



class ClientRunnable implements Runnable{

	
	private static final String MSG_ORDER		 = "NEW_ORDER";
	private static final String MSG_PAYMENT		 = "PAYMENT_CARD";
	private static final String MSG_PAYMENT_P	 = "PAYMENT_PAYPAL";
	private static final String MSG_CLOSE		 = "CLOSE";
	private static final String MSG_UPDATE_ORDER = "UPDATE_ORDER";
	private static final String MSG_CANCEL_ORDER = "CANCEL_ORDER";
	private static final String MSG_USER_CONFIRM = "PAYMENT_CONFIRM";
	private static final String MSG_PAYMENT_OK = "+OK";
	private static final String MSG_PAYMENT_ERR = "ERR";


	
	private static final String URL 			 = "http://95.85.47.151:8080/food/webapi/payment/";
	//private static final String URL = "http://localhost:8080/food/webapi/payment/";
	
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
				
				System.out.println("Waiting for PAYMENT | PAYMENT_PAYPAL | UPDATE_ORDER | CANCEL_ORDER");
				PaymentWrapper paymentWrapper = new PaymentWrapper();
				
				switch(in.readLine()){
				
					case MSG_PAYMENT:
						System.out.println("Waiting for Payment from: "+hostname);
						if(paymentWrapper.handlePayment(in.readLine(),order)){
							socket.getOutputStream().write(MSG_PAYMENT_OK.getBytes("UTF-8"));
							isEnded=true;
						}else{
							isEnded=false;
							socket.getOutputStream().write(MSG_PAYMENT_ERR.getBytes("UTF-8"));
						}
						break;
						
					case MSG_PAYMENT_P:
						System.out.println("Initializing Paypal payment...");
						String paymentId = null;
						paymentId = paymentWrapper.handlePaymentPaypal(order);
						
						if(paymentId==null)
							break;
						
						// wait for user confirm
						System.out.println("Waiting for user confirm");
						if(in.readLine().equalsIgnoreCase(MSG_USER_CONFIRM)){
							
							// check server response
							try{
								// create the client
						        System.out.println(URL+paymentId);
						        Client c = ClientBuilder.newClient();
						        WebTarget target = c.target(URL+paymentId);
								Response responseMsg = target.request()
 						   							   	     .get();
								// print response
								System.out.println(responseMsg.toString());
								
								// UPDATE GUI
								if(responseMsg.getStatus()==204){
									socket.getOutputStream().write(MSG_PAYMENT_OK.getBytes("UTF-8"));
									paymentWrapper.updateGUIOnResult(order.getid(), true);
									isEnded=true;

								}
								else{
									socket.getOutputStream().write(MSG_PAYMENT_ERR.getBytes("UTF-8"));
									paymentWrapper.updateGUIOnResult(order.getid(), false);
								}
									
							} catch(Exception e){
								e.printStackTrace();
								System.out.println("Error while get information from server");
								socket.getOutputStream().write(MSG_PAYMENT_ERR.getBytes("UTF-8"));
								paymentWrapper.updateGUIOnResult(order.getid(), false);
							}
						}
						
						break;
						
						
					case MSG_UPDATE_ORDER:
						System.out.println("Waiting for UPDATED_ORDER from: "+hostname);
						Order current_order = getOrderFromJson(in.readLine());
						
						if(current_order.getid().equals(order.getid())){
							remove(order.getid());
							this.db.add(current_order);
						}
						else
							LoggerWrapper.getInstance().DEBUG_INFO(Level.SEVERE, "Cannot update different order ids received from "+hostname);
						
						//UPDATE GUI
						updateGUIWithNewOrder(current_order);
						
						break;
				
						
						
						
					case MSG_CANCEL_ORDER:
						remove(order.getId());
						isEnded=true;
						break;
						
					default:
						break;
				
			
				}
			}
			
			socket.close();
	
		} catch (IOException|JsonSyntaxException e) {
			LoggerWrapper.getInstance().DEBUG_INFO(Level.SEVERE,e.getMessage()+" from: "+hostname);
		}
		
		
		
	}

	private void remove(String id) {
			String id_to_remove = id;
			this.db.remove(id_to_remove);
			Display.getDefault().asyncExec(new Runnable(){
					
				@Override
				public void run() {
					TableItem item_to_remove = MainWindow.getOrderItems().get(id_to_remove);
					MainWindow.getTable().remove(MainWindow.getTable().indexOf(item_to_remove));
					MainWindow.getOrderItems().remove(id_to_remove);
					LoggerWrapper.getInstance().DEBUG_INFO(Level.INFO, "Order "+id_to_remove+" canceled from "+hostname);
				
					if(MainWindow.selected_id.equals(id_to_remove)){
						MainWindow.getMenuTable().removeAll();
						MainWindow.selected_id = "";
					}	
					
				}
			});		
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
				 
				 MainWindow.getOrderItems().put(id, new_item);
				 
				 
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
