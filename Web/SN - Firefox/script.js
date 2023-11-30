// Fonction pour copier le contenu dans le presse-papiers
function copyToClipboard(event) {
  const contentToCopy = event.target.previousSibling.dataset.content;
  navigator.clipboard.writeText(contentToCopy);
  console.log('Content copied to clipboard');
}

function addContainer(event) {
  const containers = document.getElementById("containers");
  const newContainer = document.createElement("div");
  newContainer.className = "container";

  const newTitle = document.createElement("div");
  newTitle.className = "title";
  newTitle.contentEditable = true;
  newTitle.textContent = "Nouveau Titre";
  newTitle.dataset.title = "Nouveau Titre";

  const newContent = document.createElement("div");
  newContent.className = "content";
  newContent.contentEditable = true;
  newContent.textContent = "Nouveau Contenu";
  newContent.dataset.content = "Nouveau Contenu";

  const copyButton = document.createElement("button");
  copyButton.className = "copyBtn";
  copyButton.textContent = "Copier";
  copyButton.addEventListener("click", copyToClipboard);

  const deleteButton = document.createElement("button");
  deleteButton.className = "deleteBtn";
  deleteButton.textContent = "Supprimer";
  deleteButton.addEventListener("click", function () {
    containers.removeChild(newContainer);
    updateLocalStorage();
  });

  newContainer.appendChild(newTitle);
  newContainer.appendChild(newContent);
  newContainer.appendChild(copyButton);
  newContainer.appendChild(deleteButton);
  containers.appendChild(newContainer);

  // Focus sur le titre éditable du nouveau conteneur
  newTitle.focus();

  // Sauvegarde des données dans les attributs data-*
  saveContainer(newTitle, newContent);

  console.log('Container added');
}

// Sauvegarde les données du conteneur dans les attributs data-*
function saveContainer(titleElement, contentElement) {
  titleElement.dataset.title = titleElement.textContent;
  contentElement.dataset.content = contentElement.textContent;
  // Mettre à jour le stockage local ici si nécessaire
  updateLocalStorage();
}

// Mettre à jour le stockage local
function updateLocalStorage() {
  const containers = document.getElementById("containers");
  const containerData = {};

  // Parcourez tous les conteneurs et enregistrez leurs données
  containers.querySelectorAll(".container").forEach((container, index) => {
    const title = container.querySelector(".title").dataset.title;
    const content = container.querySelector(".content").dataset.content;
    containerData[index] = { title, content };
  });

  // Stockez les données dans le stockage local
  localStorage.setItem("containerData", JSON.stringify(containerData));
}

// Charge les conteneurs sauvegardés depuis le stockage local
function loadSavedContainers() {
  const containers = document.getElementById("containers");
  containers.innerHTML = "";

  const containerData = JSON.parse(localStorage.getItem("containerData"));
  if (containerData) {
    for (const index in containerData) {
      const newContainer = document.createElement("div");
      newContainer.className = "container";

      const newTitle = document.createElement("div");
      newTitle.className = "title";
      newTitle.contentEditable = true;
      newTitle.textContent = containerData[index].title;
      newTitle.dataset.title = containerData[index].title;

      const newContent = document.createElement("div");
      newContent.className = "content";
      newContent.contentEditable = true;
      newContent.textContent = containerData[index].content;
      newContent.dataset.content = containerData[index].content;

      const copyButton = document.createElement("button");
      copyButton.className = "copyBtn";
      copyButton.textContent = "Copier";
      copyButton.addEventListener("click", copyToClipboard);

      const deleteButton = document.createElement("button");
      deleteButton.className = "deleteBtn";
      deleteButton.textContent = "Supprimer";
      deleteButton.addEventListener("click", function () {
        containers.removeChild(newContainer);
        updateLocalStorage();
      });

      newContainer.appendChild(newTitle);
      newContainer.appendChild(newContent);
      newContainer.appendChild(copyButton);
      newContainer.appendChild(deleteButton);
      containers.appendChild(newContainer);
    }
  }
}

// Ajouter des gestionnaires d'événements pour sauvegarder lors de la modification du titre ou du contenu
document.getElementById("containers").addEventListener("input", function (event) {
  if (event.target.classList.contains("title") || event.target.classList.contains("content")) {
    saveContainer(event.target, event.target);
  }
});

// Ajouter un gestionnaire d'événement pour activer l'édition du titre ou du contenu
document.getElementById("containers").addEventListener("click", function (event) {
  if (event.target.classList.contains("title") || event.target.classList.contains("content")) {
    event.target.contentEditable = true;
    event.target.focus();
  }
});

// Ajouter un gestionnaire d'événement pour activer l'édition du titre ou du contenu
document.getElementById("addContainerBtn").addEventListener("click", addContainer);

// Fonction pour exporter le contenu des conteneurs en tant que fichier texte
function exportContainers() {
  const containerData = JSON.parse(localStorage.getItem("containerData"));
  if (containerData) {
    let exportContent = "";

    for (const index in containerData) {
      const title = containerData[index].title;
      const content = containerData[index].content;
      exportContent += `Title: ${title}\nContent: ${content}\n\n`;
    }

    // Créer un objet Blob avec le contenu à exporter
    const blob = new Blob([exportContent], { type: "text/plain;charset=utf-8" });

    // Créer un objet URL à partir du Blob
    const url = URL.createObjectURL(blob);

    // Créer un élément 'a' pour le téléchargement
    const a = document.createElement("a");
    a.href = url;
    a.download = "exported_containers.txt";

    // Ajouter l'élément 'a' à la page et déclencher le téléchargement
    document.body.appendChild(a);
    a.click();

    // Nettoyer l'URL de l'objet Blob après le téléchargement
    URL.revokeObjectURL(url);
  }
}

// Ajouter un gestionnaire d'événements pour le bouton "Export"
document.getElementById("exportBtn").addEventListener("click", exportContainers);


// Fonction pour importer le contenu depuis un fichier texte
function importContainers(event) {
  const fileInput = event.target;
  const file = fileInput.files[0];

  if (file) {
    const reader = new FileReader();

    reader.onload = function (e) {
      const importedContent = e.target.result;

      // Vous pouvez traiter le contenu importé ici selon vos besoins
      // Dans cet exemple, nous affichons simplement le contenu dans la console
      console.log("Imported Content:\n", importedContent);

      // Vous devez mettre à jour le stockage local et recharger les conteneurs ici
      // En supposant que le contenu du fichier texte est au format JSON
      try {
        const importedData = JSON.parse(importedContent);

        // Mettez à jour le stockage local avec les données importées
        localStorage.setItem("containerData", JSON.stringify(importedData));

        // Rechargez les conteneurs avec les données importées
        loadSavedContainers();

        console.log("Containers imported successfully.");
      } catch (error) {
        console.error("Error importing containers:", error);
      }
    };

    reader.readAsText(file);
  }
}

// Ajouter un gestionnaire d'événements pour le bouton "Import"
document.getElementById("importBtn").addEventListener("click", function () {
  // Cliquez sur le véritable input de type fichier pour déclencher la boîte de dialogue du fichier
  document.getElementById("importInput").click();
});

// Ajouter un gestionnaire d'événements pour le changement de fichier
document.getElementById("importInput").addEventListener("change", importContainers);
// Charger les conteneurs sauvegardés lors du chargement de l'extension
loadSavedContainers();
