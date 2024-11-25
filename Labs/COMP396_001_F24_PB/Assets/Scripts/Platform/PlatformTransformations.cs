using UnityEngine;

using static Platform.PlatformTransformationSettings;

namespace Platform
{
    public class PlatformTransformations : MonoBehaviour
    {
        [Header("------------------------------------------------------------------------------------------------------")]
        [Header("LOCOMOTION")]
        [Header("------------------------------------------------------------------------------------------------------")]
        [Header("Locomotion Attributes")]
        [Range(0f, 30f)]
        [SerializeField] private float _movementDurationPerRound = 1f;
        [Tooltip("Enabling this will make Locomotion - X duration half")]
        [SerializeField] private bool _useLocomotionModuloFunction_X;
        [SerializeField] private float _displacement_X = 3.5f;
        [Tooltip("Enabling this will make Locomotion - Y duration half")]
        [SerializeField] private bool _useLocomotionModuloFunction_Y;
        [SerializeField] private float _displacement_Y = 3.5f;
        [Tooltip("Enabling this will make Locomotion - Z duration half")]
        [SerializeField] private bool _useLocomotionModuloFunction_Z;
        [SerializeField] private float _displacement_Z = 3.5f;

        [Header("Locomotion Axes")]
        [SerializeField] private TransformDimension _movementDimension = TransformDimension.X;

        [Header("Locomotion Function")]
        [SerializeField] private TransformFunction _movementFunction_X;
        [SerializeField] private TransformFunction _movementFunction_Y;
        [SerializeField] private TransformFunction _movementFunction_Z;

        [Header("Debug Information")]
        [SerializeField] private Vector3 _initialPosition;
        [SerializeField] private float _movementElapsedTime = 0f;

        [Header("------------------------------------------------------------------------------------------------------")]
        [Header("SCALE")]
        [Header("------------------------------------------------------------------------------------------------------")]
        [Header("Scale Attributes")]
        [Range(0f, 100f)]
        [SerializeField] private float _scaleDurationPerRound = 1f;
        [Tooltip("Enabling this will make Scale - X duration half")]
        [SerializeField] private bool _useScaleModuloFunction_X;
        [SerializeField] private float _scale_X = 5f;
        [Tooltip("Enabling this will make Scale - Y duration half")]
        [SerializeField] private bool _useScaleModuloFunction_Y;
        [SerializeField] private float _scale_Y = 5f;
        [Tooltip("Enabling this will make Scale _ Z duration half")]
        [SerializeField] private bool _useScaleModuloFunction_Z;
        [SerializeField] private float _scale_Z = 5f;

        [Header("Scale Axes")]
        [SerializeField] private TransformDimension _scaleDimension;

        [Header("Scale Function")]
        [SerializeField] private TransformFunction _scaleFunction_X;
        [SerializeField] private TransformFunction _scaleFunction_Y;
        [SerializeField] private TransformFunction _scaleFunction_Z;

        [Header("Debug Information")]
        [SerializeField] private Vector3 _initialScale;
        [SerializeField] private float _scaleElapsedTime = 0f;

        [Header("------------------------------------------------------------------------------------------------------")]
        [Header("ROTATION")]
        [Header("------------------------------------------------------------------------------------------------------")]
        [Header("Rotation Attributes")]
        [Range(0f, 359f)]
        [SerializeField] private float _rotationDurationPerRound = 1f;
        [Tooltip("Enabling this will make Rotation duration half")]
        [SerializeField] private bool _useRotationModuloFunction_X;
        [SerializeField] private float _rotation_X = 5f;
        [Tooltip("Enabling this will make Rotation duration half")]
        [SerializeField] private bool _useRotationModuloFunction_Y;
        [SerializeField] private float _rotation_Y = 5f;
        [Tooltip("Enabling this will make Rotation duration half")]
        [SerializeField] private bool _useRotationModuloFunction_Z;
        [SerializeField] private float _rotation_Z = 5f;

        [Header("Rotation Axes")]
        [SerializeField] private TransformDimension _rotationDimension;

        [Header("Rotation Function")]
        [SerializeField] private TransformFunction _rotationFunction_X;
        [SerializeField] private TransformFunction _rotationFunction_Y;
        [SerializeField] private TransformFunction _rotationFunction_Z;

        [Header("Debug Information")]
        [SerializeField] private Vector3 _initialRotation;
        [SerializeField] private float _rotationElapsedTime = 0f;

        private void Awake()
        {
            InitMovementAttributes();
            InitScaleAttributes();
            InitRotationAttributes();
        }

        private void Update()
        {
            MovePlatform();
            ScalePlatform();
            RotationPlatform();
        }

        #region ROTATION
        private void InitRotationAttributes()
        {
            _rotationElapsedTime = 0f;
            _initialRotation = transform.rotation.eulerAngles;
        }

        private void RotationPlatform()
        {
            var normalizedElapsedTime = GetNormalizedElapsedTime(ref _rotationElapsedTime, _rotationDurationPerRound);

            var rotation = Vector3.zero;

            switch (_rotationDimension)
            {
                case TransformDimension.NONE:
                    break;
                case TransformDimension.X:
                    rotation += Rotate_X(_rotation_X, normalizedElapsedTime, _useRotationModuloFunction_X);
                    break;
                case TransformDimension.Y:
                    rotation += Rotate_Y(_rotation_Y, normalizedElapsedTime, _useRotationModuloFunction_Y);
                    break;
                case TransformDimension.Z:
                    rotation += Rotate_Z(_rotation_Z, normalizedElapsedTime, _useRotationModuloFunction_Z);
                    break;
                case TransformDimension.XY:
                    rotation += Rotate_X(_rotation_X, normalizedElapsedTime, _useRotationModuloFunction_X);
                    rotation += Rotate_Y(_rotation_Y, normalizedElapsedTime, _useRotationModuloFunction_Y);
                    break;
                case TransformDimension.XZ:
                    rotation += Rotate_X(_rotation_X, normalizedElapsedTime, _useRotationModuloFunction_X);
                    rotation += Rotate_Z(_rotation_Y, normalizedElapsedTime, _useRotationModuloFunction_Z);
                    break;
                case TransformDimension.YZ:
                    rotation += Rotate_Y(_rotation_Y, normalizedElapsedTime, _useRotationModuloFunction_Y);
                    rotation += Rotate_Z(_rotation_Z, normalizedElapsedTime, _useRotationModuloFunction_Z);
                    break;
                case TransformDimension.ALL_DIMENSIONS:
                    rotation += Rotate_X(_rotation_X, normalizedElapsedTime, _useRotationModuloFunction_X);
                    rotation += Rotate_Y(_rotation_Y, normalizedElapsedTime, _useRotationModuloFunction_Y);
                    rotation += Rotate_Z(_rotation_Z, normalizedElapsedTime, _useRotationModuloFunction_Z);
                    break;
            }

            transform.eulerAngles = _initialRotation + rotation;
        }

        private Vector3 Rotate_X(float magnitude, float normalizedElapsedTime, bool useModuloFunction)
        {
            return TransformWithFunction(_rotationFunction_X, magnitude, normalizedElapsedTime, Vector3.right, useModuloFunction);
        }

        private Vector3 Rotate_Y(float magnitude, float normalizedElapsedTime, bool useModuloFunction)
        {
            return TransformWithFunction(_rotationFunction_Y, magnitude, normalizedElapsedTime, Vector3.up, useModuloFunction);
        }

        private Vector3 Rotate_Z(float magnitude, float normalizedElapsedTime, bool useModuloFunction)
        {
            return TransformWithFunction(_rotationFunction_Z, magnitude, normalizedElapsedTime, Vector3.forward, useModuloFunction);
        }
        #endregion

        #region SCALE
        private void InitScaleAttributes()
        {
            _scaleElapsedTime = 0f;
            _initialScale = transform.localScale;
        }

        private void ScalePlatform()
        {
            var normalizedElapsedTime = GetNormalizedElapsedTime(ref _scaleElapsedTime, _scaleDurationPerRound);

            var scale = Vector3.zero;

            switch (_scaleDimension)
            {
                case TransformDimension.NONE:
                    break;
                case TransformDimension.X:
                    scale += Scale_X(_scale_X, normalizedElapsedTime, _useScaleModuloFunction_X);
                    break;
                case TransformDimension.Y:
                    scale += Scale_Y(_scale_Y, normalizedElapsedTime, _useScaleModuloFunction_Y);
                    break;
                case TransformDimension.Z:
                    scale += Scale_Z(_scale_Z, normalizedElapsedTime, _useScaleModuloFunction_Z);
                    break;
                case TransformDimension.XY:
                    scale += Scale_X(_scale_X, normalizedElapsedTime, _useScaleModuloFunction_X);
                    scale += Scale_Y(_scale_Y, normalizedElapsedTime, _useScaleModuloFunction_Y);
                    break;
                case TransformDimension.XZ:
                    scale += Scale_X(_scale_X, normalizedElapsedTime, _useScaleModuloFunction_X);
                    scale += Scale_Z(_scale_Z, normalizedElapsedTime, _useScaleModuloFunction_Z);
                    break;
                case TransformDimension.YZ:
                    scale += Scale_Y(_scale_Y, normalizedElapsedTime, _useScaleModuloFunction_Y);
                    scale += Scale_Z(_scale_Z, normalizedElapsedTime, _useScaleModuloFunction_Z);
                    break;
                case TransformDimension.ALL_DIMENSIONS:
                    scale += Scale_X(_scale_X, normalizedElapsedTime, _useScaleModuloFunction_X);
                    scale += Scale_Y(_scale_Y, normalizedElapsedTime, _useScaleModuloFunction_Y);
                    scale += Scale_Z(_scale_Z, normalizedElapsedTime, _useScaleModuloFunction_Z);
                    break;
            }

            transform.localScale = _initialScale + scale;
        }

        private Vector3 Scale_X(float magnitude, float normalizedElapsedTime, bool useModuloFunction)
        {
            return TransformWithFunction(_scaleFunction_X, magnitude, normalizedElapsedTime, Vector3.right, useModuloFunction);
        }

        private Vector3 Scale_Y(float magnitude, float normalizedElapsedTime, bool useModuloFunction)
        {
            return TransformWithFunction(_scaleFunction_Y, magnitude, normalizedElapsedTime, Vector3.up, useModuloFunction);
        }

        private Vector3 Scale_Z(float magnitude, float normalizedElapsedTime, bool useModuloFunction)
        {
            return TransformWithFunction(_scaleFunction_Z, magnitude, normalizedElapsedTime, Vector3.forward, useModuloFunction);
        }
        #endregion

        #region MOVEMENT
        private void InitMovementAttributes()
        {
            _movementElapsedTime = 0f;
            _initialPosition = transform.position;
        }

        private void MovePlatform()
        {
            var normalizedElapsedTime = GetNormalizedElapsedTime(ref _movementElapsedTime, _movementDurationPerRound);

            var position = Vector3.zero;

            switch (_movementDimension)
            {
                case TransformDimension.NONE:
                    break;
                case TransformDimension.X:
                    position += Move_X(_displacement_X, normalizedElapsedTime, _useLocomotionModuloFunction_X);
                    break;
                case TransformDimension.Y:
                    position += Move_Y(_displacement_Y, normalizedElapsedTime, _useLocomotionModuloFunction_Y);
                    break;
                case TransformDimension.Z:
                    position += Move_Z(_displacement_Z, normalizedElapsedTime, _useLocomotionModuloFunction_Z);
                    break;
                case TransformDimension.XY:
                    position += Move_X(_displacement_X, normalizedElapsedTime, _useLocomotionModuloFunction_X);
                    position += Move_Y(_displacement_Y, normalizedElapsedTime, _useLocomotionModuloFunction_Y);
                    break;
                case TransformDimension.XZ:
                    position += Move_X(_displacement_X, normalizedElapsedTime, _useLocomotionModuloFunction_X);
                    position += Move_Z(_displacement_Z, normalizedElapsedTime, _useLocomotionModuloFunction_Z);
                    break;
                case TransformDimension.YZ:
                    position += Move_Z(_displacement_Z, normalizedElapsedTime, _useLocomotionModuloFunction_Y);
                    position += Move_Y(_displacement_Y, normalizedElapsedTime, _useLocomotionModuloFunction_Z);
                    break;
                case TransformDimension.ALL_DIMENSIONS:
                    position += Move_X(_displacement_X, normalizedElapsedTime, _useLocomotionModuloFunction_X);
                    position += Move_Y(_displacement_Y, normalizedElapsedTime, _useLocomotionModuloFunction_Y);
                    position += Move_Z(_displacement_Z, normalizedElapsedTime, _useLocomotionModuloFunction_Z);
                    break;
            }

            transform.position = _initialPosition + position;
        }

        private Vector3 Move_X(float magnitude, float normalizedElapsedTime, bool useModuloFunction)
        {
            return TransformWithFunction(_movementFunction_X, magnitude, normalizedElapsedTime, Vector3.right, useModuloFunction);
        }

        private Vector3 Move_Y(float magnitude, float normalizedElapsedTime, bool useModuloFunction)
        {
            return TransformWithFunction(_movementFunction_Y, magnitude, normalizedElapsedTime, Vector3.up, useModuloFunction);
        }

        private Vector3 Move_Z(float magnitude, float normalizedElapsedTime, bool useModuloFunction)
        {
            return TransformWithFunction(_movementFunction_Z, magnitude, normalizedElapsedTime, Vector3.forward, useModuloFunction);
        }
        #endregion

        #region UTILITY
        private float GetNormalizedElapsedTime(ref float elapsedTime, float durationPerRound)
        {
            elapsedTime += Time.deltaTime;
            return elapsedTime / durationPerRound;
        }

        private Vector3 TransformWithFunction(TransformFunction function, float magnitude, float normalizedElapsedTime, Vector3 unitDirection, bool useModuloFunction)
        {
            var functionOfTime = 0f;

            switch (function)
            {
                case TransformFunction.SIN:
                    functionOfTime = Mathf.Sin(normalizedElapsedTime * 2f * Mathf.PI);
                    break;
                case TransformFunction.COS:
                    functionOfTime = Mathf.Cos(normalizedElapsedTime * 2f * Mathf.PI);
                    break;
            }

            return (useModuloFunction ? Mathf.Abs(functionOfTime) : functionOfTime) * magnitude * unitDirection;
        }
        #endregion

        #region DEBUG
        private void D(string message)
        {
            Debug.Log($"<<PlatformTransformations>> {message}");
        }
        #endregion
    }
}
