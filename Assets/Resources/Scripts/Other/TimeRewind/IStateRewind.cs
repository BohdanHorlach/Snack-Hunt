public interface IStateRewind
{
    public void Record(bool needRemove);
    public void Rewind();
}