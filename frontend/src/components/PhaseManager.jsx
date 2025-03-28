import React, { useState, useEffect, useContext } from 'react';
import { LoanContext } from '../context/LoanContext';
import TakeLoan from './TakeLoan';
import LoanInfo from './LoanInfo';

const PhaseManager = () => {
    const [phase, setPhaseData] = useState(null);
    const { amount, setAmount } = useContext(LoanContext); // Get amount from LoanContext
    const [currentRequest, setCurrentRequest] = useState({ init: { amount } }); // Start with the initial phase
    const [isManualFetch, setIsManualFetch] = useState(false); // Track if the fetch is manual
    const [isInitialFetchDone, setIsInitialFetchDone] = useState(false); // Track if the initial fetch is done

    const fetchPhaseData = async (request) => {
        console.log("⏳ Fetching phase data...");
        const token = localStorage.getItem('token');

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

                    if (loanData.status === 1) {
                        setPhaseData({ component: componentMap['TakeLoan'], props: { loan: null } });
                        fetchPhaseData({ Amount: amount }); // Use dynamic amount from slider
                    } else {
                        setPhaseData({ component: ComponentToRender, props: { loan: loanData } });
                    }

                    const loanStatus = fetchedData.loanData?.status;
                    console.log("🎉 Fetched loanStatus:", loanStatus);

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
        // Fetch the initial phase data only once
        if (!isInitialFetchDone) {
            fetchPhaseData({ init: { amount } }); // Use dynamic amount from slider
            console.log("Initial fetch done");
            setIsInitialFetchDone(true); // Mark the initial fetch as done
        }
    }, [isInitialFetchDone, amount]);

    if (!phase) {
        return <p>Loading phase data...</p>;
    }

    const ComponentToRender = phase.component;
    console.log("Props being passed to ComponentToRender:", phase.props);

    return (
        <div>
            <h2>Phase Manager</h2>

            {/* Slider to adjust the amount */}
                        {phase.props?.loan?.status === 1 && (
                            <div>
                                <label htmlFor="amount-slider">Loan Amount: {amount}</label>
                                <input
                                    id="amount-slider"
                                    type="range"
                                    min="100"
                                    max="1000"
                                    step="50"
                                    value={amount}
                                    onChange={(e) => setAmount(Number(e.target.value))} // Update amount dynamically
                                />
                            </div>
                        )}

                        {/* Render the current phase component */}
            <ComponentToRender {...phase.props} />

            {/* Button to trigger the next phase */}
            <button
                onClick={() => {
                    setIsManualFetch(true); // Mark this as a manual fetch
                    fetchPhaseData({ Amount: amount }); // Use dynamic amount from slider
                }}
            >
                Go to Next Phase
            </button>
        </div>
    );
};

export default PhaseManager;