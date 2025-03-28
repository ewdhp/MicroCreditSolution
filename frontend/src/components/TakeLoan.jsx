import React, { useState, useEffect, useContext } from 'react';
import { LoanContext } from '../context/LoanContext';

const TakeLoan = ({ loan=null, onFetchNextPhase }) => {
    const { amount, setAmount } = useContext(LoanContext);
    const [sliderValue, setSliderValue] = useState(amount || 0);

    useEffect(() => {
        if (amount !== sliderValue) {
            setSliderValue(amount || 0);
        }
    }, [amount]);

    const handleSliderChange = (value) => {
        setSliderValue(value); 
        setAmount(value);
    };

    return (
        <div><h3>Toma un credito</h3>
                <div>
                    <label htmlFor="amount-slider">
                        Cantidad: {sliderValue}</label>
                    <input
                        id="amount-slider"
                        type="range"
                        min="100"
                        max="300"
                        step="50"
                        value={sliderValue}
                        onChange={(e) => handleSliderChange(Number(e.target.value))}
                    />
                    <button onClick={() => onFetchNextPhase(sliderValue)}>
                        Aceptar
                    </button>
                </div>
        </div>
    );
};

export default TakeLoan;