import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

const Login = () => {
  const [phoneNumber, setPhoneNumber] = useState('');
  const navigate = useNavigate();

  const handleChange = (e) => {
    setPhoneNumber(e.target.value);
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    // Add your login logic here
    navigate('/dashboard');
  };

  const styles = {
    container: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      height: 'calc(100vh - 100px)', // Adjust the height to account for the navbar
      padding: '30px',
    },
    form: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      borderRadius: '8px',
      maxWidth: '400px',

            maxWidth: '250px',
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
       
      <form style={styles.form} onSubmit={handleSubmit}>
         <h2 style={styles.heading}>Ingresa tu numero</h2>
        <input
          type="text"
          placeholder="Telefono"
          value={phoneNumber}
          onChange={handleChange}
          style={styles.input}
        />
        <button type="submit" style={styles.button}>Aceptar</button>
      </form>
    </div>
  );
};

export default Login;