package it.polito.project.foodservice;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.Properties;

import javax.ws.rs.ServiceUnavailableException;

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
		 *  Get all data from DB
		 * @return
		 * @throws SQLException
		 * @throws JSONException
		 */
		public String getInformation() throws SQLException, JSONException{
			
			// connect and check if connection is ok
			connect();			
			if(connection==null)
				throw new ServiceUnavailableException();
			
			// extract information from DB
			Statement statement = connection.createStatement();
			String sql = ("SELECT * FROM Food;");
			ResultSet result = statement.executeQuery(sql);
			
			// save information in a JSON Array
			JSONArray jArray = new JSONArray();
			while (result.next())
			{
			    String  id_json=result.getString("id");
			    String name_json=result.getString("name");
			    String ingredients_json=result.getString("ingredients");
			    String price_json=result.getString("price");
			    JSONObject json_obj = new JSONObject();
			    json_obj.put("id", id_json);
			    json_obj.put("name", name_json);
			    json_obj.put("ingredients", ingredients_json);
			    json_obj.put("price", price_json);
			   
			    jArray.put(json_obj);
			}
			
			String res = jArray.toString();
			disconnect();
			
			return res;
		}
		
		/**
		 * Add element to DB
		 * @return
		 * @throws SQLException 
		 */
		public String addItem(JSONObject json_item) throws SQLException{
			
			try {
				String name = json_item.getString("name");
				String ingredients = json_item.getString("ingredients");
				String price = json_item.getString("price");
				
				// add element to DB
				connect();
				Statement statement = connection.createStatement();
				String sql = "INSERT INTO `Food` (`id`, `name`, `ingredients`, `price`) "
						+ "VALUES (NULL, '"+name+"', '"+ingredients+"', '"+price+"')";
				
				ResultSet result = statement.executeQuery(sql);
				disconnect();
				return result.getString("id");
			} catch (JSONException e) {
				disconnect();
				return null;
			}
		}
		
		

}
