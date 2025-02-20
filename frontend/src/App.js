import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Navbar from './components/Navbar';
import Home from './views/Home';
import SignUp from './components/SignUp';
import Login from './components/Login';
import Dashboard from './views/Dashboard';
import DashboardLayout from './components/DashboardLayout';
import History from './views/History';
import Profile from './views/Profile';
import Refer from './views/Refer';
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
            <Route path="history" element={<History />} />
            <Route path="profile" element={<Profile />} />
            <Route path="refer" element={<Refer />} />
          </Route>
          <Route path="*" element={<Navigate to="/" />} /> {/* Redirect any unknown routes to home */}
        </Routes>
      </Router>
    </AuthProvider>
  );
};

export default App;