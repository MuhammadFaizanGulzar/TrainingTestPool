import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import EditItemModal from './ItemEditList'; // Import the EditItemModal component
import './update.css';
import { useNavigate } from 'react-router-dom';

function UpdateItem() {
  const navigate = useNavigate();
  const { itemId } = useParams();
  const token = localStorage.getItem('token'); // Retrieve the authentication token

  const [formData, setFormData] = useState({
    name: '',
    description: '',
    // Add other fields as needed
  });
  const [items, setItems] = useState([]);
  const [showEditModal, setShowEditModal] = useState(false);
  const [selectedItem, setSelectedItem] = useState(null);
  const [successMessage, setSuccessMessage] = useState(null);

  useEffect(() => {
    if (!token) {
      // Token is missing, navigate to the login page
      navigate('/login'); // Assuming you have a login route
      return;
    }

    // Fetch the item to be updated by its ID and populate the form
    async function fetchItem() {
      try {
        const headers = {
          'Authorization': `Bearer ${token}`,
        };
        const response = await fetch(`https://localhost:7061/Item/updateItem/${itemId}`, {
          method: 'GET',
          headers: headers,
        });
        const itemData = await response.json();
        setFormData(itemData);
      } catch (error) {
        console.error('Error fetching item:', error);
      }
    }

    // Fetch the list of items
    async function fetchItemList() {
      try {
        const headers = {
          'Authorization': `Bearer ${token}`,
        };
        const response = await fetch('https://localhost:7061/Item/getAllItems', {
          method: 'GET',
          headers: headers,
        });
        const itemList = await response.json();
        setItems(itemList);
      } catch (error) {
        console.error('Error fetching item list:', error);
      }
    }

    fetchItem();
    fetchItemList();
  }, [itemId]);

  const handleEditClick = (item) => {
    setSelectedItem(item);
    setShowEditModal(true);
  };

  const handleCloseEditModal = () => {
    setShowEditModal(false);
  };

  const handleUpdateSuccess = async () => {
    try {
      const response = await fetch(`/api/items/${selectedItem.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData),
      });

      if (response.status === 200) {
        setSuccessMessage('Item updated successfully');
      } else {
        console.error('Error updating item:', response.status);
      }
    } catch (error) {
      console.error('Error updating item:', error);
    }
  };

  return (
    <div>
      <h2 style={{ color: "white" }}>Update Item</h2>
      {successMessage && <div style={{ color: 'green' }}>{successMessage}</div>}
      {/* ... (form and item list) */}
      <div>
        <h3>Item List</h3>
        <ul className="item-list">
          {items.map((item) => (
            <li key={item.id}>
              {item.name} - <button onClick={() => handleEditClick(item)}>Edit</button>
            </li>
          ))}
        </ul>
      </div>

      {/* Render the EditItemModal here */}
      {showEditModal && selectedItem && (
        <EditItemModal
          item={selectedItem}
          onClose={handleCloseEditModal}
          onUpdateSuccess={handleUpdateSuccess}
        />
      )}
    </div>
  );
}

export default UpdateItem;