<?php

/*
** =====================================================================================================
** Create a segment
** =====================================================================================================
*/

//Parametres
$type = 'static';	//Code pour creer un segment Statique
$name = 'SegmentStatic';	//Nom du segment
$description = 'SegmentStatic';	//Description du segment
$expiration = '2016-01-08T12:11:00Z';	//Date d'expiration du segment
$isTest = true;	//Le test est un segment de test : oui = 'true' / non = 'false'

//Creation du Json du message
$arr = array(
		'type' => $type,
		'name' => $name,
		'description' => $description,
		'expiration' => $expiration,
		'isTest' => $isTest);

//On affiche le message
$message = json_encode($arr);
echo $message . "\n";


/*
** =====================================================================================================
** Retrieve a segment
** =====================================================================================================
*/

$id = 0123;
$url = 'http://v8.mailperformance.com/segments/' . $id;

echo $url . "\n";


/*
** =====================================================================================================
** Retrieve all segments
** =====================================================================================================
*/

$url = 'http://v8.mailperformance.com/segments/';

echo $url . "\n";


/*
** =====================================================================================================
** Retrieve general segment information
** =====================================================================================================
*/

$url = 'http://v8.mailperformance.com/segments/informations';

echo $url . "\n";


/*
** =====================================================================================================
** Update a segment
** =====================================================================================================
*/

//Parametres
$type = 'static';	//static - dynamic
$name = 'SegmentStatic';	//Nom du segment
$description = 'SegmentStatic';	//Description du segment
$expiration = '2016-01-08T12:11:00Z';	//Date d'expiration du segment
$isTest = true;	//Le test est un segment de test : oui = 'true' / non = 'false'

//Creation du Json du message
$arr = array(
	'type' => $type,
	'name' => $name,
	'description' => $description,
	'expiration' => $expiration,
	'isTest' => $isTest);

//On affiche le message
$message = json_encode($arr);
echo $message . "\n";

?>