import React, { useState, useEffect } from "react";
import ReactDOM from "react-dom";

const modalStyles = {
  overlay: {
    position: "fixed",
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    backgroundColor: "rgba(0, 0, 0, 0.5)",
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
  },
  modal: {
    backgroundColor: "#fff",
    padding: "20px",
    borderRadius: "5px",
    maxWidth: "300px",
    textAlign: "center",
    transition: "opacity 1s ease-out",
    opacity: 1,
  },
  content: {
    opacity: 1,
  },
  buttonContainer: {
    marginTop: "20px",
    display: "flex",
    justifyContent: "center",
  },
};

const Alert = ({ type, message, onEvent, useTransparency }) => {
  const [modalOpacity, setModalOpacity] = useState(1);

  const handleFadeOut = () => {
    setModalOpacity(0);
  };

  useEffect(() => {
    if (useTransparency && modalOpacity === 0) {
      const timer = setTimeout(onEvent, 2000);
      return () => clearTimeout(timer);
    }
  }, [useTransparency, modalOpacity, onEvent]);

  useEffect(() => {
    if (useTransparency) {
      handleFadeOut();
    }
  }, [useTransparency]);

  return ReactDOM.createPortal(
    <div style={modalStyles.overlay}>
      <div style={{ ...modalStyles.modal, opacity: modalOpacity }}>
        <div style={modalStyles.content}>
          <p>{message}</p>
        </div>
        {!useTransparency && (
          <div style={modalStyles.buttonContainer}>
            <button 
            onClick={onEvent}>
            {type === "error" ? "Back" : "Close"}
            </button>
          </div>
        )}
      </div>
    </div>,
    document.body
  );
};

export default Alert;
