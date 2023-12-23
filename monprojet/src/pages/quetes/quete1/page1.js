import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import PageLayout from '../../layout.js';

const Quete1Page1 = () => {
  const [isVisible, setIsVisible] = useState(false);
  const [userLatitude, setUserLatitude] = useState(null);
  const [userLongitude, setUserLongitude] = useState(null);
  const athensCoordinates = { latitude: 37.9838, longitude: 23.7275 }; // Coordonnées d'Athènes
  const [completed, setCompleted] = useState(false);

  useEffect(() => {
    const timeout = setTimeout(() => {
      setIsVisible(true);
      fetchUserLocation();
    }, 0);

    // Nettoyez le timeout pour éviter des problèmes potentiels
    return () => {
      clearTimeout(timeout);
    };
  }, []);

  const fetchUserLocation = () => {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          setUserLatitude(position.coords.latitude);
          setUserLongitude(position.coords.longitude);
        },
        (error) => {
          console.error('Erreur lors de la récupération de la localisation :', error);
        }
      );
    } else {
      console.error('La géolocalisation n\'est pas prise en charge par votre navigateur.');
    }
  };

  const calculateDistance = () => {
    if (userLatitude !== null && userLongitude !== null) {
      const earthRadius = 6371; // Rayon moyen de la Terre en kilomètres
      const userCoordinates = { latitude: userLatitude, longitude: userLongitude };

      const dLat = deg2rad(athensCoordinates.latitude - userCoordinates.latitude);
      const dLon = deg2rad(athensCoordinates.longitude - userCoordinates.longitude);

      const a =
        Math.sin(dLat / 2) * Math.sin(dLat / 2) +
        Math.cos(deg2rad(userCoordinates.latitude)) * Math.cos(deg2rad(athensCoordinates.latitude)) *
        Math.sin(dLon / 2) * Math.sin(dLon / 2);

      const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));

      const distance = earthRadius * c;
      return distance.toFixed(2) - 1950; // Distance arrondie à 2 chiffres après la virgule
    }
    return 'N/A';
  };

  const deg2rad = (deg) => {
    return deg * (Math.PI / 180);
  };

  const handleCompleteQuest = () => {
    // Set the quest as completed
    setCompleted(true);

    // Store in local storage
    localStorage.setItem('quest1Completed', 'true');
  };

  return (
    <PageLayout backgroundImage='url("/images/airfrance.jpg")'>
      <>
        <div style={{ position: 'absolute', top: '10px', left: '50%', transform: 'translateX(-50%)', textAlign: 'center', zIndex: '1000' }}>
          {userLatitude !== null && userLongitude !== null ? (
            <p className="digital-display">{calculateDistance()} km</p>
          ) : (
            <p className="digital-display">Chargement de la localisation...</p>
          )}
        </div>
        <p className={`paragraph-container ${isVisible ? 'visible' : ''}`}>
          Votre protecteur vous remet des papyrus rédigés de la main de la déesse Athéna. 
          Ils comportent d'étranges symboles. La quête sera terminée lorsque l'afficheur numérique affichera moins de 50 km.
        </p>
        <div className='button-container-left'>
          <Link to="/progression" className='button'>
            Précédent
          </Link>
        </div>
        {(userLatitude !== null && userLongitude !== null && parseFloat(calculateDistance()) < 50) && (
          <div className='button-container-right'>
            <Link to="/quete1page2" onClick={handleCompleteQuest} className='button'>
              Suivant
            </Link>
          </div>
        )}
      </>
    </PageLayout>
  );
};

export default Quete1Page1;
