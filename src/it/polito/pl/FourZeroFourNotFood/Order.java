package it.polito.pl.FourZeroFourNotFood;

import java.util.ArrayList;

public class Order {

	private enum State {
		WAITING, IN_PROGRESS, IN_SERVICE, CLOSE
	};
	
	private String 			name;
	private String 			amount;
	private ArrayList<Food> foods;
	private State			state;
	
	public Order(String name, String amount, ArrayList<Food> foods) {
		this.name = name;
		this.amount = amount;
		this.foods = foods;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
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
