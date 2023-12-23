import { Link } from 'react-router-dom';
import PageLayout from './layout.js';

const HomePage = () => {

  const selectedFaction = localStorage.getItem('selectedFaction');
  const badgeClassName = `badge ${selectedFaction || ''}`;

  return (
    <PageLayout backgroundImage='url("/images/background.webp")'>
      <h1 className='title'>Le voyage de Célia</h1>
      <h2 className='title'>L'anneau divin</h2>
      <Link to="/intro1" className="button">
        Introduction
      </Link>
      {selectedFaction && (
        <div style={{ marginTop: '50px' }}>
          <Link to="/progression" className='button'>Commencer l'aventure</Link>
        </div>
      )}
      <div className={`badge ${badgeClassName}`}></div>
    </PageLayout>
  );
};

export default HomePage;
