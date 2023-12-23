import { Link } from 'react-router-dom';
import PageLayout from '../layout.js';

const IntroPage4 = () => {
  return (
    <PageLayout backgroundImage='url("/images/intro4.jpg")'>
      {(isVisible) => (
        <>
          <p className={`paragraph-container ${isVisible ? 'visible' : ''}`}>
            Cependant, Hadès, le sombre leader de la rébellion, élabora un plan machiavélique. Il envoya Perséphone pour séduire et enivrer Poséidon, cherchant ainsi à l'affaiblir pour lui arracher l'anneau. Comprenant leur machination, Poséidon, préférant ne pas laisser l'anneau à Hadès, le laissa tomber délibérément dans le monde des humains.
          </p>
          <div className='button-container-left'>
            <Link to="/intro3" className='button'>
              Précédent
            </Link>
          </div>
          <div className='button-container-right'>
            <Link to="/intro5" className='button'>
              Suivant
            </Link>
          </div>
        </>
      )}
    </PageLayout>
  );
};

export default IntroPage4;
