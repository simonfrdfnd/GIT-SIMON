import { Link } from 'react-router-dom';
import PageLayout from '../layout.js';

const IntroPage1 = () => {
  return (
    <PageLayout backgroundImage='url("/images/intro1.jpg")'>
      {(isVisible) => (
        <>
          <p className={`paragraph-container ${isVisible ? 'visible' : ''}`}>
            Dans l'ombre des cieux olympiens, une guerre menée par Zeus contre les Titans laissa le roi des dieux affaibli,
            ses cicatrices témoignant de sa puissance passée. Sentant sa vulnérabilité, Zeus pressentit une menace grandissante parmi les dieux.
            Les murmures du pouvoir suscitèrent des inquiétudes croissantes.
            Sage et prévoyant, Zeus décida de prévenir toute rébellion imminente.
          </p>
          <div className='button-container-left'>
            <Link to="/" className='button'>
              Précédent
            </Link>
          </div>
          <div className='button-container-right'>
            <Link to="/intro2" className='button'>
              Suivant
            </Link>
          </div>
        </>
      )}
    </PageLayout>
  );
};

export default IntroPage1;
