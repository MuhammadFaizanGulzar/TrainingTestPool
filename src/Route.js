import React from 'react';
import Register from './components/Register/Register';
import CreateItem from './components/Create/CreateItem';
import ReadItems from './components/Read/ReadItems';
import UpdateItem from './components/Update/update';

import { createRoot } from "react-dom/client";
import {
  createBrowserRouter,
  RouterProvider,
  Route,
  Link,
} from "react-router-dom";
import DeleteItem from './components/Delete/delete';

const router = createBrowserRouter([
  {
    path: "/",
    element: <Register />,
  },
  {
    path: "/create",
    element: <CreateItem/>,
  },
  {
    path: "/read",
    element: <ReadItems/>,
  },
  {
    path: "/update",
    element: <UpdateItem/>,
  },
  {
    path: "/delete",
    element: <DeleteItem/>,
  },
]);

function Routes() {
  return (
    <RouterProvider router={router} />
  );
}

export default Routes;