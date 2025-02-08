import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

const SignUp = () => {
  const [formData, setFormData] = useState({
    phoneNumber: '',
    name: '',
  });
  const [verificationCode, setVerificationCode] = useState('');
  const [codeSent, setCodeSent] = useState(false);
  const navigate = useNavigate();

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
  };

  const handleCodeChange = (e) => {
    setVerificationCode(e.target.value);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post('http://localhost:5000/api/auth/send-code', {
        phoneNumber: formData.phoneNumber,
      });
      if (response.data.success) {
        setCodeSent(true);
      }
    } catch (error) {
      console.error('Error sending code:', error);
    }
  };

  const handleVerifyCode = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post('http://localhost:5000/api/auth/verify-code', {
        phoneNumber: formData.phoneNumber,
        code: verificationCode,
      });
      if (response.data.success) {
        navigate('/dashboard');
      } else {
        alert('Invalid code. Please try again.');
      }
    } catch (error) {
      console.error('Error verifying code:', error);
    }
  };

  return (
    <div>
      <h1>Sign Up</h1>
      {!codeSent ? (
        <form onSubmit={handleSubmit}>
          <div>
            <label>Phone Number</label>
            <input
              type="text"
              name="phoneNumber"
              value={formData.phoneNumber}
              onChange={handleChange}
              required
            />
          </div>
          <div>
            <label>Name</label>
            <input
              type="text"
              name="name"
              value={formData.name}
              onChange={handleChange}
              required
            />
          </div>
          <button type="submit">Sign Up</button>
        </form>
      ) : (
        <form onSubmit={handleVerifyCode}>
          <div>
            <label>Verification Code</label>
            <input
              type="text"
              value={verificationCode}
              onChange={handleCodeChange}
              required
            />
          </div>
          <button type="submit">Verify Code</button>
        </form>
      )}
    </div>
  );
};

export default SignUp;