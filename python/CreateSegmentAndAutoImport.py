"""
Ce script permet de créer un Import Automatique.
"""

import json
import requests
import Utils

from CreateSegment import create_segment

def create_auto_import(config, segment_json):
    """
    Création de la campagne

    Description du fichier autoImport.json

	name                      => Nom de l'Import automatique.
	binding                   => ID du binding.
	source                    :
        type                  => Type de la source de l'import : "ftp" || "file"
	scheduler                 :
		type                  => Type de planificateur
		name                  => Nom du planificateur
		frequency             :
			occurs            :
				type          => Type d'occurence
			periodicity       :
				type          => Type de periodicité
				value         :
					...       => Heure à laquelle l'import est éféctué
		validity              :
			start             :
				...           => Date du début de la validitée
			end               :
				...           => Date de fin de la validitée ()
	features                  :
		type                  => Option de segmentation
		segmentId             => ID du ségment vers lequel seront ajouter les cibles importées.
		emptyExisitingSegment => Vider ou non le segment avant l'import
        ,
		type                  => Option de dupplication
		rules                 :
			ignore            => Ignorer ou non les duplicata
        ,
		type                  => Option de Rapport
		sendFinalReport       => Envoyer un rapport ou non à la fin de l'import.
		sendErrorReport       => Envoyer un rapport ou non si une erreur est survenue.
		contactGuids          => ID des utilisateurs à contacter pour informer du status de l'import.
		groupIds              => ID des groupes à contacter pour informer du status de l'import.
        ,
		type                  => Option de la DataBase
		updateExisting        => Update ou non la DataBase existante
		crushData             => Ecraser ou non les Cibles éxistante avec les valeurs importées

    """

    with open('Models/autoImport.json') as data_file:
        data_json = json.load(data_file)


    data_json['features'][0]['segmentId'] = segment_json['id']
    data = json.JSONEncoder().encode(data_json)

    print("Creation of auto import.")
    url = config['API']['url'] + 'imports/'
    headers = Utils.create_headers(config['API']['xKey'], len(data))
    req = requests.post(url, data=data, headers=headers)
    req.raise_for_status()

    print("Auto import has been Created.")

def main():
    config = Utils.load_config()
    segment_json = create_segment(config)
    create_auto_import(config, segment_json)

if __name__ == '__main__':
    main()
