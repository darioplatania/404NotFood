package it.polito.pl.FourZeroFourNotFood;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.Locale;
import java.util.logging.Level;
import org.eclipse.swt.SWT;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.Table;
import com.google.gson.Gson;
import com.google.gson.JsonSyntaxException;
import com.paypal.api.payments.Amount;
import com.paypal.api.payments.CreditCard;
import com.paypal.api.payments.Details;
import com.paypal.api.payments.FundingInstrument;
import com.paypal.api.payments.Links;
import com.paypal.api.payments.Payer;
import com.paypal.api.payments.Payment;
import com.paypal.api.payments.RedirectUrls;
import com.paypal.api.payments.Transaction;
import com.paypal.base.rest.APIContext;
import com.paypal.base.rest.PayPalRESTException; 

class PaymentWrapper {

	private static final String clientId = "Abn0dxjisnKjhpPKzZE2VMC4HRunPCwyw3VtpgEzovZmxKxtljGICyBCyp8Fs_qekr7NbKqfFfGIQUU5";
	private static final String clientSecret = "ENORQ1sWo1FU1v0nYt63L2zRXvBOd5GriNiaKi86PSRYyuplnySu0XnEIsUpMAFspFzNFseq_tOCWjZE";
	private Amount amount;
	private Transaction transaction;
	private RedirectUrls redirectUrls;
	private ArrayList<Transaction> transactions;
	
	public void initPayment(Order order){
		
		// Set redirect URLs
		redirectUrls = new RedirectUrls();
		redirectUrls.setCancelUrl("http://localhost:8080/food/webapi/payment/cancel");
		redirectUrls.setReturnUrl("http://localhost:8080/food/webapi/payment");
		
		// Set total amount
		amount = new Amount();
		amount.setCurrency("EUR");
		amount.setTotal(String.format(Locale.ROOT,"%.02f", order.getPrice()));
	
		// Transaction details
		transaction = new Transaction();
		transactions = new ArrayList<Transaction>();
		transaction.setAmount(amount);
		transaction.setDescription("Payment for Order: "+order.getId());
		transactions.add(transaction);
		
		// Set payment details
		Details details = new Details();
		details.setShipping("0");
		details.setSubtotal(String.format(Locale.ROOT,"%.02f", order.getPrice()));
		details.setTax("0");
		
	}
	
	public boolean handlePayment(String cardAsJson,Order order){
		
		// Initializing payment
		initPayment(order);
		
		// Parsing data of credit card
		CreditCard card;
		Gson gson = new Gson();
		
		try{
			card = gson.fromJson(cardAsJson, CreditCard.class);
			
		}catch (JsonSyntaxException e) {
			LoggerWrapper.getInstance().DEBUG_INFO(Level.SEVERE,e.getMessage()+" while paying");
			return false;
		}
		
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
	
		
		try {
			  // Create payment
			  APIContext apiContext = new APIContext(clientId, clientSecret, "sandbox");
			  createdPayment = payment.create(apiContext);
			  order.setPaid(true);
			  LoggerWrapper.getInstance().DEBUG_INFO(Level.INFO, "Created payment with id = " + createdPayment.getId());
			  updateGUIOnResult(order.getId(),true);
			  return true;
			  
		} catch (PayPalRESTException e) {
			
			LoggerWrapper.getInstance().DEBUG_INFO(Level.SEVERE, e.getMessage());
			updateGUIOnResult(order.getId(),false);
			return false;
		}	
		
	}
	
	
	public String handlePaymentPaypal(Order order){
		
		initPayment(order);
		
		// Set payer details
		Payer payer = new Payer();
		payer.setPaymentMethod("paypal");
		
		// Add payment details
		Payment payment = new Payment();
		payment.setIntent("sale");
		payment.setPayer(payer);
		payment.setRedirectUrls(redirectUrls);
		payment.setTransactions(transactions);
		Payment createdPayment = null;
		
		// Create payment
		try {
			// Create payment
			APIContext apiContext = new APIContext(clientId, clientSecret, "sandbox");
			createdPayment = payment.create(apiContext);

			Iterator<Links> links = createdPayment.getLinks().iterator();
			while (links.hasNext()) {
				Links link = links.next();
				if (link.getRel().equalsIgnoreCase("approval_url")) {
					// TODO: REDIRECT USER TO link.getHref()	
					System.out.println(link.getHref());
					
					// mi ritorno l'id del pagamento
					return createdPayment.getId();
					
				}	
			}
		} catch (PayPalRESTException e) {
		    System.err.println(e.getDetails());
		    return null;
		}
			
		return null;
	}
	
	
	public static void updateGUIOnResult(final String id,boolean paid){
		  // UPDATE GUI
		  Display.getDefault().asyncExec(new Runnable() {
				 public void run() {
					 
					 Table	table = MainWindow.getTable();
					 int	index = table.indexOf(MainWindow.getOrderItems().get(id));
					 
					 table.getItem(index).setForeground(3, Display.getDefault().getSystemColor(SWT.COLOR_WHITE));
					 
					 if(paid){
						 
						 table.getItem(index).setText(3, "SUCCESS");
						 table.getItem(index).setBackground(3, Display.getDefault().getSystemColor(SWT.COLOR_DARK_GREEN));
						 
					 }else{
						 table.getItem(index).setText(3, "FAILED");
						 table.getItem(index).setBackground(3, Display.getDefault().getSystemColor(SWT.COLOR_RED));
					 }
					 
					 
					 
				 }
		  });
	}
}
