namespace Tebex.Common
{
    public delegate void ApiSuccessCallback(int code, string body);
    public delegate void PluginApiErrorCallback(PluginApiError error);
    public delegate void ServerErrorCallback(ServerError error);
}