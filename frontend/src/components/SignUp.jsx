import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import Stepper from "./Stepper";
import Loader from "./Loader";
import Alert from "./Alert";

const SignUp = () => {
  const [formData, setFormData] = useState({ phoneNumber: "", name: "" });
  const [verificationCode, setVerificationCode] = useState("");
  const [currentStep, setCurrentStep] = useState(0);
  const [isLoading, setIsLoading] = useState(false);
  const [alert, setAlert] = useState({ type: "", message: "", isModal: false, useTransparency: false });
  const [attempts, setAttempts] = useState(0);
  const navigate = useNavigate();

  const steps = ["Ingresar Información", "Verificar Código", "Éxito"];

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleCodeChange = (e) => {
    setVerificationCode(e.target.value);
  };

  const handleSubmitStep0 = (e) => {
    e.preventDefault();
    const phoneNumberPattern = /^\d{10}$/;

    if (!phoneNumberPattern.test(formData.phoneNumber)) {
      setAlert({ type: "error", message: "Formato de número de teléfono inválido. Debe ser un número de 10 dígitos.", isModal: true, useTransparency: false });
      return;
    }

    if (formData.name.trim() === "") {
      setAlert({ type: "error", message: "El nombre no puede estar vacío.", isModal: true, useTransparency: false });
      return;
    }

    setIsLoading(true);

    // Simular llamada al backend
    setTimeout(() => {
      setCurrentStep(1); // Avanzar al siguiente paso sin mostrar una alerta de éxito
      setIsLoading(false);
    }, 300);
  };

  const handleSubmitStep1 = (e) => {
    e.preventDefault();
    setIsLoading(true);

    // Simular llamada al backend
    setTimeout(() => {
      if (verificationCode === "123456") { // Código de verificación simulado
        setCurrentStep(2); // Avanzar al siguiente paso sin mostrar una alerta de éxito
      } else {
        const newAttempts = attempts + 1;
        setAttempts(newAttempts);

        console.log(`Intentos: ${newAttempts}`);

        if (newAttempts < 3) {
          setAlert({ type: "error", message: `Código inválido. Intentos restantes: ${3 - newAttempts}`, isModal: true, useTransparency: false });
        } else {
          setAlert({ type: "error", message: "Demasiados intentos fallidos. Volviendo al primer paso.", isModal: true, useTransparency: false });
          setCurrentStep(0);
          setAttempts(0);
        }

        // Limpiar el campo de entrada para asegurar que handleCodeChange se dispare
        setVerificationCode("");
      }
      setIsLoading(false);
    }, 300);
  };

  useEffect(() => {
    if (currentStep === 2) {
      // Redirigir al dashboard después de un registro exitoso
      setTimeout(() => navigate("/dashboard"), 500);
    }
  }, [currentStep, navigate]);

  const handleEvent = () => {
    setAlert({ type: "", message: "", isModal: false, useTransparency: false });
    if (alert.message.includes("Demasiados intentos fallidos")) {
      setCurrentStep(0);
      setAttempts(0);
    }
  };

  const goBack = () => {
    setCurrentStep(0); // Reiniciar al primer paso
    setAlert({ type: "", message: "", isModal: false, useTransparency: false });
    setAttempts(0);
  };

  return (
    <div>
      <h1>Registrarse</h1>
      <Stepper steps={steps} currentStep={currentStep} />

      {alert.message && (
        <Alert
          type={alert.type}
          message={alert.message}
          onEvent={handleEvent}
          useTransparency={alert.useTransparency}
        />
      )}

      {currentStep === 0 && (
        <form onSubmit={handleSubmitStep0}>
          <div>
            <label>Número de Teléfono</label>
            <input 
              type="text" 
              name="phoneNumber" 
              value={formData.phoneNumber} 
              onChange={handleChange}
            />
          </div>
          <div>
            <label>Nombre</label>
            <input type="text" name="name" value={formData.name} onChange={handleChange} />
          </div>
          <button type="submit" disabled={isLoading}>Registrarse</button>
          {isLoading && <Loader />}
        </form>
      )}

      {currentStep === 1 && (
        <form onSubmit={handleSubmitStep1}>
          <div>
            <label>Código de Verificación</label>
            <input type="text" value={verificationCode} onChange={handleCodeChange} />
          </div>
          <button type="submit" disabled={isLoading}>Verificar Código</button>
          {isLoading && <Loader />}
        </form>
      )}

      {currentStep === 2 && <h2>✅ ¡Registro Exitoso!</h2>}
    </div>
  );
};

export default SignUp;
