using System;
using System.Collections.Generic;

namespace uServiceAPI.Models;

public partial class User
{
    public long Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int ProviderIdRegister { get; set; }

    public int? EmailVerified { get; set; }

    public string? UserProviderId { get; set; }
}
