import React from 'react';
import PhaseManager from '../components/PhaseManager';

const Dashboard = () => {
  const styles = {
    container: {
      backgroundColor: '#fff',
      maxWidth: '800px',
      textAlign: 'center',
    },
    heading: {
      fontSize: '2em',
      marginBottom: '20px',
    },
  };

  return (
    <div style={styles.container}>
      <PhaseManager />
    </div>
  );
};

export default Dashboard;