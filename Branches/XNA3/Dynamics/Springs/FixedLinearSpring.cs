using System;
using FarseerGames.FarseerPhysics.Controllers;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Dynamics.Springs
{
    public class FixedLinearSpring : Controller
    {
        private float _breakpoint = float.MaxValue;
        private float _dampningConstant;
        private Vector2 _difference = Vector2.Zero;
        private float _restLength;
        private float _springConstant;

        private float _springError;
        private Body _body;

        private Vector2 _bodyAttachPoint;
        private Vector2 _worldAttachPoint;

        public FixedLinearSpring()
        {
        }

        public FixedLinearSpring(Body body, Vector2 bodyAttachPoint, Vector2 worldAttachPoint, float springConstant,
                                 float dampningConstant)
        {
            _body = body;
            _bodyAttachPoint = bodyAttachPoint;
            _worldAttachPoint = worldAttachPoint;
            _springConstant = springConstant;
            _dampningConstant = dampningConstant;
            _difference = worldAttachPoint - _body.GetWorldPosition(bodyAttachPoint);
            _restLength = _difference.Length();
        }

        public Body Body
        {
            get { return _body; }
            set { _body = value; }
        }

        public Vector2 Position
        {
            get { return _worldAttachPoint; }
            set { _worldAttachPoint = value; }
        }

        public Vector2 BodyAttachPoint
        {
            get { return _bodyAttachPoint; }
            set { _bodyAttachPoint = value; }
        }

        public Vector2 WorldAttachPoint
        {
            get { return _worldAttachPoint; }
            set { _worldAttachPoint = value; }
        }

        public float SpringConstant
        {
            get { return _springConstant; }
            set { _springConstant = value; }
        }

        public float DampningConstant
        {
            get { return _dampningConstant; }
            set { _dampningConstant = value; }
        }

        public float RestLength
        {
            get { return _restLength; }
            set { _restLength = value; }
        }

        public float Breakpoint
        {
            get { return _breakpoint; }
            set { _breakpoint = value; }
        }

        public float SpringError
        {
            get { return _springError; }
        }

        public event EventHandler<EventArgs> Broke;

        public override void Validate()
        {
            //if either of the joint's connected bodies are disposed then dispose the joint.
            if (_body.IsDisposed)
            {
                Dispose();
            }
        }

        public override void Update(float dt)
        {
            if (Enabled && Math.Abs(_springError) > _breakpoint)
            {
                Enabled = false;
                if (Broke != null) Broke(this, new EventArgs());
            }

            if (IsDisposed)
            {
                return;
            }
            if (_body.isStatic)
            {
                return;
            }

            //calculate and apply spring _force
            //F = -{s(L-r) + d[(v1-v2).L]/l}L/l   : s=spring const, d = dampning const, L=_difference vector (p1-p2), l = _difference magnitude, r = rest length,            
            _body.GetWorldPosition(ref _bodyAttachPoint, out _bodyWorldPoint);
            Vector2.Subtract(ref _bodyWorldPoint, ref _worldAttachPoint, out _difference);
            float differenceMagnitude = _difference.Length();
            if (differenceMagnitude < _epsilon)
            {
                return;
            } //if already close to rest length then return

            //calculate spring _force (kX)
            _springError = differenceMagnitude - _restLength;
            Vector2.Normalize(ref _difference, out _differenceNormalized);
            _springForce = _springConstant*_springError; //kX

            //calculate relative velocity 
            _body.GetVelocityAtLocalPoint(ref _bodyAttachPoint, out _bodyVelocity);

            //calculate dampning _force (bV)
            Vector2.Dot(ref _bodyVelocity, ref _difference, out _temp);
            _dampningForce = _dampningConstant*_temp/differenceMagnitude; //bV     

            //calculate final _force (spring + dampning)
            Vector2.Multiply(ref _differenceNormalized, -(_springForce + _dampningForce), out _force);

            _body.ApplyForceAtLocalPoint(ref _force, ref _bodyAttachPoint);
        }

        #region ApplyForce variables

        private const float _epsilon = .00001f;
        private Vector2 _bodyVelocity = Vector2.Zero;
        private Vector2 _bodyWorldPoint = Vector2.Zero;
        private float _dampningForce;
        private Vector2 _differenceNormalized;
        private Vector2 _force;
        private float _springForce;
        private float _temp;

        #endregion
    }
}