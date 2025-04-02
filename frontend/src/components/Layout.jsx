import React from 'react';
import { NavLink, Outlet } from 'react-router-dom';

const Layout = () => {
  const styles = {
    layout: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
           
    },
    nav: {
 marginTop: '10px', 
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
              style={({ isActive }) => (isActive ? styles.activeLink : styles.link)}
            >
              Crédito
            </NavLink>
          </li>
          <li style={styles.li}>
            <NavLink
              to="/dashboard/history"
              end
              style={({ isActive }) => (isActive ? styles.activeLink : styles.link)}
            >
              Historial
            </NavLink>
          </li>
          <li style={styles.li}>
            <NavLink
              to="/dashboard/refer"
              end
              style={({ isActive }) => (isActive ? styles.activeLink : styles.link)}
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

export default Layout;