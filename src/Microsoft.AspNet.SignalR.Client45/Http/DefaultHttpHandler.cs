﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System.Net.Http;
#if !NETFX_CORE
using System.Security.Cryptography.X509Certificates;
#endif

namespace Microsoft.AspNet.SignalR.Client.Http
{
#if !NETFX_CORE && !SILVERLIGHT && !__ANDROID__ && !IOS
    public class DefaultHttpHandler : WebRequestHandler
#else
    public class DefaultHttpHandler : HttpClientHandler
#endif
    {
        private readonly IConnection _connection;

        public DefaultHttpHandler(IConnection connection)
        {
            _connection = connection;

            Credentials = _connection.Credentials;

            if (_connection.CookieContainer != null)
            {
                CookieContainer = _connection.CookieContainer;
            }

#if !SILVERLIGHT
            if (Proxy != null)
            {
                Proxy = Proxy;
            }
#endif

#if (NET4 || NET45)
            foreach (X509Certificate cert in _connection.Certificates)
            {
                ClientCertificates.Add(cert);
            }
#endif
        }
    }
}