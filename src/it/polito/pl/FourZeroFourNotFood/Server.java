package it.polito.pl.FourZeroFourNotFood;

import java.awt.image.BufferedImage;
import java.io.BufferedReader;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.io.UnsupportedEncodingException;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.ArrayList;
import java.util.logging.Level;

import org.eclipse.swt.SWT;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.TableItem;

import com.google.gson.Gson;
import com.google.gson.JsonSyntaxException;

import javax.imageio.ImageIO;

import org.json.simple.JSONObject;
import javax.ws.rs.client.Client;
import javax.ws.rs.client.ClientBuilder;
import javax.ws.rs.client.Entity;
import javax.ws.rs.client.WebTarget;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;

import org.h2.security.XTEA;



class ClientRunnable implements Runnable{

	
	private static final String MSG_ORDER		 = "NEW_ORDER";
	private static final String MSG_PAYMENT		 = "PAYMENT_CARD";
	private static final String MSG_PAYMENT_P	 = "PAYMENT_PAYPAL";
	private static final String MSG_CLOSE		 = "CLOSE";
	private static final String MSG_UPDATE_ORDER = "UPDATE_ORDER";
	private static final String MSG_CANCEL_ORDER = "CANCEL_ORDER";
	private static final String MSG_USER_CONFIRM = "PAYMENT_CONFIRM";
	private static final String MSG_PAYMENT_OK   = "+OK";
	private static final String MSG_PAYMENT_ERR  = "ERR";

	private static final String URL 			 = "http://95.85.47.151:8080/food/webapi/payment/";
	private static final String URL_ORDER		 = "http://95.85.47.151:8080/food/webapi/order/";
	//private static final String URL = "http://localhost:8080/food/webapi/payment/";
	private static final String filePath = "../QRcode";
	
	
	
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
			
			loadOrderWebService(order);
			
			LoggerWrapper.getInstance().DEBUG_INFO(Level.INFO, "ORDER "+order.getid()+" received from "+hostname);
			
			//UPDATE GUI
			updateGUIWithNewOrder(order);
			PaymentWrapper paymentWrapper = new PaymentWrapper();
			String paymentId = null;
			while(!isEnded){
				
				System.out.println("Waiting for PAYMENT | PAYMENT_PAYPAL | UPDATE_ORDER | CANCEL_ORDER");
				
				switch(in.readLine()){
				
					case MSG_PAYMENT:
						System.out.println("Waiting for Payment from: "+hostname);
						
						if(paymentWrapper.handlePayment(decrypt(in.readLine()),order)){
							socket.getOutputStream().write(MSG_PAYMENT_OK.getBytes("UTF-8"));
							updatePaymentWebService(order);
							// waiting for last message from FEZ
							
							if(in.readLine().equals(MSG_USER_CONFIRM));
								isEnded=true;
						}else{
							isEnded=false;
							socket.getOutputStream().write(MSG_PAYMENT_ERR.getBytes("UTF-8"));
						}
						break;
						
					case MSG_PAYMENT_P:
						System.out.println("Initializing Paypal payment...");
						if(paymentId==null)
							paymentId = paymentWrapper.handlePaymentPaypal(order);
						
						if(paymentId==null){
							socket.getOutputStream().write(MSG_PAYMENT_ERR.getBytes("UTF-8"));
						}else{
							try{
								socket.getOutputStream().write(MSG_PAYMENT_OK.getBytes("UTF-8"));
								sendQR(order.getId());
							}catch(Exception e){
								e.printStackTrace();
							}
							// wait for user confirm
							System.out.println("Waiting for user confirm");
							if(in.readLine().equals(MSG_USER_CONFIRM))
								checkPaymentStatus(order, paymentId, paymentWrapper, isEnded, socket);
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
						
						// important to create a new bill for new order
						paymentId = null;
						
						//UPDATE GUI
						updateGUIWithNewOrder(current_order);
						
						break;
				
						
						
						
					case MSG_CANCEL_ORDER:
						remove(order.getId());
						isEnded=true;
						break;
						
					case MSG_USER_CONFIRM:
						checkPaymentStatus(order, paymentId, paymentWrapper, isEnded, socket);
						
					default:
						break;
				
			
				}
			}

			socket.close();
			System.out.println("socket closed correctly");
			
		} catch (IOException|JsonSyntaxException|NullPointerException e) {
			LoggerWrapper.getInstance().DEBUG_INFO(Level.SEVERE,e.getMessage()+" from: "+hostname);
		
		}finally{
			Server.sockets.remove(socket);
		}
		
		
		
	}

	private void checkPaymentStatus(Order order, String paymentId, PaymentWrapper paymentWrapper, boolean isEnded, Socket socket) throws UnsupportedEncodingException, IOException {
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
				updatePaymentWebService(order);
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

	private String decrypt(String text) {
		// TODO remove encryption when FEZ app is ready
		//final String key = Long.toHexString(Double.doubleToLongBits(Math.random()));
		
		final String key = "1234567890123456";
		
		
		// initialize XTEA
		XTEA x = new XTEA();
		x.setKey(key.getBytes());
		byte[] byteString = hexStringToByteArray(text);
		
		
		// decrypt
		x.decrypt(byteString, 0, byteString.length); //byteString now contains the decrypted data
		String str = new String(byteString).replaceAll("\r\n", ""); //decrypted String without padding
		System.out.println("Stringa decriptata: "+str);
		
		return str;
		}
	
	public static String bytesToHex(byte[] bytes) {
		char[] hexArray = "0123456789ABCDEF".toCharArray();
		char[] hexChars = new char[bytes.length * 2];
	    for ( int j = 0; j < bytes.length; j++ ) {
	        int v = bytes[j] & 0xFF;
	        hexChars[j * 2] = hexArray[v >>> 4];
	        hexChars[j * 2 + 1] = hexArray[v & 0x0F];
	    }
	    return new String(hexChars);
	}
	
	public static byte[] hexStringToByteArray(String s) {
	    int len = s.length();
	    byte[] data = new byte[len / 2];
	    for (int i = 0; i < len; i += 2) {
	        data[i / 2] = (byte) ((Character.digit(s.charAt(i), 16) << 4)
	                             + Character.digit(s.charAt(i+1), 16));
	    }
	    return data;
	}
	
	private static byte[] convertStringToByteArray(String stringToConvert) {
	    byte[] theByteArray = stringToConvert.getBytes();
	    return theByteArray;
	}

	private void sendQR(String id) throws IOException {
        File file = new File(filePath+id+".jpg");
        BufferedImage image = ImageIO.read(file);

        ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
        ImageIO.write(image, "jpg", byteArrayOutputStream);
        
        socket.getOutputStream().write(String.valueOf(byteArrayOutputStream.size()).getBytes("UTF-8"));
        
        int i = 0;
        while(i!=byteArrayOutputStream.toByteArray().length){
        	
        	int diff = byteArrayOutputStream.toByteArray().length-i;
        	if(diff<1024){
        		socket.getOutputStream().write(byteArrayOutputStream.toByteArray(), i, diff);
        		i=byteArrayOutputStream.toByteArray().length;
        	}
        	else{
        		socket.getOutputStream().write(byteArrayOutputStream.toByteArray(), i, 1024);
        		i=i+1024;
        	}
        	socket.getOutputStream().flush();
        }
        socket.getOutputStream().write("END".getBytes("UTF-8"));
        socket.getOutputStream().flush();
        
        System.out.println("Ho inviato: "+byteArrayOutputStream.toByteArray().length);
        file.delete();
	}
	
	private void loadOrderWebService(Order order){
		
		try{
			
			 String items = "";
			 int it = order.getFoods().size();
			 while(it>0){
				 String el = order.getFoods().get(it-1).getFood().getName() + " x" + order.getFoods().get(it-1).getQuantity() + "; ";
				 items = items + el;
				 it--;
			 }
			 System.out.println(items);
			 
			 JSONObject json_obj = new JSONObject();
			 // populating json object
			 json_obj.put("id", order.getId());
			 json_obj.put("items", items);
			 json_obj.put("price", order.getPrice());
			 json_obj.put("status", "NOT OK");
			 
			
			 // create the client
		     Client c = ClientBuilder.newClient();
		     WebTarget target = c.target(URL_ORDER);
		     Response resp = target.request()
			   	   				   .post(Entity.entity(json_obj.toString(), MediaType.APPLICATION_JSON));
		     
		     System.out.println(resp.getStatus());
		}
		catch(Exception e){
			System.out.println("Error during upload order in Web Service");
			e.printStackTrace();
		}
		
	}
	
	private void updatePaymentWebService(Order order){
		try{
			Client c = ClientBuilder.newClient();
			WebTarget target = c.target(URL_ORDER+order.getId());
			Response responseMsg = target.request()
										 .put(Entity.entity("ok", MediaType.TEXT_PLAIN));
			System.out.println(responseMsg.getStatus());
		}
		catch(Exception e){
			System.out.println("Error during modify payment status in Web Service");
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
				 new_item.setText(new String[] { id,menu,price,paid});
				 
				 MainWindow.getOrderItems().put(id, new_item);
				 
				 
			 }
			});
	}
	
}

public class Server {

	private ServerSocket socket;
	private LoggerWrapper logger;
	
	public static ArrayList<Socket> sockets = new ArrayList<Socket>();
	
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
			
			sockets.add(new_client);
			
			logger.DEBUG_INFO(Level.INFO, "New connection from "+host);
			ClientRunnable cr = 
					new ClientRunnable(host, new_client);
			// START NEW THREAD TO HANDLE NEW CONNECTION
			Thread t = new Thread(cr);
			t.start();
			
			
						
		
		}
	}
	
	
	public void stop() throws IOException{
		this.socket.close();
    }
	
}
