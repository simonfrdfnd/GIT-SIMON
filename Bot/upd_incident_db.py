import requests
import sqlite3
from datetime import datetime, timedelta

# Remplacez ces valeurs par les informations de votre instance ServiceNow
instance_url = "https://nexioprod.service-now.com"
username = "simon.froidefond@nexiogroup.com"
password = "Nexio171296SF"

# Connectez-vous à la base de données SQLite (cela créera la base de données s'il n'existe pas)
conn = sqlite3.connect('incidents.db')
cursor = conn.cursor()

# Créez la table s'il n'existe pas
cursor.execute('''
    CREATE TABLE IF NOT EXISTS incidents (
        incident_number TEXT PRIMARY KEY,
        short_description TEXT,
        description TEXT,
        resolve_notes TEXT,
        comments_and_work_notes TEXT,
        sys_updated_on DATE
    )
''')

# Exécuter une requête pour récupérer les dates
cursor.execute('SELECT sys_updated_on FROM incidents')
sys_updated_on = cursor.fetchall()
dates = [datetime.strptime(date[0], '%d/%m/%Y %H:%M:%S') for date in sys_updated_on]
date_mise_a_jour = max(dates)
date_formatee = date_mise_a_jour.strftime('%d/%m/%Y %H:%M:%S')
print("Dernière mise à jour :", date_formatee)

def get_incidents():
    # Construct the API URL with pagination parameters
    api_url = f"{instance_url}/api/now/v2/table/incident"
    params = {
    'sysparm_query': f'sys_updated_on>javascript:gs.dateGenerate("{date_mise_a_jour}")',
    'ORDERBY': 'number',  # Ajout du tri par numéro
    'sysparm_display_value': 'true',
    }
    
    # Add authentication information to the request header
    headers = {
        "Content-Type": "application/json",
        "Accept": "application/json",
    }
    auth = (username, password)

    # Make the GET request
    response = requests.get(api_url, params=params, headers=headers, auth=auth)

    # Check the response
    if response.status_code == 200:
        # The request was successful, retrieve the data
        incident_data = response.json().get("result")
        incidents_list = []

        if incident_data:
            # Extract and append specific fields for each incident
            for incident in incident_data:
                incident_number = incident.get("number")
                short_description = incident.get("short_description")
                description = incident.get("description")
                resolve_notes = incident.get("resolve_notes")
                comments_and_work_notes = incident.get("comments_and_work_notes")
                sys_updated_on = incident.get("sys_updated_on")
                incidents_list.append([incident_number, short_description, description, resolve_notes, comments_and_work_notes, sys_updated_on])

            return incidents_list
        else:
            print("No incidents found.")
    else:
        # The request failed, display the error code
        print(f"Request failed with error code {response.status_code}: {response.text}")

    return None


incidents = get_incidents()

if incidents:
    # Insert the batch of incidents into the database
    for incident in incidents:
        cursor.execute('''
            INSERT OR REPLACE INTO incidents VALUES (?, ?, ?, ?, ?, ?)
        ''', incident)

    # Commit changes to the database
    conn.commit()
    print(f"Inserted {len(incidents)} incidents.")

# Close the database connection
conn.close()

print("Data successfully stored in the SQLite database.")
