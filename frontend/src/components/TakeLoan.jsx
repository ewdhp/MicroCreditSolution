import React from 'react';

const TakeLoan = ({ amount, setAmount }) => {
  const handleSliderChange = (event) => {
    setAmount(event.target.value);
  };

  const styles = {
    container: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      backgroundColor: '#fff',
      padding: '15px',
      borderRadius: '8px',
      boxShadow: '0 0 10px rgba(0, 0, 0, 0.1)',
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
      background: 'rgb(0, 123, 255)',
      cursor: 'pointer',
      borderRadius: '50%',
    },
    button: {
      width: '100%',
      padding: '10px',
      borderRadius: '4px',
      border: 'none',
      backgroundColor: 'rgb(0, 123, 255)',
      color: '#fff',
      fontSize: '1em',
      cursor: 'pointer',
    },
  };

  return (
    <div style={styles.container}>
      <h2 style={styles.heading}>Selecciona cantidad</h2>
      <input
        type="range"
        min="50"
        max="350"
        value={amount}
        onChange={handleSliderChange}
        style={styles.slider}
      />
      <div>Cantidad: ${amount}</div>
    </div>
  );
};

export default TakeLoan;