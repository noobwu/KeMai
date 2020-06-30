using System;

namespace KeMai.DataAnnotations
{
    /// <summary>
    /// Compute attribute.
    /// Use to indicate that a property is a Calculated Field 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ComputeAttribute : AttributeBase
    {
        public string Expression { get; set; }

        public ComputeAttribute() : this(string.Empty) { }

        public ComputeAttribute(string expression)
        {
            Expression = expression;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ComputedAttribute : AttributeBase {}
}

