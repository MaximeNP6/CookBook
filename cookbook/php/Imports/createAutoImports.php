<?php

/**
 *  Ce script permet de créer un Import Automatique.
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
 * Creation du JSON pour l'import automatique.
 *
 *
 * @return string
 */
function createAutoImportJson()
{
  /**
   * Nom de l'import.
   *
   * @var string
   */
  $importName = "Nom de votre Import automatique";
  /**
   * Nom du scheduler.
   *
   * @var string
   */
  $schedulerName = "Nom du scheduler";
  /**
   * ID du binding.
   *
   * @var integer
   */
  $binding = 00000;
  /**
   * ID du segment.
   *
   * @var integer
   */
  $segmentId = 000000;
  /**
   * ID des utilisateurs à contacter pour informer du status de l'import.
   *
   * @var array
   */
  $contactsId = array();
  /**
   * ID des groupes à contacter pour informer du status de l'import.
   *
   * @var array
   */
  $groupsContactsId = array();



  $createImportData = array(
      "name" => $importName,
      "binding" => $binding,
      "source" => array(
              "type" =>"ftp",
      ),
      "scheduler" => array(
          "type" => "periodic",
          "name" => $schedulerName,
          "frequency" => array(
              "occurs" => array(
                  "type" => "daily", // Quotidien

                  //Autres possibilites
                  /*
                  "type" => "weekly", // Hebdomadaire
                  "days" => array("Mon", "Tue", "Wed", "Thu", "Fri", "Sat"), // les jours selectionnes de la semaine

                  "type" => "monthly", // Mensuel
                  "days" => array( 20 ) // le numeros du jour des imports
                  */
              ),
              "periodicity" => array( // Heure de l'import
                  "type" => "once",
                  "value" => array(
                      "hour" => 6,
                      "minute" => 0,
                      "second" => 0
                  )
              )
          ),
          "validity" => array(  // La validation n'est pas obligatoire
              "start" => array( // Date du commencement
                  "year" => 2016, // Les dates doivent etre coherentes
                  "month" => 12,
                  "date" => 1,
                  "hour" => 0,
                  "minute" => 0,
                  "second" => 0
              ),
              "end" => array( // Date de fin
                  "year" => 2017, // Les dates doivent etre coherentes
                  "month" => 10,
                  "date" => 1,
                  "hour" => 0,
                  "minute" => 0,
                  "second" => 0
              )
          )
      ),
      "features" => array( // POUR SUPPRIMER NIMPORTE QUELLE FONCTION IL SUFFIT DE METTRE EN COMMENTAIRE !!! (exemple avec 'redList')
          array( // Ajout des cibles a un Segment
              "type" => "segmentation",
              "segmentId" => $segmentId,
              "emptyExisitingSegment" => true
          ),
          /*array( // Ajout de TOUTES les cibles (de l'import) a la liste rouge
              "type" => "redList",
              "destination" => array( "sms" => true, "email" => true )
          ),*/
          array( // Regles a appliquer en cas de doublon
              "type" => "duplicate",
              "rules" => array( "ignore" => true ) // ou "rules" => array( "first" => true ) ou "rules" => array( "last" => true )
          ),
          array( // Parametres des rapports
              "type" => "report",
              "sendFinalReport" => false, // Envoi d'un rapport final
              "sendErrorReport" => true, // Envoi d'un rapport d'erreur

              // Si un des rapport doit être envoyer, il faut au moins un destinataire ('contactGuids'/'groupIds')
              "contactGuids" => $contactsId, // Id des utilisateurs : Administration -> Utilisateurs -> Identifiant
              "groupIds" => $groupsContactsId, // Id des groupes : Administration -> Groupes -> Identifiant
          ),
          array( // Parametres pour mettre a jour une cible dans la base de donnees
              "type" => "database",
              "updateExisting" => true,
              "crushData" => true
          )
      )
  );

  $createImportJson = json_encode($createImportData);

  //on affiche le json
  echo "json :\n" . $createImportJson . "\n\n";

  return ($createImportJson);
}

// Script


// Lancement de la requete
$url = $configs['url'] . "imports/";
$con = connect($url, $configs['xKey'], createAutoImportJson(), 'POST');
if ($con['result'] == false) {
  // Erreur lors de la requete
  // Affichage de l'erreur
  echo "Error : " . $con['info']['http_code'] . "\n" . $result;
  return;
}

?>
