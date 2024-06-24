export const Login = () => {
    return (
        <div class="form">
            <form>
                <input type="email" name="Email" id="email" placeholder="Enter Email"></input>
                <input type="password" name="Password" id="password" placeholder="Enter Password"></input>
                <button type="submit">Login</button>
            </form>
        </div>
    )
}