<?php


/*
** =====================================================================================================
** Create a category
** =====================================================================================================
*/

//Parametres
$name = 'Category';
$description = 'Category';

//Creation du Json du message
$arr = array(
	'name' => $name,
	'description' => $description);


//On affiche le message
$message = json_encode($arr);
echo $message . "\n";


/*
** =====================================================================================================
** Delete a category
** =====================================================================================================
*/

$id = 0123;
$url = 'https://backoffice.mailperformance.com/categories/' . $id;

echo $url . "\n";


/*
** =====================================================================================================
** Retrieve a category
** =====================================================================================================
*/

$id = 0123;
$url = 'https://backoffice.mailperformance.com/categories/' . $id;

echo $url . "\n";


/*
** =====================================================================================================
** Retrieve all categories
** =====================================================================================================
*/

$url = 'https://backoffice.mailperformance.com/categories/';

echo $url . "\n";


/*
** =====================================================================================================
** Update a category
** =====================================================================================================
*/

//Parametres
$name = 'Category';
$description = 'Category';

//Creation du Json du message
$arr = array(
'name' => $name,
'description' => $description);


//On affiche le message
$message = json_encode($arr);
echo $message . "\n";

?>