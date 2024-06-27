import axios from "../config/axios.js";
import { useEffect, useRef, useState } from "react";
import { useLocation, useNavigate } from "react-router";
import { endpoints } from "../data/endpoints";
import { drawCaptcha, generateCaptchaText } from "../captcha-service/captcha.js";
import { validateEmail, validateName, validatePassword } from "../validation-service/validate.js";


export const UserDashboard = () => {
    const navigate = useNavigate();

    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [email, setEmail] = useState('');

    const [updateContainerFirstName, setUpdateContainerFirstName] = useState(false);
    const [updateContainerLastName, setUpdateContainerLastName] = useState(false);
    const [updateContainerEmail, setUpdateContainerEmail] = useState(false);
    const [updateContainerPassword, setUpdateContainerPassword] = useState(false);

    const [errorsFirstName, setErrorsFirstName] = useState({});
    const [errorsLastName, setErrorsLastName] = useState({});
    const [errorsEmail, setErrorsEmail] = useState({});
    const [errorsPassword, setErrorsPassword] = useState({});

    const [updateFirstName, setUpdateFirstName] = useState("");
    const [updateLastName, setUpdateLastName] = useState("");
    const [updateEmail, setUpdateEmail] = useState("");
    const [updatePassword, setUpdatePassword] = useState("");

    useEffect(() => {
        if(localStorage.getItem("userId") === null){
            navigate('/login');
        }

        else{
            const expirationDate = new Date(localStorage.getItem("expirationDate"));
            const currentTime = new Date();

            if(expirationDate.getTime() <= currentTime.getTime()){
                localStorage.removeItem("userId");
                localStorage.removeItem("expirationDate");
                navigate('/login');
            }

            axios.get(endpoints.getUserById + localStorage.getItem("userId"))
                 .then((res) => {
                     const data = res.data;
                     const [userFirstName, userLastName] = data.name.split(' ');
                     const userEmail = data.email; 
                     
                     setFirstName(userFirstName);
                     setLastName(userLastName);
                     setEmail(userEmail);
                 })
                 .catch((error) => {
                     navigate('/login');
                 })
        }
    }, []);

    const handleLogOut = () => {
        axios.post(endpoints.logout)
                 .then((res) => {
                    localStorage.removeItem("userId");
                    localStorage.removeItem("expirationDate");
                    navigate('/login');
                 })
    }

    const handleUpdateFirstName = (value) => {
        setUpdateFirstName(value);

        var errorText = validateName(value);
        setErrorsFirstName({error: errorText});
    }

    const handleUpdateLastName = (value) => {
        setUpdateLastName(value);

        var errorText = validateName(value);
        setErrorsLastName({error: errorText});
    }

    const handleUpdateEmail = (value) => {
        setUpdateEmail(value);

        var errorText = validateEmail(value);
        setErrorsEmail({error: errorText});
    }

    const handleUpdatePassword = (value) => {
        setUpdatePassword(value);

        var errorText = validatePassword(value);
        setErrorsPassword({error: errorText});
    }

    const showOrHide = (inputField, value) => {
        switch (inputField) {
            case 'first name':
                setUpdateContainerFirstName(value);
                break;

            case 'last name':
                setUpdateContainerLastName(value);
                break;
        
            case 'email':
                setUpdateContainerEmail(value);
                break;


            case 'password':
                setUpdateContainerPassword(value);
                break;
        }

        if(!value)
            resetContainer(inputField);
    }

    const resetContainer = (inputField) => {
        switch (inputField) {
            case 'first name':
                setErrorsFirstName({});
                setUpdateFirstName("");
                break;

            case 'last name':
                setErrorsLastName({});
                setUpdateLastName("");
                break;
        
            case 'email':
                setErrorsEmail({});
                setUpdateEmail("");
                break;


            case 'password':
                setErrorsPassword({});
                setUpdatePassword("");
                break;
        }
    }

    const handleNameUpdate = (inputField) => {
        if(!(errorsFirstName.error || errorsLastName.error)){
            const userFullName = inputField === 'first name' ? updateFirstName + ' ' + lastName : firstName + ' ' + updateLastName;

            const requestBody = [{
                'path': '/name',
                'op': 'replace',
                'value': userFullName
            }];

            axios.patch(endpoints.updateUser + localStorage.getItem('userId'), requestBody)
                 .then((res) => {
                     window.location.reload();
                 })
                 .catch((error) => {
                     if (inputField === 'first name')
                        setErrorsFirstName({error: error.response.data.message});

                    else
                        setErrorsLastName({error: error.response.data.message});
                 })
        }
    }

    const handleEmailUpdate = () => {
        if(!errorsEmail.error){
            const requestBody = [{
                'path': '/email',
                'op': 'replace',
                'value': updateEmail
            }];

            axios.patch(endpoints.updateUser + localStorage.getItem('userId'), requestBody)
                 .then((res) => {
                     window.location.reload();
                 })
                 .catch((error) => {
                    setErrorsEmail({error: error.response.data.message});
                 })
        }
    }

    const handlePasswordUpdate = () => {
        if(!errorsPassword.error){
            const requestBody = [{
                'path': '/password',
                'op': 'replace',
                'value': updatePassword
            }];

            axios.patch(endpoints.updateUser + localStorage.getItem('userId'), requestBody)
                 .then((res) => {
                     window.location.reload();
                 })
                 .catch((error) => {
                    setErrorsPassword({error: error.response.data.message});
                 })
        }
    }

    return(
        <div>
            <p>Welcome, {firstName} {lastName}</p>
            <button onClick={() => handleLogOut()} type="button">Logout</button>

            <div id="user-info">
                <div id="info-container">
                    <p>First name: {firstName}</p>
                    <p onClick={(e) => showOrHide('first name', !updateContainerFirstName)}>Edit</p>
                    {updateContainerFirstName && 
                    <div className="update-container">
                        <input type="text" name="firstName" id="first-name" placeholder="Enter new first name" value={updateFirstName} onChange={(e) => handleUpdateFirstName(e.currentTarget.value)} />
                        <div class="error-message">{errorsFirstName.error}</div>
                        <button type="button" onClick={() => handleNameUpdate('first name')}>Save Changes</button>
                        <button type="button" onClick={() => showOrHide('first name', false)}>Cancel</button>
                    </div>}
                </div>

                <div id="info-container">
                    <p>Last name: {lastName}</p>
                    <p onClick={() => showOrHide('last name', !updateContainerLastName)}>Edit</p>
                    {updateContainerLastName &&
                    <div className="update-container">
                        <input type="text" name="lastName" id="last-name" placeholder="Enter new last name" value={updateLastName} onChange={(e) => handleUpdateLastName(e.currentTarget.value)} />
                        <div class="error-message">{errorsLastName.error}</div>
                        <button type="button" onClick={() => handleNameUpdate('last name')}>Save Changes</button>
                        <button type="button" onClick={() => showOrHide('last name', false)}>Cancel</button>
                    </div>}
                </div>

                <div id="info-container">
                    <p>Email: {email}</p>
                    <p onClick={() => showOrHide('email', !updateContainerEmail)}>Edit</p>
                    {updateContainerEmail && 
                    <div className="update-container">
                        <input type="email" name="email" id="email" placeholder="Enter new email" value={updateEmail} onChange={(e) => handleUpdateEmail(e.currentTarget.value)} />
                        <div class="error-message">{errorsEmail.error}</div>
                        <button type="button" onClick={() => handleEmailUpdate()}>Save Changes</button>
                        <button type="button" onClick={() => showOrHide('email', false)}>Cancel</button>
                    </div>}
                </div>

                <p onClick={() => showOrHide('password', !updateContainerPassword)}>Change Password</p>
                {updateContainerPassword && 
                <div className="update-container">
                    <input type="password" name="password" id="password" placeholder="Enter new password" value={updatePassword} onChange={(e) => handleUpdatePassword(e.currentTarget.value)} />
                    <div class="error-message">{errorsPassword.error}</div>
                    <button type="button" onClick={() => console.log()}>Save Changes</button>
                    <button type="button" onClick={() => showOrHide('password', false)}>Cancel</button>
                </div>}
            </div>
        </div>
    );
};

            
            
            
            