<?php

/**
 * Ce script permet de créer ou modifier un Champ.
 *
 * @package cookbook
 */

require 'utils.php';

/**
 * Variable contenant les configurations pour se connecter à l'API
 *
 * @var array
 */
$configs = parse_ini_file("config.ini");
/**
 * Id du champ à créer/modifier ('null' si le champ doit être créée)
 *
 * @var integer
 */
$fieldId = null;
/**
 * Type du champ à créer/modifier
 * (pour le détail des différents types : https://backoffice.mailperformance.com/doc/#api-Field)
 *
 * @var string
 */
$type = 'numeric';
/**
 * Nom du champ
 *
 * @var string
 */
$name = 'createField (php)';
/**
 * Défini si le champ doit être un critère d'unicité ou non
 *
 * @var bool
 */
$isUnicity = false;
/**
 * Défini si le champ doit être oubligatoire ou non
 *
 * @var bool
 */
$isMandatory = true;
/**
 * Pour les valeurs numeriques et les dates : 1 = '>' / 2 = '<' / 3 = '>='
 * 4 = '<=' / 5 = '==' ('null' pour ne pas ajouter)
 *
 * @var integer
 */
$constraintOperator = 1;
/**
 * Valeur de la contrainte ('null' pour ne pas ajouter)
 *
 * @var string
 */
$constraintValue = "42";
/**
 * Id de la liste associée ('null' pour ne pas ajouter)
 *
 * @var string
 */
$valueListId = null;

/**
 * Creation du JSON contenant les informations
 * (pour plus de détails : https://backoffice.mailperformance.com/doc/#api-Field)
 *
 * @var array
 */
$data = array(
	'type' 					=> $type,
	'name' 					=> $name,
	'isUnicity' 		=> $isUnicity,
	'isMandatory' 	=> $isMandatory
);
if ($constraintOperator != null && $constraintValue != null) {
	$data['constraint'] = array(
		'operator' 		=> $constraintOperator,
		'value' 			=> $constraintValue);
}
if ($valueListId != null) {
		$data['valueListId'] = $valueListId;
}
$dataJson = json_encode($data);

//On affiche le message
echo $dataJson . "\n";

//Connexion
$url = $configs['url'] . 'fields/' . $fieldId;
if ($fieldId != null) {
	$con = connect($url, $configs['xKey'], $dataJson, 'PUT');
}
else {
	$con = connect($url, $configs['xKey'], $dataJson, 'POST');
}

$result = $con['result'];
$info = $con['info'];

if ($info['http_code'] == 200) {
	//La champ a bien été créée
	echo "\nThe field  : " . $name . " has been created / updated.";
}
else {
	echo 'Error : ' . $info['http_code'];
}

?>
