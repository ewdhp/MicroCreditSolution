import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import TwilioSMS from './TwilioSMS';

const Facebook = () => {
  const [currentStep, setCurrentStep] = useState(0); // 0: SMS verification, 1: Image upload and selection
  const [errorMessage, setErrorMessage] = useState(''); // Store error messages
  const [uploadedImages, setUploadedImages] = useState([]); // Store uploaded images
  const [selectedImages, setSelectedImages] = useState([]); // Store selected images
  const navigate = useNavigate();
  const { login } = useAuth();

  const handleVerifySuccess = (data) => {
    const { token } = data;
    login(token); // Update the authentication state
    setCurrentStep(1); // Move to the image upload and selection step
  };

  const handleImageUpload = (event) => {
    const files = Array.from(event.target.files);
    const imageUrls = files.map((file) => URL.createObjectURL(file));
    setUploadedImages((prev) => [...prev, ...imageUrls]);
  };

  const handleImageSelection = (image) => {
    setSelectedImages((prev) =>
      prev.includes(image) ? 
    prev.filter((img) => img !== image) : 
    [...prev, image]
    );
  };

  const handleSubmit = async () => {
    try {
      const token = localStorage.getItem('token'); // Retrieve the token from localStorage
      const response = await 
      fetch('https://localhost:5001/auth/add-login-provider', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
        },
        body: JSON.stringify({ 
          provider: 'facebook', 
          images: selectedImages }),
      });

      if (response.ok) {
        navigate('/dashboard'); // Redirect to the dashboard after successful submission
      } else {
        const errorData = await response.json();
        setErrorMessage(`Error: 
          ${errorData.message || 
            'Failed to add login provider'}`);
      }
    } catch (error) {
      setErrorMessage(`Error: ${error.message}`);
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
    errorMessage: {
      color: 'red',
      marginTop: '1rem',
    },
    imageGrid: {
      display: 'grid',
      gridTemplateColumns: 'repeat(3, 1fr)',
      gap: '10px',
      marginTop: '20px',
    },
    image: {
      width: '100px',
      height: '100px',
      cursor: 'pointer',
      border: '2px solid transparent',
    },
    selectedImage: {
      border: '2px solid blue',
    },
    uploadInput: {
      marginTop: '20px',
    },
    submitButton: {
      marginTop: '20px',
      padding: '10px 20px',
      backgroundColor: '#007bff',
      color: 'white',
      border: 'none',
      borderRadius: '5px',
      cursor: 'pointer',
    },
  };

  return (
    <div style={styles.container}>
      <h1>Acceso</h1>
      {errorMessage && <p style={styles.errorMessage}>{errorMessage}</p>}

      {currentStep === 0 && (
        <TwilioSMS
          onVerifySuccess={handleVerifySuccess}
          onError={setErrorMessage}
        />
      )}

      {currentStep === 1 && (
        <>
          <h2>Upload and Select Images</h2>
          <input
            type="file"
            accept="image/*"
            multiple
            style={styles.uploadInput}
            onChange={handleImageUpload}
          />
          <div style={styles.imageGrid}>
            {uploadedImages.map((image, index) => (
              <img
                key={index}
                src={image}
                alt={`Uploaded ${index}`}
                style={{
                  ...styles.image,
                  ...(selectedImages.includes(image) ? styles.selectedImage : {}),
                }}
                onClick={() => handleImageSelection(image)}
              />
            ))}
          </div>
          <button style={styles.submitButton} onClick={handleSubmit}>
            Submit
          </button>
        </>
      )}
    </div>
  );
};

export default Facebook;