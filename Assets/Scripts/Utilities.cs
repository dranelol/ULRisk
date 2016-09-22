using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

[Serializable]
public class Pair<T1, T2>
{
    public Pair(T1 item1, T2 item2)
    {
        First = item1;
        Second = item2;
    }

    [SerializeField]
    public T1 First { get; set; }

    [SerializeField]
    public T2 Second { get; set; }

    public override string ToString()
    {
        return "< " + First.ToString() + ", " + Second.ToString() + " >";
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Pair<T1, T2> other = (Pair<T1, T2>)obj;
        return (First.Equals(other.First) && Second.Equals(other.Second));
    }

    public override int GetHashCode()
    {
        if (First == null || Second == null)
        {
            return 0;
        }

        else
        {
            return First.GetHashCode() + 17 * Second.GetHashCode();
        }
    }
}

public static class EnumUtil<T>
{
    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }
}

public static class Utilities 
{
    static char[] SplitChars = new char[] { ' ', '\t' };

    public static string ColorToHexString(Color color)
    {
        Color32 color32 = color;
        return color32.r.ToString("X2") + color32.g.ToString("X2") + color32.b.ToString("X2") + color32.a.ToString("X2");
    }

    public static string[] ParseText(string text, int MaxChars)
    {
        string[] words = StrExplode(text);
        int lineLength = 0;
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < words.Length; i++)
        {
            string word = words[i];

            // if adding the new word to the current line would be too long,
            // then put it on a new line

            int wordLength = StripTags(word).Length;
            //int wordLength = word.Length;

            if (lineLength + wordLength > MaxChars)
            {
                // only move down to a new line if we have text on the current line

                // avoids situation where wrapped whitespace causes emptylines in text
                if (lineLength > 0)
                {
                    builder.Append(System.Environment.NewLine);
                    lineLength = 0;
                }

                // remove leading whitespace from the word so the new line starts flush to the left.
                word = word.TrimStart();
            }

            builder.Append(word);
            lineLength += wordLength;

        }

        string[] lines = builder.ToString().Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.None);

        return lines;
    }

    static string[] StrExplode(string str)
    {
        List<string> words = new List<string>();
        int start = 0;

        while (true)
        {
            // look for the first word split, starting from 0
            int index = str.IndexOfAny(SplitChars, start);


            if (index == -1)
            {
                // no more word splits, we're done!
                words.Add(str.Substring(start));

                return words.ToArray();
            }

            // we need to split a word off
            string newWord = str.Substring(start, index - start);

            char next = str.Substring(index, 1)[0];

            if (char.IsWhiteSpace(next))
            {
                // stick dashes, etc to the previous word; whitespace doesnt matter

                words.Add(newWord);
                words.Add(next.ToString());
            }

            else
            {
                words.Add(newWord + next);
            }

            start = index + 1;
        }
    }

    static string StripTags(string text)
    {
        string newText = "";

        char[] charArray = text.ToCharArray();

        for (int i = 0; i < charArray.Length; i++)
        {
            if (charArray[i] == '[')
            {
                // parse text, find end bracket, increase counter by width of tag

                int length = 0;

                for (int j = i; j < charArray.Length; j++)
                {
                    if (charArray[j] == ']')
                    {
                        length = j - i;
                        break;
                    }
                }

                i += length;
            }

            else
            {

                newText += charArray[i];
            }

        }

        return newText;
    }

    
}

public static class MathHelper
{
    public static Vector3 GetRandomVector3(Vector3 min, Vector3 max)
    {
        Vector3 rand = Vector3.zero;

        rand.x = UnityEngine.Random.Range(min.x, max.x);
        rand.y = UnityEngine.Random.Range(min.y, max.y);
        rand.z = UnityEngine.Random.Range(min.z, max.z);

        return rand;
    }

    public static Vector3 GetExtendedForwardVector(GameObject source, int distanceFromTarget)
    {
        Vector3 distanceInFront = source.transform.position + source.transform.forward * distanceFromTarget;
        return distanceInFront;
    }
}

public static class Rotations
{
    public static Vector3 RotateAboutX(Vector3 vectorToRotate, float angle)
    {
        Quaternion rotation = Quaternion.Euler(angle, 0, 0);
        return rotation * vectorToRotate;
    }
    public static Vector3 RotateAboutY(Vector3 vectorToRotate, float angle)
    {
        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        return rotation * vectorToRotate;
    }
    public static Vector3 RotateAboutZ(Vector3 vectorToRotate, float angle)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        return rotation * vectorToRotate;
    }

    /// <summary>
    /// Determine the signed angle between two vectors, with normal 'n'
    /// as the rotation axis.
    /// </summary>

    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
    {

        return Mathf.Atan2(

            Vector3.Dot(n, Vector3.Cross(v1, v2)),

            Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;

    }
}