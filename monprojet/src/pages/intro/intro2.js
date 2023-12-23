import { Link } from 'react-router-dom';
import PageLayout from '../layout.js';

const IntroPage2 = () => {
  return (
    <PageLayout backgroundImage='url("/images/intro2.jpg")'>
      {(isVisible) => (
        <>
          <p className={`paragraph-container ${isVisible ? 'visible' : ''}`}>
            Conscient que seul lui pouvait maîtriser la puissante foudre, il chargea Hephaïstos, le forgeron divin, de créer un artefact exceptionnel : l'Éclat Céleste.
            Un anneau forgé du souffle des vents célestes, destiné à transmettre le pouvoir de la foudre à un dieu digne de confiance.
            Hephaïstos façonna l'Éclat Céleste avec soin, imprégnant l'anneau du pouvoir de la foudre.
            Ainsi naquit un rempart divin contre toute tentative de rébellion.
          </p>
          <div className='button-container-left'>
            <Link to="/intro1" className='button'>
              Précédent
            </Link>
          </div>
          <div className='button-container-right'>
            <Link to="/intro3" className='button'>
              Suivant
            </Link>
          </div>
        </>
      )}
    </PageLayout>
  );
};

export default IntroPage2;
