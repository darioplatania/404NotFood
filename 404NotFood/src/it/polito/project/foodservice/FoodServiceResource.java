package it.polito.project.foodservice;

import java.sql.SQLException;

import javax.ws.rs.Consumes;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.PUT;
import javax.ws.rs.Path;
import javax.ws.rs.Produces;
import javax.ws.rs.core.MediaType;

import org.json.JSONException;

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
    @ApiOperation(value = "add menu", notes = "")
	@ApiResponses(value = {
	    @ApiResponse(code = 200, message = "OK"),
	    @ApiResponse(code = 500, message = "Something wrong in Server")})
	//@Consumes(MediaType.TEXT_PLAIN)
	@Produces(MediaType.TEXT_PLAIN)
	public String addItem() {
		String string = "ok POST";
		return string;
	}
	    
	@PUT
	@ApiOperation(value = "add item", notes = "")
	@ApiResponses(value = {
	  	@ApiResponse(code = 200, message = "OK"),
	  	@ApiResponse(code = 500, message = "Something wrong in Server")})
	//@Consumes(MediaType.TEXT_PLAIN)
	@Produces(MediaType.TEXT_PLAIN)
	public String modifyIdem() {
	    String string = "ok PUT";
	    return string;
	}
	    
	@GET
	@ApiOperation(value = "get menu", notes = "get the menu ")
	@ApiResponses(value = {
		@ApiResponse(code = 200, message = "OK"),
		@ApiResponse(code = 500, message = "Something wrong in Server")})
	@Produces(MediaType.TEXT_PLAIN)
	public String getMenu() {
		FoodServiceImpl res = new FoodServiceImpl();
		String string;
		try {
			string = res.getInformation();
		} catch (SQLException | JSONException e) {
			// TODO Auto-generated catch block
			string = e.getMessage();
		}
		return string;
	}
	    
}
