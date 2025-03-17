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
    container: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      padding: '30px',
      margin: '50px',
      marginTop: '100px',
      backgroundColor: 'rgba(20, 224, 54, 0.76)', // Semi-transparent white background
      backdropFilter: 'blur(20px)', // Blur effect
      color: 'white',
    },
    secondContainer: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',

      padding: '5px',
      backgroundColor: 'rgba(141, 26, 218, 0.77)', // Semi-transparent white background
      backdropFilter: 'blur(5px)', // Blur effect
      color: 'white',
    },
    '@media (max-width: 600px)': {
      container: {
        padding: '5px',
        margin: '5px',
        marginTop: '50px',
      },
      secondContainer: {
        padding: '5px',
        margin: '5px',
      },
    },
  };

  return (
    <div>
      <div style={styles.container}>
        <h2>Turn Interest Into Opportunity</h2>
        <p>
          With every loan repayment, you can allocate a portion of your paid interest to fund your account, empowering you to start offering loans to others in a safe way using a user trust score that you may accept or not. You may also fund your account by credit card or by other deposit methods like Crypto currencies and stores near to you.
        </p>
      </div>
      <div style={styles.secondContainer}>
        <h2>Earn always 7.5% over the interest paid when your referral's pays a loan.</h2>
      </div>
    </div>
  );
};

export default Home;