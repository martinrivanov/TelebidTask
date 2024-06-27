import { useState } from "react"
import { validateEmail, validateName, validatePassword } from "../validation-service/validate";
import { useNavigate } from 'react-router-dom'
import axios from '../config/axios.js'
import { endpoints } from "../data/endpoints";
import { Navbar } from "./Navbar";

export const Register = () => {
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');

    const [generalError, setGeneralError] = useState({});
    const [errorsFirstName, setErrorsFirstName] = useState({});
    const [errorsLastName, setErrorsLastName] = useState({});
    const [errorsEmail, setErrorsEmail] = useState({});
    const [errorsPassword, setErrorsPassword] = useState({});

    const navigate = useNavigate();

    const handleFirstNameInput = (value) => {
        setFirstName(value);
        
        var errorText = validateName(value);
        setErrorsFirstName({error: errorText});
    };

    const handleLastNameInput = (value) => {
        setLastName(value);
        
        var errorText = validateName(value);
        setErrorsLastName({error: errorText});
    };

    const handleEmailInput = (value) => {
        setEmail(value);

        var errorText = validateEmail(value);
        setErrorsEmail({error: errorText});
    };

    const handlePasswordInput = (value) => {
        setPassword(value);
        
        var errorText = validatePassword(value);
        setErrorsPassword({error: errorText});
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        setGeneralError({});

        
        if(!(errorsFirstName.error || errorsLastName.error || errorsEmail.error || errorsPassword.error)){
            const userData = {
                name: firstName + ' ' + lastName,
                email: email,
                password: password
            };

            axios.post(endpoints.register, userData)
                 .then((res) => {
                     navigate('/login');
                 })
                 .catch((error) => {
                     setGeneralError({error: error.data.message})
                 })
        }
            
    };

    return (
        <>
            <Navbar />
            <div class="form">
                <form onSubmit={(e) => handleSubmit(e)}>
                    <input className="register-input" type="text" name="FirstName" id="first-name" placeholder="First Name" value={firstName} onChange={(e) => handleFirstNameInput(e.currentTarget.value)} required/>
                    <div className="error-message">{errorsFirstName.error}</div>
                    <input className="register-input" type="text" name="LastName" id="last-name" placeholder="Last Name" value={lastName} onChange={(e) => handleLastNameInput(e.currentTarget.value)} required/>
                    <div className="error-message">{errorsLastName.error}</div>
                    <input className="register-input" type="email" name="Email" id="email" placeholder="Email" value={email} onChange={(e) => handleEmailInput(e.currentTarget.value)} required/>
                    <div className="error-message">{errorsEmail.error}</div>
                    <input className="register-input" type="password" name="Password" id="password" placeholder="Password" value={password} onChange={(e) => handlePasswordInput(e.currentTarget.value)} required/>
                    <div className="error-message">{errorsPassword.error}</div>
                    <button className="register-btn" type="submit">Register</button>
                </form>
            </div>
            <div class="error-message">{generalError.error}</div>
        </>
    )
}