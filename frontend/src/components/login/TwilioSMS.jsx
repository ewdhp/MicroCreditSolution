import React, { useState } from 'react';
import axios from 'axios';

const TwilioSMS = ({ onVerifySuccess, onError }) => {
  const [phone, setPhoneNumber] = useState('');
  const [smsCode, setSmsCode] = useState('');
  const [currentStep, setCurrentStep] = useState(0);

  const handlePhoneNumberChange = (e) => {
    setPhoneNumber(e.target.value);
  };

  const handleSmsCodeChange = (e) => {
    setSmsCode(e.target.value);
  };

  const handleSendSms = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post('https://localhost:5001/api/testauth/send', {
        Action: 'signup',
        Phone: `+52${phone}`, // Ensure the phone number includes the country code
      });
      if (response.status === 200) {
        setCurrentStep(1); // Move to SMS verification step
      } else {
        onError('Failed to send SMS. Please try again.');
      }
    } catch (error) {
      console.error('Error sending SMS:', error);
      onError('Failed to send SMS. Please try again.');
    }
  };

  const handleVerifySms = async (e) => {
    e.preventDefault();
    try {
      const response = await axios
      .post('https://localhost:5001/api/testauth/verify', {
        Action: 'signup',
        Phone: `+52${phone}`, // Ensure the phone number includes the country code
        Code: smsCode,
      });

      if (response.status === 200) {
        onVerifySuccess(response.data); // Pass the response data to the parent component
      } else {
        onError('Invalid SMS code. Please try again.');
      }
    } catch (error) {
      console.error('Error verifying SMS:', error);
      onError('Invalid SMS code. Please try again.');
    }
  };

  const styles = {
    form: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      backgroundColor: '#fff',
      padding: '20px',
      borderRadius: '8px',
      boxShadow: '0 0 10px rgba(0, 0, 0, 0.1)',
      maxWidth: '400px',
      width: '100%',
    },
    input: {
      width: '100%',
      padding: '10px',
      marginBottom: '20px',
      borderRadius: '4px',
      border: '1px solid #ccc',
      boxSizing: 'border-box',
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
      boxSizing: 'border-box',
    },
    heading: {
      margin: '0 0 20px 0',
    },
  };

  return (
    <>
      {currentStep === 0 && (
        <form style={styles.form} onSubmit={handleSendSms}>
          <h2 style={styles.heading}>Ingresa tu télefono</h2>
          <input
            type="text"
            placeholder="Teléfono"
            value={phone}
            onChange={handlePhoneNumberChange}
            style={styles.input}
          />
          <button type="submit" style={styles.button}>Enviar Código</button>
        </form>
      )}
      {currentStep === 1 && (
        <form style={styles.form} onSubmit={handleVerifySms}>
          <h2 style={styles.heading}>Verificar Código</h2>
          <input
            type="text"
            placeholder="Código SMS"
            value={smsCode}
            onChange={handleSmsCodeChange}
            style={styles.input}
          />
          <button type="submit" style={styles.button}>Verificar</button>
        </form>
      )}
    </>
  );
};

export default TwilioSMS;