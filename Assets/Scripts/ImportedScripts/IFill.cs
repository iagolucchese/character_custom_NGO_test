namespace ImportedScripts
{
    public interface IFill
    {
        public float NormalizedFill { get;}
#if UNITY_2021_1_OR_NEWER
        public virtual float SizeMultiplier => 1f;
        public virtual float RotationAngle => -1f;
#else
        public float SizeMultiplier { get; }
        public float RotationAngle { get; }
#endif
    }
}