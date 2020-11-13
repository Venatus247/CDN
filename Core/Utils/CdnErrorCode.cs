namespace Core.Utils
{
    
    public enum CdnErrorCode
    {
        
        Ok = 200,
        ClientErrorGeneral = 400,
        ClientRequestIncomplete = 401,
        ClientNotAuthorized = 402,
        ClientPermissionMissing = 403,
        FileNotFound = 404,
        BackendErrorGeneral = 500,
        NoDatabaseConnection = 501
    }
}