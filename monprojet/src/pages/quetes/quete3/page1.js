import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import PageLayout from '../../layout.js';

const Quete3Page1 = () => {
  return (
    <PageLayout backgroundImage='url("/images/athena-nuit.jpg")'>
      {(isVisible) => (
        <>
          <p className={`paragraph-container ${isVisible ? 'visible' : ''}`}>
            Surgissant des étoiles, la déesse Nyx vous offre un indice en rapport avec l'anneau : "Fils de la déesse de l'amour et du mortel aimé, 
            né du sang cristallisé, je suis douceur incarnée. Consolatrice des âmes blessées, 
            je guéris les maux du corps et de l'esprit. 
            Mon toucher apaise, renforce la confiance en soi, et enseigne l'acceptation de soi-même. 
            Dans mes entrailles, des veines silencieuses ne transportent aucun sang, mais plutôt la sagesse millénaire.
            Sur le cercle de pouvoir je suis enchassé.
            Qui suis-je?"
          </p>

          <div className='button-container-left'>
            <Link to="/progression" className='button'>
              Précédent
            </Link>
          </div>
            <div className='button-container-right'>
              <Link to="/quete2page2" className='button'>
                Suivant
              </Link>
            </div>
        </>
      )}
    </PageLayout>
  );
};

export default Quete3Page1;
