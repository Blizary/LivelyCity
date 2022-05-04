public class Finger
{
    public FingerType Type {get;}
    public float Current {get; set;}
    public float Target{get; set;}

    public Finger(FingerType type)
    {
        Type = type;
        Current = 0.0f;
        Target = 0.0f;
    }
}