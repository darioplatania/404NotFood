package it.polito.project.foodservice;

import java.net.URISyntaxException;
import java.sql.SQLException;

import javax.ws.rs.GET;
import javax.ws.rs.NotFoundException;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.QueryParam;
import javax.ws.rs.ServiceUnavailableException;
import javax.ws.rs.core.Response;

import com.wordnik.swagger.annotations.Api;
import com.wordnik.swagger.annotations.ApiOperation;
import com.wordnik.swagger.annotations.ApiResponse;
import com.wordnik.swagger.annotations.ApiResponses;

/**
 * Root resource
 */
@Path("payment")
@Api(value = "/payment", description = "paypal payments")
public class PaymentServiceResource {

	
	@GET
	@ApiOperation(value = "redirecting for client")
	@ApiResponses(value = {
		@ApiResponse(code = 200, message = "OK"),
		@ApiResponse(code = 500, message = "Something wrong in Server"),
		@ApiResponse(code = 503, message = "Connection with DB not works correctly")})
	public Response addPayment(@QueryParam("PayerID") String PayerID, @QueryParam("paymentId") String paymentId) {
		PaymentServiceImpl res = new PaymentServiceImpl();
		try{
			res.addPayment(PayerID, paymentId);
		}catch(SQLException e){
			throw new ServiceUnavailableException();
		}
		
		java.net.URI location = null;
		try {
			location = new java.net.URI("../continue_payment.html");
		} catch (URISyntaxException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		// redirect user to html page
	    return Response.temporaryRedirect(location).build();
	}
	
	@GET
	@Path("{id}")
	@ApiOperation(value = "check payment")
	@ApiResponses(value = {
		  	@ApiResponse(code = 204, message = "No Content"),
		  	@ApiResponse(code = 404, message = "Item not found"),
		  	@ApiResponse(code = 500, message = "Something wrong in Server"),
			@ApiResponse(code = 503, message = "Connection with DB not works correctly")})
	public void checkPayment(@PathParam("id") String id){
		PaymentServiceImpl service = new PaymentServiceImpl();
		try{
			if(!service.checkPayment(id))
				throw new NotFoundException();
		} catch(SQLException e){
			throw new ServiceUnavailableException();
		}
				
	}
	
	@GET
	@Path()
	@ApiOperation(value = "cancel payment")
	@ApiResponses(value = {
		@ApiResponse(code = 200, message = "OK")})
	public Response addPayment() {
		
		java.net.URI location = null;
		try {
			location = new java.net.URI("../cancel_payment.html");
		} catch (URISyntaxException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		// redirect user to html page
	    return Response.temporaryRedirect(location).build();
	}
	
	
	
}
