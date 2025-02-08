// src/components/Login.js
import React, { useState } from 'react';

const Login = () => {
  const [loginData, setLoginData] = useState({
    phoneNumber: '',
    password: '',
  });

  const handleChange = (e) => {
    setLoginData({
      ...loginData,
      [e.target.name]: e.target.value,
    });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    // Add login logic here
    console.log('User logged in:', loginData);
  };

  return (
    <div>
      <h1>Login</h1>
      <form onSubmit={handleSubmit}>
        <div>
          <label>Phone Number</label>
          <input
            type="text"
            name="phoneNumber"
            value={loginData.phoneNumber}
            onChange={handleChange}
            required
          />
        </div>
        <div>
          <label>Password</label>
          <input
            type="password"
            name="password"
            value={loginData.password}
            onChange={handleChange}
            required
          />
        </div>
        <button type="submit">Login</button>
      </form>
    </div>
  );
};

export default Login;