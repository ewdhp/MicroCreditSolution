import React from "react";
import styled from "styled-components";

const StepperContainer = styled.div`
  display: flex;
  justify-content: center;
  margin-bottom: 20px;
`;

const Step = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  margin: 0 10px;
`;

const StepNumber = styled.div`
  width: 30px;
  height: 30px;
  border-radius: 50%;
  background-color: ${(props) => (props.$active ? "#007bff" : "lightgray")};
  color: ${(props) => (props.$active ? "white" : "black")};
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  transition: background-color 0.3s;
`;

const StepLabel = styled.div`
  margin-top: 5px;
  font-size: 12px;
`;

const Stepper = ({ steps, cs }) => {
  return (
    <StepperContainer>
      {steps.map((step, index) => (
        <Step key={index}>
          <StepNumber 
          $active={index === cs}>{index + 1}
          </StepNumber>
          <StepLabel>{step}</StepLabel>
        </Step>
      ))}
    </StepperContainer>
  );
};

export default Stepper;