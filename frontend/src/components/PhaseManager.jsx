import React, { useState, useEffect, useContext } from 'react';
import { LoanContext } from '../context/LoanContext';
import TakeLoan from './TakeLoan';
import LoanInfo from './LoanInfo';
import Loader from './Loader'; // Import the Loader component

const PhaseManager = () => {
    const [phase, setPhaseData] = useState(null);
    const { amount } = useContext(LoanContext);
    const [isFirstLoad, setIsFirstLoad] = useState(true);
    const [isLoading, setIsLoading] = useState(false); // Add loading state
    const [loanStatus, setLoanStatus] = useState(null); // Track loan status

    const fetchPhaseData = async (request) => {
        const token = localStorage.getItem('token'); 
        setIsLoading(true); // Start loading
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
                if (fetchedData.success) {
                    const componentMap = {
                        'TakeLoan': TakeLoan,
                        'LoanInfo': LoanInfo
                    };
                    const ComponentToRender = fetchedData.component
                        ? componentMap[fetchedData.component]
                        : null;
                    const loanData = fetchedData.loanData || null;
                    if (!ComponentToRender) return;
                    if (loanData.status === 7) {
                        fetchPhaseData({ Amount: 0 });
                        return;
                    }
                    setLoanStatus(loanData.status); // Update loan status
                    setPhaseData({
                        component: ComponentToRender, 
                        props: { loan: loanData }
                    });                 
                } else {
                    console.error("Error:", fetchedData.msg);
                }
            } else {
                const errorData = await response.json();
                console.error("Error::", errorData);
                if (errorData.response.msg === "Amount error.") {
                    console.log("🚫 Amount error detected. Showing TakeLoan component.");
                    setPhaseData({ component: TakeLoan, props: { loan: null } });
                }
            }
        } catch (error) {
            console.error(error);
        } finally {
            setIsLoading(false); // Stop loading
        }
    };

    const handleFetchAmount = (amount) => {
        console.log("Fetching next:", amount);
        fetchPhaseData({ Amount: amount });       
    };

    useEffect(() => {
        if (isFirstLoad) {
            fetchPhaseData({ amount: 0 });
            setIsFirstLoad(false);
        }
    }, [isFirstLoad]);

    // Automatically fetch data every 5 seconds if loanStatus > 1
    useEffect(() => {
        if (loanStatus > 1) {
            const interval = setInterval(() => {
                console.log("Auto-fetching data...");
                fetchPhaseData({ Amount: 0 });
            }, 2000); // Fetch every 5 seconds

            return () => clearInterval(interval); // Cleanup interval on unmount
        }
    }, [loanStatus]);

    // Show the loader if loading or phase is not yet set
    if (isLoading || !phase) {
        return <Loader />;
    }

    const ComponentToRender = phase.component;

    return (
        <div>
            <ComponentToRender 
                {...phase.props} 
                onFetchNextPhase={handleFetchAmount} 
            />
        </div>
    );
};

export default PhaseManager;