# GravityLib

[![Build Status](https://github.com/vasilp/GravityLib)](https://github.com/vasilp/GravityLib)

This repo contains NuGet packages for common shared code between applications.

**GravityLib.Common*

[![GravityLib.Common package](https://github.com/vasilp/GravityLib/Badge)](https://github.com/vasilp/GravityLib)

🌱 <span style="color:#008da6">Target Framework: **.Net Standard 2.0** / Coverage: **.Net Framework 4.7+, .Net and .Net Core 2.0+**</span>

Contains classes that usually sits on the very bottom of the projects hierarchy and is intended to be reused by any kind of application.
It is intended this to store only core .Net Core logic and dependencies and no special or custom libraries dependencies.

Contains custom exception classes for common use: ValidationException, AccessDeniedException, NotFoundException, RemoteException, etc.,
as well as many handy class extensions.

_ISessionData_ - an interface that is intended to store the user information for the current execution/session of an action.
In Web API this could be obtained from the authentication mechanisms for the current request.
For the services / console / desktop apps, this could be obtained from the current thread's information for the user/system that is performing the specific action(s).

**GravityLib.EntityFramework**

[![GravityLib.EntityFramework package](https://github.com/vasilp/GravityLib/Badge)](https://github.com/vasilp/GravityLib)

🌱 <span style="color:#008da6">Target Framework: **.Net 8.0+**</span>

Contains base DbContext class and helper classes to utilize the best of EntityFramework Core like:
- Query filters
- Automatic handling of soft-deletable data
- Auditable data
- Delete many DB records on batches

**GravityLib.Http**

[![GravityLib.Http package](https://github.com/vasilp/GravityLib/Badge)](https://github.com/vasilp/GravityLib)

🌱 <span style="color:#008da6">Target Framework: **.Net Standard 2.0** / Coverage: **.Net Framework 4.7+, .Net and .Net Core 2.0+**</span>

Contains common classes related to network communication.
It contains **APIClient** - a wrapper of HttpClient with retry mechanism built in and single line.
