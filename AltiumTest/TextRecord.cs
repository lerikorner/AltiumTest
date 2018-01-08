using System;


namespace AltiumTest
{
    //создает класс TextRecord, содеражщий нужные нам свойства Code (до 1й точки в строке файла, Description (остальная часть строки),
    //для последующей сортировки
    public class TextRecord: IEquatable<TextRecord>
    {
        public UInt32 Code { get; set; }
        public string Description { get; set; }
        public override string ToString()
        {
            return Code + Description;
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
            return (int)Code;
        }
        public bool Equals(TextRecord other)
        {
            if (other == null) return false;
            return (this.Code.Equals(other.Code));
        }      
    }
    
}

