package it.polito.project.foodservice;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.PreparedStatement;
import java.sql.Statement;
import java.util.Properties;

import javax.ws.rs.BadRequestException;
import javax.ws.rs.NotFoundException;
import javax.ws.rs.ServiceUnavailableException;

import org.json.simple.JSONArray;
import org.json.JSONException;
import org.json.simple.JSONObject;

public class OrderServiceImpl {
	private static final String url = "jdbc:mysql://172.17.0.2:3306/Food_DB";
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
	 * @return
	 * @throws SQLException 
	 */
	@SuppressWarnings("unchecked")
	public void addItem(JSONObject json_item) throws SQLException{
		
		String id = json_item.get("id").toString();
		String items = json_item.get("items").toString();
		String price = json_item.get("price").toString();
		String status = json_item.get("status").toString();
		
		if(id==null || items==null || price==null || status==null)
			throw new BadRequestException();
		
		// add element to DB
		connect();

		String sql = "INSERT INTO `Ordini` (`id`, `items`, `price`, `status`) "
				+ "VALUES ('"+id+"', '"+items+"', '"+price+"', '"+status+"');";
		PreparedStatement ps = connection.prepareStatement(sql, Statement.RETURN_GENERATED_KEYS);
		ps.executeUpdate();
        ResultSet rs = ps.getGeneratedKeys();
        int id1 = 0;
        if(rs.next()){
            id1=rs.getInt(1);
        }
        
        disconnect();
	}
	
	
	public String checkPayment(String id) throws SQLException{
		
		if(id==null)
			throw new BadRequestException();
		
		connect();
		
		// extract information from DB
		Statement statement = connection.createStatement();
		final String sql = "SELECT `status` FROM `Ordini` WHERE id= '"+id+"'";
		ResultSet result = statement.executeQuery(sql);
		result.next();
		String status = result.getString("status");

		disconnect();

		return status;
	}
	
	public void modifyStatusPayment(String id) throws SQLException{
		
		if(id==null)
			throw new BadRequestException();
		
		connect();
		
		// create our java preparedstatement using a sql update query
	    Statement statement = connection.createStatement();
	    final String sql = "UPDATE `Ordini` SET `status`= 'OK' WHERE id = '"+id+"'";
	    statement.execute(sql);
	    
	    disconnect();
	}
	
}
