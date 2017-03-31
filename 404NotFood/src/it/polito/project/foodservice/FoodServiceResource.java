package it.polito.project.foodservice;

import java.sql.SQLException;

import javax.ws.rs.BadRequestException;
import javax.ws.rs.Consumes;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.PUT;
import javax.ws.rs.Path;
import javax.ws.rs.Produces;
import javax.ws.rs.ServiceUnavailableException;
import javax.ws.rs.core.MediaType;

import org.json.JSONException;
import org.json.simple.JSONObject;
import org.json.simple.parser.JSONParser;
import org.json.simple.parser.ParseException;

import com.wordnik.swagger.annotations.Api;
import com.wordnik.swagger.annotations.ApiOperation;
import com.wordnik.swagger.annotations.ApiResponse;
import com.wordnik.swagger.annotations.ApiResponses;

/**
 * Root resource
 */
@Path("food")
@Api(value = "/food", description = "menu")
public class FoodServiceResource {
	    
	    
	@POST
    @ApiOperation(value = "add item to menu")
	@ApiResponses(value = {
	    @ApiResponse(code = 200, message = "OK"),
	    @ApiResponse(code = 500, message = "Something wrong in Server"),
	    @ApiResponse(code = 503, message = "Connection with DB not works correctly")})
	@Consumes(MediaType.APPLICATION_JSON)
	@Produces(MediaType.APPLICATION_JSON)
	public String addItem(String item) {
		FoodServiceImpl res = new FoodServiceImpl();
		String id;

		try {
			// convert string to json
			JSONParser parser = new JSONParser();
			JSONObject json_item = (JSONObject) parser.parse(item);
			id = res.addItem(json_item);
		} catch (SQLException e) {
			throw new ServiceUnavailableException();
		} catch (ParseException e) {
			throw new BadRequestException();
		}
		
		return id;
	}
	 
	
	@PUT
	@ApiOperation(value = "modify an item")
	@ApiResponses(value = {
	  	@ApiResponse(code = 200, message = "OK"),
	  	@ApiResponse(code = 404, message = "Item not found"),
	  	@ApiResponse(code = 500, message = "Something wrong in Server")})
	@Consumes(MediaType.APPLICATION_JSON)
	public void modifyIdem(String item) {
		FoodServiceImpl service = new FoodServiceImpl();
		
		try{
			// convert string to json
			JSONParser parser = new JSONParser();
			JSONObject json_item =(JSONObject) parser.parse(item);
			service.modifyItem(json_item);
		} catch (SQLException e) {
			throw new ServiceUnavailableException();
		} catch (ParseException e) {
			throw new BadRequestException();
		}
	}
	  
	    
	@GET
	@ApiOperation(value = "get menu")
	@ApiResponses(value = {
		@ApiResponse(code = 200, message = "OK"),
		@ApiResponse(code = 500, message = "Something wrong in Server"),
		@ApiResponse(code = 503, message = "Connection with DB not works correctly")})
	@Produces(MediaType.APPLICATION_JSON)
	public String getMenu() {
		FoodServiceImpl res = new FoodServiceImpl();
		String jArray = null;
		try {
			jArray = res.getInformation();
		} catch (SQLException | JSONException e) {
			throw new ServiceUnavailableException();
		}
		
		return jArray;
	}
	    
}
