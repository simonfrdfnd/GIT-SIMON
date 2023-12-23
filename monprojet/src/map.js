import 'leaflet/dist/leaflet.css';
// const [showMap, setShowMap] = useState(false);
// const [storyText, setStoryText] = useState('');
//const [showMap, setShowMap] = useState(false);
//setShowMap(true);

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