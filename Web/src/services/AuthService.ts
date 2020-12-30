import {Cookies} from "react-cookie";
import CookieService from "./CookieService";



export default class AuthService {

    private static readonly sessionTokenCookie = 'sessionToken'

    private sessionToken?: string

    public async isAuthenticated(): Promise<boolean> {
        this.sessionToken = CookieService.instance().getCookie(AuthService.sessionTokenCookie)

        return false
    }

}