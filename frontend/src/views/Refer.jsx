import React from 'react';
import Button from '../components/Button';

const Refer = () => {
  const styles = {
        container: {
        maxWidth: '300px',
        margin: '0px auto',
        padding: '10px',
        border: '1px solid #ccc',
        borderRadius: '8px',
        boxShadow: '0 4px 8px rgba(0, 0, 0, 0.1)',
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
      <p style={styles.paragraph}>Tu cliente o referido solo necesita ingresar tu nombre al momento de solicitar un prestamo y cada vez que pague un credito te depositaremos el 35% del interes sobre el monto total. </p>
      <p style={styles.paragraph}>Ejemplo: Tu referido paga un credito por $1000 y tu obtienes $350.  </p>
    </div>
  );
};

export default Refer;