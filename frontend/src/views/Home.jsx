import React from 'react';
import TakeLoan from '../components/TakeLoan';
import backgroundImage from '../assets/land.png'; // Ensure you have a background image in the assets folder

const Home = () => {
  const styles = {
    container: {
      height: 'calc(100vh - 100px)',
      backgroundSize: 'cover',
      backgroundPosition: 'center',
      backgroundRepeat: 'no-repeat',
      borderRadius: '8px',
      padding: '20px',
      boxShadow: '0 0 10px rgba(117, 127, 202, 0.1)',
      maxWidth: '1200px',
      margin: '20px auto',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'space-between',
    },
    contentContainer: {
      flex: '1',
      textAlign: 'center',
      backgroundColor: 'rgba(60, 117, 230, 0.45)', // Add a semi-transparent background to make text readable
      padding: '20px',
      borderRadius: '8px',
      marginTop: '50px',
    },
    heading: {
      fontSize: '2em',
      marginBottom: '20px',
    },
    paragraph: {
      fontSize: '1.2em',
      marginBottom: '10px',
    },
    loanContainer: {
      flex: '1',
      margin: '20px',
    },
  };
  styles.loanContainer = {
    ...styles.loanContainer,
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'flex-end',
  };
  return (
    <div style={{ ...styles.imageContainer}}>
      <div style={styles.loanContainer}>
        <div style={styles.contentContainer}>
          <h3 style={styles.paragraph}>Aqui seguro te prestamos.</h3>
          <TakeLoan />
        </div>
      </div>
    </div>
  );
};

export default Home;