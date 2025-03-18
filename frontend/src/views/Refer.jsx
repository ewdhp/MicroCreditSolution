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
      <h1 style={styles.heading}>Obtienes el 35%</h1>
      <p style={styles.paragraph}>Tu cliente o referido solo necesita ingresar tu nombre por solo una vez al momento de solicitar un prestamo y cuando sea pagado te depositaremos el 35% del interes sobre el monto total. </p>
      <p style={styles.paragraph}>Ejemplo: Tu referido paga un credito por $1000 y tu obtienes $350.  </p>
    </div>
  );
};

export default Refer;