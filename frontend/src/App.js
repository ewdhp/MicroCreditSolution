import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Navbar from './components/Navbar';
import Home from './views/Home';
import LoginPage from './components/login/Login';
import Dashboard from './views/Dashboard';
import Layout from './components/Layout';
import History from './views/History';
import Refer from './views/Refer';

import { AuthProvider, useAuth } from './context/AuthContext';
import './App.css';

// ProtectedRoute to handle authentication
const ProtectedRoute = ({ children }) => {
  const { isAuthenticated } = useAuth();
  return isAuthenticated ? children : <Navigate to="/login" />;
};

const App = () => {
  const styles = {
    navbar: {},
    mainContent: {
      marginTop: '65px', // Adjust this value based on the height of your Navbar
      backgroundColor: 'rgb(255, 255, 255)',
    },
  };

  return (
    <Router>
      <AuthProvider>
        <Navbar style={styles.navbar} />
        <div style={styles.mainContent}>
          <Routes>
            {/* Public Routes */}
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/signup" element={<LoginPage />} />

            {/* Protected Routes */}
            <Route
              path="/dashboard"
              element={
                <ProtectedRoute>
                  <Layout />
                </ProtectedRoute>
              }
            >
              <Route index element={<Dashboard />} />
              <Route path="history" element={<History />} />
              <Route path="refer" element={<Refer />} />
            </Route>

            {/* Catch-All Route */}
            <Route path="*" element={<Navigate to="/" />} />
          </Routes>
        </div>
      </AuthProvider>
    </Router>
  );
};

export default App;