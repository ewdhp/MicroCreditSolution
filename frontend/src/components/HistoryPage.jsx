import React, { useState, useEffect } from 'react';
import axios from 'axios';

const HistoryPage = () => {
  const [loans, setLoans] = useState([]);
  const [error, setError] = useState(null);
  const token = localStorage.getItem('token');

  useEffect(() => {
    const fetchLoans = async () => {
      try {
        const response = await axios
        .get('https://localhost:5001/api/loan/all', {
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
        });
        
        if (response.status === 200) {
          setLoans(response.data);
        } else {
          throw new Error('Failed to fetch loans');
        }
      } catch (error) {
        console.error('Error fetching loans:', error);
        setError('Failed to fetch loans. Please try again later.');
      }
    };
    fetchLoans();
  }, [token]);

  const styles = {
    container: {     
      maxWidth: '450px',
      margin: '0 auto',
    },

    table: {
      width: '100%',
      borderCollapse: 'collapse',
    },
    th: {
      border: '1px solid #ddd',
      padding: '8px',
      backgroundColor: '#f2f2f2',
    },
    td: {
      border: '1px solid #ddd',
      padding: '8px',
    },
    error: {
      color: 'red',
      marginBottom: '20px',
    },
    '@media (max-width: 600px)': {
      th: {
        fontSize: '12px',
        padding: '6px',
      },
      td: {
        fontSize: '12px',
        padding: '6px',
      },
    },
  };

  return (
    <div style={styles.container}>
      {error && <div style={styles.error}>
        {error}</div>}
      <table style={styles.table}>
        <thead>
          <tr>
            <th style={styles.th}>#</th>
            <th style={styles.th}>Cantidad</th>
            <th style={styles.th}>Estado</th>
             <th style={styles.th}>Fecha</th>
          </tr>
        </thead>
        <tbody>
          {loans.map((loan, index) => (
            <tr key={loan.id}>
              <td style={styles.td}>{index + 1}</td>
              <td style={styles.td}>${loan.amount}</td>
              <td style={styles.td}>{loan.status}</td>
              <td style={styles.td}>{loan.endDate}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default HistoryPage;