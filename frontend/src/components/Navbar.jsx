import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const Navbar = () => {
  const { isAuthenticated, logout } = useAuth();

  const styles = {
    nav: {
      display: 'flex',
      justifyContent: 'center',
      padding: '10px',
      backgroundColor: '#f8f9fa',
      borderBottom: '1px solid #e7e7e7',
    },
    ul: {
      listStyle: 'none',
      display: 'flex',
      gap: '20px',
      padding: 0,
      margin: 0,
    },
    li: {
      display: 'inline',
    },
    link: {
      textDecoration: 'none',
      color: '#007bff',
    },
    button: {
      background: 'none',
      border: 'none',
      color: '#007bff',
      cursor: 'pointer',
      textDecoration: 'underline',
    },
  };

  return (
    <nav style={styles.nav}>
      <ul style={styles.ul}>
        <li style={styles.li}><Link to="/" style={styles.link}>Home</Link></li>
        {isAuthenticated ? (
          <>
            <li style={styles.li}><Link to="/dashboard" style={styles.link}>Dashboard</Link></li>
            <li style={styles.li}><button onClick={logout} style={styles.button}>Logout</button></li>
          </>
        ) : (
          <>
            <li style={styles.li}><Link to="/signup" style={styles.link}>Sign Up</Link></li>
            <li style={styles.li}><Link to="/login" style={styles.link}>Login</Link></li>
          </>
        )}
      </ul>
    </nav>
  );
};

export default Navbar;