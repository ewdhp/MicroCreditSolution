import React, { useState, useEffect, useContext } from 'react';
import { LoanContext } from '../context/LoanContext';
import TakeLoan from './TakeLoan';
import LoanInfo from './LoanInfo';

const PhaseManager = () => {
    const [phase, setPhaseData] = useState(null);
    const { amount } = useContext(LoanContext); // Get amount from LoanContext
    const [isFirstLoad, setIsFirstLoad] = useState(true); // Track if it's the first page load

    const fetchPhaseData = async (request) => {
        console.log("⏳ Fetching phase data...");
        const token = localStorage.getItem('token');
        if (amount == null) {
            console.error("Amount is null, cannot fetch phase data.");
            return;
        }
        try {
            console.log("🚀 Sending request", request);
            const response = await fetch('https://localhost:5001/api/loan/next', {
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

                    const ComponentToRender = fetchedData.component
                        ? componentMap[fetchedData.component]
                        : null;
                    const loanData = fetchedData.loanData || null;

                    console.log("Loan Data:", loanData);
                    console.log("Component:", fetchedData.component);
                    console.log("ComponentToRender:", ComponentToRender);

                    if (!ComponentToRender) {
                        console.error(`Component "${fetchedData.component}" not found.`);
                        return;
                    }

       
                        setPhaseData({ component: ComponentToRender, props: { loan: loanData } });
                    

                    const loanStatus = fetchedData.loanData?.status;
                    console.log("🎉 Fetched loanStatus:", loanStatus);

                } else {
                    console.error("Error:", fetchedData.msg);
                }
            } else {
                const errorData = await response.json();
                console.error("Error::", errorData);
                // Handle "Amount error." case
                if (errorData.response.msg === "Amount error.") {
                    console.log("🚫 Amount error detected. Showing TakeLoan component.");
                    setPhaseData({ component: TakeLoan, props: { loan: null } });
                }
            }
        } catch (error) {
            console.error("Error fetching:", error);
        }
    };

    const handleFetchNextPhase = (amount) => {
        console.log("Fetching next phase with amount:", amount);
        fetchPhaseData({ Amount: amount});
       
    };

    useEffect(() => {
        // Fetch the initial phase data only once
        if (isFirstLoad) {
            fetchPhaseData({ init: { amount: 0 } }); // Start with amount 0
            setIsFirstLoad(false); // Mark the first load as handled
        }
    }, [isFirstLoad]);

    if (!phase) {
        return <p>Loading phase data...</p>;
    }

    const ComponentToRender = phase.component;
    console.log("Props being passed to ComponentToRender:", phase.props);

    return (
        <div>
            <h2>Phase Manager</h2>

            {/* Render the current phase component */}
            <ComponentToRender {...phase.props} onFetchNextPhase={handleFetchNextPhase} />
        </div>
    );
};

export default PhaseManager;