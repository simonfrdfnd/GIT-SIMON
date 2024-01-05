import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import PageLayout from '../../layout.js';

const Quete3Page1 = () => {
  const [firstParagraphVisible, setFirstParagraphVisible] = useState(false);

  const handleButtonClick = () => {
    setFirstParagraphVisible(!firstParagraphVisible);
    console.log(firstParagraphVisible);
  };

  return (
    <PageLayout backgroundImage='url("/images/athena-nuit.jpg")'>
      {(isVisible) => (
        <>
          <button className='button-hidden' onClick={handleButtonClick}>Afficher le premier paragraphe</button>

          <p className={`paragraph-container ${firstParagraphVisible ? 'visible' : 'hidden'}`}>
            "
            Sur le cercle de pouvoir je suis enchassé, sinon nul héros ne saurait le porter.
            Comme l'étreinte délicate d'Aphrodite, je porte en moi l'éclat de l'amour.
            Consolatrice des âmes blessées, je guéris les maux de l'esprit.
            Mon toucher apaise, enseigne l'acceptation de soi-même et d'autrui.
            Dans mes entrailles, mes veines ne transportent aucun sang, mais plutôt la sagesse millénaire.
            A la manière de la fille d'Athéna, on dit de moi que je suis solaire.
            "
          </p>

          <p className={`paragraph-container ${firstParagraphVisible ? 'hidden' : 'visible'}`}>
            La déesse Nyx a dissimulé un indice dans les étoiles. Qui sait, il pourrait vous servir plus tard.
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
