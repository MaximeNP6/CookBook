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
$idBinding = 1234;

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
  $importName = "Manual Import PHP"; //Nom de l'import

  $segmentId = 12345; // Id du segment
  $contactsId = ["XXXXXXX"]; // Id des utilisateurs : Administration -> Utilisateurs -> Idenfitifant
  $groupsContactsId = []; // Id des groupes : Administration -> Groupes -> Idenfitifant

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
  $fromFile = "./";
  $nameFile = "import.csv";
  $filePath = $fromFile . $nameFile;
  $valid = false;
  $validExt = [
      'txt',
      'csv',
      'zip',
      'tar.gz',
      'tgz',
      'gz'
  ];
  $validExtLength = count($validExt);

  $fileParts = pathinfo($filePath);

  if (!$fileParts || !isset($fileParts['extension']))
  {
      throw new Exception('There was a problem with your file');
  }

  for ($i = 0; $i < $validExtLength; $i++)
  {
      if ($validExt[$i] == $fileParts['extension'])
      {
          $valid = true;
      }
  }

  if (!$valid)
  {
      throw new Exception('Wrong type of file');
  }

  $finfo = finfo_open(FILEINFO_MIME_TYPE);

  $contentType = finfo_file($finfo, $filePath);

  finfo_close($finfo);

  $cfile = curl_file_create($filePath, $contentType, $fileParts['basename']);

  $data = ['data' => $cfile];

  return $data;
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

echo "Result : " . $con['result'] . ":\n";


?>
