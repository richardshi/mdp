/*
* Farseer Physics Engine based on Box2D.XNA port:
* Copyright (c) 2011 Ian Qvist
* 
* Original source Box2D:
* Copyright (c) 2006-2011 Erin Catto http://www.box2d.org 
* 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/

using System.Diagnostics;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Joints
{
    // p = attached point, m = mouse point
    // C = p - m
    // Cdot = v
    //      = v + cross(w, r)
    // J = [I r_skew]
    // Identity used:
    // w k % (rx i + ry j) = w * (-ry i + rx j)

    /// <summary>
    /// A mouse joint is used to make a point on a body track a
    /// specified world point. This a soft constraint with a maximum
    /// force. This allows the constraint to stretch and without
    /// applying huge forces.
    /// NOTE: this joint is not documented in the manual because it was
    /// developed to be used in the testbed. If you want to learn how to
    /// use the mouse joint, look at the testbed.
    /// </summary>
    public class FixedMouseJoint : FarseerJoint
    {
        private FVector2 _targetA;
        private float _frequency;
        private float _dampingRatio;
        private float _beta;

        // Solver shared
        private FVector2 _impulse;
        private float _maxForce;
        private float _gamma;

        // Solver temp
        private int _indexA;
        private int _indexB;
        private FVector2 _rB;
        private FVector2 _localCenterB;
        private float _invMassB;
        private float _invIB;
        private Mat22 _mass;
        private FVector2 _C;

        /// <summary>
        /// This requires a world target point,
        /// tuning parameters, and the time step.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="worldAnchor">The target.</param>
        public FixedMouseJoint(Body body, FVector2 worldAnchor)
            : base(body)
        {
            JointType = JointType.FixedMouse;
            Frequency = 5.0f;
            DampingRatio = 0.7f;
            MaxForce = 1000 * body.Mass;

            Debug.Assert(worldAnchor.IsValid());

            _targetA = worldAnchor;
            LocalAnchorB = MathUtils.MulT(BodyA.Xf, worldAnchor);
        }

        public FVector2 LocalAnchorB { get; set; }

        public override FVector2 WorldAnchorA
        {
            get { return BodyA.GetWorldPoint(LocalAnchorB); }
        }

        public override FVector2 WorldAnchorB
        {
            get { return _targetA; }
            set
            {
                BodyA.Awake = true;
                _targetA = value;
            }
        }

        /// <summary>
        /// The maximum constraint force that can be exerted
        /// to move the candidate body. Usually you will express
        /// as some multiple of the weight (multiplier * mass * gravity).
        /// </summary>
        public float MaxForce
        {
            get { return _maxForce; }
            set
            {
                Debug.Assert(MathUtils.IsValid(value) && value >= 0.0f);

                _maxForce = value;
            }
        }

        /// <summary>
        /// The response speed.
        /// </summary>
        public float Frequency
        {
            get { return _frequency; }
            set
            {
                Debug.Assert(MathUtils.IsValid(value) && value >= 0.0f);
                _frequency = value;
            }
        }

        /// <summary>
        /// The damping ratio. 0 = no damping, 1 = critical damping.
        /// </summary>
        public float DampingRatio
        {
            get { return _dampingRatio; }
            set
            {
                Debug.Assert(MathUtils.IsValid(value) && value >= 0.0f);
                _dampingRatio = value;
            }
        }

        public override FVector2 GetReactionForce(float inv_dt)
        {
            return inv_dt * _impulse;
        }

        public override float GetReactionTorque(float inv_dt)
        {
            return inv_dt * 0.0f;
        }

        internal override void InitVelocityConstraints(ref SolverData data)
        {
            _indexB = BodyA.IslandIndex;
            _localCenterB = BodyA.Sweep.LocalCenter;
            _invMassB = BodyA.InvMass;
            _invIB = 0;

            FVector2 cB = data.positions[_indexB].c;
            float aB = data.positions[_indexB].a;
            FVector2 vB = data.velocities[_indexB].v;
            float wB = data.velocities[_indexB].w;

            Rot qB = new Rot(aB);

            float mass = BodyA.Mass;

            // Frequency
            float omega = 2.0f * Settings.Pi * Frequency;

            // Damping coefficient
            float d = 2.0f * mass * DampingRatio * omega;

            // Spring stiffness
            float k = mass * (omega * omega);

            // magic formulas
            // gamma has units of inverse mass.
            // beta has units of inverse time.
            float h = data.step.dt;
            Debug.Assert(d + h * k > Settings.Epsilon);
            _gamma = h * (d + h * k);
            if (_gamma != 0.0f)
            {
                _gamma = 1.0f / _gamma;
            }

            _beta = h * k * _gamma;

            // Compute the effective mass matrix.
            _rB = MathUtils.Mul(qB, LocalAnchorB - _localCenterB);
            // K    = [(1/m1 + 1/m2) * eye(2) - skew(r1) * invI1 * skew(r1) - skew(r2) * invI2 * skew(r2)]
            //      = [1/m1+1/m2     0    ] + invI1 * [r1.Y*r1.Y -r1.X*r1.Y] + invI2 * [r1.Y*r1.Y -r1.X*r1.Y]
            //        [    0     1/m1+1/m2]           [-r1.X*r1.Y r1.X*r1.X]           [-r1.X*r1.Y r1.X*r1.X]
            Mat22 K = new Mat22();
            K.ex.X = _invMassB + _invIB * _rB.Y * _rB.Y + _gamma;
            K.ex.Y = -_invIB * _rB.X * _rB.Y;
            K.ey.X = K.ex.Y;
            K.ey.Y = _invMassB + _invIB * _rB.X * _rB.X + _gamma;

            _mass = K.Inverse;

            _C = cB + _rB - _targetA;
            _C *= _beta;

            // Cheat with some damping
            wB *= 0.98f;

            if (Settings.EnableWarmstarting)
            {
                _impulse *= data.step.dtRatio;
                vB += _invMassB * _impulse;
                wB += _invIB * MathUtils.Cross(_rB, _impulse);
            }
            else
            {
                _impulse = FVector2.Zero;
            }

            data.velocities[_indexB].v = vB;
            data.velocities[_indexB].w = wB;
        }

        internal override void SolveVelocityConstraints(ref SolverData data)
        {
            FVector2 vB = data.velocities[_indexB].v;
            float wB = data.velocities[_indexB].w;

            // Cdot = v + cross(w, r)
            FVector2 Cdot = vB + MathUtils.Cross(wB, _rB);
            FVector2 impulse = MathUtils.Mul(ref _mass, -(Cdot + _C + _gamma * _impulse));

            FVector2 oldImpulse = _impulse;
            _impulse += impulse;
            float maxImpulse = data.step.dt * MaxForce;
            if (_impulse.LengthSquared() > maxImpulse * maxImpulse)
            {
                _impulse *= maxImpulse / _impulse.Length();
            }
            impulse = _impulse - oldImpulse;

            vB += _invMassB * impulse;
            wB += _invIB * MathUtils.Cross(_rB, impulse);

            data.velocities[_indexB].v = vB;
            data.velocities[_indexB].w = wB;
        }

        internal override bool SolvePositionConstraints(ref SolverData data)
        {
            return true;
        }
    }
}