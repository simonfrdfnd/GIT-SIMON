import sqlite3
from datetime import datetime

# Se connecter à la base de données SQLite
conn = sqlite3.connect('incidents.db')
cursor = conn.cursor()

# Exécuter une requête pour récupérer toutes les lignes de la table incidents
cursor.execute('SELECT * FROM incidents')
rows = cursor.fetchall()

# Afficher les données
print(str(len(rows)) + " incidents")

# for row in rows[-10:]:
#     print(row)
#     print()

# Fermer la connexion à la base de données
conn.close()