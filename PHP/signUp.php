<?php
function abort($message, $connection)
{
  http_response_code(400);
  $connection->close();
  die($message);
}

$servername = "sql77.lh.pl";
$username = "serwer137660_tomaszwjr";
$password = "Sa2was@mypi";
$dbname = "serwer137660_tomaszwjr";

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);
// Check connection
if ($conn->connect_error) {
  die("Connection failed: " . $conn->connect_error);
}

// get arguments
$username = $conn->real_escape_string($_POST['username']);
$password = $_POST['password'];

// prepare validation query
$query = mysqli_query($conn, "SELECT * FROM users WHERE username = '$username' LIMIT 1");

// check if record exists
if (!$query) {
  abort('Error: ' . mysqli_error($conn), $conn);
} else if (mysqli_num_rows($query) > 0) {
  abort("'$username' already exists!", $conn);
}

// hash password
$passwordHash = password_hash($password, PASSWORD_DEFAULT);

// create user query
$query = mysqli_query($conn, "INSERT INTO users (username, passwordHash) VALUES ('$username', '$passwordHash')");


$result = mysqli_query($conn, "SELECT * from users");

if (!$result) {
    abort('Error: ' . mysqli_error($conn), $conn);
}

$conn->close();