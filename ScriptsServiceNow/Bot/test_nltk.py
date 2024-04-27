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

preprocessed_tickets = [preprocess_text(short_description) for short_description in existing_tickets]

# Nouveau ticket (à remplacer par la saisie de l'utilisateur)
new_ticket_short_description = "le parametre du measurement time n'est pas envoyé correctement à l'ESW8 (EMI test receiver)"
new_ticket_description = "Suite à la derniere mise à jour de BAT, il y a un nouveau probleme avec le driver de l'ESW8 qui est apparu. Le mesurement time est systematiquement réglé à 1ms sur l'EMI test receiver quelque soit la valeur indiquée dans BAT. Le probleme apparait quelque soit le mode de l'EMI (time domain ou linear scan). En configurant l ESW avec le driver de l ESR, on s apercoit que le probleme est résolu. Mais c etait juste pour identifier que le probleme vient bien du driver ESW.dll Pouvez-vous rapidement fixer ce probleme ?"

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

end_time = time.time()
print(f"Time taken to analyzer the incidents: {end_time - start_time} seconds")

# Nombre de top tickets similaires à afficher
top_x = 20

# Affichage des top X similarités
print(f"Top {top_x} similarités avec les tickets existants:")
for i in range(min(top_x, len(sorted_similarities))):
    index, similarity = sorted_similarities[i]
    print(f"Similarity with existing ticket {rows[index][0]}: {similarity}")

