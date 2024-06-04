<?php
$servername = "localhost";
$username = "id21692974_abc123";
$password = "123456789@Bc";
$dbname = "id21692974_thirdpersonsuviral";

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {

  die("Connection failed: " . $conn->connect_error);

}

$sql = "SELECT user_name, high_score FROM user_information ORDER BY high_score DESC LIMIT 100";
$result = $conn->query($sql);
$data = array();

if ($result->num_rows > 0) {
  //user information
    while ($row = $result->fetch_assoc()) {

        $data[] = $row;
    }
} else {   
    echo "0";
}
$json_data = json_encode($data);
echo $json_data;

$conn->close();

?>