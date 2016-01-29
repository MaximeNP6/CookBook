<?php

//Ici, renseignez la xKey
$xKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

$unicity = "test@test.com";	//Email de la cible
$idSegment = array(0123);	//Id du segment
$idAction = "000ABC";	//Id de l'action a dupliquer
$idTestSegment = array(0123);	//Id du segment de test

//On cherche la cible a ajouter dans le segment

//Nouvelle url
$url = "http://v8.mailperformance.com/targets?unicity=" . $unicity;

//Lancement de la connexion
$con = connect($url, $xKey, null, "GET");

$result = $con['result'];
$info = $con['info'];
$req = $con['req'];

if ($info['http_code'] != 200)
{
  echo "Error request 'GET' : " . $info['http_code'];
}
else
{
  //Lecture des donnees
  echo $result . "\n";

  //On recupere l'id de la cible
  $tab = json_decode($result, TRUE);
  $idTarget = $tab['id'];

  //Nouvelle url
  $url = "http://v8.mailperformance.com/targets/" . $idTarget . "/segments/" . $idSegment[0];

  //Lancement de la connexion
  $con = connect($url, $xKey, null, "POST");

  $result = $con['result'];
  $info = $con['info'];
  $req = $con['req'];

  if ($info['http_code'] != 204)
  {
    echo "Error request 'POST' : " . $info['http_code'];
  }
  else
  {
    //Lecture des donnees
    echo "The target " . $unicity . " is added to the segment " . $idSegment[0] . ".\n\n";

    //Duplication de l'action

    //Nouvelle url
    $url = "http://v8.mailperformance.com/actions/" . $idAction . "/duplication";

    //On duplique a l'identique
    $con = connect($url, $xKey, null, "POST");

    $result = $con['result'];
    $info = $con['info'];
    $req = $con['req'];

    if ($info['http_code'] != 200)
    {
      echo "Error request 'POST' duplication : " . $info['http_code'];
    }
    else
    {
      //On affiche la cible
      echo "The action duplicate ( " . json_decode($result, TRUE)['name'] . " ) is created.\n";
      echo $result . "\n\n";

      //On recupere l'id de l'action
      $tab = json_decode($result, TRUE);
      $idNewAction = $tab['id'];

      $data = json_decode($result, TRUE);

      //Modification de la nouvelle action
      $data['scheduler'] = array(
          "type" => "asap",	//Envoi : immediat = 'asap' / Date = 'scheduled'
          //"startDate" => "2015-07-27T08:15:00Z",	//Si type = 'scheduled' sinon a enlever
          "segments" => array(
            "selected" => $idSegment));

      $data = json_encode($data);


      //Nouvelle url
      $url = "http://v8.mailperformance.com/actions/" . $idNewAction;

      //On modifie l'action
      $con = connect($url, $xKey, $data, "PUT");

      $result = $con['result'];
      $info = $con['info'];
      $req = $con['req'];

      if ($info['http_code'] != 200)
      {
        echo "Error request 'PUT' modification : " . $info['http_code'];
      }
      else
      {
        echo "The action duplicate ( " . json_decode($result, TRUE)['name'] . " ) is update.\n";

        //On teste l'action

        //Nouvelle url
        $url = "http://v8.mailperformance.com/actions/" . $idNewAction . "/validation";

        //Creation du Json du message pour le test
        $arr = array(
          'fortest' => true,	//Phase de test
          'campaignAnalyser' => false,	//Campaign Analyzer : 'true' = oui / 'false' = non
          'testSegments' => $idTestSegment,	//Les Ids des differents segments de tests
          'mediaForTest' => null,	//Rediriger tous les tests vers une seule adresse ('null' pour aucune adresse)
          'textandHtml' => false,	//Envoyer la version texte et la version html : 'true' = oui / 'false' = non
          'comments' => null);	//Commentaire ('null' pour aucuns commentaires)

        //On affiche le Json
        $message = json_encode($arr);
        echo $message . "\n";

        //Connexion
        $con = connect($url, $xKey, $message, 'POST');
        $result = $con['result'];
        $info = $con['info'];
        $req = $con['req'];

        //On attend que la phase de test soit terminee (peut prendre plusieurs minutes)
        $actionState = waitForState($idNewAction, $xKey);

        //On verifie les reponses
        if ($info['http_code'] != 204 || $actionState != 38)
        {
          if ($actionState == 20)
            echo 'Error : the test failed';
          else if ($actionState == 10)
            echo 'Error : check the campaign in the Backoffice.';
          else
            echo 'Error : ' . $info['http_code'];
        }
        else
        {
          //La phase de test a reussi
          echo 'The action ' . json_decode($result, TRUE)['name'] . " is tested.\n\n";


          //Creation du Json du message pour la validation
          $arr = array(
            'fortest' => false,	//Phase de test
            'campaignAnalyser' => false,	//Campaign Analyzer : 'true' = oui / 'false' = non
            'testSegments' => $idTestSegment,	//Les Ids des differents segments de tests
            'mediaForTest' => null,	//Rediriger tous les tests vers une seule adresse ('null' pour aucune adresse)
            'textandHtml' => false,	//Envoyer la version texte et la version html : 'true' = oui / 'false' = non
            'comments' => null);	//Commentaire ('null' pour aucuns commentaires)

          $message = json_encode($arr);

          echo $message . "\n";

          //Connexion
          $con = connect($url, $xKey, $message, 'POST');

          $result = $con['result'];
          $info = $con['info'];
          $req = $con['req'];

          //On verifie les reponses
          if ($info['http_code'] != 204)
          {
            echo 'Error : ' . $info['http_code'];
          }
          else
          {
            //La phase de validation a reussi
            echo 'The action ' . json_decode($result, TRUE)['name'] . "is validated.\n\n";
          }
        }
      }
    }
  }
}

curl_close($req);



//Fonctions -----



//Utilisation de cURL pour remplir les requetes
function startCurlInit($url)
{
  $init = curl_init();
  curl_setopt($init, CURLOPT_URL, $url);
  curl_setopt($init, CURLOPT_RETURNTRANSFER, true);
  return ($init);
}

//Fonction de connexion
function connect($url, $xKey, $message, $method)
{
  //On remplit la requete
  $req = startCurlInit($url);
  curl_setopt($req, CURLOPT_CUSTOMREQUEST, $method);

  //Mise en place du xKey et des options
  curl_setopt($req, CURLOPT_HTTPHEADER, array(
  'X-Key: ' . $xKey,
  'Content-Type: application/json',
  'Content-Length: ' . strlen($message)));
  curl_setopt($req, CURLOPT_POSTFIELDS, $message);

  //Execution de la requete
  $result = curl_exec($req);

  //Verification des reponses
  $info = curl_getinfo($req);

  return (array('result' => $result, 'info' => $info, 'req' => $req));
}

//Fonction qui verifie que la phase de test soit fini (peut prendre plusieurs minutes)
function waitForState($idAction, $xKey)
{
  $actionState = 30;

  while ($actionState != 38 && $actionState != 20 && $actionState != 10)
  {
    //On attend 20 secondes
    echo "Wait 20sec...\n";
    sleep(20);

    //On trouve l'adresse pour la requete
    $url = 'http://v8.mailperformance.com/actions/' . $idAction;

    //On remplit la requete 'GET'
    $req = startCurlInit($url);
    curl_setopt($req, CURLOPT_CUSTOMREQUEST, 'GET');

    //Mise en place du xKey et des options
    curl_setopt($req, CURLOPT_HTTPHEADER, array(
    'X-Key: ' . $xKey,
    'Content-Type: application/json'));

    //Execution de la requete
    $result = curl_exec($req);

    $tab = json_decode($result, TRUE);
    $actionState = $tab['informations']['state'];
  }
  return ($actionState);
}

?>
