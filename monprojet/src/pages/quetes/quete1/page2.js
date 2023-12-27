import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import PageLayout from '../../layout.js';

const Quete1Page2 = () => {
  return (
    <PageLayout backgroundImage='url("/images/hublot.jpg")'>
      {(isVisible) => (
        <>
          <p className={`paragraph-container ${isVisible ? 'visible' : ''}`}>
            Vous apercevez le Mont Olympe, depuis le hublot. L'anneau étant tombé depuis la demeure des dieux, il y a fort à parier pour qu'il se trouve quelque part en Grèce.
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

export default Quete1Page2;
