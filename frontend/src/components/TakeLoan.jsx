import React, { useState, useEffect, useContext } from 'react';
import { LoanContext } from '../context/LoanContext';

const TakeLoan = ({ loan = null, onFetchNextPhase }) => {
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
        <div>
            <h3>Toma un prestamo</h3>
            <div>
                <p>Contrato por 7 dias</p>
                
                <input
                    id="amount-slider"
                    type="range"
                    min="100"
                    max="300"
                    step="50"
                    value={sliderValue}
                    onChange={(e) => handleSliderChange(Number(e.target.value))}
                />
                <p htmlFor="amount-slider">
                    $ {sliderValue}
                </p>
                <p>
                    Interes diario $ {sliderValue * 0.1}
                </p>

                <button onClick={() => onFetchNextPhase(sliderValue)}>
                    Aceptar
                </button>
            </div>
        </div>
    );
};

export default TakeLoan;