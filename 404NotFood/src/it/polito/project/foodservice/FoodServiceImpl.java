package it.polito.project.foodservice;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;
import java.util.Properties;

public class FoodServiceImpl {

		private static final String url = "jdbc:mysql://localhost:3306/Food_DB";
		private static final String username = "java";
		private static final String password = "password";
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
		
		// connect database
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
		
		// disconnect database
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
		
		public String try_connect(){
			Connection c = connect();
			if(c==null)
				return "Unable to connect";
			
			disconnect();
			return "Connected to DB!";
		}

}
