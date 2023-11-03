import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './CreateItem.css';

function CreateItem() {
  const initialFormData = {
    name: '',
    description: '',
    // Add other fields as needed
  };

  const [formData, setFormData] = useState(initialFormData);
  const [itemCreated, setItemCreated] = useState(false);
  const navigate = useNavigate();
  const token = localStorage.getItem('token'); // Retrieve the authentication token

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!token) {
      // Token is missing, navigate to the login page
      navigate('/login'); // Assuming you have a login route
      return;
    }

    try {
      const headers = {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      };

      const response = await fetch('https://localhost:7061/Item/createItem', {
        method: 'POST',
        headers: headers,
        body: JSON.stringify(formData),
      });

      if (response.status === 200) {
        setItemCreated(true);
        setFormData(initialFormData); // Reset the form fields
        const data = await response.json();
        console.log('Item created:', data);
      } else {
        console.error('Error creating item:', response.status);
      }
    } catch (error) {
      console.error('Error creating item:', error);
    }
  };

  return (
    <div>
      <h2 style={{ color: 'white' }}>Create Item</h2>
      {itemCreated ? (
        <div>
          <p style={{ color: 'green' }}>Item created successfully!</p>
          <button onClick={() => setItemCreated(false)}>Create Another Item</button>
        </div>
      ) : (
        <form onSubmit={handleSubmit}>
          <input type="text" name="name" placeholder="Name" value={formData.name} onChange={handleChange} />
          <input type="text" name="description" placeholder="Description" value={formData.description} onChange={handleChange} />
          {/* Add other input fields as needed */}
          <button type="submit">Create</button>
        </form>
      )}
    </div>
  );
}

export default CreateItem;