import React, { useState, useEffect } from 'react';
import axios from 'axios';
import TakeLoan from './TakeLoan';
import LoanInfo from './LoanInfo';

const phases = ['Initial', 'Pending', 'Approved', 'Active', 'Paid', 'Due', 'Cancelled', 'Rejected'];

const PhaseManager = () => {
  const [currentPhase, setCurrentPhase] = useState(null);
  const [token] = useState(localStorage.getItem('token'));
  const [loanDetails, setLoanDetails] = useState(null);
  const [error, setError] = useState(null);

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
            setCurrentPhase(0); 
          }
        } else if (response.status === 404) {
          setCurrentPhase(0); 
        } else {
          setError('Failed to fetch initial phase.');
          setCurrentPhase(0);
        }
      } catch (error) {
        if (error.response && error.response.status === 404) {
          setCurrentPhase(0);
        } else {
          setError('Failed to fetch initial phase.');
          setCurrentPhase(0); 
        }
      }
    };

    fetchInitialPhase();
  }, [token]);

  const handleLoanAccept = (newPhase, loanDetails) => {
    setCurrentPhase(newPhase);
    setLoanDetails(loanDetails);
  };

  const handleNextPhase = async (newPhase, loanDetails) => {
    try {
      const response = await axios.post(
        'https://localhost:5001/api/loans/update-phase',
        { phase: newPhase, loanDetails },
        {
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          }
        }
      );
      if (response.status === 200) {
        setCurrentPhase(newPhase);
        setLoanDetails(loanDetails);
      } else {
        setError('Failed to update phase.');
      }
    } catch (error) {
      setError('Failed to update phase.');
    }
  };

  const resetToInitial = async () => {
    try {
      const response = await axios.post(
        'https://localhost:5001/api/loans/reset-phase',
        { phase: 0 },
        {
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          }
        }
      );
      if (response.status === 200) {
        setCurrentPhase(0);
        setLoanDetails(null);
      } else {
        setError('Failed to reset phase.');
      }
    } catch (error) {
      setError('Failed to reset phase.');
    }
  };

  const phaseComponents = {
    Initial: <TakeLoan onAccept={handleLoanAccept} />,
    Pending: <LoanInfo loanDetails={loanDetails} phases={phases} onNext={handleNextPhase} />,
    Approved: <LoanInfo loanDetails={loanDetails} phases={phases} onNext={handleNextPhase} />,
    Active: <LoanInfo loanDetails={loanDetails} phases={phases} onNext={handleNextPhase} />,
    Due: <LoanInfo loanDetails={loanDetails} phases={phases} onNext={handleNextPhase} />,
    Paid: (
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
          onClick={resetToInitial}
        >
          Take Loan
        </button>
      </div>
    ),
    Cancelled: <LoanInfo loanDetails={loanDetails} phases={phases} onNext={handleNextPhase} />,
    Rejected: (
      <div>
        <p>Credito rechazado</p>
        <button
          style={{
            padding: '10px 20px',
            backgroundColor: '#f44336',
            color: 'white',
            border: 'none',
            borderRadius: '5px',
            cursor: 'pointer',
          }}
          onClick={resetToInitial}
        >
          Intentar de nuevo
        </button>
      </div>
    ),
  };

  const renderPhaseComponent = () => {
    if (error) {
      return <div style={{ color: 'red' }}>{error}</div>;
    }

    if (currentPhase === null) {
      return <div>Loading...</div>;
    }

    return phaseComponents[phases[currentPhase]] || <div>Phase: {phases[currentPhase]}</div>;
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