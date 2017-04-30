package it.polito.pl.FourZeroFourNotFood;

import java.util.ArrayList;

public class Order {

	private enum State {
		WAITING, IN_PROGRESS, IN_SERVICE, CLOSE
	};
	
	
	private String 			id;
	private String 			amount;
	private ArrayList<Food> foods;
	private State			state;
	
	public Order(String id, String amount, ArrayList<Food> foods) {
		this.id = id;
		this.amount = amount;
		this.foods = foods;
	}

	public String getid() {
		return id;
	}

	public void setid(String id) {
		this.id = id;
	}

	public String getAmount() {
		return amount;
	}

	public void setAmount(String amount) {
		this.amount = amount;
	}

	public ArrayList<Food> getFoods() {
		return foods;
	}

	public void setFoods(ArrayList<Food> foods) {
		this.foods = foods;
	}
	
	
	
	
	
	
}
