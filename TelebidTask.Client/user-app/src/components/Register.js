import { useState } from "react"
import { validateEmail, validateName, validatePassword } from "../validation-service/validate";

export const Register = () => {
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');

    const [errorsFirstName, setErrorsFirstName] = useState({});
    const [errorsLastName, setErrorsLastName] = useState({});
    const [errorsEmail, setErrorsEmail] = useState({});
    const [errorsPassword, setErrorsPassword] = useState({});

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
        
        if(errorsFirstName.error || errorsLastName.error || errorsEmail.error || errorsPassword.error)
            console.log("Invalid form");

        else
            console.log("Valid form");
    };

    return (
        <>
            <div class="form">
                <form onSubmit={(e) => handleSubmit(e)}>
                    <input type="text" name="FirstName" id="first-name" placeholder="First Name" value={firstName} onChange={(e) => handleFirstNameInput(e.currentTarget.value)} required/>
                    <input type="text" name="LastName" id="last-name" placeholder="Last Name" value={lastName} onChange={(e) => handleLastNameInput(e.currentTarget.value)} required/>
                    <input type="email" name="Email" id="email" placeholder="Email" value={email} onChange={(e) => handleEmailInput(e.currentTarget.value)} required/>
                    <input type="password" name="Password" id="password" placeholder="Password" value={password} onChange={(e) => handlePasswordInput(e.currentTarget.value)} required/>
                    <button type="submit">Register</button>
                </form>
            </div>
            <div class="error-message">{errorsFirstName.error}</div>
            <div class="error-message">{errorsLastName.error}</div>
            <div class="error-message">{errorsEmail.error}</div>
            <div class="error-message">{errorsPassword.error}</div>
        </>
    )
}