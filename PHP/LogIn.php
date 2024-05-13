<?php

$servername = "localhost";

$username = "id21692974_abc123";

$password = "123456789@Bc";

$dbname = "id21692974_thirdpersonsuviral";



$user_name = $_POST["userName"];

$user_pass = $_POST["userPass"];



// Create connection

$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection

if ($conn->connect_error) {

  die("Connection failed: " . $conn->connect_error);

}



$sql = "SELECT pass_word FROM user_information WHERE user_name = '" . $user_name . "'";

$result = $conn->query($sql);



if ($result->num_rows > 0) {

  // output data of each row

  while($row = $result->fetch_assoc()) {

    if ($row["pass_word"] == $user_pass) {

      echo "Success";

    } else {

      echo "Password is not correct";

    }

  }

} else {

  echo "Username is not exists";

}

$conn->close();

?>