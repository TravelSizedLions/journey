using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Attributes {
    /// <summary>
    /// This attribute stub makes it possible to mark a public variable
    /// as visible to the unity editor, but not editable.
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute {}
}