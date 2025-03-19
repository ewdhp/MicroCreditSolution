import React from 'react';
import { NavLink, Outlet } from 'react-router-dom';

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
      color: '#000', // Default color is black
    },
    activeLink: {
      textDecoration: 'none',
      color: '#007bff', // Active color is blue
    },
    content: {
      backgroundColor: 'white',
      borderRadius: '8px',
      margin: '0',
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
            <NavLink
              to="/dashboard"
              end
              style={({ isActive }) => {
                console.log('Prestamo isActive:', isActive);
                return isActive ? styles.activeLink : styles.link;
              }}
            >
              Prestamo
            </NavLink>
          </li>
          <li style={styles.li}>
            <NavLink
              to="/dashboard/history"
              end
              style={({ isActive }) => {
                console.log('Historial isActive:', isActive);
                return isActive ? styles.activeLink : styles.link;
              }}
            >
              Historial
            </NavLink>
          </li>
          <li style={styles.li}>
            <NavLink
              to="/dashboard/refer"
              end
              style={({ isActive }) => {
                console.log('Referidos isActive:', isActive);
                return isActive ? styles.activeLink : styles.link;
              }}
            >
              Referidos
            </NavLink>
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