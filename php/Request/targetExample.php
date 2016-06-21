<?php

/*
** =====================================================================================================
** Add a target to a segment
** =====================================================================================================
*/

$targetId = '000ABCDE';
$segmentId = 0123;
$url = 'https://backoffice.mailperformance.com/targets/' . $targetId . '/segments/' . $segmentId;

echo $url . "\n";


/*
** =====================================================================================================
** Create a target
** =====================================================================================================
*/

//Parametres
$string = "name";	//Chaine de caracteres
$listOfValues = "Mr";	//Liste de valeurs
$email = "test@test.com";	// E-mail
$phoneNumber = "0123456789";	// Telephone
$textZone = "150 caracters max";	//Zone de texte
$numbers = 123;	//Valeur numerique
$date = "2016-07-30T09:01:00Z";	//Date
$listMultipleValues = array("valeur 1", "valeur 2");	//Liste de valeurs multiples

//Creation du tableau en fonction de l'id des champs de la fiche cible : "id-champ" => "valeur de l'information"
$arr = array(
		"5398" => $string,
		"5399" => $listOfValues,
		"5400" => $email,
		"5452" => $phoneNumber,
		"5453" => $textZone,
		"5454" => $numbers,
		"5455" => $date,
		"5456" => $listMultipleValues);

//On affiche le message
$message = json_encode($arr);
echo $message . "\n";


/*
** =====================================================================================================
** Delete a target
** =====================================================================================================
*/

$id = '000ABCDE';
$url = 'https://backoffice.mailperformance.com/targets/' . $id;

echo $url . "\n";


/*
** =====================================================================================================
** Remove a target from a segment
** =====================================================================================================
*/

$targetId = '000ABCDE';
$segmentId = 0123;
$url = 'https://backoffice.mailperformance.com/targets/' . $targetId . '/segments/' . $segmentId;

echo $url . "\n";


/*
** =====================================================================================================
** Retrieve a target
** =====================================================================================================
*/

$id = '000ABCDE';
$url = 'https://backoffice.mailperformance.com/targets/' . $id;

echo $url . "\n";


/*
** =====================================================================================================
** Retrieve all segments associated with a target
** =====================================================================================================
*/

$id = '000ABCDE';
$url = 'https://backoffice.mailperformance.com/targets/' . $id . '/segments';

echo $url . "\n";


/*
** =====================================================================================================
** Retrieve the total number of targets
** =====================================================================================================
*/

$url = 'https://backoffice.mailperformance.com/targets/count';

echo $url . "\n";


/*
** =====================================================================================================
** Update a target
** =====================================================================================================
*/

//Parametres
$string = "name";	//Chaine de caracteres
$listOfValues = "Mr";	//Liste de valeurs
$email = "test@test.com";	// E-mail
$phoneNumber = "0123456789";	// Telephone
$textZone = "150 caracters max";	//Zone de texte
$numbers = 123;	//Valeur numerique
$date = "2016-07-30T09:01:00Z";	//Date
$listMultipleValues = array("valeur 1", "valeur 2");	//Liste de valeurs multiples

//Creation du tableau en fonction de l'id des champs de la fiche cible : "id-champ" => "valeur de l'information"
$arr = array(
		"5398" => $string,
		"5399" => $listOfValues,
		"5400" => $email,
		"5452" => $phoneNumber,
		"5453" => $textZone,
		"5454" => $numbers,
		"5455" => $date,
		"5456" => $listMultipleValues);

//On affiche le message
$message = json_encode($arr);
echo $message . "\n";

?>