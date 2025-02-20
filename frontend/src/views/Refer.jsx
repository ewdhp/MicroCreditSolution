import React from 'react';
import Button from '../components/Button';

const Refer = () => {
  const styles = {
    container: {
      backgroundColor: '#fff',
      borderRadius: '8px',
      padding: '20px',
      boxShadow: '0 0 10px rgba(0, 0, 0, 0.1)',
      maxWidth: '800px',
      margin: '20px auto',
      textAlign: 'center',
    },
    heading: {
      fontSize: '2em',
      marginBottom: '20px',
    },
    paragraph: {
      fontSize: '1.2em',
    },
  };

  const handleRefer = () => {
    // Handle refer action
    console.log('Refer action triggered');
  };

  return (
    <div style={styles.container}>
      <h1 style={styles.heading}>Gana $1000 Gratis</h1>
      <p style={styles.paragraph}>Refiere a tus amigos y gana $1000 gratis.</p>
      <Button onClick={handleRefer}>Referir a un Amigo</Button>
    </div>
  );
};

export default Refer;