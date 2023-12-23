import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import './progression.css';

const Progression = () => {
  const QuestInventory = ({ quests, activeQuestId, onQuestButtonClick }) => {
    return (
      <div
        style={{
          backgroundImage: 'url("/images/quetes.jpg")',
          backgroundSize: 'cover',
          backgroundPosition: 'center',
          minHeight: '100vh',
        }}
      >
        <div className="quest-grid">
          {quests.map((quest, index) => (
            <Link
              key={quest.id}
              to={index === 0 ? '/quete1page1' : '/'}
              className="quest-link"  // Ajoutez une classe pour le style si nécessaire
              onClick={() => onQuestButtonClick(quest.id)}
              style={{ display: index === 0 || quests[index - 1]?.completed ? 'inline-block' : 'none' }}
            >
              <button
                disabled={activeQuestId !== quest.id}
                className="quest-button"
              >
                {quest.title}
              </button>
            </Link>
          ))}
        </div>
        <div className="button-container-left">
          <Link to="/" className="button">
            Retour
          </Link>
        </div>
      </div>
    );
  };

  const quests = [
    { id: 1, title: 'Quête 1', description: 'Description de la quête 1', completed: localStorage.getItem('quest1Completed') === 'true' },
    { id: 2, title: 'Quête 2', description: 'Description de la quête 2', completed: localStorage.getItem('quest2Completed') === 'true' },
    { id: 3, title: 'Quête 3', description: 'Description de la quête 3', completed: localStorage.getItem('quest3Completed') === 'true' },
    // ... Ajoutez d'autres quêtes ...
  ];

  const [activeQuestId, setActiveQuestId] = useState(1);

  const handleQuestButtonClick = (questId) => {
    setActiveQuestId(questId);
  };

  useEffect(() => {
    // Check local storage for completed quests and set the active quest accordingly
    const completedQuests = quests.filter((quest) => quest.completed);
    if (completedQuests.length > 0) {
      const lastCompletedQuest = completedQuests[completedQuests.length - 1];
      setActiveQuestId(lastCompletedQuest.id + 1);
    }
  }, [quests]);

  return (
    <div className="progression-container">
      <QuestInventory
        quests={quests}
        activeQuestId={activeQuestId}
        onQuestButtonClick={handleQuestButtonClick}
      />
    </div>
  );
};

export default Progression;
