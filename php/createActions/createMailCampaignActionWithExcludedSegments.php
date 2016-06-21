<?php

/**
 * Ce script permet de créer une Campagne Mail en excluant un Ségment.
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
 * Code pour créer une campagne mail.
 *
 * @var string
 */
$type = 'mailCampaign';
/**
 * Nom de la campagne.
 *
 * @var string
 */
$name = 'MailCampaignFromApi (php)';
/**
 * Description de la campagne.
 *
 * @var string
 */
$description = 'MailCampaignFromApi (php)';
/**
 * Id du dossier dans lequel vous voulez mettre la campagne.
 * ('null' pour aucun dossier)
 *
 * @var integer
 */
$informationFolder = null;
/**
 * Id de la categorie de campagne.
 * (Infos compte > Parametrage > Categories de campagnes).
 *
 * @var integer
 */
$informationCategory = null;
/**
 * Adresse expéditrice.
 *
 * @var string
 */
$contentHeadersFromPrefix = 'prefix';
/**
 * Libellé de l'expéditeur.
 *
 * @var string
 */
$contentHeadersFromLabel = 'label';
/**
 * Adresse vers laquelle sera redirigée la réponse.
 *
 * @var string
 */
$contentHeadersReply = 'address@reply.com';
/**
 * Objet du Mail
 *
 * @var string
 */
$contentSubject = 'Subject of the message';
/**
 * Contenu (en HTML) du Mail.
 *
 * @var string
 */
$contentHTML = 'Html message';
/**
 * Contenu (en texte) du Mail.
 *
 * @var string
 */
$contentText = 'Text message';
/**
 * Id du segment de test qui sera utilisé pour la phase de validation.
 *
 * @var integer
 */
$idTestSegment = 01234;
/**
 * Id des segments qui seront utilisés pour la campagne.
 *
 * @var array
 */
$idSelectSegment = array(01234);
/**
 * Id des segments qui seront éxclus de la campagne. (deux maximums)
 *
 * @var array
 */
$idExcludedSegment = array(01234);

// Creation du Json
$arr = array(
  'type'              => $type,
  'name'              => $name,
  'description'       => $description,
  'informations'      => [],
  'scheduler'         => [],
  'content'           => []
);
$arr['informations'] = array(
  'folder'            => $informationFolder,
  'category'          => $informationCategory
);
$arr['scheduler'] = array(
  'type'              => 'asap',	// Envoie : immediat = 'asap' / Date = 'scheduled'
  // 'startDate'         => '2016-07-27T08:15:00Z',	// Si type = 'scheduled' sinon a enlever
  'segments'          => []
);
$arr['scheduler']['segments'] = array(
  'selected'          => $idSelectSegment,
  'excluded'          => $idExcludedSegment
);
$arr['content'] =  array(
  'headers'           => [],
  'subject'           => $contentSubject,
  'html'              => $contentHTML,
  'text'              => $contentText
);
$arr['content']['headers'] = array(
  'from'              => [],
  'reply'             => $contentHeadersReply
);
$arr['content']['headers']['from'] = array(
  'prefix'            => $contentHeadersFromPrefix,
  'label'             => $contentHeadersFromLabel
);

// On affiche le message
$dataJson = json_encode($arr);
echo $dataJson . "\n";

// Connexion
$url = $configs['url'] . 'actions' ;
$con = connect($url, $configs['xKey'], $dataJson, 'POST');

// Verification des reponses
if ($con['info']['http_code'] == 200) {
  // L'action a bien ete cree
  echo 'Action : ' . $name . " created.\n\n";

  // On recupere l'id de l'action
  $tab = json_decode($con['result'], TRUE);
  $idAction = $tab['id'];


  /**
   * Creation du Json pour la phase de test
   * (détail : https://backoffice.mailperformance.com/doc/#api-Action-ValidateAsync)
   *
   * @var array
   */
  $arr = array(
  'fortest'           => true,	// Phase de test
  'campaignAnalyser'  => false,	// Campaign Analyzer : 'true' = oui / 'false' = non
  'testSegments'      => array($idTestSegment),	//Les Ids des differents segments de tests
  'mediaForTest'      => null,	// Rediriger tous les tests vers une seule adresse ('null' pour aucune adresse)
  'textandHtml'       => false,	// Envoyer la version texte et la version html : 'true' = oui / 'false' = non
  'comments'          => null);	// Commentaire ('null' pour aucuns commentaires)

  // On affiche le Json
  $dataJson = json_encode($arr);
  echo $dataJson . "\n";

  // Connexion
  $url =  $configs['url'] . 'actions/' . $idAction . '/validation';
  $con = connect($url, $configs['xKey'], $dataJson, 'POST');

  // On attend que la phase de test soit terminee (peut prendre plusieurs minutes)
  $actionState = waitForState($idAction, $configs['url'], $configs['xKey']);

  // On verifie les reponses
  if ($con['info']['http_code'] != 204 || $actionState != 38) {
    if ($actionState == 20) {
      echo 'Error : the test failed';
    }
    else if ($actionState == 10) {
      echo 'Error : check the campaign in the Backoffice.';
    }
    else {
      echo 'Error : ' . $con['info']['http_code'];
    }
  }
  else {
    // La phase de test a reussi
    echo 'The action ' . $name . " is tested.\n\n";

    // On fait passer la demande de test en demande de validation
    $arr['fortest'] = false;
    $arr['testSegments'] = null;

    // On affiche le Json
    $dataJson = json_encode($arr);
    echo $dataJson . "\n";

    // Connexion
    $con = connect($url, $configs['xKey'], $dataJson, 'POST');

    // On verifie les reponses
    if ($con['info']['http_code'] == 204) {
      // La phase de validation a reussi
      echo 'The action ' . $name . "is validated.\n\n";
    }
    else {
      echo 'Error : ' . $con['info']['http_code'];
    }
  }
}
else {
  echo 'Error : ' . $con['info']['http_code'];
}

?>
