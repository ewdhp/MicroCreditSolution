import React from "react";

const Loader = () => {
  const loaderContainerStyle = {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    height: '50px',
  };

  const loaderStyle = {
    width: '30px',
    height: '30px',
    border: '3px solid transparent',
    borderTop: '3px solid #007bff',
    borderRadius: '50%',
    animation: 'spin 1s linear infinite',
  };

  const keyframes = `
    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }
  `;

  return (
    <div style={loaderContainerStyle}>
      <div style={loaderStyle}></div>
      <style>{keyframes}</style>
    </div>
  );
};

export default Loader;
