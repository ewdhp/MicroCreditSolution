import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import TakeLoan from '../components/TakeLoan';
import backgroundImage from '../assets/land.png'; // Ensure you have a background image in the assets folder

const Home = () => {
  const { isAuthenticated } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (isAuthenticated) {
      navigate('/dashboard');
    }
  }, [isAuthenticated, navigate]);

  const styles = {
    firstContainer: {
      backgroundColor: 'rgb(245, 242, 242)',
      color:'rgb(129, 94, 161)',
      padding: '10px',
    },
    container: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      padding: '25px',
      backgroundColor: 'rgba(20, 224, 54, 0.76)', // Semi-transparent white background
      backdropFilter: 'blur(40px)', // Blur effect
      color: 'white',
    },
    secondContainer: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      padding: '25px',
      backgroundColor: 'rgba(141, 26, 218, 0.77)', // Semi-transparent white background
      backdropFilter: 'blur(5px)', // Blur effect
      color: 'white',
    },
    button: {
      backgroundColor: 'rgb(46, 5, 65)', // Matching the color palette
      color: 'white',
      border: 'none',
      padding: '10px 20px',
      cursor: 'pointer',
      borderRadius: '5px',
      fontSize: '16px',
    },
    '@media (max-width: 600px)': {
      container: {
        padding: '5px',
        margin: '5px',
        marginTop: '10px',
      },
      secondContainer: {
        padding: '5px',
        margin: '5px',
      },
    },
  };

  return (
    <>
      <div style={styles.firstContainer}>
        <h1>Pagas solamente el interes diario acumulado</h1>
        <button style={styles.button} onClick={() => navigate('/signup')}>Tomar un credito</button>
      </div>
      <div style={styles.container}>
        <h2>
          Segmenta una parte del interes a pagar en tu credito y acumula dinero en tu cuenta para retirar o prestarle a otros usuarios.
        </h2>
      </div>
      <div style={styles.secondContainer}>
        <h3>Cada referido te bonifica en tu cuenta el 33% de interes por credito.</h3>
      </div>
    </>
  );
};

export default Home;