import React from 'react';

const Navbar = ({ style }) => {
  const defaultStyles = {
    navbar: {
      display: 'flex',
      justifyContent: 'space-between',
      alignItems: 'center',
      padding: '10px 20px',
      position: 'fixed',
      width: '100%',
      top: 0,
      zIndex: 1000,
    },
    logo: {
      fontSize: '1.5em',
      fontWeight: 'bold',
      color: '#f7f7f7',
    },
    navLinks: {
      display: 'flex',
      gap: '20px',
    },
    link: {
      textDecoration: 'none',
      color: '#fff',
      fontSize: '1em',
    },
  };

  const combinedStyles = {
    ...defaultStyles.navbar,
    ...style,
  };

  return (
    <nav style={combinedStyles}>
      <div style={defaultStyles.logo}>MicroCredit</div>
      <div style={defaultStyles.navLinks}>
        <a href="/" style={defaultStyles.link}>Home</a>
        <a href="/signup" style={defaultStyles.link}>Sign Up</a>
        <a href="/login" style={defaultStyles.link}>Login</a>
      </div>
    </nav>
  );
};

export default Navbar;