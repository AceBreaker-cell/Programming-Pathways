using UnityEngine;

[System.Serializable]
public class Question
{
    public string questionText;
    public string optionA;
    public string optionB;
    public bool correctAnswer;

    [HideInInspector]
    public int statueIndex;
}

[System.Serializable]
public class StatueQuestions
{
    public string statueName;
    public Question[] questions;
}