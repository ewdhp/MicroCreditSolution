import React from 'react';

const phases = [
  'Pre',
  'Inicial',
  'Creado',
  'Pendiente',
  'Aprovado',
  'Depositado',
  'Activo',
  'Pagado',
  'Due',
  'Cancelado',
  'Rechazado',
  'Unknown'
];

const LoanInfo = ({ loan }) => {
  if (!loan) {
    console.error("🚨 LoanInfo received null or undefined 'data'!", loan);
    return <p>Espera...</p>;  // Avoid crash
  }

  return (
    <div>
      <h2>Detalles del credito</h2>
      <p>Estado: {phases[loan?.status ?? 10]}</p>
      <p>Cantidad: ${(loan?.amount) ?? "N/A"} MXN</p>
      <p>{loan?.loanDescription ?? "N/A"}</p>
    </div>
  );
};

export default LoanInfo;