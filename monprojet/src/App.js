import React, { useState, useEffect } from 'react';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import './App.css'; // N'oubliez pas d'importer vos styles CSS ici
import { icon } from "leaflet"

function Map() {
  const [showPanel, setShowPanel] = useState(false);
  const [quests, setQuests] = useState([]);

  useEffect(() => {
    const center = [37.983810, 23.727539];
    const mapSettings = {
      center,
      zoom: 11,
    };

    const map = L.map('map', mapSettings);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors',
    }).addTo(map);

    const controlButton = L.control({ position: 'topright' });

    controlButton.onAdd = function () {
      const div = L.DomUtil.create('div', 'leaflet-control leaflet-bar');
      div.innerHTML = '<button onclick="showPanel()">Mes quêtes</button>';
      return div;
    };

    controlButton.addTo(map);

    window.showPanel = function () {
      setShowPanel(!showPanel);
    };
    
    // Exemple de quête (ajoutez autant d'éléments qu'il y a de quêtes)
    const questExample = {
      id: 1,
      title: 'A',
      description: 'A',
      location: [37.983810, 23.727539], // Coordonnées du point de départ
    };

    // Utilisez setQuests pour mettre à jour la liste des quêtes
    setQuests([questExample]);

    // Ajout du marqueur pour chaque quête
    quests.forEach(quest => {
      const marker = L.circleMarker(quest.location, {
        radius: 8, // ajustez la taille du cercle selon vos besoins
        fillColor: 'green',
        color: 'green',
        weight: 1,
        opacity: 1,
        fillOpacity: 0.8,
      }).addTo(map);
      marker.bindPopup(`<b>${quest.title}</b><br>${quest.description}`).openPopup();
      
    });

    return () => {
      map.remove();
    };
  }, [showPanel]);

  return (
    <div id="map" style={{ height: '100vh', width: '100%', position: 'absolute', top: 0, left: 0 }}>
      {showPanel && (
        <div
          style={{
            position: 'absolute',
            top: 0,
            right: 0,
            width: '200px',
            height: '100%',
            backgroundColor: 'rgba(0, 0, 0, 0.7)',
            color: '#fff',
            padding: '20px',
            boxSizing: 'border-box',
            zIndex: 1000,
            overflowY: 'scroll', // Ajout de défilement si le contenu est trop long
          }}
        >
          <h2>Quêtes</h2>
          <ul>
            {quests.map(quest => (
              <li key={quest.id}>
                <strong>{quest.title}</strong>
                <p>{quest.description}</p>
                <p>Point de départ: {quest.location.join(', ')}</p>
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}

function App() {
  const [showMap, setShowMap] = useState(false);

  const backgroundStyle = {
    backgroundImage: 'url("/images/background.webp")',
    backgroundSize: 'cover',
    backgroundPosition: 'center',
    height: '100vh',
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    justifyContent: 'center',
    color: '#fff',
    textAlign: 'center',
  };

  const headerStyle = {
    fontFamily: 'Georgia, serif',
    textShadow: '2px 2px 4px #000',
    padding: '20px',
  };

  const startButtonStyle = {
    backgroundColor: '#8B4513',
    color: 'white',
    padding: '15px 30px',
    fontSize: '1.2em',
    textDecoration: 'none',
    borderRadius: '8px',
    cursor: 'pointer',
    border: '2px solid #CD853F',
    fontFamily: 'Palatino, "Palatino Linotype", "Palatino LT STD", "Book Antiqua", Georgia, serif',
  };

  const handleStartButtonClick = () => {
    setShowMap(true);
  };

  return (
    <div style={backgroundStyle}>
      {showMap ? (
        <Map />
      ) : (
        <header style={headerStyle}>
          <h1 style={{ fontSize: '2.5em' }}>Aventure Mythologique</h1>
          <p style={{ fontSize: '1.3em' }}>
            Bienvenue dans une aventure légendaire, Celia ! Prépare-toi à plonger dans un monde mythologique rempli de mystères et de défis. Ton voyage commence ici.
          </p>
          <p style={{ fontSize: '1.3em' }}>
            Es-tu prête à affronter les dieux et les monstres pour découvrir le secret qui t'attend?
          </p>
          <a style={startButtonStyle} href="#" onClick={handleStartButtonClick}>
            Commencer l'aventure
          </a>
        </header>
      )}
    </div>
  );
}

export default App;
