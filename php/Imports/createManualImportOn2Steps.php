<?php

/**
 * Ce script permet de créer un Import Manuel en 2 étapes seulements.
 *
 * @package cookbook
 */

require __DIR__ . '/../utils.php';

/**
 * Variable contenant les configurations pour se connecter à l'API.
 *
 * @var array
 */
$configs = parse_ini_file(__DIR__. '/../config.ini');
/**
 * ID du binding contenant les configurations du fichier à importer.
 *
 * @var int
 */
$idBinding = 8330;

// Fonctions

/**
 * Création du Json contenant les informations de l'import.
 *
 * @param integer $idBinding
 *
 * @return string
 */
function createImportJson($idBinding) {

  // TODO
  $importName = "2Steps4"; //Nom de l'import

  $segmentId = 14098; // Id du segment
  $contactsId = array("044EAE5A"); // Id des utilisateurs : Administration -> Utilisateurs -> Idenfitifant
  $groupsContactsId = array(); // Id des groupes : Administration -> Groupes -> Idenfitifant

  $importData = array(
    "name"                  => $importName,
    "features"              => array(),
    "binding"               => $idBinding,
  );
  $importData["features"] = [
  array( // Ajout des cibles a un Segment
    "type"                  => "segmentation",
    "segmentId"             => $segmentId,
    "emptyExisitingSegment" => false
    ),
  array( // Regles a appliquer en cas de doublon
    "type"                  => "duplicate",
    "rules"                 => ["ignore" => true]
   ),
  array( // Parametres du rapport
    "type"                  => "report",
    "sendFinalReport"       => true,
    "sendErrorReport"       => true,
    "contactGuids"          => $contactsId, // Id des utilisateurs : Administration -> Utilisateurs -> Idenfitifant
    "groupIds"              => $groupsContactsId, // Id des groupes : Administration -> Groupes -> Idenfitifant
    ),
  array( // Parametres pour mettre a jour une cible dans la base de donnees
    "type"                  => "database",
    "updateExisting"        => true,
    "crushData"             => false
    )
  ];

  $createImportJson = json_encode($importData);

  echo "json :\n" . $createImportJson . "\n";

  return ($createImportJson);
}

/**
 * Création du Json contenant le contenue de l'import.
 *
 *
 * @return array
 */
function getExecutionImportJson() {
  // Remplissez les informations obligatoires
  $fromFile = "C:\\votre\\chemin\\vers\\le\\fichier\\";
  $nameFile = "nomDeVotreFichier.extension";

  $sourceImportData = array(
    "data" => file_get_contents($fromFile . $nameFile),
    "name" => $nameFile
  );
  $sourceImportJson = json_encode($sourceImportData);

  echo "json :\n" . $sourceImportJson . "\n";

  return ($sourceImportData);
}

// On cree l'import de base
echo "CREATION IMPORT\n";
$url = $configs['url'] . "imports";
$con = connect($url, $configs['xKey'], createImportJson($idBinding), 'POST');
if ($con['result'] == false) {
  // Erreur lors de la requete
  // Affichage de l'erreur
  echo "Error : " . $con['info']['http_code'] . "\n" . $con['result'];
  return;
}

$importId = json_decode($con['result'], true)['id'];

// On execute l'import
echo "EXECUTION IMPORT\n";
$url = $configs['url'] . "imports/" . $importId . "/executions";
$con = sendFile($url, $configs['xKey'], getExecutionImportJson(), 'POST');
if ($con['result'] == false) {
  // Erreur lors de la requete
  // Affichage de l'erreur
  echo "Error : " . $con['info']['http_code'] . "\n" . $con['result'];
  return;
}
echo "Error : " . $con['info']['http_code'] . "\n";

echo "Result : " . $con['result'] . ":\n";


?>
