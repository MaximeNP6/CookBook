<?php

/**
 * Ce script permet de dupliquer une action et de la valider.
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
 * Id de l'action a dupliquer
 *
 * @var string
 */
$idAction = "XXXXXX";
/**
 * Id du segment de test pour permettre de valider l'action dupliquée
 *
 * @var array
 */
$idTestSegment = array(01234);
/**
 * Creation du JSON contenant les informations
 * (pour plus de détails : https://backoffice.mailperformance.com/doc/#api-Action-ValidateAsync)
 *
 * @var array
 */
$arr = array(
  'fortest'           => true,	// Phase de test
  'campaignAnalyser'  => false,
  'testSegments'      => $idTestSegment,
  'mediaForTest'      => null,	// Tests vers une adresse
  'textandHtml'       => false,
  'comments'          => null);

// Duplication de l'action

$url = $configs['url'] . "actions/" . $idAction . "/duplication";
$con = connect($url, $configs['xKey'], null, "POST");

$result = $con['result'];
$info = $con['info'];

if ($info['http_code'] == 200) {
  // On affiche la cible
  echo "The action duplicate ( " . json_decode($result, TRUE)['name'] . " ) is created.\n";
  echo $result . "\n\n";

  // On recupere l'id de l'action
  $tab = json_decode($result, TRUE);
  $idNewAction = $tab['id'];

  $data = json_decode($result, TRUE);

  // On teste l'action

  // On affiche le Json
  $message = json_encode($arr);
  echo $message . "\n";

  // Connexion
  $url = $configs['url'] . "actions/" . $idNewAction . "/validation";
  $con = connect($url, $configs['xKey'], $message, 'POST');
  $result = $con['result'];
  $info = $con['info'];

  // On attend que la phase de test soit terminée
  $actionState = waitForState($idNewAction, $configs['url'],$configs['xKey']);

  // On verifie les reponses
  if ($info['http_code'] != 204 || $actionState != 38) {
    if ($actionState == 20)
      echo 'Error : the test failed';
    else if ($actionState == 10)
      echo 'Error : check the campaign in the Backoffice.';
    else
      echo 'Error : ' . $info['http_code'];
  }
  else {
    // La phase de test a reussi
    echo "The action " .
         json_decode($result, TRUE)['name'] .
         " has been tested.\n\n";


    // Modification du Json pour valider l'action
    $arr['fortest'] = false;

    $message = json_encode($arr);
    echo $message . "\n";

    $con = connect($url, $configs['xKey'], $message, 'POST');
    $result = $con['result'];
    $info = $con['info'];

    // On verifie les reponses
    if ($info['http_code'] == 204) {
      // La phase de validation a reussi
      echo "The action " .
           json_decode($result, TRUE)['name'] .
           "has been validated.\n\n";
    }
    else {
      echo 'Error : ' . $info['http_code'];
    }
  }
}
else {
  echo "Error request 'POST' duplication : " . $info['http_code'];
}

?>
