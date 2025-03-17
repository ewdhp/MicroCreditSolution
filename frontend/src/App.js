import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Navbar from './components/Navbar';
import Home from './views/Home';
import Login from './components/Login';
import Dashboard from './views/Dashboard';
import DashboardLayout from './components/DashboardLayout';
import History from './views/History';
import Refer from './views/Refer';
import Signup from './components/Signup';

import { AuthProvider, useAuth } from './context/AuthContext';
import './App.css';

const ProtectedRoute = ({ element }) => {
  const { isAuthenticated } = useAuth();
  return isAuthenticated ? element : <Navigate to="/login" />;
};

const App = () => {
  const styles = {
    navbar: {},
    mainContent: {
      marginTop: '65px', // Adjust this value based on the height of your Navbar
    },
  };

  return (
    <AuthProvider>
      <Router>
        <Navbar style={styles.navbar} />
        <div style={styles.mainContent}>
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />
            <Route path="/signup" element={<Signup />} />
            <Route path="/dashboard" element={<ProtectedRoute element={<DashboardLayout />} />}>
              <Route index element={<Dashboard />} />
              <Route path="history" element={<History />} />
              <Route path="refer" element={<Refer />} />
            </Route>
            <Route path="*" element={<Navigate to="/" />} />
          </Routes>
        </div>
      </Router>
    </AuthProvider>
  );
};

export default App;