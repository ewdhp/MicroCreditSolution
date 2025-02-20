import React from 'react';

const Dashboard = () => {
  const styles = {
    container: {
      backgroundColor: '#fff',
      borderRadius: '8px',
      padding: '20px',
      boxShadow: '0 0 10px rgba(0, 0, 0, 0.1)',
      maxWidth: '800px',
      margin: '20px auto',
      textAlign: 'center',
    },
    heading: {
      fontSize: '2em',
      marginBottom: '20px',
    },
    paragraph: {
      fontSize: '1.2em',
    },
  };

  return (
    <div style={styles.container}>
      <h1 style={styles.heading}>Welcome to your Dashboard</h1>
      <p style={styles.paragraph}>This is your dashboard after successful verification.</p>
    </div>
  );
};

export default Dashboard;