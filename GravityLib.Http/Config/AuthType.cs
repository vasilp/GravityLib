namespace GravityLib.Http.Config
{
    public enum AuthType
    {
        /// <summary>
        /// Indicates that the connection should be made without passing credentials
        /// </summary>
        Anonymous = 0,

        /// <summary>
        /// Indicates that basic authentication should be used on the connection
        /// </summary>
        Basic = 1,

        /// <summary>
        /// Indicates that Token-based authentication should be used on the connection
        /// </summary>
        Token = 2,

        /// <summary>
        /// Indicates that Bearer-based authentication should be used on the connection
        /// </summary>
        Bearer = 3,

        /// <summary>
        /// Indicates that OAuth2 authentication should be used on the connection
        /// </summary>
        OAuth2 = 4
    }
}
