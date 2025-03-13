import React, { useState } from 'react';

const TakeLoan = () => {
  const [loanAmount, setLoanAmount] = useState(100);

  const handleSliderChange = (e) => {
    setLoanAmount(e.target.value);
  };

  const handleAccept = () => {
    console.log(`Accepted loan amount: $${loanAmount}`);
    // Add your accept logic here
  };

  const interestRate = 0.05;
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
      margin: '0 0 20px 0', // Remove extra margin and add bottom margin
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
          <h2>Toma un Credito</h2>
      <p style={styles.heading}>Selecciona</p>
      <input
        type="range"
        min="100"
        max="1000"
        value={loanAmount}
        onChange={handleSliderChange}
        style={styles.slider}
      />
      <div style={styles.amount}>Monto   ${loanAmount}</div>
      <div style={styles.interest}>Interés diario ${totalInterest}</div>
      <button style={styles.button} onClick={handleAccept}>Aceptar</button>
    </div>
  );
};

export default TakeLoan;