import { useState } from "react";

export const Login = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');

    const handleEmailInput = (value) => setEmail(value);
    const handlePasswordInput = (value) => setPassword(value);

    return (
        <div class="form">
            <form>
                <input type="email" name="Email" id="email" placeholder="Email" value={email} onChange={(e) => handleEmailInput(e.currentTarget.value)} required/>
                <input type="password" name="Password" id="password" placeholder="Password" value={password} onChange={(e) => handlePasswordInput(e.currentTarget.value)} required/>
                <button type="submit">Login</button>
            </form>
        </div>
    )
}