import React, { createContext, useState } from 'react';

export const LoanContext = createContext();

export const LoanProvider = ({ children }) => {
    const [amount, setAmount] = useState(100); // Track amount for TakeLoan slider

    return (
        <LoanContext.Provider value={{ amount, setAmount }}>
            {children}
        </LoanContext.Provider>
    );
};