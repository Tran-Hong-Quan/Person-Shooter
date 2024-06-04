<?php

$servername = "localhost";

$username = "id21692974_abc123";

$password = "123456789@Bc";

$dbname = "id21692974_thirdpersonsuviral";



$user_name = $_POST["userName"];

$user_pass = $_POST["userPass"];

$email = $_POST["email"];



// Create connection

$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection

if ($conn->connect_error) {

  die("Connection failed: " . $conn->connect_error);

}



//Kiểm tra user_name đã có trong DB chưa

$sql = "SELECT user_name FROM user_information WHERE user_name = '" . $user_name . "'";

$result = $conn->query($sql);



if ($result->num_rows > 0) {

  echo "Username already exists";

} else {

  //Kiểm tra email đã có trong DB chưa

  $sql = "SELECT email FROM user_information WHERE email = '" . $email . "'";

  $result = $conn->query($sql);



  if ($result->num_rows > 0) {

    echo "Email already exists";

  } else {

    //Đăng ký tài khoản của bạn

    $sql2 = "INSERT INTO user_information (user_name, pass_word, email) 

            VALUES ('" . $user_name . "', '" . $user_pass . "', '" . $email . "')";

    if ($conn->query($sql2) === TRUE) {

        echo "Success";

    } else {

        echo "Error: " . $sql2 . "<br>" . $conn->error;

    }

  }

}

$conn->close();

?>