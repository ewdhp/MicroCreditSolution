import React, { useState, useEffect } from 'react';
import axios from 'axios';
import TakeLoan from './TakeLoan';
import LoanInfo from './LoanInfo';

const phases = ['Initial', 'Pending', 'Approved', 'Active', 'Paid', 'Due'];

const PhaseManager = () => {
  const [currentPhase, setCurrentPhase] = useState(null); // Initial phase index
  const [token] = useState(localStorage.getItem('token')); // Get token from localStorage
  const [loanDetails, setLoanDetails] = useState(null);
  const [error, setError] = useState(null); // Error state

  useEffect(() => {
    const fetchInitialPhase = async () => {
      try {
        const response = await axios.get(
          'https://localhost:5001/api/loans/current-loan',
          {
            headers: {
              'Content-Type': 'application/json',
              Authorization: `Bearer ${token}`,
            },
          }
        );

        if (response.status === 200) {
          const loan = response.data.loan;
          if (loan) {
            setLoanDetails(loan);
            setCurrentPhase(loan.status);
          } else {
            setCurrentPhase(0); // Default to Initial phase if no current loan
          }
        } else if (response.status === 404) {
          setCurrentPhase(0); // Set to Initial phase if no loan found
        } else {
          setError('Failed to fetch initial phase.');
          setCurrentPhase(0); // Set to Initial phase if any other error occurs
        }
      } catch (error) {
        if (error.response && error.response.status === 404) {
          setCurrentPhase(0); // Set to Initial phase if no loan found
        } else {
          setError('Failed to fetch initial phase.');
          setCurrentPhase(0); // Set to Initial phase if any other error occurs
        }
      }
    };

    fetchInitialPhase();
  }, [token]);

  const handleLoanAccept = (newPhase, loanDetails) => {
    setCurrentPhase(newPhase);
    setLoanDetails(loanDetails);
  };

  const handleNextPhase = (newPhase, loanDetails) => {
    setCurrentPhase(newPhase);
    setLoanDetails(loanDetails);
  };

  const renderPhaseComponent = () => {
    if (error) {
      return <div style={{ color: 'red' }}>{error}</div>;
    }

    if (currentPhase === null) {
      return <div>Loading...</div>;
    }

    switch (phases[currentPhase]) {

      case 'Initial':
        return <TakeLoan 
        onAccept={handleLoanAccept} 
        />;

      case 'Pending':
        return <LoanInfo 
        loanDetails={loanDetails} 
        phases={phases} 
        onNext={handleNextPhase} 
        />;

      case 'Approved':
        return <LoanInfo 
        loanDetails={loanDetails} 
        phases={phases} 
        onNext={handleNextPhase} 
        />;

      case 'Active':
        return  <LoanInfo 
        loanDetails={loanDetails} 
        phases={phases} 
        onNext={handleNextPhase} 
        />;

      case 'Paid':
        return (
          <div>
            <p>Payment complete!</p>
            <button
              style={{
                padding: '10px 20px',
                backgroundColor: '#f44336',
                color: 'white',
                border: 'none',
                borderRadius: '5px',
                cursor: 'pointer',
              }}
              onClick={() => setCurrentPhase(0)}
            >
              Take Loan
            </button>
          </div>
        );
      default:
        return <div>Phase: {phases[currentPhase]}</div>;
    }
  };

  const styles = {
    container: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
    },
  };

  return (
    <div style={styles.container}>
      {renderPhaseComponent()}
    </div>
  );
};

export default PhaseManager;