import { Link } from 'react-router-dom';
import PageLayout from '../layout.js';

const IntroPage5 = () => {
  return (
    <PageLayout backgroundImage='url("/images/intro5.jpg")'>
      {(isVisible) => (
        <>
          <p className={`paragraph-container ${isVisible ? 'visible' : ''}`}>
          Les dieux dépêchèrent rapidement les héros de la mythologie vers le lieu de la chute afin de récupérer l'anneau perdu. 
          Une course effrénée s'engagea alors, chaque faction aspirant au pouvoir décisif qui pourrait soit préserver, soit anéantir le royaume céleste. 
          Célia, fille d'Athéna, ta quête commence dès à présent. 
          L'anneau n'a pas encore été trouvé, et il est encore temps pour toi d'obtenir son pouvoir.
          </p>
          <div className='button-container-left'>
            <Link to="/intro4" className='button'>
              Précédent
            </Link>
          </div>
          <div className='button-container-right'>
            <Link to="/faction" className='button'>
              Suivant
            </Link>
          </div>
        </>
      )}
    </PageLayout>
  );
};

export default IntroPage5;
