import React, { useState, useEffect, useContext } from 'react';
import { LoanContext } from '../context/LoanContext';
import TakeLoan from './TakeLoan';
import LoanInfo from './LoanInfo'; 

const PhaseManager = () => {
    const [phase, setPhaseData] = useState(null);
    const { amount, setAmount } = useContext(LoanContext);
    const [currReq, setCurrReq] = useState({ Init: null });   

    const fetchPhaseData = async (request) => {
        console.log("⏳ Fetching phase data...");
        const token = localStorage.getItem('token');
        const requests = {
            0: {},
            1: { Init: { amount } },
            2: { Approval: {} },
            3: { Pay: { method: 'CreditCard' } }
        };
        try {
            const response = await fetch('https://localhost:5001/api/phases/next', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(request) 
            });
            if (response.status === 200) {
                const fetchedData = await response.json();
                console.log("✅ Fetched Data:", fetchedData);
                if (fetchedData.success) {
                    const componentMap = {
                        'TakeLoan': TakeLoan,
                        'LoanInfo': LoanInfo
                    };
                    const Component = componentMap[fetchedData.component];
                    let props = {};
                    if (fetchedData.loanData) {
                        props = { loan: fetchedData.loanData };
                        if (fetchedData.loanData.status === 1) { // Assuming 1 corresponds to 'Initial'
                            props.amount = amount;
                            props.setAmount = setAmount;
                        }
                    }
                    setPhaseData({ component: Component, props });
                    const loanStatus = fetchedData.loanData?.status;          
                    console.log("🚀 Next phase:", requests[loanStatus]);
                    setCurrReq(requests[loanStatus]); // Update next request only once
                } else {
                    console.error("❌ Error:", fetchedData.msg);
                }
            } else {
                const errorData = await response.json();
                console.error("❌ Request failed:", errorData);
            }
        } catch (error) {
            console.error("❌ Error fetching:", error);
        }
    };

    useEffect(() => {
        fetchPhaseData(currReq); // Fetch initial phase data on mount
    }, []); // Empty dependency array ensures this runs only once

    // ✅ Only render when phase is ready
    if (!phase) {
        return <p>Loading phase data...</p>;
    }

    const Component = phase.component;

    return (
        <div>
            <h2>Phase Manager</h2>
            {/* Render dynamically based on the fetched component */}
            <Component {...phase.props} />

            {/* Button to trigger the next phase */}
            <button onClick={() => {
                fetchPhaseData(currReq); // Trigger fetch with the updated current request
            }}>
                Go to Next Phase
            </button>
        </div>
    );
};

export default PhaseManager;