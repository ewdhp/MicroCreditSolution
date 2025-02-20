import React, { useState } from 'react';
import FormInput from './FormInput';
import Button from './Button';

const ProfileForm = () => {
  const [formData, setFormData] = useState({ name: '', email: '' });

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    // Handle form submission
    console.log('Form submitted:', formData);
  };

  const styles = {
    form: {
      textAlign: 'left',
    },
  };

  return (
    <form onSubmit={handleSubmit} style={styles.form}>
      <FormInput label="Nombre" type="text" name="name" value={formData.name} onChange={handleChange} />
      <FormInput label="Correo Electrónico" type="email" name="email" value={formData.email} onChange={handleChange} />
      <Button>Actualizar</Button>
    </form>
  );
};

export default ProfileForm;