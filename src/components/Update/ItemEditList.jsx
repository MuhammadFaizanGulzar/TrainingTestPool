import React, { useState, useEffect } from 'react';

function EditItemModal({ item, onClose }) {
  const [formData, setFormData] = useState({
    name: item.name || '', // Initialize with item's existing data
    description: item.description || '', // Initialize with item's existing data
    // Add other fields as needed
  });

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await fetch(`https://localhost:7061/Item/updateItem/${item.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData),
      });
      if (response.status === 200) {
        console.log('Item updated successfully');
        onClose(); // Close the modal after successful update
      } else {
        console.error('Error updating item:', response.status);
      }
    } catch (error) {
      console.error('Error updating item:', error);
    }
  };

  return (
    <div className="modal">
      <div className="modal-content">
        <h2>Edit Item</h2>
        <form onSubmit={handleSubmit}>
          <input type="text" name="name" placeholder="Name" value={formData.name} onChange={handleChange} />
          <input type="text" name="description" placeholder="Description" value={formData.description} onChange={handleChange} />
          {/* Add other input fields as needed */}
          <button type="submit">Update</button>
        </form>
        <button onClick={onClose}>Close</button>
      </div>
    </div>
  );
}

export default EditItemModal;