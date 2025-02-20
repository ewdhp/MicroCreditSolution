import React, { useState } from 'react';
import Button from './Button';

const TakeLoan = () => {
  const [loanAmount, setLoanAmount] = useState(50);

  const handleSliderChange = (e) => {
    setLoanAmount(e.target.value);
  };

  const handleTakeLoan = () => {
    console.log(`Taking a loan of $${loanAmount}`);
    // Handle the loan taking logic here
  };

  const styles = {
    container: {
    backgroundColor: '#fff',
    borderRadius: '8px',
    padding: '20px',
    boxShadow: '0 0 10px rgba(0, 0, 0, 0.08)',
    maxWidth: '800px',
    margin: '20px auto',
    textAlign: 'center',
    opacity: 0.78,
    },
    heading: {
      fontSize: '2em',
      marginBottom: '20px',
    },
    sliderContainer: {
      marginBottom: '20px',
    },
    slider: {
      width: '100%',
    },
    loanAmount: {
      fontSize: '1.5em',
      marginBottom: '20px',
    },
  };

  return (
    <div style={styles.container}>
      <h1 style={styles.heading}>Tomar un Crédito</h1>
      <div style={styles.sliderContainer}>
        <input
          type="range"
          min="50"
          max="1000"
          value={loanAmount}
          onChange={handleSliderChange}
          style={styles.slider}
        />
        <div style={styles.loanAmount}>Monto del Crédito: ${loanAmount}</div>
      </div>
      <Button onClick={handleTakeLoan}>Tomar el Crédito</Button>
    </div>
  );
};

export default TakeLoan;