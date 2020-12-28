export default function Login() {
    return (
        <div>
            <form onSubmit={event => {
                event.preventDefault()
                console.log("Login clicked")
            }}>
                <input type="text" placeholder="e-mail address"/>
                <input type="password" placeholder="password"/>
                <button type="submit">Login</button>
            </form>
        </div>
    )
}