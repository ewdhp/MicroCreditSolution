import React from 'react';

const Button = ({ children, onClick, disabled }) => {
  const styles = {
    button: {
      padding: '10px 20px',
      backgroundColor: '#3498db',
      color: 'white',
      border: 'none',
      borderRadius: '5px',
      cursor: 'pointer',
      opacity: disabled ? '0.6' : '1',
    },
  };

  return (
    <button 
      onClick={onClick} 
      disabled={disabled} 
      style={styles.button}
    >
      {children}
    </button>
  );
};

export default Button;