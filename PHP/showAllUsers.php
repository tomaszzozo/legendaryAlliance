<?php
header('Content-type: text/plain');
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

$result = $conn->query("SELECT * from users");

$rows = $result->fetch_all(MYSQLI_ASSOC);
foreach ($rows as $row) {
    printf("id:%s username:'%s' passwordHash:%s banned:%s\n", $row["id"], $row["username"], $row["passwordHash"], $row["banned"]);
}

$conn->close();
