import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import ItemList from './ItemList';
import './delete.css';

function DeleteItem() {
  const navigate = useNavigate();
  const [items, setItems] = useState([]);
  const [deletedItem, setDeletedItem] = useState(null);
  const token = localStorage.getItem('token'); // Retrieve the authentication token

  useEffect(() => {
    if (!token) {
      // Token is missing, navigate to the login page
      navigate('/login'); // Assuming you have a login route
      return;
    }

    // Fetch the list of items from your API with the token in the headers
    async function fetchItems() {
      try {
        const headers = {
          'Authorization': `Bearer ${token}`,
        };
        const response = await fetch('https://localhost:7061/Item/getAllItems', {
          method: 'GET',
          headers: headers,
        });
        const data = await response.json();
        setItems(data);
      } catch (error) {
        console.error('Error fetching items:', error);
      }
    }

    fetchItems();
  }, [token]);

  const handleDelete = async (itemId) => {
    try {
      const headers = {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      };

      const response = await fetch(`https://localhost:7061/Item/deleteItem/${itemId}`, {
        method: 'DELETE',
        headers: headers,
      });

      if (response.status === 200) {
        console.log('Item deleted');
        setDeletedItem(itemId); // Store the deleted item ID
        // Optionally, you can refresh the list of items after deletion
        setItems(items.filter((item) => item.id !== itemId)); // Remove the deleted item from the list
      } else {
        console.error('Error deleting item:', response.status);
      }
    } catch (error) {
      console.error('Error deleting item:', error);
    }
  };

  return (
    <div>
      <h2>Delete Item</h2>
      {deletedItem ? (
        <p>Item with ID {deletedItem} has been successfully deleted.</p>
      ) : (
        <p>Select an item to delete:</p>
      )}
      <ItemList items={items} handleDelete={handleDelete} />
    </div>
  );
}

export default DeleteItem;