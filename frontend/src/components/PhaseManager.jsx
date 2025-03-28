import React, { useState, useEffect, useContext } from 'react';
import { LoanContext } from '../context/LoanContext';
import TakeLoan from './TakeLoan';
import LoanInfo from './LoanInfo';

const PhaseManager = () => {
    const [phase, setPhaseData] = useState(null);
    const { amount } = useContext(LoanContext);
    const [isFirstLoad, setIsFirstLoad] = useState(true);

    const fetchPhaseData = async (request) => {
        console.log("⏳ Fetching phase data...");

        const token = localStorage.getItem('token');       
        //this is not used?
        if (amount == null) {
            console.error("Amount is null, cannot fetch.");
            return;
        }

        try {
            console.log("🚀 Sending request", request);
            const response = await 
                fetch('https://localhost:5001/api/loan/next', {
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
                        ? componentMap[fetchedData.component]: null;
                    const loanData = fetchedData.loanData || null;
                    if (!ComponentToRender) {
                        console.error(`Component "
                        ${fetchedData.component}" not found.`);
                        return;
                    }     
                    setPhaseData({ component: ComponentToRender, 
                        props: { loan: loanData } });                 
                    const loanStatus = fetchedData.loanData?.status;
                    console.log("loanStatus:", loanStatus);
                } else {
                    console.error("Error:", fetchedData.msg);
                }
            } else {
                const errorData = await response.json();
                console.error("Error::", errorData);
                if (errorData.response.msg === "Amount error.") {
                    console.log("🚫 Amount error detected." + 
                        "Showing TakeLoan component.");
                    setPhaseData({ component: TakeLoan, 
                        props: { loan: null } });
                }
            }
        } catch (error) {
            console.error("Error fetching:", error);
        }
    };

    const handleFetchNextPhase = (amount) => {
        console.log
        ("Fetching next:", amount);
        fetchPhaseData({ Amount: amount});
       
    };

    useEffect(() => {
        if (isFirstLoad) {
            fetchPhaseData
            ({ init: { amount: 0 } });
            setIsFirstLoad(false);
        }
    }, [isFirstLoad]);

    if (!phase) {
        return <p>Loading phase</p>;
    }

    const ComponentToRender = phase.component;

    return (
        <div>
            <h2>Phase Manager</h2>
            <ComponentToRender {...phase.props} 
            onFetchNextPhase={handleFetchNextPhase} 
             />
        </div>
    );
};

export default PhaseManager;