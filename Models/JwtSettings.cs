using System;
using System.Collections.Generic;

namespace uServiceAPI.Models;

public partial class JwtSettings
{
    public string SecurityKey { get; set; } = null!;
}
