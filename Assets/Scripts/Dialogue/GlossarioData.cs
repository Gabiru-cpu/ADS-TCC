using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct Glossary
{

    public string name;
    [TextArea(3, 10)]
    public string description;
    public Sprite image;
}

[CreateAssetMenu(fileName = "GlossarioData", menuName = "ScriptableObject/GlossaryScript", order = 2)]
public class GlossarioData : ScriptableObject
{

    public List<Glossary> glossaryScript;

}