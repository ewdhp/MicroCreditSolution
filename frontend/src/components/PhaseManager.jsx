import React, { useState, useEffect } from 'react';
import axios from 'axios';
import TakeLoan from './TakeLoan';
import LoanInfo from './LoanInfo';

const phases = [
  'Initial',
  'Create',
  'Pending',
  'Approved',
  'Disbursed',
  'Active',
  'Paid',
  'Due',
  'Canceled',
  'Rejected',
  'Unknown'
];

const PhaseManager = () => {
  const [currentPhase, setCurrentPhase] = useState(null);
  const [token] = useState(localStorage.getItem('token'));
  const [loanDetails, setLoanDetails] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    handleChangeStatus();
  }, [token]);

  const handleLoanAccept = (loanDetails) => {
    setLoanDetails(loanDetails);
    setCurrentPhase(1); // Move to the next phase
  };

const handleChangeStatus = async () => {
  try {
    const requestData = {
      discriminator: "InitialRequest", // Change this based on the specific request type
      data: {
        status: "Initial",
      }
    };

    const response = await axios.post(
      'https://localhost:5001/api/phases/next',
      requestData,
      {
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        }
      }
    );

    if (response.status === 200) {
      console.log(response.data);
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
      setError('Failed to update phase.');
    }
  } catch (error) {
    if (error.response && error.response.status === 404) {
      setCurrentPhase(0);
    } else {
      setError('Failed to update phase.');
    }
  }
};
  const phaseComponents = {
    Initial: <TakeLoan onAccept={handleLoanAccept} />,
    Pending: <LoanInfo loanDetails={loanDetails} phases={phases} />,
    Approved: <LoanInfo loanDetails={loanDetails} phases={phases} />,
    Active: <LoanInfo loanDetails={loanDetails} phases={phases} />,
    Due: <LoanInfo loanDetails={loanDetails} phases={phases} />,
    Paid: (
      <div>
        <p>Payment complete!</p>
      </div>
    ),
    Cancelled: <LoanInfo loanDetails={loanDetails} phases={phases} />,
    Rejected: (
      <div>
        <p>Credito rechazado</p>
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

    return phaseComponents[phases[currentPhase]] || 
    <div>Phase: {phases[currentPhase]}</div>;
  };

  const styles = {
    container: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
    },
    button: {
      marginTop: '20px',
      padding: '10px 20px',
      backgroundColor: '#007bff',
      color: 'white',
      border: 'none',
      borderRadius: '5px',
      cursor: 'pointer',
    },
  };

  return (
    <div style={styles.container}>
      {renderPhaseComponent()}
      <button style={styles.button} 
      onClick={handleChangeStatus}>
        Change Status
      </button>
    </div>
  );
};

export default PhaseManager;