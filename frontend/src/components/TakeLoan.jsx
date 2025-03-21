import React, { useState } from 'react';
import axios from 'axios';
import ToggleSwitch from './ToggleSwitch';

const TakeLoan = ({ onAccept }) => {
  const [referidoEnabled, setReferidoEnabled] = useState(false);
  const [referido, setReferido] = useState('');
  const token = localStorage.getItem('token');
  const [loanAmount, setLoanAmount] = useState(100);

  const handleSliderChange = (e) => {
    setLoanAmount(parseInt(e.target.value, 10));
  };

  const handleReferidoChange = (e) => {
    setReferido(e.target.value);
  };

  const handleReferidoToggle = () => {
    setReferidoEnabled(!referidoEnabled);
  };

  const handleAccept = async () => {
    try {
      if (!token) {
        throw new Error('Token not found.');
      }

      const response = await axios.post(
        'https://localhost:5001/api/phases/next',
        {
          Status: 0,
          Amount: loanAmount,
          Referido: referidoEnabled ? referido : null,
        },
        {
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (response.status === 200) {
        console.log('Loan accepted and phase updated:', response.data);
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

  const interestRate = 0.05;
  const totalInterest = (loanAmount * interestRate).toFixed(2);
  const total = (loanAmount * interestRate * 7).toFixed(2);

  const styles = {
    container: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      backgroundColor: '#fff',
      padding: '15px',
      borderRadius: '8px',
      boxShadow: '0 0 10px rgba(0, 0, 0, 0.1)',
      maxWidth: '350px',
      width: '100%',
    },
    heading: {
      margin: '0 0 20px 0',
    },
    slider: {
      width: '100%',
      marginBottom: '20px',
      WebkitAppearance: 'none',
      appearance: 'none',
      height: '10px',
      background: '#ddd',
      outline: 'none',
      opacity: '0.7',
      transition: 'opacity .2s',
    },
    sliderThumb: {
      WebkitAppearance: 'none',
      appearance: 'none',
      width: '20px',
      height: '20px',
      background: 'rgb(0, 123, 255)',
      cursor: 'pointer',
      borderRadius: '50%',
    },
    amount: {
      fontSize: '1em',
      marginBottom: '10px',
    },
    interest: {
      fontSize: '1em',
      marginBottom: '10px',
      color: 'green',
    },
    totalInterest: {
      fontSize: '1em',
      marginBottom: '20px',
      color: 'green',
    },
    input: {
      width: '100%',
      padding: '10px',
      marginBottom: '20px',
      borderRadius: '4px',
      border: '1px solid #ccc',
      boxSizing: 'border-box',
      textAlign: 'center',
    },
    checkboxContainer: {
      display: 'flex',
      alignItems: 'center',
      marginBottom: '20px',
    },
    checkbox: {
      marginRight: '10px',
    },
    button: {
      width: '100%',
      padding: '10px',
      borderRadius: '4px',
      border: 'none',
      backgroundColor: 'rgb(0, 123, 255)',
      color: '#fff',
      fontSize: '1em',
      cursor: 'pointer',
    },
  };

  return (
    <div style={styles.container}>
      <h2 style={styles.heading}>Selecciona cantidad</h2>
      <input
        type="range"
        min="50"
        max="350"
        value={loanAmount}
        onChange={handleSliderChange}
        style={styles.slider}
      />
      <style>
        {`
          input[type='range']::-webkit-slider-thumb {
            -webkit-appearance: none;
            appearance: none;
            width: ${styles.sliderThumb.width};
            height: ${styles.sliderThumb.height};
            background: ${styles.sliderThumb.background};
            cursor: ${styles.sliderThumb.cursor};
            border-radius: ${styles.sliderThumb.borderRadius};
          }
          input[type='range']::-moz-range-thumb {
            width: ${styles.sliderThumb.width};
            height: ${styles.sliderThumb.height};
            background: ${styles.sliderThumb.background};
            cursor: ${styles.sliderThumb.cursor};
            border-radius: ${styles.sliderThumb.borderRadius};
          }
        `}
      </style>
      <div style={styles.amount}>Cantidad: ${loanAmount}</div>
      <div style={styles.interest}>Interes diario: ${totalInterest}</div>
      <div style={styles.totalInterest}>Total 7 dias: ${total}</div>
      <div style={styles.checkboxContainer}>
        <ToggleSwitch isChecked={referidoEnabled} onToggle={handleReferidoToggle} />
        <label>Referido</label>
      </div>
      {referidoEnabled && (
        <input
          type="text"
          placeholder="Nombre"
          value={referido}
          onChange={handleReferidoChange}
          style={styles.input}
        />
      )}
      <button style={styles.button} onClick={handleAccept}>
        Accept
      </button>
    </div>
  );
};

export default TakeLoan;