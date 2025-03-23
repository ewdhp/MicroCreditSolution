import React, { useState } from 'react';
import ToggleSwitch from './ToggleSwitch';

const TakeLoan = ({ 
  amount: initialLoanAmount, 
  referral: initialReferral, 
  onNextPhase,
  onAccept, 
  token, 
}) => {
       
  const [referralEnabled, setReferralEnabled] = useState(false);
  const [referral, setReferral] = useState(initialReferral || '');
  const [amount, setLoanAmount] = useState(initialLoanAmount || 100);

  const SliderChange = (e) => {
    setLoanAmount(parseInt(e.target.value, 10));
  };

  const referralChange = (e) => {
    setReferral(e.target.value);
  };

  const handleReferidoToggle = () => {
    setReferralEnabled(!referralEnabled);
  };

  const handleAccept = () => {
    if (!token) {
      alert('Token not found.');
      return;
    }

    const loanDetails = {
      Status: 0,
      Amount: amount,
      Referido: referralEnabled ? referral : null,
    };

    console.log('Phase updated:', loanDetails);

    if (onAccept) {
      onAccept(loanDetails);
    }
    if (onNextPhase) {
      onNextPhase(loanDetails);
    }
  };

  const interestRate = 0.05;
  const totalInterest = (amount * interestRate).toFixed(2);
  const total = (amount * interestRate * 7).toFixed(2);

  const styles = {
    container: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      backgroundColor: '#fff',
      padding: '15px',
      borderRadius: '8px',
      boxShadow: 
      '0 0 10px rgba(0, 0, 0, 0.1)',
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
      background: 
      'rgb(0, 123, 255)',
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
      backgroundColor: 
      'rgb(0, 123, 255)',
      color: '#fff',
      fontSize: '1em',
      cursor: 'pointer',
    },
  };

  return (
    <div style={styles.container}> 
      <h2 style={styles.heading}>
        Selecciona cantidad</h2>
      <input
        type="range"
        min="50"
        max="350"
        value={amount}
        onChange={SliderChange}
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
      <div style={styles.amount}>Cantidad: ${amount}</div>
      <div style={styles.interest}>Interes diario: ${totalInterest}</div>
      <div style={styles.totalInterest}>Total 7 dias: ${total}</div>
      <div style={styles.checkboxContainer}>
        <ToggleSwitch 
        isChecked={referralEnabled} 
        onToggle={handleReferidoToggle} />
        <label>Referido</label>
      </div>
      {referralEnabled && (
        <input
          type="text"
          placeholder="Nombre"
          value={referral}
          onChange={referralChange}
          style={styles.input}
        />
      )}
      <button 
      style={styles.button} 
      onClick={handleAccept}>
        Accept
      </button>
    </div>
  );
};

export default TakeLoan;