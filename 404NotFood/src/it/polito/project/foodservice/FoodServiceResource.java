package it.polito.project.foodservice;

import java.sql.SQLException;

import javax.ws.rs.Consumes;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.PUT;
import javax.ws.rs.Path;
import javax.ws.rs.Produces;
import javax.ws.rs.ServiceUnavailableException;
import javax.ws.rs.core.MediaType;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

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
	public String addItem(JSONObject json_item) {
		FoodServiceImpl res = new FoodServiceImpl();
		String id = null;
		try {
			id = res.addItem(json_item);
		} catch (SQLException e) {
			throw new ServiceUnavailableException();
		}
		
		return id;
	}
	 
	/*
	@PUT
	@ApiOperation(value = "modify an item")
	@ApiResponses(value = {
	  	@ApiResponse(code = 200, message = "OK"),
	  	@ApiResponse(code = 500, message = "Something wrong in Server")})
	//@Consumes(MediaType.TEXT_PLAIN)
	@Produces(MediaType.TEXT_PLAIN)
	public String modifyIdem() {
	    String string = "ok PUT";
	    return string;
	}
	*/
	  
	    
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
