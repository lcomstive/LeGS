using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LEGS
{
    public static class Extensions
    {
        public static bool CompareTags(this GameObject gameObject, string[] tags)
		{
            for(int i = 0; i < tags.Length; i++)
                if(gameObject.CompareTag(tags[i]))
                    return true;
            return false;
		}

        public static bool CompareTags(this Collider collider, string[] tags) => CompareTags(collider.gameObject, tags);
        public static bool CompareTags(this Collision collision, string[] tags) => CompareTags(collision.gameObject, tags);
    }
}
