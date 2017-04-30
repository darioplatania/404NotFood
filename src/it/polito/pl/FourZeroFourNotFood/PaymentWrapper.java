package it.polito.pl.FourZeroFourNotFood;

import java.io.IOException;
import java.util.ArrayList;
import java.util.logging.Level;

import com.google.gson.Gson;
import com.google.gson.JsonSyntaxException;
import com.paypal.api.payments.Address;
import com.paypal.api.payments.Amount;
import com.paypal.api.payments.CreditCard;
import com.paypal.api.payments.Details;
import com.paypal.api.payments.FundingInstrument;
import com.paypal.api.payments.Payer;
import com.paypal.api.payments.Payment;
import com.paypal.api.payments.Transaction;
import com.paypal.base.rest.APIContext;
import com.paypal.base.rest.PayPalRESTException; 

class PaymentWrapper {

	public static boolean handlePayment(String cardAsJson,Order order){
		
		
		// Replace these values with your clientId and secret. You can use these to get started right now.
		String clientId = "Abn0dxjisnKjhpPKzZE2VMC4HRunPCwyw3VtpgEzovZmxKxtljGICyBCyp8Fs_qekr7NbKqfFfGIQUU5";
		String clientSecret = "ENORQ1sWo1FU1v0nYt63L2zRXvBOd5GriNiaKi86PSRYyuplnySu0XnEIsUpMAFspFzNFseq_tOCWjZE";
		
		
		/*
		 * 
		// Set address info
		//Address billingAddress = new Address();
		//billingAddress.setCity("Johnstown");
		//billingAddress.setCountryCode("US");
		//billingAddress.setLine1("52 N Main ST");
		//billingAddress.setPostalCode("43210");
		//billingAddress.setState("OH");

	
		// Payment details
		//Details details = new Details();
		//details.setShipping("1");
		//details.setSubtotal("5");
		//details.setTax("1");
		// Credit card info

		// Create a Credit Card
		
		 CreditCard card = new CreditCard()
		    .setType("visa")
		    .setNumber("4567516310777851")
		    .setExpireMonth(11)
		    .setExpireYear(2018)
		    .setCvv2("874")
		    .setFirstName("Joe")
		    .setLastName("Shopper");
		   	.setBillingAddress(billingAddress);
		*
		*/
		
		// Parsing data of credit card
		
		CreditCard card;
		Gson gson = new Gson();
		
		try{
			card = gson.fromJson(cardAsJson, CreditCard.class);
			
		}catch (JsonSyntaxException e) {
			LoggerWrapper.getInstance().DEBUG_INFO(Level.SEVERE,e.getMessage()+" while paying");
			return false;
		}
		
		// Total amount
		Amount amount = new Amount();
		amount.setCurrency("EUR");
		amount.setTotal("7");
		
		//TODO: FIX THIS
		//amount.setTotal(String.valueOf(order.getPrice()));
		//amount.setDetails(details);
	
		// Transaction details
		Transaction transaction = new Transaction();
		transaction.setAmount(amount);
		transaction.setDescription("Payment for Order: "+order.getId());
	
		ArrayList<Transaction> transactions = new ArrayList<Transaction>();
		transactions.add(transaction);
	
		
		// Set funding instrument
		FundingInstrument fundingInstrument = new FundingInstrument();
		fundingInstrument.setCreditCard(card);
	
		ArrayList<FundingInstrument> fundingInstrumentList = new ArrayList<FundingInstrument>();
		fundingInstrumentList.add(fundingInstrument);
	
		// Set payer details
		Payer payer = new Payer();
		payer.setFundingInstruments(fundingInstrumentList);
		payer.setPaymentMethod("credit_card");
	
		
	
		// Set payment details
		Payment payment = new Payment();
		payment.setIntent("sale");
		payment.setPayer(payer);
		payment.setTransactions(transactions);
		Payment createdPayment = null;
	
		System.out.println(card.toJSON());
		
		try {
			  // Create payment
			  APIContext apiContext = new APIContext(clientId, clientSecret, "sandbox");
			  createdPayment = payment.create(apiContext);
			  
			  order.setPaid(true);
			  LoggerWrapper.getInstance().DEBUG_INFO(Level.SEVERE, "Created payment with id = " + createdPayment.getId());
			  
			  return true;
			  
		} catch (PayPalRESTException e) {
			
			LoggerWrapper.getInstance().DEBUG_INFO(Level.SEVERE, e.getMessage());
			return false;
		}

		
		
		
	}
	
	
}
