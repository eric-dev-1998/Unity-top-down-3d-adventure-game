using UnityEngine;

namespace EventSystem
{
    [CreateAssetMenu(fileName = "Event sequence", menuName = "My tools/Event sequence")]
    public class Sequence : ScriptableObject
    {
        public Event startEvent;

        public override string ToString()
        {
            string res = "";
            Event evt = null;

            res += startEvent.GetType().AssemblyQualifiedName.Split(',')[0] + "\n";
            //evt = startEvent.next[0];

            while (evt != null)
            {
                res += evt.GetType().AssemblyQualifiedName.Split(",")[0] + "\n";
                //evt = evt.next[0];
            }

            return res;
        }
    }
}
