import './Register.css';
import user_icon from '../Assets/user_icon.png';
import email_icon from '../Assets/email_icon.png';
import password_icon from '../Assets/password_icon.png';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

const Register = () => {
  const navigate = useNavigate();

  const [action, setAction] = useState("Sign Up");
  const [email, setEmail] = useState(""); // Add state for email
  const [username, setUsername] = useState(""); // Add state for username
  const [password, setPassword] = useState(""); // Add state for password

  const handleRegister = async () => {
    try {
      const response = await fetch('https://localhost:7061/Auth/RegisterUser', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email, username, password }),
      });

      if (response.status === 200) {
        console.log('Registration successful');
        // Optionally, perform actions after successful registration

        // Redirect to the 'read' page after successful registration
        navigate('/read');
      } else {
        console.error('Registration failed');
        // Optionally, handle registration failure
      }
    } catch (error) {
      console.error('Error registering:', error);
    }
  };

  const handleLogin = async () => {
    try {
      const response = await fetch('https://localhost:7061/Auth/Login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ username, password }),
      });

      if (response.status === 200) {
        const data = await response.json();
        const token = data.token; // Assuming your API returns the token in a 'token' field

        // Store the token in localStorage
        localStorage.setItem('token', token);

        // Redirect to the 'read' page after successful login
        navigate('/read');
      } else {
        console.error('Login failed');
        // Optionally, handle login failure
      }
    } catch (error) {
      console.error('Error logging in:', error);
    }
  };

  return (
    <div className='container'>
      <div className='header'>
        <div className='text'>{action}</div>
        <div className='underline'></div>
      </div>
      <div className='inputs'>
        {action === "Login" ? (
          <div></div>
        ) : (
          <div className="input">
            <img src={email_icon} alt="" />
            <input
              type='email'
              placeholder='Email'
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
          </div>
        )}

        <div className="input">
          <img src={user_icon} alt="" />
          <input
            type='text'
            placeholder='UserName'
            value={username}
            onChange={(e) => setUsername(e.target.value)}
          />
        </div>

        <div className="input">
          <img src={password_icon} alt="" />
          <input
            type='password'
            placeholder='Password'
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>
        {action === "Sign Up" ? (
          <div></div>
        ) : (
          <div className='forgot-password'>
            Lost Password? <span>Click Here!</span>
          </div>
        )}

        <div className='submit-container'>
          <div
            className={action === "Login" ? "submit gray" : "submit"}
            onClick={action === "Sign Up" ? handleRegister : handleLogin}
          >
            {action}
          </div>
          <div
            className={action === "Sign Up" ? "submit gray" : "submit"}
            onClick={() => {
              setAction(action === "Sign Up" ? "Login" : "Sign Up");
            }}
          >
            {action === "Sign Up" ? "Login" : "Sign Up"}
          </div>
        </div>
      </div>
    </div>
  );
};

export default Register;