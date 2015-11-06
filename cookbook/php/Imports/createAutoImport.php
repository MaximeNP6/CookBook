<?php

//Url de base
$urlBase = 'http://v8.mailperformance.com/';

//X-Key
$xKey = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';



function getCreateImportJson()
{
    // Remplissez les informations obligatoires
    $importName = "Nom de votre Import automatique"; //Nom de l'import
    $schedulerName = "Nom du scheduler"; //Nom du scheduler
    $binding = 1234; // Id du binding
    $segmentId = 1234; // Id du segment
    $fieldsId = 1234; // Id du field
    $contactsId = array("1234ABCD"); // Id des utilisateurs : Administration -> Utilisateurs -> Identifiant
    $groupsContactsId = array(1234); // Id des groupes : Administration -> Groupes -> Identifiant



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
            array( // Parametres de normalisation
                "type" => "normalization",
                "fields" => array(
                    $fieldsId => array("FRA") // Pour changer de pays de normalisation utiliser l'ISO ALPHA-3 Code (https://en.wikipedia.org/wiki/ISO_3166-1_alpha-3)
                )
            ),
            array( // Ajout des cibles a un Segment
                "type" => "segmentation",
                "segmentId" => $segmentId,
                "emptyExisitingSegment" => true
            ),/*
            array( // Ajout de TOUTES les cibles (de l'import) a la liste rouge
                "type" => "redList",
                "destination" => array( "sms" => true, "email" => true )
            ),*/
            array( // Regles a appliquer en cas de doublon
                "type" => "duplicate",
                "rules" => array( "ignore" => true ) /// ou "rules" => array( "first" => true ) ou "rules" => array( "last" => true )
            ),
            array( // Parametres des rapports
                "type" => "report",
                "sendFinalReport" => false, //Envoi d'un rapport final
                "sendErrorReport" => true, //Envoi d'un rapport d'erreur

                //Si un des rapport doit être envoyer, il faut au moins un destinataire ('contactGuids'/'groupIds')
                "contactGuids" => $contactsId, // Id des utilisateurs : Administration -> Utilisateurs -> Idenfitifiant
                "groupIds" => $groupsContactsId, // Id des groupes : Administration -> Groupes -> Idenfitifiant
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



//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////FIN DES CREATIONs DES JSONS//////////////////////////////////////////////////////////////////////////////////////////////////
/////DEBUT DU PROGRAMME///////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


$req = NULL;

//Nouvelle url
$url = $urlBase . "imports/";

//lancement de la requete
$con = urlPostOrPut($req, 'POST', getCreateImportJson(), $xKey, $url);
if (($req = verification($con)) == null)
    return (0);

curl_close($req);
return (1);



//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////FIN DU PROGRAMME/////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////DEBUT DES FONCTIONS//////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//On verifie les reponses
function verification($con)
{
    $result = $con['result'];
    $info = $con['info'];
    $req = $con['req'];

    //Verification des reponses
    if ($result == false)
    {
        //Erreur lors de la requete

        //Affichage de l'erreur
        $info = curl_getinfo($req);
        echo 'Error  => ' . $info['http_code'] . "\n";
        curl_close($req);
        return (null);
    }
    else
    {
        //On affiche la reponse
        echo 'OK : ' . $result . "\n\n";
        return ($req);
    }
}



//Utilisation de cURL pour remplir les requetes
//Fonctions de connexion
function startCurlInit($url)
{
    $init = curl_init();
    curl_setopt($init, CURLOPT_URL, $url);
    curl_setopt($init, CURLOPT_RETURNTRANSFER, true);
    return ($init);
}

function urlPostOrPut($req, $request, $dataJson, $xKey, $url)
{
    //On remplit la requete avec le bon verbe ($request) : POST / PUT
    $req = startCurlInit($url);
    curl_setopt($req, CURLOPT_CUSTOMREQUEST, $request);
    curl_setopt($req, CURLOPT_POSTFIELDS, $dataJson);

    //Mise en place du xKey et des options
    curl_setopt($req, CURLOPT_HTTPHEADER, array(
        'X-Key: ' . $xKey,
        'Content-Type: application/json',
        'Accept: application/vnd.mperf.v8.import.v1+json',
        'Content-Length: ' . strlen($dataJson),
    ));

    //Execution de la requete
    $result = curl_exec($req);

    //Verification des reponses
    $info = curl_getinfo($req);

    return (array('result' => $result, 'info' => $info, 'req' => $req));
}

?>
