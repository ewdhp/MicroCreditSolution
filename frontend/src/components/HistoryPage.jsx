import React, { useState, useEffect } from 'react';
import axios from 'axios';
import Loader from './Loader'; // Import the Loader component

const phases = [
  'Inicial',
  'Inicial',
  'Create',
  'Pendiente',
  'Aprovado',
  'Disbursed',
  'Activo',
  'Pagado',
  'Due',
  'Cancelado',
  'Rechazado',
  'Unknown',
];

const HistoryPage = () => {
  const [loans, setLoans] = useState([]);
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(true); // Add loading state
  const token = localStorage.getItem('token');

  useEffect(() => {
    const fetchLoans = async () => {
      setIsLoading(true); // Start loading
      try {
        const response = await axios.get('https://localhost:5001/api/loan/all', {
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
      } finally {
        setIsLoading(false); // Stop loading
      }
    };
    fetchLoans();
  }, [token]);

  const styles = {
    container: {
      maxWidth: '300px',
      margin: '10px auto',
      padding: '20px',
      border: '1px solid #ccc',
      borderRadius: '8px',
      boxShadow: '0 4px 8px rgba(0, 0, 0, 0.1)',
      textAlign: 'center',
    },
    table: {
      width: '100%',
      borderCollapse: 'collapse',
    },
    th: {
      padding: '10px',
      borderBottom: '1px solid #ddd',
      color: 'black',
      textAlign: 'center',
    },
    td: {
      borderBottom: '1px solid #ddd',
      padding: '10px',
      textAlign: 'center',
    },
    error: {
      color: 'red',
      marginBottom: '20px',
      textAlign: 'center',
    },
    header: {
      textAlign: 'center',
      fontSize: '1.5rem',
      color: '#333',
    },
    '@media (max-width: 600px)': {
      th: {
        fontSize: '12px',
        padding: '8px',
      },
      td: {
        fontSize: '12px',
        padding: '8px',
      },
    },
  };

  const mapStatusToPhase = (status) => {
    const phaseIndex = parseInt(status, 10); // Convert status to an integer
    return phases[phaseIndex] || 'Unknown'; // Map to phase or return 'Unknown' if out of range
  };

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('es-ES', { day: '2-digit', month: '2-digit', year: 'numeric' }).format(date);
  };

  if (isLoading) {
    return <Loader />; // Show the loader while loading
  }

  return (
    <div style={styles.container}>
      {error && <div style={styles.error}>{error}</div>}
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
              <td style={styles.td}>{mapStatusToPhase(loan.status)}</td>
              <td style={styles.td}>{formatDate(loan.endDate)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default HistoryPage;