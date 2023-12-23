import PropTypes from 'prop-types';
import React, { useState, useEffect } from 'react';

const PageLayout = ({ backgroundImage, children }) => {

  const [isVisible, setIsVisible] = useState(false);

  useEffect(() => {
    // Utilisez setTimeout pour déclencher la transition après le rendu initial
    const timeout = setTimeout(() => {
      setIsVisible(true);
    }, 0);

    // Nettoyez le setTimeout pour éviter des problèmes potentiels
    return () => clearTimeout(timeout);
  }, []);

  const pageStyle = {
    backgroundImage: backgroundImage,
    backgroundSize: 'cover',
    backgroundPosition: 'center',
    height: '100vh',
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    justifyContent: 'center',
    color: '#fff',
  };

  return (
    <div style={pageStyle}>
      {typeof children === 'function' ? children(isVisible) : children}
    </div>
  );
};

PageLayout.propTypes = {
  backgroundImage: PropTypes.string.isRequired,
  children: PropTypes.oneOfType([PropTypes.node, PropTypes.func]).isRequired,
};

export default PageLayout;
