package it.polito.project.foodservice;

import java.sql.SQLException;

import javax.ws.rs.BadRequestException;
import javax.ws.rs.Consumes;
import javax.ws.rs.DELETE;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.PUT;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
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
@Path("order")
@Api(value = "/order", description = "orders details")
public class OrderServiceResource {


	@GET
	@Path("{id}")
	@ApiOperation(value = "check payment order status")
	@ApiResponses(value = {
		@ApiResponse(code = 200, message = "OK"),
	    @ApiResponse(code = 404, message = "Order not found"),
		@ApiResponse(code = 500, message = "Something wrong in Server"),
		@ApiResponse(code = 503, message = "Connection with DB not works correctly")})
	@Produces(MediaType.TEXT_PLAIN)
	public String checkPayment(@PathParam("id") String id) {
		OrderServiceImpl res = new OrderServiceImpl();
		try{
			return res.checkPayment(id);
		} catch(SQLException e){
			return "ERR";
		}
	}
	
	
	@POST
    @ApiOperation(value = "add item to orders")
	@ApiResponses(value = {
	    @ApiResponse(code = 200, message = "OK"),
	    @ApiResponse(code = 500, message = "Something wrong in Server"),
	    @ApiResponse(code = 503, message = "Connection with DB not works correctly")})
	@Consumes(MediaType.APPLICATION_JSON)
	public void addItem(String item) {
		OrderServiceImpl res = new OrderServiceImpl();
		try {
			// convert string to json
			JSONParser parser = new JSONParser();
			JSONObject json_item = (JSONObject) parser.parse(item);
			res.addItem(json_item);
		} catch (SQLException e) {
			throw new ServiceUnavailableException();
		} catch (ParseException e) {
			throw new BadRequestException();
		}
	}
	
	@PUT
	@Path("{id}")
	@ApiOperation(value = "modify payment status")
	@ApiResponses(value = {
		    @ApiResponse(code = 200, message = "OK"),
		    @ApiResponse(code = 404, message = "Order not found"),
		    @ApiResponse(code = 500, message = "Something wrong in Server"),
		    @ApiResponse(code = 503, message = "Connection with DB not works correctly")})
	@Produces(MediaType.TEXT_PLAIN)
	public String modifyPayment(@PathParam("id") String id){
		OrderServiceImpl res = new OrderServiceImpl();
		
		try{
			res.modifyStatusPayment(id);
			return "+OK";	
		}
		catch(Exception e){
			return "ERR";
		}
	}


}
