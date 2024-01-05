import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import PageLayout from '../../layout.js';

const Quete2Page2 = () => {
  return (
    <PageLayout backgroundImage='url("/images/bonne-nuit.jpg")'>
      {(isVisible) => (
        <>
          <p className={`paragraph-container ${isVisible ? 'visible' : ''}`}>
            Bonne nuit ! Votre protecteur vous a interdit de prendre connaissance de la prochaine quête car vous avez besoin de repos.
            Vous n'oseriez tout de même pas vous laisser aller à votre curiosité ?!
            Vous regardez les étoiles une dernière fois avant de vous endormir. L'une d'elle semble briller d'un éclat particulier... 
          </p>
          <div className='button-container-right'>
            <Link to="/progression" className='button'>
              Terminer
            </Link>
          </div>
        </>
      )}
    </PageLayout>
  );
};

export default Quete2Page2;
