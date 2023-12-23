import { Link } from 'react-router-dom';
import PageLayout from '../layout.js';

const IntroPage3 = () => {
  return (
    <PageLayout backgroundImage='url("/images/intro3.jpg")'>
      {(isVisible) => (
        <>
          <p className={`paragraph-container ${isVisible ? 'visible' : ''}`}>
            Finalement, Zeus confia l'anneau à Poséidon, dieu des océans et frère loyal. La confiance placée en lui scellait une alliance solide pour préserver l'Olympe. L'Éclat Céleste brillait désormais aux côtés du trident de Poséidon, une union sacrée destinée à maintenir l'équilibre céleste.
          </p>
          <div className='button-container-left'>
            <Link to="/intro2" className='button'>
              Précédent
            </Link>
          </div>
          <div className='button-container-right'>
            <Link to="/intro4" className='button'>
              Suivant
            </Link>
          </div>
        </>
      )}
    </PageLayout>
  );
};

export default IntroPage3;
