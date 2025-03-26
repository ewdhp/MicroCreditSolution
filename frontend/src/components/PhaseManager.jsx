import React, { useState, useEffect, useContext } from 'react';
import { LoanContext } from '../context/LoanContext';
import TakeLoan from './TakeLoan'; // Import TakeLoan component
import LoanInfo from './LoanInfo'; // Import LoanInfo component

const PhaseManager = () => {
    const [phase, setPhaseData] = useState(null);
    const { amount, setAmount } = useContext(LoanContext); // Use context for amount and setAmount
    const [currentRequest, setCurrentRequest] = useState( { Action: "getLoan"});

    

const fetchPhaseData = async (request) => {
    console.log("⏳ Fetching phase data...");
    const token = localStorage.getItem('token');

    const requests = {
        1: { Init: { amount } },
        3: { Approval: {} },
        7: { Pay: { method: 'CreditCard' } }
    };

    try {
        console.log("🚀 Sending request with status:", request);
        const response = await 
            fetch('https://localhost:5001/api/phases/next', {
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

                const ComponentToRender = fetchedData.component ? 
                componentMap[fetchedData.component] : 
                componentMap['TakeLoan'];

                console.log("Component:", fetchedData.component);
                console.log("ComponentToRender:", 
                    ComponentToRender);

                if (!ComponentToRender) {
                    console.error(`Component "
                        ${fetchedData.component}
                        " not found in componentMap.`
                    );
                    return;
                }
                let props = {};
                if (fetchedData.loanData) {
                    props = { loan: fetchedData.loanData };
                    if (fetchedData.loanData.status === 1) {
                        props.amount = amount;
                        props.setAmount = setAmount;
                    }
                }
                setPhaseData({
                    component: ComponentToRender,
                    props
                });
               
                const loanStatus = fetchedData.loanData?.status;
                 console.log("🎉 Fetched loanStatus:", loanStatus);
                setCurrentRequest(requests[loanStatus]);
            } else {
                console.error("Error:", fetchedData.msg);
            }
        } else {
            const errorData = await response.json();
            console.error("Error::", errorData);
        }
    } catch (error) {
        console.error("Error fetching:", error);
    }
};
    useEffect(() => {
        fetchPhaseData(currentRequest); // Fetch initial phase data on mount
    }, []); // Empty dependency array ensures this runs only once

    // ✅ Only render when phase is ready
    if (!phase) {
        return <p>Loading phase data...</p>;
    }

    const ComponentToRender = phase.component;

    return (
        <div>
            <h2>Phase Manager</h2>
            {/* Render dynamically based on the fetched component */}
            <ComponentToRender {...phase.props} />

            {/* Button to trigger the next phase */}
            <button onClick={() => {
                fetchPhaseData(currentRequest); // Trigger fetch with the updated current request
            }}>
                Go to Next Phase
            </button>
        </div>
    );
};

export default PhaseManager;