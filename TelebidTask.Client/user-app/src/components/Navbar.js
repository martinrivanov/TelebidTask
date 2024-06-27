import { Link } from "react-router-dom";

export const Navbar = () => {
    return(
        <nav id="nav">
            <ul>
                <li><Link class="links" to="/register">Register</Link></li>
                <li><Link class="links" to="/login">Login</Link></li>
            </ul>
        </nav>
    );
};