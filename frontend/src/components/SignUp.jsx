import React, { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import Stepper from "./Stepper";
import Loader from "./Loader";
import Alert from "./Alert";
import { useAuth } from "../context/AuthContext";

const SignUp = () => {
  const [formData, setFormData] = useState({ phoneNumber: "", name: "" });
  const [verificationCode, setVerificationCode] = useState("");
  const [currentStep, setCurrentStep] = useState(0);
  const [isLoading, setIsLoading] = useState(false);
  const [alert, setAlert] = useState({ type: "", message: "", isModal: false, useTransparency: false });
  const [attempts, setAttempts] = useState(0);
  const [countdown, setCountdown] = useState(30);
  const [canResend, setCanResend] = useState(false);
  const navigate = useNavigate();
  const { login } = useAuth();

  const steps = ["Ingresar Información", "Verificar Código", "Éxito"];

  const handleChange = (e) => {
    console.log("handleChange called with:", e.target.name, e.target.value);
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleCodeChange = (e) => {
    console.log("handleCodeChange called with:", e.target.value);
    setVerificationCode(e.target.value);
  };

const handleSubmitStep0 = async (e) => {
    e.preventDefault();
    console.log("handleSubmitStep0 called with formData:", formData);
    const phoneNumberPattern = /^\+\d{12}$/;

    if (!phoneNumberPattern.test(formData.phoneNumber)) {
        console.log("Invalid phone number format");
        setAlert({ type: "error", message: "Formato de número de teléfono inválido. Debe ser un número de 10 dígitos.", isModal: true, useTransparency: false });
        return;
    }

    if (formData.name.trim() === "") {
        console.log("Name cannot be empty");
        setAlert({ type: "error", message: "El nombre no puede estar vacío.", isModal: true, useTransparency: false });
        return;
    }

    setIsLoading(true);

    try {
        const response = await axios.post("https://localhost:5001/api/auth/signup", {
            phoneNumber: formData.phoneNumber,
            name: formData.name
        }, {
            withCredentials: true
        });

        if (response.status === 200) {
            console.log("Verification code sent:", response.data.code);
            setCurrentStep(1);
            setCountdown(30);
            setCanResend(false);
        } else {
            console.log("Error sending verification code:", response.data.message);
            setAlert({ type: "error", message: response.data.message || "Error al enviar el código. Por favor, inténtelo de nuevo.", isModal: true, useTransparency: false });
        }
    } catch (error) {
        console.log("Network error:", error);
        setAlert({ type: "error", message: "Error de red. Por favor, inténtelo de nuevo.", isModal: true, useTransparency: false });
    } finally {
        setIsLoading(false);
    }
};

  const handleSubmitStep1 = async (e) => {
    e.preventDefault();
    console.log("handleSubmitStep1 called with verificationCode:", verificationCode);
    setIsLoading(true);

    try {
      const response = await axios.post("https://localhost:5001/api/auth/verify", {
        phoneNumber: formData.phoneNumber,
        code: verificationCode
      });

      if (response.status === 200) {
        console.log("Verification successful, token:", response.data.token);
        localStorage.setItem("authToken", response.data.token);
        login(response.data.token); // Update authentication state
        setCurrentStep(2);
      } else {
        console.log("Invalid verification code:", response.data.message);
        const newAttempts = attempts + 1;
        setAttempts(newAttempts);

        if (newAttempts < 3) {
          setAlert({ type: "error", message: response.data.message || `Código inválido. Intentos restantes: ${3 - newAttempts}`, isModal: true, useTransparency: false });
        } else {
          setAlert({ type: "error", message: response.data.message || "Demasiados intentos fallidos. Volviendo al primer paso.", isModal: true, useTransparency: false });
          setCurrentStep(0);
          setAttempts(0);
        }

        // Limpiar el campo de entrada para asegurar que handleCodeChange se dispare
        setVerificationCode("");
      }
    } catch (error) {
      console.log("Network error:", error);
      setAlert({ type: "error", message: "Error de red. Por favor, inténtelo de nuevo.", isModal: true, useTransparency: false });
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    if (currentStep === 2) {
      console.log("Registration successful, redirecting to dashboard");
      // Redirigir al dashboard después de un registro exitoso
      setTimeout(() => navigate("/dashboard"), 500);
    }
  }, [currentStep, navigate]);

  useEffect(() => {
    let timer;
    if (currentStep === 1 && countdown > 0) {
      timer = setTimeout(() => setCountdown(countdown - 1), 1000);
    } else if (countdown === 0) {
      setCanResend(true);
    }
    return () => clearTimeout(timer);
  }, [currentStep, countdown]);

  const handleResendCode = async () => {
    console.log("handleResendCode called");
    setIsLoading(true);
    setCanResend(false);
    setCountdown(30);

    try {
      const response = await axios.post("https://localhost:5001/api/auth/signup", {
        phoneNumber: formData.phoneNumber,
        name: formData.name
      });

      if (response.status === 200) {
        console.log("Verification code resent:", response.data.code);
      } else {
        console.log("Error resending verification code:", response.data.message);
        setAlert({ type: "error", message: response.data.message || "Error al reenviar el código. Por favor, inténtelo de nuevo.", isModal: true, useTransparency: false });
      }
    } catch (error) {
      console.log("Network error:", error);
      setAlert({ type: "error", message: "Error de red. Por favor, inténtelo de nuevo.", isModal: true, useTransparency: false });
    } finally {
      setIsLoading(false);
    }
  };

  const handleEvent = () => {
    console.log("handleEvent called");
    setAlert({ type: "", message: "", isModal: false, useTransparency: false });
    if (alert.message.includes("Demasiados intentos fallidos")) {
      setCurrentStep(0);
      setAttempts(0);
    }
  };

  const goBack = () => {
    console.log("goBack called");
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
          {!canResend && <p>Reenviar código en {countdown} segundos</p>}
          {canResend && <button type="button" onClick={handleResendCode}>Reenviar Código</button>}
        </form>
      )}

      {currentStep === 2 && <h2>✅ ¡Registro Exitoso!</h2>}
    </div>
  );
};

export default SignUp;