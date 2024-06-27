import axios from "../config/axios.js";
import { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router";
import { drawCaptcha, generateCaptchaText } from "../captcha-service/captcha";
import { endpoints } from "../data/endpoints";

export const Login = () => {
    const navigate = useNavigate();

    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');

    const captchaRef = useRef(null);
    const[captchaText, setCaptchaText] = useState('');
    const[captchaUserText, setUserCaptchaText] = useState('');

    const handleEmailInput = (value) => setEmail(value);
    const handlePasswordInput = (value) => setPassword(value);
    const handleCaptchaUserText = (value) => setUserCaptchaText(value);

    const [generalError, setGeneralError] = useState({});

    useEffect(() => {
        const canvasObj = captchaRef.current;
        const ctx = canvasObj.getContext("2d");
        createCaptcha(ctx);
    }, []);

    const createCaptcha = (ctx) => {
        setUserCaptchaText('');
        const text = generateCaptchaText();
        setCaptchaText(text);
        drawCaptcha(ctx, text);
    };

    const handleSubmit = (e) => {
        e.preventDefault();

        if(captchaUserText !== captchaText){
            setGeneralError({error: 'Invalid Captcha!'});
            createCaptcha(captchaRef.current.getContext("2d"));
        }

        else{
            setGeneralError({});

            const loginData = {
                email: email,
                password: password
            };

            axios.post(endpoints.login, loginData)
                 .then((res) => {
                    localStorage.setItem("userId", res.data.userId);

                    var expirationDate = new Date();
                    expirationDate.setMinutes(expirationDate.getMinutes() + 30)

                    localStorage.setItem("expirationDate", expirationDate.toLocaleString())
                    navigate('/');
                 })
                 .catch((error) => {
                    setGeneralError({error: error.response.data.message});
                 });
        }
    }

    return (
        <>
            <div class="form">
                <form onSubmit={(e) => handleSubmit(e)}>
                    <input type="email" name="Email" id="email" placeholder="Email" value={email} onChange={(e) => handleEmailInput(e.currentTarget.value)} required/>
                    <input type="password" name="Password" id="password" placeholder="Password" value={password} onChange={(e) => handlePasswordInput(e.currentTarget.value)} required/>
                    <canvas ref={captchaRef} width="200" height="70"></canvas>
                    <input type="text" name="Captcha" id="captcha" placeholder="Enter Captcha Text" value={captchaUserText} onChange={(e) => handleCaptchaUserText(e.currentTarget.value)} required/>
                    <button type="button" onClick={() => createCaptcha(captchaRef.current.getContext("2d"))}>Reload</button>
                    <button type="submit">Login</button>
                </form>
            </div>
            <div class="error-message">{generalError.error}</div>
        </>
    )
}