"""
Ce script permet de créer un Segment.
"""

import json
import requests
import Utils

def create_segment(config):
    """
    Création du Segment

    Pour plus d'informations : https://backoffice.mailperformance.com/doc/#api-Segment-CreateSegmentAsync

    Description du fichier createSegment.json

    type                => Le type de segment
    name                => Le nom du segment
    description         => La description du segment
    expiration          => La date d'éxpiration du segment
    isTest              => Le segment est un segment de test ou non
    """

    with open('Models/createSegment.json') as data_file:
        data = json.load(data_file)

    data_json = json.JSONEncoder().encode(data)

    print('Creation of the segment.')
    url = config['API']['url'] + 'V1/segments/'
    headers = Utils.create_headers(config['API']['xKey'], len(data_json))
    req = requests.post(url, data=data_json, headers=headers)
    req.raise_for_status()

    print('The segment has been Created.')
    return json.loads(req.text)

def main():
    config = Utils.load_config()
    create_segment(config)

if __name__ == '__main__':
    main()
