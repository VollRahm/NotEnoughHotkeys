namespace NotEnoughHotkeys.RawInputLib
{
    public class RawKeyPressEventArgs
    {
        public RawDevice Keyboard;
        public int VKey;
        public KeyPressState KeyState;

        public override string ToString()
        {
            return $"Key: {VKey} | State: {KeyState.ToString()} | Name: {Keyboard.Name}";
        }
    }

    public enum KeyPressState
    {
        Down,
        Up
    }
}
