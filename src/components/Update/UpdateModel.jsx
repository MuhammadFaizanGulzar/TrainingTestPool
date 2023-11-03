import React, { useState } from 'react';
import Modal from 'react-modal';


function EditModal({ isOpen, onRequestClose, formData, onChange, onSubmit }) {
  return (
    <Modal
      isOpen={isOpen}
      onRequestClose={onRequestClose}
      contentLabel="Edit Item"
    >
      <h2>Edit Item</h2>
      <form onSubmit={onSubmit}>
        <input
          type="text"
          name="name"
          placeholder="Name"
          value={formData.name}
          onChange={onChange}
        />
        <input
          type="text"
          name="description"
          placeholder="Description"
          value={formData.description}
          onChange={onChange}
        />
        {/* Add other input fields as needed */}
        <button type="submit">Update</button>
      </form>
    </Modal>
  );
}

export default EditModal;