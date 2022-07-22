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
$username = $conn->real_escape_string($_GET['username']);
$password = $conn->real_escape_string($_GET['password']);

$query = mysqli_query($conn, "SELECT * FROM users WHERE username = '$username' LIMIT 1");

// check if record exists
if (!$query) {
  abort('Error: ' . mysqli_error($conn), $conn);
} else if (mysqli_num_rows($query) == 0) {
  abort("Incorrect username and password combination!", $conn);
}

$query = "SELECT passwordHash FROM users where username = '$username' LIMIT 1";

if (count(mysqli_query($conn, $query)->fetch_row()) == 1 && password_verify($password, mysqli_query($conn, $query)->fetch_row()[0])) {
    $conn->close();
    echo("Ok");
    return;
}

abort("Incorrect username and password combination!", $conn);