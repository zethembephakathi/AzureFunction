namespace Azure
{
    internal class Http
    {
        internal class HttpRange
        {
            private int v;
            private int length;

            public HttpRange(int v, int length)
            {
                this.v = v;
                this.length = length;
            }
        }
    }
}