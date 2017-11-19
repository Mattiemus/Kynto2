namespace Spark.Scene
{
    using System.Collections.Generic;

    using Math;
    using Graphics;

    public class LightComparer : IComparer<Light>
    {
        private readonly Spatial _spatial;

        public LightComparer(Spatial spatial)
        {
            _spatial = spatial;
        }

        public int Compare(Light x, Light y)
        {
            float first = GetLightValue(x);
            float second = GetLightValue(y);

            if (first < second)
            {
                return -1;
            }
            else if (first > second)
            {
                return 1;
            }

            return 0;
        }

        private float GetLightValue(Light l)
        {
            if (!l.IsEnabled)
            {
                return 0;
            }

            switch (l.LightType)
            {
                case LightType.Directional:
                    return GetColorStrength(l);
                case LightType.Point:
                    return GetPointValue(l as PointLight);
                case LightType.Spot:
                    return GetSpotValue(l as SpotLight);
                default:
                    return 0;
            }
        }

        private float GetPointValue(PointLight pl)
        {
            if (!pl.Attenuate)
            {
                return GetColorStrength(pl);
            }

            Vector3 pos = pl.Position;
            float strength = GetColorStrength(pl);

            Vector3 posOfObject;
            BoundingVolume bv = _spatial.WorldBounding;
            if (bv == null)
            {
                posOfObject = _spatial.WorldTransform.Translation;
            }
            else
            {
                posOfObject = bv.Center;
            }

            float attenuation = Light.ComputeAttenuationFactor(ref posOfObject, ref pos, pl.Range);
            if (MathHelper.IsApproxZero(attenuation))
            {
                return 0;
            }

            return strength / attenuation;
        }

        private float GetSpotValue(SpotLight sl)
        {
            BoundingVolume bv = _spatial.WorldBounding;
            if (bv == null)
            {
                return 0;
            }

            Vector3 dir = sl.Direction;
            Vector3 pos = sl.Position;
            Plane plane = new Plane(dir, pos);

            if (bv.Intersects(ref plane) == PlaneIntersectionType.Back)
            {
                if (!sl.Attenuate)
                {
                    return GetColorStrength(sl);
                }

                float strength = GetColorStrength(sl);
                Vector3 center = bv.Center;

                float attenuation = Light.ComputeAttenuationFactor(ref center, ref pos, sl.Range);
                if (MathHelper.IsApproxZero(attenuation))
                {
                    return 0;
                }

                return strength / attenuation;
            }

            return 0;
        }

        private float GetColorStrength(Light l)
        {
            return Strength(l.Ambient) + Strength(l.Diffuse);
        }

        private float Strength(Color c)
        {
            float r = c.R;
            float g = c.G;
            float b = c.B;

            return (float)System.Math.Sqrt(r * r + g * g + b * b);
        }
    }
}
