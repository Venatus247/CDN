import {Cookies} from "react-cookie";

export default class CookieService {

    private cookies: Cookies = new Cookies()

    private static i:CookieService

    public static instance(): CookieService {
        if (this.i == null)
            this.i = new CookieService()
        return this.i
    }

    public getCookie<T>(name: string): T {
        return this.cookies.get(name, {doNotParse: true}) as T
    }

}