using UnityEngine;

namespace Dvalin
{
    /// <summary>
    /// A wrapper for Valheim's <see cref="Recipe" />.
    /// </summary>
    [UnityEngine.CreateAssetMenu]
    public class RecipeWrapper : Recipe
    {
        public bool includeInRealease = false;
    }
}