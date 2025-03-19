import React from 'react';
import { Link, Outlet } from 'react-router-dom';

const DashboardLayout = () => {
  const styles = {
    layout: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      padding: '10px',

    },
    nav: {
      backgroundColor: '#f8f9fa',
      borderRadius: '8px',
      padding: '10px',
      marginTop: '20px',

    },
    ul: {
      listStyle: 'none',
      padding: 0,
      display: 'flex',
      gap: '20px',
    },
    li: {
      display: 'inline',
    },
    link: {
      textDecoration: 'none',
      color: '#007bff',
    },
    content: {
      backgroundColor: 'white',
      borderRadius: '8px',
      margin:'0',
      padding: '0',
      width: '100%',
      maxWidth: '800px',
    },
  };

  return (
    <div style={styles.layout}>
      <nav style={styles.nav}>
        <ul style={styles.ul}>
          <li style={styles.li}>
            <Link 
            to="/dashboard" 
            style={styles.link}>
              Prestamo
              </Link>
          </li>
          <li style={styles.li}>
            <Link 
            to="/dashboard/history" 
            style={styles.link}>
              Historial
              </Link>
          </li>
          <li style={styles.li}>
            <Link 
            to="/dashboard/refer" 
            style={styles.link}>
              Referidos
              </Link>
          </li>
        </ul>
      </nav>
      <div style={styles.content}>
        <Outlet />
      </div>
    </div>
  );
};

export default DashboardLayout;