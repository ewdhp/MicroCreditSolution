import React from 'react';
import { LoanProvider } from '../context/LoanContext';
import PhaseManager from '../components/PhaseManager';

const Dashboard = () => {
  console.log("🚀 Dashboard Mounted");

  const styles = {
    container: {
      margin: '10px auto',
      textAlign: 'center',
    },
    heading: {
      fontSize: '2em',

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