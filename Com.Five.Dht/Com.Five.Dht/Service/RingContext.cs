namespace Com.Five.Dht.Service
{
    using Factory;
    using System;

    public class RingContext
    {
        public static RingContext Current
        {
            get;
            private set;
        }

        static RingContext()
        {
            if (string.IsNullOrWhiteSpace(Ring.Default.RingFactory))
            {
                throw new InvalidOperationException(
                    "Ringfactory type not set");
            }

            string[] split = Ring.Default.RingFactory.Split(',');
            if (2 != split.Length)
            {
                throw new InvalidOperationException(
                    "Invalid RingFactory type format should be Type, Assembly.");
            }

            Current = new RingContext
            {
                MaxNoOfBits = Ring.Default.MaxNoOfBits,
                NoOfSuccessors = Ring.Default.NoOfSuccessors
            };
            Current.Factory = (IRingFactory)Activator.CreateInstance(
                split[1].Trim(), split[0].Trim()).Unwrap();
        }

        public byte MaxNoOfBits
        {
            get;
            set;
        }

        public int NoOfSuccessors
        {
            get;
            set;
        }

        public IRingFactory Factory
        {
            get;
            set;
        }
    }
}