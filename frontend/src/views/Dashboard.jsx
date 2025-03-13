import React, { useState, useEffect } from 'react';
import axios from 'axios';

const Dashboard = () => {
  const [currentPhase, setCurrentPhase] = useState('Initial');
  const [token, setToken] = useState(localStorage.getItem('token'));

  useEffect(() => {
    if (token) {
      callNextPhase(currentPhase);
    }
  }, [token]);

  const callNextPhase = async (status) => {
    try {
      const response = await axios.post(
        'https://localhost:5001/api/phases/next-phase',
        {
          Status: status,
          Amount: 100,
          EndDate: new Date().toISOString(), // Ensure UTC format
        },
        {
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
        }
      );
      if (response.status === 200) {
        const newPhase = response.data.res.status;
        setCurrentPhase(newPhase);
        console.log('Next phase:', newPhase);
      } else {
        alert('Failed to process the next phase. Please try again.');
      }
    } catch (error) {
      console.error('Error processing the next phase:', error);
      alert('Failed to process the next phase. Please try again.');
    }
  };

  const handleNextPhaseClick = () => {
    callNextPhase(currentPhase);
  };

  const styles = {
    container: {
      backgroundColor: '#fff',
      borderRadius: '8px',
      padding: '20px',
      boxShadow: '0 0 10px rgba(0, 0, 0, 0.1)',
      maxWidth: '800px',
      margin: '20px auto',
      textAlign: 'center',
    },
    heading: {
      fontSize: '2em',
      marginBottom: '20px',
    },
    button: {
      width: '100%',
      padding: '10px',
      borderRadius: '4px',
      border: 'none',
      backgroundColor: '#007bff',
      color: '#fff',
      fontSize: '1em',
      cursor: 'pointer',
      boxSizing: 'border-box', // Ensure padding is included in the width
    },
  };

  return (
    <div style={styles.container}>
      <h1 style={styles.heading}>Dashboard</h1>
      <p>Current Phase: {currentPhase}</p>
      <button style={styles.button} onClick={handleNextPhaseClick}>Next Phase</button>
    </div>
  );
};

export default Dashboard;