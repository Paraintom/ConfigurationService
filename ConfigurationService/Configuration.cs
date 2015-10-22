namespace ConfigurationService
{
    public class Configuration
    {
        public string Instance { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        #region equality members
        protected bool Equals(Configuration other)
        {
            return string.Equals(Instance, other.Instance) && string.Equals(Key, other.Key) && string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Configuration)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Instance != null ? Instance.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Key != null ? Key.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                return hashCode;
            }
        }
        #endregion
    }
}