<?php

$servername = "localhost";

$username = "id21692974_abc123";

$password = "123456789@Bc";

$dbname = "id21692974_thirdpersonsuviral";



$user_name = $_POST["userName"];



// Create connection

$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection

if ($conn->connect_error) {

  die("Connection failed: " . $conn->connect_error);

}



$sql = "SELECT user_name, high_score, email, date_created FROM user_information WHERE user_name = '" . $user_name . "'";

$result = $conn->query($sql);



if ($result->num_rows > 0) {

  //user information

    $rows = array();

    while ($row = $result->fetch_assoc()) {

        $rows[] = $row;

    }



    echo json_encode($rows);

} else {

  echo "0";

}

$conn->close();

?>