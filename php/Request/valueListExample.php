<?php

/*
** =====================================================================================================
** Create a value list
** =====================================================================================================
*/

//Parametres
$name = 'createValueList';	//Nom de la liste
$ordered = false;	//Ordonnee : oui = 'true' / non = 'false'
$values = array(	//Valeurs de la liste
			array(	//Premiere valeur
				'index' => 1,	//Numeros de la valeur
				'value' => "Mr"),	//Valeur du champs
			array(	//Deuxieme valeur
				'index' => 2,	//Numeros de la valeur
				'value' => "Mme")	//Valeur du champs
			);


//Creation du Json du message
$arr = array(
		'name' => $name,
		'ordered' => $ordered,
		'values' => $values);

//On affiche le message
$message = json_encode($arr);
echo $message . "\n";


/*
** =====================================================================================================
** Retrieve a value list
** =====================================================================================================
*/

$id = '012';
$url = 'https://backoffice.mailperformance.com/valueLists/' . $id;

echo $url . "\n";


/*
** =====================================================================================================
** Retrieve all value lists
** =====================================================================================================
*/

$url = 'https://backoffice.mailperformance.com/valueLists/';

echo $url . "\n";


/*
** =====================================================================================================
** Update a value list
** =====================================================================================================
*/

//Parametres
$name = 'createValueList';	//Nom de la liste
$ordered = false;	//Ordonnee : oui = 'true' / non = 'false'
$values = array(	//Valeurs de la liste
			array(	//Premiere valeur
				'index' => 1,	//Numeros de la valeur
				'value' => "Mr"),	//Valeur du champs
			array(	//Deuxieme valeur
				'index' => 2,	//Numeros de la valeur
				'value' => "Mme")	//Valeur du champs
			);


//Creation du Json du message
$arr = array(
	'name' => $name,
	'ordered' => $ordered,
	'values' => $values);

//On affiche le message
$message = json_encode($arr);
echo $message . "\n";

?>