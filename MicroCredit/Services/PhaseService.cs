namespace MicroCredit.Services
{
    public class PhaseService
    {
        private int Bitmask { get; set; }

        public PhaseService(string encryptedPhase)
        {
            Bitmask = Decrypt(encryptedPhase);
        }

        public string GetEncryptedPhase()
        {
            return Encrypt(Bitmask);
        }

        private string Encrypt(int bitmask)
        {
            byte[] bitmaskBytes = BitConverter.GetBytes(bitmask);
            return Convert.ToBase64String(bitmaskBytes);
        }

        private int Decrypt(string encryptedText)
        {
            byte[] bitmaskBytes = Convert.FromBase64String(encryptedText);
            return BitConverter.ToInt32(bitmaskBytes, 0);
        }

        public void SetBit(int position)
        {
            Bitmask |= (1 << (position - 1));
        }

        public bool IsBitSet(int position)
        {
            return (Bitmask & (1 << (position - 1))) != 0;
        }

        public int? GetNextPhase()
        {
            for (int phase = 2; phase <= 7; phase++)
            {
                if (!IsBitSet(phase))
                {
                    return phase;
                }
            }
            return null; // All phases are completed
        }
    }
}
