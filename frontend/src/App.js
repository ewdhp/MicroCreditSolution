import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Navbar from './components/Navbar';
import Home from './views/Home';
import SignUp from './components/SignUp';
import Login from './components/Login';
import Dashboard from './views/Dashboard';
import DashboardLayout from './components/DashboardLayout';
import { AuthProvider, useAuth } from './context/AuthContext';

const ProtectedRoute = ({ element }) => {
  const { isAuthenticated } = useAuth();
  return isAuthenticated ? element : <Navigate to="/login" />;
};

const App = () => {
  return (
    <AuthProvider>
      <Router>
        <Navbar />
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/signup" element={<SignUp />} />
          <Route path="/login" element={<Login />} />
          <Route path="/dashboard" element={<ProtectedRoute element={<DashboardLayout />} />}>
            <Route index element={<Dashboard />} />
            <Route path="history" element={<div>Historial de prestamos</div>} />
            <Route path="profile" element={<div>Datos personales</div>} />
            <Route path="refer" element={<div>Gana $1000 gratis</div>} />
          </Route>
          <Route path="*" element={<Navigate to="/" />} /> {/* Redirect any unknown routes to home */}
        </Routes>
      </Router>
    </AuthProvider>
  );
};

export default App;