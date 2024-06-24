export const Register = () => {
    return (
        <div class="form">
            <form>
                <input type="text" name="Text" id="text" placeholder="Enter Name"></input>
                <input type="email" name="Email" id="email" placeholder="Enter Email"></input>
                <input type="password" name="Password" id="password" placeholder="Enter Password"></input>
                <button type="submit">Register</button>
            </form>
        </div>
    )
}