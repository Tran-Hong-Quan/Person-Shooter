<?php

$servername = "localhost";

$username = "id21692974_abc123";

$password = "123456789@Bc";

$dbname = "id21692974_thirdpersonsuviral";



$user_name = $_POST["userName"];

$new_score = $_POST["newScore"];



// Create connection

$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection

if ($conn->connect_error) {

  die("Connection failed: " . $conn->connect_error);

}

$sql = "UPDATE user_information
SET high_score = '".$new_score."' WHERE user_name = '".$user_name."' AND high_score < '".$new_score."';";

$conn->close();

?>