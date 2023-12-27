import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import PageLayout from '../../layout.js';

const Quete2Page1 = () => {
  const [completed, setCompleted] = useState(false);
  const [textInput, setTextInput] = useState('');

  const handleCompleteQuest = () => {
    // Set the quest as completed
    setCompleted(true);

    // Store in local storage
    localStorage.setItem('quest2Completed', 'true');
  };

  const handleInputChange = (e) => {
    // Update the text input value
    setTextInput(e.target.value);
  };

  return (
    <PageLayout backgroundImage='url("/images/athenes-nuit.jpg")'>
      {(isVisible) => (
        <>
          <div style={{ position: 'absolute', top: '10px', left: '50%', transform: 'translateX(-50%)', textAlign: 'center', zIndex: '1000' }}>
            <input
              type="text"
              value={textInput}
              onChange={handleInputChange}
              placeholder="Numéro de chambre"
            />
          </div>

          <p className={`paragraph-container ${isVisible ? 'visible' : ''}`}>
            Bienvenue à Athènes, ville antique aux mille histoires et légendes !
            Après un voyage épuisant, prenez une pause bien méritée dans une auberge.
            La quête sera terminée lorsque vous aurez trouvé le bon numéro de chambre.
          </p>

          <div className='button-container-left'>
            <Link to="/progression" className='button'>
              Précédent
            </Link>
          </div>
          {(textInput == 23) && (
            <div className='button-container-right'>
              <Link to="/quete2page2" onClick={handleCompleteQuest} className='button'>
                Suivant
              </Link>
            </div>
          )}
        </>
      )}
    </PageLayout>
  );
};

export default Quete2Page1;
