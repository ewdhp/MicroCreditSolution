import React from 'react';
import logoImage from '../assets/logo.png'; // Ensure you have a logo image in the assets folder

const Navbar = ({ style }) => {
  const defaultStyles = {
    navbar: {
      display: 'flex',
      flexDirection: 'row',
      alignItems: 'center',
      justifyContent: 'space-between',
      position: 'fixed',
      width: '100%', // Ensure the navbar spans the full width
      height: '60px', // Adjust the height of the navbar as needed
      top: 0,
      zIndex: 1000,
      padding: '0 20px', // Add padding to the left and right
      boxSizing: 'border-box', // Include padding and border in the element's total width and height

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
      color: 'gray',
    },
    navLinks: {
      display: 'flex',
      alignItems: 'center',
    },
    link: {

      color: '#gray',
      fontSize: '1em',
      marginLeft: '20px', // Space between the links
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
        <div style={defaultStyles.logoText}>MicroCredit</div>
      </div>
      <div style={defaultStyles.navLinks}>
        <a href="/login">Entrar</a>
      </div>
    </nav>
  );
};

export default Navbar;