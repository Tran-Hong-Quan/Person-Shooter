<?php
/*
$servername = "localhost";
$username = "root";
$password = "";
$dbname = "unitydatabase";
*/
$servername = "localhost";
$username = "id21692974_abc123";
$password = "123456789@Bc";
$dbname = "id21692974_thirdpersonsuviral";

$user_name = $_POST["userName"];
$new_data = $_POST["newData"];

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);
// Check connection

if ($conn->connect_error) {
  die("Connection failed: " . $conn->connect_error);
}

$sql = "UPDATE user_information SET user_data = ? WHERE user_name = ?";

// Sử dụng prepared statement để tránh SQL injection
$stmt = $conn->prepare($sql);
$stmt->bind_param("ss", $new_data, $user_name);

if ($stmt->execute()) {
    echo "Record updated successfully";
} else {
    echo "Error updating record: " . $stmt->error;
}

$stmt->close();
$conn->close();

?>