﻿using System;

namespace EF6Auditing
{
    /// <summary>
    /// This attribute indicates to the EFAuditing AuditLogBuiler that the target property's value should not be written to the audit log
    /// </summary>
    public class DoNotAudit : Attribute
    {
    }
}
