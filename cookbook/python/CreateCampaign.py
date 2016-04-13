"""
Ce script permet de créer une Campagne Mail.
"""

import json
import requests
import Utils

def create_campaign(config):
    """
    Création de la campagne

    Pour plus d'informations : http://v8.mailperformance.com/doc/#api-Action-CreateAsync

    Description du fichier createCampaign.json

    type                => Le type de la campagne
    name                => Le nom de la campagne
    description         => La description de la campagne
    informations        :
        folder          => Id du dossier dans lequel vous voulez mettre la campagne. ('null' pour aucun dossier)
        category        => Id de la catégorie de campagne. ('null' pour aucun dossier)
    scheduler           :
        type            => Code au format string pour définir le type d'envoi. (immédiat ou non)
        segments        :
            selected    => Id des segments qui seront utilisés pour la campagne.
    content             :
        headers         :
            from        :
                prefix  => Adresse expéditrice.
                label   => Libellé de l'expéditeur.
            reply       => Adresse vers laquelle sera redirigée la réponse.
        subject         => Objet du Mail.
        html            => Contenu (en HTML) du Mail.
        text            => Contenu (en texte) du Mail.
    """

    with open('Models/createCampaign.json') as data_file:
        data = json.load(data_file)

    data_json = json.JSONEncoder().encode(data)


    # Connexion à l'API
    print("Creation of the Action.")
    url = config['API']['url'] + 'actions'
    headers = Utils.create_headers(config['API']['xKey'], len(data_json))
    req = requests.post(url, data=data_json, headers=headers)
    req.raise_for_status()

    print("The Action has been Created.")

def main():
    config = Utils.load_config()
    create_campaign(config)

if __name__ == '__main__':
    main()
