﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Cs2Bot.Models;

public partial class Guild
{
    public long GuildId { get; set; }

    public bool IsActive { get; set; }

    public virtual PatchNotesSetting PatchNotesSetting { get; set; }
}