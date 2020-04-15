using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public enum DatabaseRequest { Create, Read, Update, Delete };
    public enum DatabaseResponse { None, Success, ConnectionFailed, AlreadyExists, DoesNotExist };
}
