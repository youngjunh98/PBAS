using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBAS
{
    public enum PropertyFieldType
    {
        Int,
        Float,
        Bool,
        String,
        Vector2,
        Vector3,
        Vector4,
        Quaternion
    }

    [Serializable]
    public struct PropertyFieldValue
    {
        [SerializeField]
        private PropertyFieldType m_type;
        [SerializeField]
        private int m_intValue;
        [SerializeField]
        private float m_floatValue;
        [SerializeField]
        private bool m_boolValue;
        [SerializeField]
        private string m_stringValue;
        [SerializeField]
        private Vector2 m_vector2Value;
        [SerializeField]
        private Vector3 m_vector3Value;
        [SerializeField]
        private Vector4 m_vector4Value;
        [SerializeField]
        private Quaternion m_quaternionValue;

        public PropertyFieldType Type
        {
            get => m_type;
            set => m_type = value;
        }

        public int IntValue
        {
            get => m_intValue;
            set => m_intValue = value;
        }

        public float FloatValue
        {
            get => m_floatValue;
            set => m_floatValue = value;
        }

        public bool BoolValue
        {
            get => m_boolValue;
            set => m_boolValue = value;
        }

        public string StringValue
        {
            get => m_stringValue;
            set => m_stringValue = value;
        }

        public Vector2 Vector2Value
        {
            get => m_vector2Value;
            set => m_vector2Value = value;
        }

        public Vector3 Vector3Value
        {
            get => m_vector3Value;
            set => m_vector3Value = value;
        }

        public Vector4 Vector4Value
        {
            get => m_vector4Value;
            set => m_vector4Value = value;
        }

        public Quaternion QuaternionValue
        {
            get => m_quaternionValue;
            set => m_quaternionValue = value;
        }

        public static bool operator == (PropertyFieldValue lhs, PropertyFieldValue rhs)
        {
            bool bIsEqual = false;

            if (lhs.Type == rhs.Type)
            {
                switch (lhs.Type)
                {
                    case PropertyFieldType.Int:
                    {
                        bIsEqual = lhs.IntValue == rhs.IntValue;
                        break;
                    }

                    case PropertyFieldType.Float:
                    {
                        bIsEqual = lhs.FloatValue == rhs.FloatValue;
                        break;
                    }

                    case PropertyFieldType.Bool:
                    {
                        bIsEqual = lhs.BoolValue == rhs.BoolValue;
                        break;
                    }

                    case PropertyFieldType.String:
                    {
                        bIsEqual = lhs.StringValue == rhs.StringValue;
                        break;
                    }

                    case PropertyFieldType.Vector2:
                    {
                        bIsEqual = lhs.Vector2Value == rhs.Vector2Value;
                        break;
                    }

                    case PropertyFieldType.Vector3:
                    {
                        bIsEqual = lhs.Vector3Value == rhs.Vector3Value;
                        break;
                    }

                    case PropertyFieldType.Vector4:
                    {
                        bIsEqual = lhs.Vector4Value == rhs.Vector4Value;
                        break;
                    }

                    case PropertyFieldType.Quaternion:
                    {
                        bIsEqual = lhs.QuaternionValue == rhs.QuaternionValue;
                        break;
                    }
                }
            }

            return bIsEqual;
        }

        public static bool operator != (PropertyFieldValue lhs, PropertyFieldValue rhs)
        {
            return !(lhs == rhs);
        }

        public override string ToString ()
        {
            var result = string.Empty;

            switch (Type)
            {
                case PropertyFieldType.Int:
                {
                    result = IntValue.ToString ();
                    break;
                }

                case PropertyFieldType.Float:
                {
                    result = FloatValue.ToString ();
                    break;
                }

                case PropertyFieldType.Bool:
                {
                    result = BoolValue.ToString ();
                    break;
                }

                case PropertyFieldType.String:
                {
                    result = StringValue;
                    break;
                }

                case PropertyFieldType.Vector2:
                {
                    result = Vector2Value.ToString ();
                    break;
                }

                case PropertyFieldType.Vector3:
                {
                    result = Vector3Value.ToString ();
                    break;
                }

                case PropertyFieldType.Vector4:
                {
                    result = Vector4Value.ToString ();
                    break;
                }

                case PropertyFieldType.Quaternion:
                {
                    result = QuaternionValue.ToString ();
                    break;
                }
            }

            return result;
        }
    }

    [Serializable]
    public class PropertyField
    {
        [SerializeField]
        private string m_name;
        [SerializeField]
        private PropertyFieldValue m_defaultValue;

        public string Name
        {
            get => m_name;
        }

        public PropertyFieldValue DefaultValue
        {
            get => m_defaultValue;
        }
    }
}
