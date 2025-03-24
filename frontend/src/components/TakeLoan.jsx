import React, { useContext } from 'react';
import { LoanContext } from '../context/LoanContext';

const TakeLoan = () => {
    const { amount, setAmount } = useContext(LoanContext); // Use context for amount and setAmount

    const handleAmountChange = (event) => {
        setAmount(event.target.value);
    };

    return (
        <div>
            <h3>Take a Loan</h3>
            <input
                type="range"
                min="0"
                max="1000"
                value={amount}
                onChange={handleAmountChange}
            />
            <p>Amount: {amount}</p>
        </div>
    );
};

export default TakeLoan;