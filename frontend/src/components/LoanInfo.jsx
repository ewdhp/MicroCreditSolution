import React from 'react';

const phases = [
  'Initial',
  'Create',
  'Pending',
  'Approved',
  'Disbursed',
  'Active',
  'Paid',
  'Due',
  'Canceled',
  'Rejected',
  'Unknown'
];

const LoanInfo = ({ loan }) => {
  if (!loan) {
    console.error("🚨 LoanInfo received null or undefined 'data'!", loan);
    return <p>Loading loan data...</p>;  // Avoid crash
  }

  return (
    <div>
      <h2>Loan Details</h2>
      <p>Status: {phases[loan?.status ?? 10]}</p>
      <p>Amount: {loan?.amount ?? "N/A"}</p>

      <p>Interest Rate: {loan?.interestRate ?? "N/A"}%</p>
      <p>Description: {loan?.loanDescription ?? "N/A"}</p>
    </div>
  );
};

export default LoanInfo;