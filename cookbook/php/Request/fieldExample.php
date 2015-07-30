<?php

/*
** =====================================================================================================
** Create a field
** =====================================================================================================
*/

//Parametres
$type = 'numeric';
	//Normales : email = 'email' / telephone = 'phone' / zone de texte = 'textArena' / chaine de caractere = 'textField' / valeur numerique = 'numeric' / date = 'date'
	//Listes : liste de valeurs = 'singleSelectList' / Liste de valeurs multiples = 'multipleSelectList'
	
$name = 'createField';
$isUnicity = false;	//Unicité : oui = 'true' / non = 'false'
$isMandatory = true;	//Obligatoire : oui = 'true' / non = 'false'

$constraintOperator = 1;	//Pour les valeurs numeriques et les dates : 1 = '>' / 2 = '<' / 3 = '>=' / 4 = '<=' / 5 = '=' ('null' pour rien)
$constraintValue = "42";	//Valeur de la contrainte ex : ["42"] ('null' pour rien)

$valueListId = null;	//Id de la liste dans le cas d'un 'singelSelectList' ou un 'multipleSelectList' ('null' pour rien)

//Creation du Json du message
if ($constraintOperator != null && $constraintValue != null)
{
	$arr = array(
			'type' => $type,
			'name' => $name,
			'isUnicity' => $isUnicity,
			'isMandatory' => $isMandatory,
			'constraint' => array(
					'operator' => $constraintOperator,
					'value' => $constraintValue));
	
}
else if ($valueListId != null)
{
	$arr = array(
			'type' => $type,
			'name' => $name,
			'isUnicity' => $isUnicity,
			'isMandatory' => $isMandatory,
			'valueListId' => $valueListId);
}
else
{
	$arr = array(
			'type' => $type,
			'name' => $name,
			'isUnicity' => $isUnicity,
			'isMandatory' => $isMandatory);
}


//On affiche le message
$message = json_encode($arr);
echo $message . "\n";


/*
** =====================================================================================================
** Delete a field
** =====================================================================================================
*/

$id = 0123;
$url = 'http://v8.mailperformance.com/fields/' . $id;

echo $url . "\n";


/*
** =====================================================================================================
** Retrieve a field
** =====================================================================================================
*/

$id = 0123;
$url = 'http://v8.mailperformance.com/fields/' . $id;

echo $url . "\n";


/*
** =====================================================================================================
** Retrieve all fields
** =====================================================================================================
*/

$url = 'http://v8.mailperformance.com/fields/';

echo $url . "\n";


/*
** =====================================================================================================
** Update a field
** =====================================================================================================
*/

//Parametres
$type = 'numeric';
	//Normales : email = 'email' / telephone = 'phone' / zone de texte = 'textArena' / chaine de caractere = 'textField' / valeur numerique = 'numeric' / date = 'date'
	//Listes : liste de valeurs = 'singleSelectList' / Liste de valeurs multiples = 'multipleSelectList'

$name = 'Field';
$isUnicity = false;	//Unicité : oui = 'true' / non = 'false'
$isMandatory = false;	//Obligatoire : oui = 'true' / non = 'false'

$constraintOperator = null;	//Pour les valeurs numeriques et les dates : 1 = '>' / 2 = '<' / 3 = '>=' / 4 = '<=' / 5 = '=' ('null' pour rien)
$constraintValue = null;	//Valeur de la contrainte ex : ["42"] ('null' pour rien)

$valueListId = null;	//Id de la liste dans le cas d'un 'singelSelectList' ou un 'multipleSelectList' ('null' pour rien)

//Creation du Json du message
if ($constraintOperator != null && $constraintValue != null)
{
	$arr = array(
			'type' => $type,
			'name' => $name,
			'isUnicity' => $isUnicity,
			'isMandatory' => $isMandatory,
			'constraint' => array(
					'operator' => $constraintOperator,
					'value' => $constraintValue));

}
else if ($valueListId != null)
{
	$arr = array(
			'type' => $type,
			'name' => $name,
			'isUnicity' => $isUnicity,
			'isMandatory' => $isMandatory,
			'valueListId' => $valueListId);
}
else
{
	$arr = array(
			'type' => $type,
			'name' => $name,
			'isUnicity' => $isUnicity,
			'isMandatory' => $isMandatory);
}


//On affiche le message
$message = json_encode($arr);
echo $message . "\n";

?>