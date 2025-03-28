import React from 'react';
import { LoanProvider } from '../context/LoanContext';
import PhaseManager from '../components/PhaseManager';

const Dashboard = () => {
  console.log("🚀 Dashboard Mounted");

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
      <LoanProvider>
        <PhaseManager />
      </LoanProvider>
    </div>
  );
};

export default Dashboard;