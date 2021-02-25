﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ThunderKit.Core.Data
{
    public class PackageVersion : ScriptableObject
    {
        public string version;
        [HideInInspector]
        public string dependencyId;
        public PackageVersion[] dependencies;
        public override bool Equals(object obj)
        {
            return obj is PackageVersion version &&
                   dependencyId == version.dependencyId;
        }

        public override int GetHashCode()
        {
            return 996503521 + EqualityComparer<string>.Default.GetHashCode(dependencyId);
        }
    }
}