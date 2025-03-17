import React from 'react';
import HistoryPage from '../components/HistoryPage';
const History = () => {
  const styles = {
    container: {
      backgroundColor: '#fff',

      maxWidth: '800px',

      textAlign: 'center',
    },
    heading: {
      fontSize: '2em',

    },
  };

  
  return (
    <div style={styles.container}>
       <HistoryPage />
    </div>
  );
};

export default History;