namespace InGame.Message
{
    public readonly struct BuildingCountChangeMessage
    {
        public readonly int CurrentCount;
        public BuildingCountChangeMessage(int currentCount)
        {
            CurrentCount = currentCount;
        }
    }
}