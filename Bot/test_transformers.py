from transformers import pipeline
import fitz  # PyMuPDF

def extract_text_from_pdf(pdf_file_path):
    doc = fitz.open(pdf_file_path)

    text = ""
    for page_number in range(doc.page_count):
        page = doc[page_number]
        text += page.get_text()

    doc.close()

    return text

if __name__ == "__main__":
    pdf_file_path = "test1.pdf"

    extracted_text = extract_text_from_pdf(pdf_file_path)

    # Chargez le modèle de question-réponse pré-entraîné BERT
    question_answering = pipeline('question-answering', model='bert-base-uncased', tokenizer='bert-base-uncased')

    documentation = extracted_text

    # Posez une question à l'IA
    question = "What is the transfer function of a JIG?"

    # Obtenez la réponse
    answer = question_answering(question=question, context=documentation)

    # Affichez la réponse
    print(f"Question: {question}")
    print(f"Answer: {answer['answer']}")
