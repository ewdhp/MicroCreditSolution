import React from 'react';
import axios from 'axios';

const LoanInfo = ({ loanDetails, phases, onNext }) => {
  const token = localStorage.getItem('token'); // Retrieve token from localStorage

  const handleNext = async () => {
    try {
      if (!token) {
        throw new Error('Token not found. Please log in again.');
      }

      const status = loanDetails.status;
      const response = await axios.post(
        'https://localhost:5001/api/phases/next-phase',
        { Status: status },
        {
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (response.status === 200) {
        console.log('Phase updated successfully:', response.data);

        // Notify parent component with the new phase status and loan details
        if (onNext) {
          const newPhase = response.data.result.loan.status;
          const updatedLoanDetails = response.data.result.loan;
          onNext(newPhase, updatedLoanDetails);
        }
      } else {
        console.error('Failed to update phase:', response);
        alert('Failed to update the phase. Please try again.');
      }
    } catch (error) {
      console.error('Error updating phase:', error);
      alert('Failed to update the phase. Please try again.');
    }
  };

  const styles = {
       container: {     
      maxWidth: '100%',
      margin: '0 auto',
    },
    button: {
      padding: '10px 20px',
      backgroundColor: '#007bff',
      color: 'white',
      border: 'none',
      borderRadius: '5px',
      cursor: 'pointer',
      marginTop: '20px',
    },
  };

  return (
    <div style={styles.container}>
      <h2>Details</h2>
      <p>Amount: ${loanDetails.amount}</p>
      <p>Interest Rate: {loanDetails.interestRate}%</p>
      <p>Loan Description: {loanDetails.loanDescription}</p>
      <p>Status: {phases[loanDetails.status]}</p>
      <p>End Date: {loanDetails.endDate}</p>
      <button style={styles.button} onClick={handleNext}>Next</button>
    </div> 
  );
};

export default LoanInfo;