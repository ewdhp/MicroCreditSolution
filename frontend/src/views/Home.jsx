import React from 'react';
import TakeLoan from '../components/TakeLoan';
import backgroundImage from '../assets/land.png'; // Ensure you have a background image in the assets folder

const Home = () => {
  const styles = {
    container: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      margin: '20px',
      height: 'calc(100vh - 100px)', 
      padding: '10px',
      marginTop: '100px',
    },

  };

  return (
    <div style={styles.container}>

        <TakeLoan />
    </div>
  );
};

export default Home;