<?php

//Url de base
$urlBase = 'http://v8.mailperformance.com/';

//X-Key
$xKey = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';



function getCreateImportJson($idBinding)
{
    // Remplissez les informations obligatoires
    $importName = "Nom de votre Import Manuel"; //Nom de l'import

    $segmentId = 1234; // Id du segment
    $phoneFieldsId = 1234; // Id du field
    $contactsId = array("1234ABCD"); // Id des utilisateurs : Administration -> Utilisateurs -> Idenfitifant
    $groupsContactsId = array(1234); // Id des groupes : Administration -> Groupes -> Idenfitifant

    $ImportData = array(
        "name" => $importName,// Remplissez les informations obligatoires
        "features" => array(
            array( // Parametre de normalisation
                "type" => "normalization",
                "fields" => array(
                    $phoneFieldsId => array("FRA") // Pour changer de pays de normalisation utiliser l'ISO ALPHA-3 Code (https://en.wikipedia.org/wiki/ISO_3166-1_alpha-3)
                )
            ),
            array( // Ajout des cibles a un Segment
                "type" => "segmentation",
                "segmentId" => $segmentId,
                "emptyExisitingSegment" => true
            ),
            /*array( // Ajout des cibles a la liste rouge
                "type" => "redList",
                "destination" => array(
                    "sms" => true,
                    "email" => true
                )
            ),*/
            array( // Regles a appliquer en cas de doublon
                "type" => "duplicate",
                "rules" => array( "ignore" => true ) /// ou "rules" => array( "first" => true ) ou "rules" => array( "last" => true )
            ),
            array( // Parametres du rapport
                "type" => "report",
                "sendFinalReport" => false,
                "sendErrorReport" => true,
                "contactGuids" => $contactsId, // Id des utilisateurs : Administration -> Utilisateurs -> Idenfitifant
                "groupIds" => $groupsContactsId, // Id des groupes : Administration -> Groupes -> Idenfitifant
            ),
            array( // Parametres pour mettre a jour une cible dans la base de donnees
                "type" => "database",
                "updateExisting" => true,
                "crushData" => false
            )
        ),
        "binding" =>  $idBinding,
    );

    $createImportJson = json_encode($ImportData);

    echo "json :\n" . $createImportJson . "\n";

    return ($createImportJson);
}

function getExecutionImportJson()
{
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


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////FIN DES CREATION DES JSONS///////////////////////////////////////////////////////////////////////////////////////////////////
/////DEBUT DU PROGRAMME///////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


$idBinding = 1234;

$req = NULL;

//Nouvelle url
$url = $urlBase . "imports";
//On cree l'import de base
echo "CREATION IMPORT\n";
$con = urlPostOrPut($req, 'POST', getCreateImportJson($idBinding), $xKey, $url, 0);
if (($req = verification($con)) == null)
    return (0);

$importId = json_decode($con['result'], true)['id'];

//Nouvelle url
$url = $urlBase . "imports/" . $importId . "/executions";
//On execute l'import
echo "EXECUTION IMPORT\n";
$con = urlPostOrPut($req, 'POST', getExecutionImportJson(), $xKey, $url, 1);
if (($req = verification($con)) == null)
    return (0);


curl_close($req);
return (1);


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////FIN DU PROGRAMME/////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////DEBUT DES FONCTIONS//////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


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
        echo 'Error : ' . $info['http_code'] . "\n" . $result;
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
    echo 'URL = ' . $url . "\n";
    $init = curl_init();
    curl_setopt($init, CURLOPT_URL, $url);
    curl_setopt($init, CURLOPT_RETURNTRANSFER, true);
    return ($init);
}

function urlPostOrPut($req, $request, $dataJson, $xKey, $url, $source)
{
    //On remplit la requete avec le bon verbe ($request) : POST / PUT
    $req = startCurlInit($url);
    curl_setopt($req, CURLOPT_CUSTOMREQUEST, $request);

    //Mise en place du xKey et des options
    if ($source == 0)
    {
        //Envoie normal
        curl_setopt($req, CURLOPT_POSTFIELDS, $dataJson);
        curl_setopt($req, CURLOPT_HTTPHEADER, array(
            'X-Key: ' . $xKey,
            'Content-Type: application/json',
            'Accept: application/vnd.mperf.v8.import.v1+json',
            'Content-Length: ' . strlen($dataJson)
        ));
    }
    else if ($source == 1)
    {
        //Envoie des informations du fichier selectionne
        curl_setopt($req, CURLOPT_POSTFIELDS, $dataJson["data"]);
        curl_setopt($req, CURLOPT_HTTPHEADER, array(
                'X-Key: ' . $xKey,
                'Accept: application/vnd.mperf.v8.import.v1+json',
                'Content-Type: application/octet-stream',
                'Content-Disposition: form-data; filename=' . $dataJson["name"]
        ));

    }

    //Execution de la requete
    $result = curl_exec($req);

    //Verification des reponses
    $info = curl_getinfo($req);

    return (array('result' => $result, 'info' => $info, 'req' => $req));
}

?>
