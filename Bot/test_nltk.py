import sqlite3
import nltk
from nltk.tokenize import word_tokenize
from nltk.corpus import stopwords
from nltk.stem import PorterStemmer
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.metrics.pairwise import cosine_similarity
import time

# nltk.download('punkt')
# nltk.download('stopwords')


start_time = time.time()

# Se connecter à la base de données SQLite
conn = sqlite3.connect('incidents.db')
cursor = conn.cursor()

# Récupérer toutes les lignes de la table incidents
cursor.execute('SELECT * FROM incidents')
rows = cursor.fetchall()

# Fermer la connexion à la base de données
conn.close()

end_time = time.time()
print(f"Time taken to fetch incidents from the database: {end_time - start_time} seconds")

start_time = time.time()

# Extraire les descriptions de problème de chaque ticket depuis la base de données
existing_tickets = [row[1] for row in rows]  # Utiliser à la fois la short description et la description complète

# Prétraitement des données existantes
def preprocess_text(short_description):
    text = f"{short_description}".lower()  # Combinaison de la short description et de la description
    words = word_tokenize(text)
    stop_words = set(stopwords.words('english'))
    words = [word for word in words if word not in stop_words]
    stemmer = PorterStemmer()
    words = [stemmer.stem(word) for word in words]
    return ' '.join(words)

preprocessed_tickets = [preprocess_text(ticket) for ticket in existing_tickets]

# Nouveau ticket (à remplacer par la saisie de l'utilisateur)
new_ticket_short_description = "ESW mode for scan"

# Prétraitement du nouveau ticket
preprocessed_new_ticket = preprocess_text(new_ticket_short_description)

# Utilisation de TF-IDF pour la représentation vectorielle des tickets
vectorizer = TfidfVectorizer()
tfidf_matrix = vectorizer.fit_transform(preprocessed_tickets + [preprocessed_new_ticket])

# Calcul de la similarité cosinus entre le nouveau ticket et les tickets existants
cosine_similarities = cosine_similarity(tfidf_matrix[-1], tfidf_matrix[:-1]).flatten()

# Affichage des similarités
similarities_with_indices = [(i, similarity) for i, similarity in enumerate(cosine_similarities)]
sorted_similarities = sorted(similarities_with_indices, key=lambda x: x[1], reverse=True)

# Nombre de top tickets similaires à afficher
top_x = 20

# Affichage des top X similarités
print(f"Top {top_x} similarités avec les tickets existants:")
for i in range(min(top_x, len(sorted_similarities))):
    index, similarity = sorted_similarities[i]
    print(f"Similarity with existing ticket {rows[index][0]}: {similarity}")
