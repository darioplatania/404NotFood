package it.polito.pl.FourZeroFourNotFood;

import java.util.ArrayList;

public class Food {

	private String				name;
	private float				price;
	private ArrayList<String>	ingredients;
	

	
	public Food(String name, float price, ArrayList<String> ingredients) {
		
		this.name = name;
		this.price = price;
		this.ingredients = ingredients;
		
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public float getPrice() {
		return price;
	}

	public void setPrice(float price) {
		this.price = price;
	}
	
	public ArrayList<String> getIngredients() {
		return ingredients;
	}

	public void setIngredients(ArrayList<String> ingredients) {
		this.ingredients = ingredients;
	}
	
}
