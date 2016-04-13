"""
Ce script permet de créer un Import Automatique.
"""

import json
import requests
import Utils

# TODO

def create_auto_import(config):
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
        data = json.load(data_file)

    data_json = json.JSONEncoder().encode(data)

    print("Creation of the auto import.")
    url = config['API']['url'] + 'imports/'
    headers = Utils.create_headers(config['API']['xKey'], len(data_json))
    req = requests.post(url, data=data_json, headers=headers)
    req.raise_for_status()

    print("The auto import has been Created.")

def main():
    config = Utils.load_config()
    create_auto_import(config)

if __name__ == '__main__':
    main()
