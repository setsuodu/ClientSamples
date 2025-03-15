using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public partial class UIChatHandler : MonoBehaviour
{
    //[Category("Filter Bad Words")]
    [Header("Filter Bad Words")]
    [Tooltip("Replace each character of bad word with this CHARACTER")]
    [SerializeField] protected string replaceBadWordWith = "*";
    [Tooltip("List of words to be filter")]
    [SerializeField] protected List<string> filterWordsList = new List<string>();

    //For UnityEditor use only
    [Tooltip("Editor tool to fill the FilterWordsList quickly using a string-list of words separated by newline. Please use carefully.")]
    [SerializeField][TextArea(10, 20)] protected string fillFilterListWith;

    /// <summary>
    /// FilterBadWords from the _message text using the filterWordsList and replace with bad word character.
    /// </summary>
    /// <param name="_message"></param>
    /// <returns></returns>
    /// Call this method in UIChatHandler.cs from OnReceiveChat()
    protected string FilterBadWords(string _message)
    {
        //Only execute if we have words to filter, else return back original message as it is
        if (filterWordsList.Count > 0)
        {
            foreach (string fWord in filterWordsList)
            {
                if (fWord.Trim() != "")
                {
                    //  Replace the word with replaceBadWordWith (but keep it the same length)
                    string strReplace = "";
                    for (int i = 0; i <= fWord.Length; i++)
                    {
                        strReplace += replaceBadWordWith;
                    }
                    _message = Regex.Replace(_message, fWord, strReplace, RegexOptions.IgnoreCase);
                }
            }
            return _message;
        }
        return _message;
    }

    [SerializeField] InputField chatInput;
    public void Send()
    {
        var result = FilterBadWords(chatInput.text);
        Debug.Log(result);

        chatInput.text = string.Empty;
    }
}