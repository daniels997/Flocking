//I dedicate this project to Franky. You were an awesome pet birb. RIP and keep chirping little guy, love you bro

using System.Collections.Generic;
using AI.SteeringBehaviors.Core;

namespace AI.SteeringBehaviors.StudentAI
{
    public class Flock
    {
        //private Vector3 accel { get; set; }
        public float AlignmentStrength { get; set; }
        public float CohesionStrength { get; set; }
        public float SeparationStrength { get; set; }
        public List<MovingObject> Boids { get; protected set; }
        public Vector3 AveragePosition { get; set; }
        protected Vector3 AverageForward { get; set; }
        public float FlockRadius { get; set; }

        #region TODO
        public Flock()
        {
            //Vector3 accel = Vector3.Empty;
            AverageForward = Vector3.Empty;
            AveragePosition = Vector3.Empty;
            AlignmentStrength = 1.0f;
            CohesionStrength = 1.0f;
            SeparationStrength = 1.0f;
            FlockRadius = 10.0f;
        }
        public virtual void Update(float deltaTime) 
        {
            Vector3 totalSumVelocity = Vector3.Empty;
            Vector3 totalSumPosition = Vector3.Empty;
            for (int i = 0; i < Boids.Count; i++)
            {
                totalSumVelocity += Boids[i].Velocity;
                totalSumPosition += Boids[i].Position;
            }
            AverageForward = totalSumVelocity / Boids.Count;
            AveragePosition = totalSumPosition / Boids.Count;

            for (int i = 0; i < Boids.Count; i++)
            {
                Vector3 accel = Vector3.Empty;
                accel = calcAlignmentAccel(Boids[i]);
                accel += calcCohesionAccel(Boids[i]);
                accel += calcSeperationAccel(Boids[i]);
                accel *= Boids[i].MaxSpeed * deltaTime;               
                Boids[i].Velocity += accel;
                Boids[i].Update(deltaTime);
            }
        }
        private Vector3 calcAlignmentAccel(MovingObject _boid)
        {
            Vector3 accel = Vector3.Empty;
            accel = AverageForward / _boid.MaxSpeed;
            if (accel.Length > 1.0f)
            {
                accel = Vector3.Normalize(accel);
            }
            return accel * AlignmentStrength;
        }
        private Vector3 calcCohesionAccel(MovingObject _boid)
        {
            Vector3 accel = Vector3.Empty;
            accel = AveragePosition - _boid.Position;
            float distance = accel.Length;
            accel = Vector3.Normalize(accel);
            if (distance < FlockRadius)
            {
                accel *= distance / FlockRadius;
            }
            return accel * CohesionStrength;
        }
        private Vector3 calcSeperationAccel(MovingObject _boid)
        {
            Vector3 sum = Vector3.Empty;
            for (int i = 0; i < Boids.Count; i++)
            {
                if (_boid == Boids[i])
                {
                    continue;
                }
                Vector3 accel = Vector3.Empty;
                float distance = 0.0f;
                float safeDistance = 0.0f;
                accel = _boid.Position - Boids[i].Position;
                distance = accel.Length;
                safeDistance = _boid.SafeRadius + Boids[i].SafeRadius;
                if (distance < safeDistance)
                {
                    accel = Vector3.Normalize(accel);
                    accel *= (safeDistance - distance) / safeDistance;
                    sum += accel;
                }
            }
            if (sum.Length > 1.0f)
            {
                sum = Vector3.Normalize(sum);
            }
            return sum * SeparationStrength;
        }
        #endregion
    }
}
