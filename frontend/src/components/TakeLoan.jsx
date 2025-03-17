import React, { useState } from 'react';
import axios from 'axios';

const TakeLoan = ({ onAccept }) => {
  const [loanAmount, setLoanAmount] = useState(100);
  const token = localStorage.getItem('token'); // Retrieve token from localStorage

  const handleSliderChange = (e) => {
    setLoanAmount(parseInt(e.target.value,10));
  };

  const handleAccept = async () => {
    try {
      if (!token) {
        throw new Error('Token not found. Please log in again.');
      }

      // Call the API endpoint to process the initial phase
      const response = await axios.post(
        'https://localhost:5001/api/phases/next-phase',
        { Status: 0, Amount: loanAmount }, // Include loanAmount for the "Initial" phase
        {
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (response.status === 200) {
        console.log('Loan accepted and phase updated successfully:', response.data);

        // Notify parent component with the new phase status and loan details
        if (onAccept) {
          const newPhase = response.data.result.loan.status;
          const loanDetails = response.data.result.loan;
          onAccept(newPhase, loanDetails);
        }
      } else {
        console.error('Failed to process loan:', response);
        alert('Failed to process the loan. Please try again.');
      }
    } catch (error) {
      console.error('Error sending loan amount:', error);
      alert('Failed to process the loan. Please try again.');
    }
  };

  const interestRate = 0.05; // Example interest rate
  const totalInterest = (loanAmount * interestRate).toFixed(2);

  const styles = {
    container: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      backgroundColor: '#fff',
      padding: '20px',
      borderRadius: '8px',
      boxShadow: '0 0 10px rgba(0, 0, 0, 0.1)',
      maxWidth: '300px',
      width: '100%',
    },
    heading: {
      margin: '0 0 20px 0',
    },
    slider: {
      width: '100%',
      marginBottom: '20px',
    },
    amount: {
      fontSize: '1em',
      marginBottom: '10px',
    },
    interest: {
      fontSize: '1em',
      marginBottom: '20px',
      color: 'green',
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
    },
  };

  return (
    <div style={styles.container}>
      <h2>Take a Loan</h2>
      <p style={styles.heading}>Select the amount</p>
      <input
        type="range"
        min="100"
        max="1000"
        value={loanAmount}
        onChange={handleSliderChange}
        style={styles.slider}
      />
      <div style={styles.amount}>Amount: ${loanAmount}</div>
      <div style={styles.interest}>Daily Interest: ${totalInterest}</div>
      <button style={styles.button} onClick={handleAccept}>Accept</button>
    </div>
  );
};

export default TakeLoan;