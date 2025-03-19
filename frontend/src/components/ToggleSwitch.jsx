import React from 'react';

const ToggleSwitch = ({ isChecked, onToggle }) => {
  const styles = {
    switch: {
      position: 'relative',
      display: 'inline-block',
      width: '60px',
      height: '34px',
      margin: '10px'
    },
    slider: {
      position: 'absolute',
      cursor: 'pointer',
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      backgroundColor: isChecked ? 'rgb(0, 123, 255)' : '#ccc',
      transition: '.4s',
      borderRadius: '34px',
    },
    sliderBefore: {
      position: 'absolute',
      content: '""',
      height: '26px',
      width: '26px',
      left: '4px',
      bottom: '4px',
      backgroundColor: 'white',
      transition: '.4s',
      borderRadius: '50%',
      transform: isChecked ? 'translateX(26px)' : 'translateX(0)',
    },
  };

  return (
    <label style={styles.switch}>
        <input 
        type="checkbox" 
        checked={isChecked} 
        onChange={onToggle} 
        style={{ display: 'none' }} />
      <span style={styles.slider}>
        <span style={styles.sliderBefore}></span>
      </span>
    </label>
  );
};

export default ToggleSwitch;