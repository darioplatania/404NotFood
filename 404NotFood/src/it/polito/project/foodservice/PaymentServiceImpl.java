package it.polito.project.foodservice;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.Properties;

import javax.ws.rs.BadRequestException;

public class PaymentServiceImpl {
	private static final String url = "jdbc:mysql://localhost:3306/Food_DB";
	private static final String username = "root";
	private static final String password = "root";
	private static final String max_pool = "200";
	
	private Connection connection;
	private Properties properties;
	
	
	private Properties getProperties() {
	    if (properties == null) {
	        properties = new Properties();
	        properties.setProperty("user", username);
	        properties.setProperty("password", password);
	        properties.setProperty("MaxPooledStatements", max_pool);
	    }
	    return properties;
	}
	
	/** 
	 * Connect database
	 * @return
	 */
	public Connection connect() {
	    if (connection == null) {
	        try {
	            Class.forName("com.mysql.jdbc.Driver");
	            connection = DriverManager.getConnection(url, getProperties());
	        } catch (ClassNotFoundException | SQLException e) {
	            return null;
	        }
	    }
	    return connection;
	}
	
	/**
	 *  Disconnect database
	 */
	public void disconnect() {
	    if (connection != null) {
	        try {
	            connection.close();
	            connection = null;
	        } catch (SQLException e) {
	            e.printStackTrace();
	        }
	    }
	}
	

	/**
	 * Add element to DB
	 * @throws SQLException 
	 */
	public void addPayment(String PayerID, String paymentId) throws SQLException{
		
		if(PayerID==null || paymentId==null)
			throw new BadRequestException();
		
		// add element to DB
		connect();

		// create a Statement from the connection
		Statement statement = connection.createStatement();
		String sql = "INSERT INTO `Payment` (`paymentId`, `PayerID`) VALUES ('"+paymentId+"', '"+PayerID+"');";
		// insert the data
		statement.executeUpdate(sql);
        disconnect();
	}
	
	public boolean checkPayment(String paymentId) throws SQLException{
		
		if(paymentId==null)
			throw new BadRequestException();
		
		connect();
		
		Statement statement = connection.createStatement();
		final String sql = "SELECT * FROM Payment WHERE paymentId = '"+paymentId+"'";
		ResultSet result = statement.executeQuery(sql);

		boolean res = result.absolute(1);
		disconnect();
		
		return res;
	}
	
}
