"""
Ce script permet de créer une Cible et de l'ajouter à un Segment.
"""

import json
import requests
import Utils

def create_target(config):
    """
    Création d'une cible

    Pour plus d'informations : https://backoffice.mailperformance.com/doc/#api-Target-CreateTarget

    Remplacez les ID des champs avec les valeur corréspondant au ID de vos propres champs
    """

    with open('Models/target.json') as data_file:
        data = json.load(data_file)

    data_json = json.JSONEncoder().encode(data)

    # Remplacez unicity par votre critère d'unicité
    unicity = data['XXXX']
    print("Creation of the target.")
    url = config['API']['url'] + 'targets?unicity=' + unicity
    headers = Utils.create_headers(config['API']['xKey'], len(data_json))
    req = requests.post(url, data=data_json, headers=headers)
    req.raise_for_status()

    print("The target has been Created.")
    return json.loads(req.text)['id']

def add_to_segment(config, target_id, segment_id):
    """Ajout de la cible au segment"""
    print("Target is beeing added to the segment.")
    url = config['API']['url'] + 'targets/' + target_id + '/segments/' + segment_id
    headers = Utils.create_headers(config['API']['xKey'], 0)
    req = requests.post(url, headers=headers)
    req.raise_for_status()

    print("The target has been added.")

def main():
    """
    Pensez à modifier l'id du Segment.
    """
    segment_id = "XXXXXX"
    config = Utils.load_config()
    target_id = create_target(config)
    add_to_segment(config, target_id, segment_id)

if __name__ == '__main__':
    main()
