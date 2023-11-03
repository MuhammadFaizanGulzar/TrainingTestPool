import React from 'react';

function ItemList({ items, handleDelete }) {
  return (
    <div>
      <h2>Item List</h2>
      <ul className="item-list">
        {items.map((item) => (
            <li key={item.id}>
            <span>{item.name}</span>
            <button onClick={() => handleDelete(item.id)}>Delete</button>
            </li>
        ))}
        </ul>
    </div>
  );
}

export default ItemList;