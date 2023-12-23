import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import PageLayout from '../../layout.js';

const Quete1Page2 = () => {
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
    <PageLayout backgroundImage='url("/images/hublot.jpg")'>
      <>
        <p className={`paragraph-container ${isVisible ? 'visible' : ''}`}>
          Vous apercevez le Mont Olympe, depuis le hublot. L'anneau étant tombé depuis la demeure des dieux, il y a fort à parier pour qu'il se trouve quelque part en Grèce.
        </p>
        {(userLatitude !== null && userLongitude !== null && parseFloat(calculateDistance()) < 50) && (
          <div className='button-container-right'>
            <Link to="/progression" onClick={handleCompleteQuest} className='button'>
              Terminer
            </Link>
          </div>
        )}
      </>
    </PageLayout>
  );
};

export default Quete1Page2;
