import React, { useState, useEffect, useContext } from 'react';
import { LoanContext } from '../context/LoanContext';

const TakeLoan = ({ loan, onFetchNextPhase }) => {
    const { amount, setAmount } = useContext(LoanContext); // Use context for amount and setAmount
    const [sliderValue, setSliderValue] = useState(amount || 0); // Initialize slider value

    // Synchronize sliderValue with amount from LoanContext when the component is rendered
    useEffect(() => {
        if (amount !== sliderValue) {
            setSliderValue(amount || 0);
        }
    }, [amount]);

    const handleSliderChange = (value) => {
        setSliderValue(value); // Update the slider value
        setAmount(value); // Update the amount in LoanContext
    };

    return (
        <div>
            <h3>Take a Loan</h3>

            {/* Slider to adjust the amount */}

                <div>
                    <label htmlFor="amount-slider">Loan Amount: {sliderValue}</label>
                    <input
                        id="amount-slider"
                        type="range"
                        min="200"
                        max="1000"
                        step="50"
                        value={sliderValue}
                        onChange={(e) => handleSliderChange(Number(e.target.value))} // Update slider value dynamically
                    />
                    <button onClick={() => onFetchNextPhase(sliderValue)}>Fetch Next Phase</button>
                </div>
        </div>
    );
};

export default TakeLoan;