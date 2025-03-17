import React from 'react';
import HistoryPage from '../components/HistoryPage';
const History = () => {
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
  };

  
  return (
    <div style={styles.container}>
       <HistoryPage />
    </div>
  );
};

export default History;