import React from 'react';
import DataGrid from '../components/DataGrid';

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

  const columns = ['Loan ID', 'Amount', 'Date', 'Status'];
  const data = [
    { 'Loan ID': '1', 'Amount': '$1000', 'Date': '2023-01-01', 'Status': 'Paid' },
    { 'Loan ID': '2', 'Amount': '$2000', 'Date': '2023-02-01', 'Status': 'Pending' },
    // Add more data as needed
  ];

  return (
    <div style={styles.container}>
      <h1 style={styles.heading}>Historial de Prestamos</h1>
      <DataGrid columns={columns} data={data} />
    </div>
  );
};

export default History;