import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { useAuth } from '../context/AuthContext';
import { useEffect } from 'react';

const Login = () => {
  const [phoneNumber, setPhoneNumber] = useState('');
  const [smsCode, setSmsCode] = useState('');
  const [currentStep, setCurrentStep] = useState(0);
  const navigate = useNavigate();
  const { login, isAuthenticated } = useAuth();

   useEffect(() => {
    if (isAuthenticated) {
      navigate('/dashboard');
    }
  }, [isAuthenticated, navigate]);

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
        Action: 'login',
        Phone: `+52${phoneNumber}`, // Ensure the phone number includes the country code
      });
      if (response.status === 200) {
        setCurrentStep(1);
      } else {
        alert('Failed to send SMS. Please try again.');
      }
    } catch (error) {
      console.error('Error sending SMS:', error);
      alert('Failed to send SMS. Please try again.');
    }
  };

  const handleVerifySms = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post('https://localhost:5001/api/testauth/verify', {
        Action: 'login',
        Phone: `+52${phoneNumber}`, // Ensure the phone number includes the country code
        Code: smsCode,
      });

      if (response.status === 200) {
        const token = response.data.token;
        login(token); // Update the authentication state
        console.log('Verification successful, navigating to dashboard...');
        navigate('/dashboard');
      } else {
        alert('Invalid SMS code. Please try again.');
      }
    } catch (error) {
      console.error('Error verifying SMS:', error);
      alert('Invalid SMS code. Please try again.');
    }
  };

  const styles = {
    container: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      height: 'calc(100vh - 100px)', // Adjust the height to account for the navbar
      padding: '30px',
      backgroundColor: '#f9f9f9',
    },
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
      boxSizing: 'border-box', // Ensure padding is included in the width
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
      boxSizing: 'border-box', // Ensure padding is included in the width
    },
    heading: {
      margin: '0 0 20px 0', // Remove extra margin and add bottom margin
    },
  };

  return (
    <div style={styles.container}>
      <h1>Acceso</h1>
      {currentStep === 0 && (
        <form style={styles.form} onSubmit={handleSendSms}>
          <h2 style={styles.heading}>Ingresa tu télefono</h2>
          <input
            type="text"
            placeholder="Teléfono"
            value={phoneNumber}
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
    </div>
  );
};

export default Login;