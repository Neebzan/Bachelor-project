using System;
using System.Collections.Generic;
using System.Text;

namespace Models {
    public enum DatabaseRequest { Create, Read, Update, Delete };
    public enum DatabaseResponse { None, Success, ConnectionFailed, AlreadyExists, DoesNotExist };
    public enum InstallationStatus { Unchecked, Verified, NotInstalled, UpdateRequired, NotFoundOnServer, IsInstalling, IsDeleting }
    public enum VersionBranch {
        None = 0,
        Release = 1,
        Beta = 2,
        Alpha = 4,
        Development = 3
    }
}
