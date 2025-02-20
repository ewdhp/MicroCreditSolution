import React from 'react';

const FormInput = ({ label, type, name, value, onChange }) => {
  const styles = {
    container: {
      marginBottom: '15px',
    },
    label: {
      display: 'block',
      marginBottom: '5px',
    },
    input: {
      width: '100%',
      padding: '10px',
      borderRadius: '5px',
      border: '1px solid #ddd',
    },
  };

  return (
    <div style={styles.container}>
      <label style={styles.label}>{label}</label>
      <input
        type={type}
        name={name}
        value={value}
        onChange={onChange}
        style={styles.input}
      />
    </div>
  );
};

export default FormInput;