using System;

namespace ImprovedRespawning;
public static class Easings
{
    public enum EasingsList
    {
        Linear,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        InSine,
        OutSine,
        InOutSine,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc,
        InElastic,
        OutElastic,
        InOutElastic,
        InBack,
        OutBack,
        InOutBack,
        InBounce,
        OutBounce,
        InOutBounce
    }
    
    public static float RunEasingType(EasingsList type, float value)
    {
        float result = type switch
        {
            EasingsList.Linear => Linear(value),
            EasingsList.InQuad => InQuad(value),
            EasingsList.OutQuad => OutQuad(value),
            EasingsList.InOutQuad => InOutQuad(value),
            EasingsList.InCubic => InCubic(value),
            EasingsList.OutCubic => OutCubic(value),
            EasingsList.InOutCubic => InOutCubic(value),
            EasingsList.InQuart => InQuart(value),
            EasingsList.OutQuart => OutQuart(value),
            EasingsList.InOutQuart => InOutQuart(value),
            EasingsList.InQuint => InQuint(value),
            EasingsList.OutQuint => OutQuint(value),
            EasingsList.InOutQuint => InOutQuint(value),
            EasingsList.InSine => InSine(value),
            EasingsList.OutSine => OutSine(value),
            EasingsList.InOutSine => InOutSine(value),
            EasingsList.InExpo => InExpo(value),
            EasingsList.OutExpo => OutExpo(value),
            EasingsList.InOutExpo => InOutExpo(value),
            EasingsList.InCirc => InCirc(value),
            EasingsList.OutCirc => OutCirc(value),
            EasingsList.InOutCirc => InOutCirc(value),
            EasingsList.InElastic => InElastic(value),
            EasingsList.OutElastic => OutElastic(value),
            EasingsList.InOutElastic => InOutElastic(value),
            EasingsList.InBack => InBack(value),
            EasingsList.OutBack => OutBack(value),
            EasingsList.InOutBack => InOutBack(value),
            EasingsList.InBounce => InBounce(value),
            EasingsList.OutBounce => OutBounce(value),
            EasingsList.InOutBounce => InOutBounce(value),
            _ => 0f
        };

        return result;
    }

    private static float Linear(float t) => t;

    private static float InQuad(float t) => t * t;

    private static float OutQuad(float t) => 1f - InQuad(1f - t);

    private static float InOutQuad(float t)
    {
        if (t < 0.5) return InQuad(t * 2f) / 2f;
        return 1f - InQuad((1f - t) * 2f) / 2f;
    }

    private static float InCubic(float t) => t * t * t;

    private static float OutCubic(float t) => 1f - InCubic(1f - t);

    private static float InOutCubic(float t)
    {
        if (t < 0.5) return InCubic(t * 2f) / 2f;
        return 1f - InCubic((1f - t) * 2f) / 2f;
    }

    private static float InQuart(float t) => t * t * t * t;

    private static float OutQuart(float t) => 1f - InQuart(1f - t);

    private static float InOutQuart(float t)
    {
        if (t < 0.5) return InQuart(t * 2f) / 2f;
        return 1f - InQuart((1f - t) * 2f) / 2f;
    }

    private static float InQuint(float t) => t * t * t * t * t;

    private static float OutQuint(float t) => 1f - InQuint(1f - t);

    private static float InOutQuint(float t)
    {
        if (t < 0.5) return InQuint(t * 2f) / 2f;
        return 1f - InQuint((1f - t) * 2f) / 2f;
    }

    private static float InSine(float t) => -(float)Math.Cos(t * 3.141592653589793 / 2.0);

    private static float OutSine(float t) => (float)Math.Sin(t * 3.141592653589793 / 2.0);

    private static float InOutSine(float t) => (float)(Math.Cos(t * 3.141592653589793) - 1.0) / -2f;

    private static float InExpo(float t) => (float)Math.Pow(2.0, 10f * (t - 1f));

    private static float OutExpo(float t) => 1f - InExpo(1f - t);

    private static float InOutExpo(float t)
    {
        if (t < 0.5) return InExpo(t * 2f) / 2f;
        return 1f - InExpo((1f - t) * 2f) / 2f;
    }

    private static float InCirc(float t) => -((float)Math.Sqrt(1f - t * t) - 1f);

    private static float OutCirc(float t) => 1f - InCirc(1f - t);

    private static float InOutCirc(float t)
    {
        if (t < 0.5) return InCirc(t * 2f) / 2f;
        return 1f - InCirc((1f - t) * 2f) / 2f;
    }

    private static float InElastic(float t) => 1f - OutElastic(1f - t);

    private static float OutElastic(float t)
    {
        const float p = 0.3f;
        return (float)Math.Pow(2.0, -10f * t) * (float)Math.Sin((t - p / 4f) * 6.283185307179586 / p) + 1f;
    }

    private static float InOutElastic(float t)
    {
        if (t < 0.5) return InElastic(t * 2f) / 2f;
        return 1f - InElastic((1f - t) * 2f) / 2f;
    }

    private static float InBack(float t)
    {
        const float s = 1.70158f;
        return t * t * ((s + 1f) * t - s);
    }

    private static float OutBack(float t) => 1f - InBack(1f - t);

    private static float InOutBack(float t)
    {
        if (t < 0.5) return InBack(t * 2f) / 2f;
        return 1f - InBack((1f - t) * 2f) / 2f;
    }

    private static float InBounce(float t) => 1f - OutBounce(1f - t);

    private static float OutBounce(float t)
    {
        switch (t)
        {
            case < 0.36363637f:
                return 7.5625f * t * t;
            case < 0.72727275f:
                t -= 0.54545456f;
                return 7.5625f * t * t + 0.75f;
        }

        if (t < 0.9090909090909091)
        {
            t -= 0.8181818f;
            return 7.5625f * t * t + 0.9375f;
        }

        t -= 0.95454544f;
        return 7.5625f * t * t + 0.984375f;
    }

    private static float InOutBounce(float t)
    {
        if (t < 0.5) return InBounce(t * 2f) / 2f;
        return 1f - InBounce((1f - t) * 2f) / 2f;
    }
}