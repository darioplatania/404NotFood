package it.polito.project.foodservice;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.Properties;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

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
		
		public String getInformation() throws SQLException, JSONException{
			
			// first connect to DB
			connect();
			
			// extract information from DB
			Statement statement = connection.createStatement();
			String sql = ("SELECT * FROM Food;");
			ResultSet result = statement.executeQuery(sql);
			
			// save information in a   JSON Array
			JSONArray jArray = new JSONArray();
			while (result.next())
			{
			    String  id_json=result.getString("id");
			    String name_json=result.getString("name");
			    String price_json=result.getString("price");
			    JSONObject json_obj = new JSONObject();
			    json_obj.put("id", id_json);
			    json_obj.put("name", name_json);
			    json_obj.put("price", price_json);
			    
			    // inserisci l'elemento nell'array
			    jArray.put(json_obj);
			}
			
			String res = jArray.toString();
			disconnect();
			
			return res;
		}

}
