using UnityEngine;
using System.Collections.Generic;

public class EventLine : Line
{
    public string eventDictKey;
    public List<string> conditionDictKeys;
    public Condition[] conditions;
}
