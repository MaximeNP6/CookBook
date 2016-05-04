"""
Ce script permet de créer un Binding.
"""

import json
import requests
import Utils

def create_binding(config):
    """
    Créer un Binding.
    """
    with open('Models/binding.json') as data_file:
        data_json = json.load(data_file)

    data = json.JSONEncoder().encode(data_json)

    print('Creation of binding.')
    url = config['API']['url'] + 'importFormats/'
    headers = Utils.create_headers(config['API']['xKey'], len(data))
    req = requests.post(url, data=data, headers=headers)
    req.raise_for_status()

    print('Binding has been Created.')

    return json.loads(req.text)

def main():
    config = Utils.load_config()
    create_binding(config)

if __name__ == '__main__':
    main()
