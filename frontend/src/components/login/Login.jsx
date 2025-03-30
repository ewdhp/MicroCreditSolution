import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import TwilioSMS from './TwilioSMS';
import Facebook from './Facebook';

const Login = () => {
  const [currentStep, setCurrentStep] = useState(0); // 0: SMS verification, 1: Facebook login
  const [errorMessage, setErrorMessage] = useState(''); // Store error messages
  const navigate = useNavigate();
  const { login } = useAuth();

  const handleVerifySuccess = (data) => {
    const { token, loginProviders } = data;
    console.log('received:', data);
    login(token); // Update the authentication state
console.log('login provider: ', loginProviders[0]);
  if (loginProviders[0] === 'facebook') {
    console.log('Login successful');
    navigate('/dashboard');
  }
    console.log('Login successful');
    setCurrentStep(1);
  };

  const handleProviderSubmitSuccess = () => {
    navigate('/dashboard'); // Redirect to the dashboard after successful login
  };

  const handleProviderSubmitError = (error) => {
    setErrorMessage(`Error with Facebook login: ${error}`);
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
    errorMessage: {
      color: 'red',
      marginTop: '1rem',
    },
  };

  return (
    <div style={styles.container}>
      <h1>Acceso</h1>
      {errorMessage && 
      <p style={styles.errorMessage}>
        {errorMessage}</p>}

      {currentStep === 0 && (
        <TwilioSMS
          onVerifySuccess={handleVerifySuccess}
          onError={setErrorMessage}
        />
      )}

      {currentStep === 1 && (
        <Facebook
          onSubmit={handleProviderSubmitSuccess}
          onError={handleProviderSubmitError}
        />
      )}
    </div>
  );
};

export default Login;