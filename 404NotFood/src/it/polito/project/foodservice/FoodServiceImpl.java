package it.polito.project.foodservice;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.Properties;

import javax.ws.rs.BadRequestException;
import javax.ws.rs.NotFoundException;
import javax.ws.rs.ServiceUnavailableException;

import org.json.simple.JSONArray;
import org.json.JSONException;
import org.json.simple.JSONObject;


public class FoodServiceImpl {

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
		 *  Get all data from DB
		 * @return
		 * @throws SQLException
		 * @throws JSONException
		 */
		@SuppressWarnings("unchecked")
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
			   
			    jArray.add(json_obj);
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
		@SuppressWarnings("unchecked")
		public String addItem(JSONObject json_item) throws SQLException{
			
			String name = json_item.get("name").toString();
			String ingredients = json_item.get("ingredients").toString();
			String price = json_item.get("price").toString();
			
			if(name==null || ingredients==null || price==null)
				throw new BadRequestException();
			
			// add element to DB
			connect();

			String sql = "INSERT INTO `Food` (`id`, `name`, `ingredients`, `price`) "
					+ "VALUES (NULL, '"+name+"', '"+ingredients+"', '"+price+"');";
			PreparedStatement ps = connection.prepareStatement(sql, Statement.RETURN_GENERATED_KEYS);
			ps.executeUpdate();
            ResultSet rs = ps.getGeneratedKeys();
            int id = 0;
            if(rs.next()){
                id=rs.getInt(1);
            }
            
            JSONObject json_obj = new JSONObject();
            json_obj.put("id", id);
            disconnect();
			return json_obj.toString();
		}
		
		/**
		 * Modify an existing element in DB
		 * @throws SQLException 
		 */
		public void modifyItem(String id, JSONObject json_item) throws SQLException{
			
			String name = json_item.get("name").toString();
			String ingredients = json_item.get("ingredients").toString();
			String price = json_item.get("price").toString();
			
			if(name==null || ingredients==null || price==null)
				throw new BadRequestException();
			
			connect();
			
			String sql = "UPDATE Food SET name = '"+name+"', price = '"+price+"', ingredients = '"+ingredients+"' WHERE id = "+id;
			PreparedStatement ps = connection.prepareStatement(sql);
			int res = ps.executeUpdate();
			
			disconnect();
			if(res==0)
				throw new NotFoundException();
			
		}
		
		/**
		 * Delete a single element from DB
		 * @throws SQLException 
		 */
		public void deleteElement(String id) throws SQLException{
			
			connect();
			
			String sql = "DELETE FROM `Food` WHERE id = "+id;
			PreparedStatement ps = connection.prepareStatement(sql);
			int res = ps.executeUpdate();
			
			disconnect();
			if(res==0)
				throw new NotFoundException();
			
		}
		

}