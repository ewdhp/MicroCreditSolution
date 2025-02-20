import React from 'react';
import ProfileForm from '../components/ProfileForm';

const Profile = () => {
  const styles = {
    container: {
      backgroundColor: '#fff',
      borderRadius: '8px',
      padding: '20px',
      boxShadow: '0 0 10px rgba(0, 0, 0, 0.1)',
      maxWidth: '800px',
      margin: '20px auto',
      textAlign: 'center',
    },
    heading: {
      fontSize: '2em',
      marginBottom: '20px',
    },
  };

  return (
    <div style={styles.container}>
      <h1 style={styles.heading}>Datos Personales</h1>
      <ProfileForm />
    </div>
  );
};

export default Profile;