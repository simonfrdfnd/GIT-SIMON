import { Link } from 'react-router-dom';
import PageLayout from '../layout.js';
import React, { useState } from 'react';

const Faction = () => {
  const [selectedFaction, setSelectedFaction] = useState(null);
  const handleFactionSelect = (faction) => {
    setSelectedFaction(faction);
    localStorage.setItem('selectedFaction', faction);
  };

  return (
    <PageLayout backgroundImage='url("/images/faction.jpg")'>
      <h2 className='title' style={{ textAlign: 'center', marginBottom: '20px' }}>
        {selectedFaction
          ? `Vous avez choisi la faction ${selectedFaction}`
          : 'Choisissez une faction'}
      </h2>
      <div className='button-container-left'>
        <Link
          to="/"
          className='button'
          onClick={() => handleFactionSelect('Poseidon')}
        >
          Poséidon
        </Link>
      </div>
      <div className='button-container-right'>
        <Link
          to="/"
          className='button'
          onClick={() => handleFactionSelect('Hades')}
        >
          Hadès
        </Link>
      </div>
    </PageLayout>
  );
};

export default Faction;
