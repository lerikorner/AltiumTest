﻿using System;
using System.Collections.Generic;

namespace AltiumTest
{
    public class TextRecord: IEquatable<TextRecord>
    {
        public Int32 Code { get; set; }
        public string Description { get; set; }
        public override string ToString()
        {
            return Code + "." + Description;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            TextRecord objAsTextRecord = obj as TextRecord;
            if (objAsTextRecord == null) return false;
            else return Equals(objAsTextRecord);
        }
        public override int GetHashCode()
        {
            return Code;
        }
        public bool Equals(TextRecord other)
        {
            if (other == null) return false;
            return (this.Code.Equals(other.Code));
        }
    }
}
