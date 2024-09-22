using UnityEngine;

namespace C__Script.Bots
{
    public class Defender : MonoBehaviour
    {
        [SerializeField] private float radiusDetect;
        [SerializeField] private LayerMask unitsLayerMask;
        [SerializeField] private ParticleSystem particlse;
        [SerializeField] private SpriteRenderer spriteColor;
        [SerializeField] private Transform transformForceFields;
        [SerializeField] private float speedMoving;

        [SerializeField] private Color complexity1Color;
        [SerializeField] private Color complexity2Color;
        [SerializeField] private Color complexity3Color;
        [SerializeField] private Color complexityDefoltColor;
        private Transform _target;

        private Vector2 _startPosition;
        private Vector3 _protectedPosition;

        public void InitDependencies(Vector3 startPosition, Vector2 protectedPosition, int complexity)
        {
            if (complexity == 1)
            {
                spriteColor.color = complexity1Color;
            }
            else if (complexity == 2)
            {
                spriteColor.color = complexity2Color;
            }
            else if (complexity == 3)
            {
                spriteColor.color = complexity3Color;
            }
            else
            {
                spriteColor.color = complexityDefoltColor;
            }

            particlse.startColor = spriteColor.color;
            _startPosition = startPosition;
            _protectedPosition = protectedPosition;
        }

        public void SetTarget(Transform tr)
        {
            if (_target != null)
            {
                return;
            }

            _target = tr;
        }

        public void DeletTarget()
        {
            _target = null;
        }

        public Transform GetTarget()
        {
            return _target;
        }

        private void FixedUpdate()
        {
            // transform.position = Vector2.MoveTowards(transform.position, startPosition, speedMoving * Time.fixedDeltaTime);

            if (_target != null)
            {
                if (Vector2.Distance(_target.position, transform.position) > radiusDetect)
                {
                    DeletTarget();
                }
                else
                {
                    transformForceFields.position = _target.position;
                }
            }
            else
            {
                transformForceFields.position = transform.position;
            }

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radiusDetect);
            foreach (var cl in colliders)
            {
                Defender df = cl.GetComponent<Defender>();
                Harpoon harpon = cl.GetComponent<Harpoon>();
                if (df != null && df != this)
                {
                    if (df.GetTarget() != this.transform)
                    {
                        SetTarget(df.transform);
                    }
                    else if (df.transform == _target)
                    {
                        DeletTarget();
                    }
                }

                if (harpon != null)
                {
                    harpon.RepulsionFromPos(_protectedPosition);
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.blue;

            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, radiusDetect);
        }
#endif
    }
}