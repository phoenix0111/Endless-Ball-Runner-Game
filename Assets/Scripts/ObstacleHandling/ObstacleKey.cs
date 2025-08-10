
public struct ObstacleKey
{
    public ObstacleType Type;
    public LaneHeightType Height;

    public ObstacleKey(ObstacleType type, LaneHeightType height)
    {
        Type = type;
        Height = height;
    }
}