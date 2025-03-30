import React, { useState } from 'react';
import axios from 'axios';

const TwilioSMS = ({ onVerifySuccess, onError }) => {
  const [phone, setPhoneNumber] = useState('');
  const [smsCode, setSmsCode] = useState('');
  const [currentStep, setCurrentStep] = useState(0);
  const [errorMessage, setErrorMessage] = useState(''); // State for error messages

  const handlePhoneNumberChange = (e) => {
    setPhoneNumber(e.target.value);
  };

  const handleSmsCodeChange = (e) => {
    setSmsCode(e.target.value);
  };

  const validatePhoneNumber = (phone) => {
    const phoneRegex = /^\+\d{10,15}$/; // E.164 format
    return phoneRegex.test(phone);
  };

  const validateSmsCode = (code) => {
    const codeRegex = /^\d{6}$/; // Exactly 6 digits
    return codeRegex.test(code);
  };

  const handleSendSms = async (e) => {
    e.preventDefault();
    setErrorMessage(''); // Clear previous errors

    if (!validatePhoneNumber(`+52${phone}`)) {
      setErrorMessage('El número de teléfono no es válido.');
      return;
    }

    try {
      const response = await axios
      .post('https://localhost:5001/api/testauth/send', {
        Phone: `+52${phone}`, // Ensure the phone number includes the country code
      });
      if (response.status === 200) {
        setCurrentStep(1); // Move to SMS verification step
      } else {
        onError('No se pudo enviar el SMS. Inténtalo de nuevo.');
      }
    } catch (error) {
      console.error('Error sending SMS:', error);
      onError('No se pudo enviar el SMS. Inténtalo de nuevo.');
    }
  };

const handleVerifySms = async (e) => {
  e.preventDefault();
  setErrorMessage(''); // Clear previous errors

  if (!validateSmsCode(smsCode)) {
    setErrorMessage('El código SMS debe tener exactamente 6 dígitos.');
    return;
  }

  try {
    let token = localStorage.getItem('token');
    if (token) {
      token = `Bearer ${token}`;
    } else {
      console.warn('Token is missing from localStorage');
    }

    const response = await axios.post(
      'https://localhost:5001/api/testauth/verify',
      {
        Phone: `+52${phone}`, // Ensure the phone number includes the country code
        Code: smsCode,
      },
      {
        headers: {
          Authorization: token || '', // Include the token in the Authorization header if it exists
        },
      }
    );

    if (response.status === 200) {

        const { token} = response.data;
        if (token) {
          localStorage.setItem('token', token);
          console.log('Token saved to localStorage:', token);
        }

      onVerifySuccess(response.data); // Pass the response data to the parent component
    } else {
      onError('El código SMS no es válido. Inténtalo de nuevo.');
    }
  } catch (error) {
    console.error('Error verifying SMS:', error);
    onError('El código SMS no es válido. Inténtalo de nuevo.');
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
    error: {
      color: 'red',
      marginBottom: '20px',
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
          {errorMessage && <p style={styles.error}>{errorMessage}</p>}
          <input
            type="text"
            placeholder="Teléfono"
            value={phone}
            onChange={handlePhoneNumberChange}
            style={styles.input}
            aria-label="Teléfono"
          />
          <button type="submit" style={styles.button}>Enviar Código</button>
        </form>
      )}
      {currentStep === 1 && (
        <form style={styles.form} onSubmit={handleVerifySms}>
          <h2 style={styles.heading}>Verificar Código</h2>
          {errorMessage && <p style={styles.error}>{errorMessage}</p>}
          <input
            type="text"
            placeholder="Código SMS"
            value={smsCode}
            onChange={handleSmsCodeChange}
            style={styles.input}
            aria-label="Código SMS"
          />
          <button type="submit" style={styles.button}>Verificar</button>
        </form>
      )}
    </>
  );
};

export default TwilioSMS;