using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextAttributeDatabase",
  menuName = "TextAttributeDatabase",
  order = 0)]
public class TextAttributeDatabase : ScriptableObject
{
    public List<TextAttributeDefinition> definitions;
    public StringTextAttributeBindingDictionary dictionary;
}

[System.Serializable]
public struct TextAttributeDefinition
{
    public string identifier;
    public enum Binding
    {
        Invalid = 0,
        DialogueUI,
        Actor,
        Camera,
    }
    public Binding binding;
}
