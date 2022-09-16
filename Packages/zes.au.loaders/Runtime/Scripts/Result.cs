namespace Au.Loaders
{
    /// <summary>
    /// Resource operation result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>
    {
        public int error;
        public string message;
        public T data;

        public bool failed => error != 0;
    }
}
