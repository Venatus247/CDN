export default class RequestService {

    private static i:RequestService

    public static instance(): RequestService {
        if (this.i == null)
            this.i = new RequestService()
        return this.i
    }



}