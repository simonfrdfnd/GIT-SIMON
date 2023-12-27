import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import './App.css';
import './styles.css';
import HomePage from './pages/home';
import IntroPage1 from './pages/intro/intro1';
import IntroPage2 from './pages/intro/intro2';
import IntroPage3 from './pages/intro/intro3';
import IntroPage4 from './pages/intro/intro4';
import IntroPage5 from './pages/intro/intro5';
import Faction from './pages/intro/faction';
import Progression from './pages/progression';
import Quete1Page1 from './pages/quetes/quete1/page1';
import Quete1Page2 from './pages/quetes/quete1/page2';
import Quete2Page1 from './pages/quetes/quete2/page1';
import Quete2Page2 from './pages/quetes/quete2/page2';
import Quete3Page1 from './pages/quetes/quete3/page1';

const App = () => {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/intro1" element={<IntroPage1 />} />
        <Route path="/intro2" element={<IntroPage2 />} />
        <Route path="/intro3" element={<IntroPage3/>} />
        <Route path="/intro4" element={<IntroPage4/>} />
        <Route path="/intro5" element={<IntroPage5/>} />
        <Route path="/faction" element={<Faction/>} />
        <Route path="/progression" element={<Progression/>} />
        <Route path="/quete1page1" element={<Quete1Page1/>} />
        <Route path="/quete1page2" element={<Quete1Page2/>} />
        <Route path="/quete2page1" element={<Quete2Page1/>} />
        <Route path="/quete2page2" element={<Quete2Page2/>} />
        <Route path="/quete3page1" element={<Quete3Page1/>} />
      </Routes>
    </Router>
  );
};

export default App;
