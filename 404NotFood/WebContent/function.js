window.url = "http://95.85.47.151:8080/food/webapi/food/";

$(document).ready(function() {
    $.ajax({
        url: window.url
    }).then(function(data) {
       var i = 0;
       var text = "";
       while(i!=data.length){
         document.getElementById("content").insertAdjacentHTML("afterend", "<tr class='"+text+"'><td><span class='label label-info'>"+data[i].id+"</span></td><td><p><b><p>Name: "+data[i].name+"</p></b></p><p><p>Price: "+data[i].price+"€</p></p><p>Ingredients: "+data[i].ingredients+"</p></p></td><td><!--<button class='btn btn-block btn-warning'>Edit <span class='glyphicon glyphicon-pencil'></span></button><p></p>--!><button class='btn btn-sm btn-danger' onclick='deleteItem("+data[i].id+")'>Delete <span class='glyphicon glyphicon-remove'></span></button></td></tr>");
         i++;
       }
    });

});

// delete item from DB using RestAPI
function deleteItem(element_id){
  var answer = confirm("Do you really want delete item n°: "+element_id+"?")
  if(answer){
    $.ajax({
    url: window.url+element_id,
    type: 'DELETE',
    success: function(result) {
      alert("Deleted")
      sessionStorage.lastitem = 0;
      location.reload()
      }
    });
  }
}

// add item to DB using RestAPI
function addItem(){
    var name = prompt("Please enter item name:", "new item")
    if (name != null) {
        var price = prompt("Price: ", "0.00")
        if(price != null) {
          price = parseFloat(price)
          var ingredients = prompt("Ingredients: ", "ingredients here")
          if(ingredients != null){

            // create json object
            var JSONObject= {"name":name, "price":price, "ingredients":ingredients};
            var jsonData = JSON.stringify( JSONObject );

            var request = $.ajax({
              url: window.url,
              type: 'POST',
              data: jsonData,
              contentType: "application/json",
              success: function(result) {
                alert("New Item inserted");
                sessionStorage.lastitem = result.id;
                location.reload();
              },
              error: function (xhr, errorType, exception) {
                  var errorMessage = exception || xhr.statusText;
                  alert(errorMessage);
              }
            });
          }
        }
      }
}
