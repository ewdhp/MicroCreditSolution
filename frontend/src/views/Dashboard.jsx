import React from 'react';
import PhaseManager from '../components/PhaseManager';

const Dashboard = () => {
  const styles = {
    container: {
      backgroundColor: '#fff',
      borderRadius: '8px',

      boxShadow: '0 0 10px rgba(0, 0, 0, 0.1)',
      maxWidth: '800px',
      margin: '20px auto',
      textAlign: 'center',
    },
    heading: {
      fontSize: '2em',
      marginBottom: '20px',
    },
  };

  return (
    <div style={styles.container}>
      {/* PhaseManager is integrated here */}
      <PhaseManager />
    </div>
  );
};

export default Dashboard;