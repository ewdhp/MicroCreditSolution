import React, { useState, useEffect, useContext, useCallback } from 'react';
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
        debounceSetAmount(value);
    };

    // Debounce function to delay updates to setAmount
    const debounceSetAmount = useCallback(
        debounce((value) => {
            setAmount(value);
        }, 200),
        []
    );

    // Inline styles
    const styles = {
        container: {
            maxWidth: '300px',
            margin: '0px auto',
            padding: '10px',
            border: '1px solid #ccc',
            borderRadius: '8px',
            boxShadow: '0 4px 8px rgba(0, 0, 0, 0.1)',
            textAlign: 'center',
        },
        title: {
            fontSize: '1.5rem',
            marginBottom: '5px',
            color: 'black',
        },
        sliderContainer: {
            margin: '20px 0',
        },
        slider: {
            width: '100%',
        },
        amountText: {

            margin: '10px 0',

        },
        button: {
            marginTop: '10px',
            padding: '10px 20px',
            fontSize: '1rem',
            color: '#fff',
            backgroundColor: '#007bff',
            border: 'none',
            borderRadius: '4px',
            cursor: 'pointer',
        },
        buttonHover: {
            backgroundColor: '#0056b3',
        },
    };

    return (
        <div style={styles.container}>
            <div style={styles.title}>
                <p style={styles.title}>Toma un Prestamo</p>
            </div>
            <div style={styles.sliderContainer}>
                <p>Contrato por 7 dias</p>
                <input
                    id="amount-slider"
                    type="range"
                    min="100"
                    max="300"
                    step="1"
                    value={sliderValue}
                    onChange={(e) => handleSliderChange(Number(e.target.value))}
                    style={styles.slider}
                />
                <p style={styles.amountText} htmlFor="amount-slider">
                    Cantidad $ {sliderValue}
                </p>
                <p style={styles.amountText}>
                    Interes diario $ {(sliderValue * 0.9 / 7).toFixed(2)}
                </p>
                <button
                    style={styles.button}
                    onMouseOver={(e) => (e.target.style.backgroundColor = styles.buttonHover.backgroundColor)}
                    onMouseOut={(e) => (e.target.style.backgroundColor = styles.button.backgroundColor)}
                    onClick={() => onFetchNextPhase(sliderValue)}
                >
                    Aceptar
                </button>
            </div>
        </div>
    );
};

// Utility function for debouncing
function debounce(func, wait) {
    let timeout;
    return (...args) => {
        clearTimeout(timeout);
        timeout = setTimeout(() => func(...args), wait);
    };
}

export default TakeLoan;