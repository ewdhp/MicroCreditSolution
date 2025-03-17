import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import logoImage from '../assets/logo.jpg'; // Ensure you have a logo image in the assets folder

const Navbar = ({ style }) => {
  const { isAuthenticated, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const defaultStyles = {
    navbar: {
      display: 'flex',
      flexDirection: 'row',
      alignItems: 'center',
      justifyContent: 'space-between',
      position: 'fixed',
      width: '100%',
      height: '66px', // Adjust the height of the navbar as needed
      marginBottom: '20px', // Add margin to the bottom
      top: 0,
      zIndex: 1000,
      padding: '0 20px', // Add padding to the left and right
      boxSizing: 'border-box', // Include padding and border in the element's total width and height
      backgroundColor: 'rgba(240, 238, 241, 0.9)', // Semi-transparent white background
    },
    logoContainer: {
      display: 'flex',
      alignItems: 'center',
    },
    logo: {
      height: '40px', // Adjust the height of the logo as needed
      marginRight: '10px', // Space between the logo image and the text
    },
    logoText: {
      fontSize: '1.3em',
      fontWeight: 'bold',
      color: 'rgb(20, 20, 20)',
    },
    navLinks: {
      display: 'flex',
      alignItems: 'center',
    },
    link: {
      color: 'gray',
      fontSize: '1em',
      marginLeft: '20px', // Space between the links
      cursor: 'pointer', // Add pointer cursor for links
    },
  };

  const combinedStyles = {
    ...defaultStyles.navbar,
    ...style,
  };

  return (
    <nav style={combinedStyles}>
      <div style={defaultStyles.logoContainer}>
        <img src={logoImage} alt="Logo" style={defaultStyles.logo} />
        <div style={defaultStyles.logoText}><h3>MicroCredit</h3></div>
      </div>
      <div style={defaultStyles.navLinks}>
        {isAuthenticated ? (
          <span style={defaultStyles.link} onClick={handleLogout}>Salir</span>
        ) : (
          <>

            <a href="/login" style={defaultStyles.link}>Acceso</a>
          </>
        )}
      </div>
    </nav>
  );
};

export default Navbar;