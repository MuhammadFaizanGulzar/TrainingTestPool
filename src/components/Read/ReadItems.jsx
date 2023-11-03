import React, { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import './ReadItem.css';

function ReadItems() {
  const navigate = useNavigate();
  const [items, setItems] = useState([]);

  useEffect(() => {
    async function fetchItems() {
      const token = localStorage.getItem('token');
      if (!token) {
        // Token is missing, navigate to the login page
        navigate('/login'); // Assuming you have a login route
        return;
      }
  
      try {
        const response = await fetch('https://localhost:7061/Item/getAllItems', {
          method: "GET",
          headers: {
            'Authorization': `Bearer ${token}`,
          },
        });
        if (response.status === 401) {
          // Unauthorized, handle reauthentication or other actions
          navigate('/login');
        } else {
          const data = await response.json();
          setItems(data);
        }
      } catch (error) {
        console.error('Error fetching items:', error);
      }
    }
  
    fetchItems();
  }, []);

  return (
    <div className="read-items">
      <h2>Items</h2>
      <ul>
        {items.map((item) => (
          <li key={item.id}>
            {item.name} - {item.description}
            {/* Display other item details */}
            <div>
              {/* Link to create a new item */}
              <Link to="/create">Create Item</Link>
              {/* Link to update the current item */}
              <Link to={`/update`}>Update</Link>
              {/* Link to delete the current item */}
              <Link to={`/delete`}>Delete</Link>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
}

export default ReadItems;





