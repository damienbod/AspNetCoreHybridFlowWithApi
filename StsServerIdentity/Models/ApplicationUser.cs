﻿// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public bool IsAdmin { get; set; }
    public string DataEventRecordsRole { get; set; }
}