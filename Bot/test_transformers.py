from transformers import pipeline

if __name__ == "__main__":
    # Chargez le modèle de question-réponse pré-entraîné BERT
    question_answering = pipeline('question-answering', model='bert-base-uncased', tokenizer='bert-base-uncased')

    documentation = "The measured disturbance level is used to control the generator level and to reach the target. The disturbance level must be measured."

    # Posez une question à l'IA
    question = "What is the closed loop method?"

    # Obtenez la réponse
    answer = question_answering(question=question, context=documentation)

    # Affichez la réponse
    print(f"Question: {question}")
    print(f"Answer: {answer['answer']}")
