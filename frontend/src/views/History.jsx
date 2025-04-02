import React, { useState, useEffect } from 'react';
import HistoryPage from '../components/HistoryPage';
import Loader from '../components/Loader'; // Import the Loader component

const History = () => {
  const [isLoading, setIsLoading] = useState(true); // Loading state

  useEffect(() => {
    // Simulate a delay for loading (e.g., fetching data)
    const timer = setTimeout(() => {
      setIsLoading(false); // Set loading to false after data is "loaded"
    }, 1000); // Adjust the delay as needed

    return () => clearTimeout(timer); // Cleanup the timer
  }, []);

  const styles = {
    container: {
      maxWidth: '800px',
      textAlign: 'center',
    },
    heading: {
      fontSize: '2em',
    },
  };

  if (isLoading) {
    return <Loader />; // Show the loader while loading
  }

  return (
    <div style={styles.container}>
      <HistoryPage />
    </div>
  );
};

export default History;