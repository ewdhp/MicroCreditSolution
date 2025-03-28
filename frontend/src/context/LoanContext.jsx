import React, { createContext, useState } from 'react';

export const LoanContext = createContext();

export const LoanProvider = ({ children }) => {
    const [amount, setAmount] = useState(100); // Track amount for TakeLoan slider
    const [isDataFetched, setIsDataFetched] = useState(false); // Track if data has been fetched
    const [phaseData, setPhaseData] = useState(null); // Store fetched phase data

    return (
        <LoanContext.Provider value={{ 
            amount, 
            setAmount, 
            isDataFetched, 
            setIsDataFetched, 
            phaseData, 
            setPhaseData }}>
            {children}
        </LoanContext.Provider>
    );
};