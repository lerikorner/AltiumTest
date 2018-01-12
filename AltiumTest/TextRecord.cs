using System;

namespace BigFileSorting
{
    // MARK: - TextRecord object, containing CodeID, Description properties 
    // MARK: - Iequatable interface implementation
    public class TextRecord: IEquatable<TextRecord>
    {
        public UInt32 CodeID { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return CodeID + Description;
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
            return (int)CodeID;
        }

        public bool Equals(TextRecord other)
        {
            if (other == null) return false;
            return (this.CodeID.Equals(other.CodeID));
        }      
    }  
}

