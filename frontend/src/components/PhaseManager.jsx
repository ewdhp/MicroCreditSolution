import React, { useState, useEffect } from 'react';
import TakeLoan from './TakeLoan'; // Import TakeLoan component
import LoanInfo from './LoanInfo'; // Import LoanInfo component

const PhaseManager = () => {
    const [phase, setPhaseData] = useState(null);
    const [amount, setAmount] = useState(50); // Track amount for TakeLoan slider

    useEffect(() => {
        console.log("⏳ Fetching phase data...");

        // Simulate API call delay
        setTimeout(() => {
            const fetchedData = {
                status: "Pending",
                component: TakeLoan, // Initially render TakeLoan
                props: { amount, setAmount },
            };

            console.log("✅ Fetched Data:", fetchedData);
            setPhaseData(fetchedData);
        }); // Simulate network delay
    }, [amount]); // Dependency on amount state, triggers update when amount changes

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

            {/* Button to simulate the next phase */}
            <button onClick={() => {
                const newData = {
                    component: LoanInfo,
                    data: {
                        loan: {
                            amount: 100,
                            interestRate: 5,
                            loanDescription: "description",
                        },
                    },

                };

                console.log("✅ Fetched Data for LoanInfo:", newData);
                setPhaseData(newData); // Update phase to render LoanInfo
            }}>
                Go to Loan Info
            </button>
        </div>
    );
};

export default PhaseManager;
