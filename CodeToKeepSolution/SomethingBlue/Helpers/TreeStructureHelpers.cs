using System;
using System.Collections.Generic;
using System.Linq;

namespace SomethingBlue.Helpers
{
    public static class TreeStructureHelpers
    {
        /// <summary>
        /// Can be used like this:
        /// AddThisToThat.DeleteNodes<PropertyDefinition>(tab, x => x.Children, x => controlTypesToDelete.Contains(x.Type));
        /// </summary>
        public static void DeleteNodes<T>(T root, Func<T, IEnumerable<T>> getChildren, Func<T, bool> condition) where T : class 
        {
            if (root == null)
                throw new ArgumentNullException("root");

            if (null == getChildren)
                return;

            var children = getChildren(root);
            var enumerable = children as IList<T> ?? children.ToList();
            foreach (var child in enumerable)
                DeleteNodes(child, getChildren, condition);

            for (int i = enumerable.Count() - 1; i >= 0; i--)
                if (condition(enumerable[i]))
                    enumerable.RemoveAt(i);
        }
    }
}