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
  die("Could not connect to database: " . $conn->connect_error);
}

// get arguments
$username = $conn->real_escape_string($_GET['username']);
$password = $conn->real_escape_string($_GET['password']);

if ($password != "Sa2was@mypi") {
    abort("Incorrect password.");
}

$query = mysqli_query($conn, "SELECT * FROM users WHERE username = '$username' LIMIT 1");

// check if record exists
if (!$query) {
  abort('Unexpected validation query error: ' . mysqli_error($conn), $conn);
} else if (mysqli_num_rows($query) == 0) {
  abort("User does not exist", $conn);
}

$query = "DELETE FROM users WHERE username = '$username'";

if (!mysqli_query($conn, $query)) {
    abort("Unexpected delete query error: " . mysqli_error($conn), $conn);
}

echo("OK");