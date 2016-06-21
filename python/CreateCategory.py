"""
Ce script permet de créer une Catégorie.
"""

import json
import requests
import Utils

def create_category(config):
    """
    Création d'une Catégorie

    Pour plus d'informations : https://backoffice.mailperformance.com/doc/#api-Category-CreateCategory

    Description du fichier createCategory.json

    name                => Le nom de la catégorie
    description         => La description de la catégorie

    """

    with open('Models/createCategory.json') as data_file:
        data = json.load(data_file)

    data_json = json.JSONEncoder().encode(data)

    print("Creation of the category.")
    url = config['API']['url'] + 'categories/'
    headers = Utils.create_headers(config['API']['xKey'], len(data_json))
    req = requests.post(url, data=data_json, headers=headers)
    req.raise_for_status()

    print("The category has been Created.")

def main():
    config = Utils.load_config()
    create_category(config)

if __name__ == '__main__':
    main()
