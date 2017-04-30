package it.polito.pl.FourZeroFourNotFood;

import java.util.ArrayList;

class OrderedFood{
	
	private Food food;
	private int quantity;
	
	
	public OrderedFood(Food food, int quantity) {
		this.food = food;
		this.quantity = quantity;
	}
	
	public OrderedFood(Food food){
		this(food, 1);
	}

	public Food getFood() {
		return food;
	}

	public void setFood(Food food) {
		this.food = food;
	}

	public int getQuantity() {
		return quantity;
	}

	public void setQuantity(int quantity) {
		this.quantity = quantity;
	}
	
	
	
}

public class Order {

	private enum State {
		RECEIVED, WAITING, IN_PROGRESS, IN_SERVICE, CLOSE
	};
	
	
	private String 					id;
	private float 					price;
	private ArrayList<OrderedFood>  foods;
	private State					state;
	
	
	public Order(String id) {
		this.id = id;
		this.price = 0;
		this.foods = new ArrayList<OrderedFood>();
	}
	
	public Order(String id, float price, ArrayList<OrderedFood> foods) {
		this.id = id;
		this.price = price;
		this.foods = foods;
	}

	//TODO L'id deve essere generato univocamente lato client(Es. Ultime2CifreIP+TimeStamp)
	public String getid() {
		return id;
	}

	public void setid(String id) {
		this.id = id;
	}

	public ArrayList<OrderedFood> getFoods() {
		return foods;
	}

	public void setFoods(ArrayList<OrderedFood> foods) {
		this.foods = foods;
	}
	
	

	public void addFood(Food food, int quantity){
		
		this.foods.add(new OrderedFood(food,quantity));
		this.price += food.getPrice()*quantity;
	}

	public void addFood(Food food){
		this.addFood(food, 1);
	}
	
	
	public float getPrice() {
		return price;
	}

	public void setPrice(float price) {
		this.price = price;
	}

	public State getState() {
		return state;
	}

	public void setState(State state) {
		this.state = state;
	}
	
	
	
	
}
