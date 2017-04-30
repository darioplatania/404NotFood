package it.polito.pl.FourZeroFourNotFood;

import java.util.concurrent.ConcurrentHashMap;

// SINGLE WITH CONCURRENTHASHMAP

public class OrderDB {

	
	private ConcurrentHashMap<String,Order> orders;
	private static OrderDB db = null;
	
	private OrderDB(){
		orders = new ConcurrentHashMap<String,Order>();
	}
	
	
	public static OrderDB getInstance(){
		if(db == null)
			db = new OrderDB();
		return db;
	}
	
	public void add(Order order){
		this.orders.put(order.getid(), order);
	}
	
	public Order get(String id){
		return this.orders.get(id);
	}
	
	
}
