import React, { useState } from 'react';
import PropTypes from 'prop-types';

const Facebook = ({ onSubmit }) => {
  const [images, setImages] = useState([]);
  const [submissionStatus, setSubmissionStatus] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleImageUpload = (e) => {
    const files = Array.from(e.target.files);
    if (files.length + images.length > 5) {
      setSubmissionStatus('You can upload up to 5 images only.');
      return;
    }
    setImages((prevImages) => [...prevImages, ...files]);
    setSubmissionStatus('');
  };

  const handleRemoveImage = (index) => {
    setImages((prevImages) => prevImages.filter((_, i) => i !== index));
  };

  const handleSubmit = async () => {
    if (images.length === 0) {
      setSubmissionStatus('Please upload at least one image.');
      return;
    }

    setIsSubmitting(true);
    setSubmissionStatus('');
    try {
      const token = localStorage.getItem('token'); 
      const formData = new FormData();
      formData.append('provider', 'facebook');
      images.forEach((image, index) => {
        formData.append(`images[${index}]`, image);
      });
      
    // Prepare the payload as a JSON object
    const payload = JSON.stringify({ Provider: 'facebook' });

    const response = await 
    fetch("https://localhost:5001/api/testauth/add-login-provider", {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
      body: payload,
    });

      if (response.ok) {
        setSubmissionStatus('Images uploaded successfully!');
        onSubmit(); 
      } else {
        const errorData = await response.json();
        setSubmissionStatus(`Error: ${errorData.message || 
          'Failed to add login provider'}`);
      }
    } catch (error) {
      console.error('Error during submission:', error);
      setSubmissionStatus('An error occurred. Please try again.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const styles = {
    container: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      maxWidth: '400px',
      margin: '0 auto',
      padding: '1rem',
      backgroundColor: '#fff',
      borderRadius: '8px',
      boxShadow: '0 0 10px rgba(0, 0, 0, 0.1)',
    },
    title: {
      marginBottom: '1rem',
      fontSize: '1.5rem',
      fontWeight: 'bold',
      color: '#333',
    },
    imagePreview: {
      display: 'flex',
      flexWrap: 'wrap',
      gap: '10px',
      marginBottom: '1rem',
    },
    image: {
      width: '80px',
      height: '80px',
      objectFit: 'cover',
      borderRadius: '4px',
      position: 'relative',
    },
    removeButton: {
      position: 'absolute',
      top: '5px',
      right: '5px',
      backgroundColor: 'red',
      color: 'white',
      border: 'none',
      borderRadius: '50%',
      cursor: 'pointer',
      width: '20px',
      height: '20px',
      fontSize: '12px',
    },
    button: {
      width: '100%',
      maxWidth: '300px',
      padding: '10px',
      backgroundColor: '#007bff',
      color: 'white',
      border: 'none',
      borderRadius: '4px',
      fontSize: '1rem',
      cursor: 'pointer',
      marginBottom: '0.5rem',
    },
    disabledButton: {
      backgroundColor: '#ccc',
      cursor: 'not-allowed',
    },
    status: {
      marginTop: '1rem',
      fontSize: '1rem',
      color: submissionStatus
      .includes('successfully') ? 'green' : 'red',
    },
  };

  return (
    <div style={styles.container}>
      <h2 style={styles.title}>Upload Images for Facebook Login</h2>
      <div style={styles.imagePreview}>
        {images.map((image, index) => (
          <div key={index} style={{ position: 'relative' }}>
            <img
              src={URL.createObjectURL(image)}
              alt={`Uploaded ${index + 1}`}
              style={styles.image}
            />
            <button
              style={styles.removeButton}
              onClick={() => handleRemoveImage(index)}
            >
              &times;
            </button>
          </div>
        ))}
      </div>
      <input
        type="file"
        accept="image/*"
        multiple
        onChange={handleImageUpload}
        disabled={isSubmitting}
      />
      <button
        style={{
          ...styles.button,
          ...(isSubmitting ? styles.disabledButton : {}),
        }}
        onClick={handleSubmit}
        disabled={isSubmitting}
      >
        {isSubmitting ? 'Submitting...' : 'Submit'}
      </button>
      {submissionStatus && 
      <p style={styles.status}>{submissionStatus}</p>}
    </div>
  );
};

Facebook.propTypes = {
  onSubmit: PropTypes.func.isRequired,
};

export default Facebook;