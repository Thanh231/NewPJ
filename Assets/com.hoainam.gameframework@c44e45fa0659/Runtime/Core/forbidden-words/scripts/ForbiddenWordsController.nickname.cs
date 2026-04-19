
using UnityEngine;

public partial class ForbiddenWordsController
{
    public enum ValidateNicknameResult
    {
        Valid,
        ContainsForbiddenWord,
        TooShort,
        TooLong,
        ContainsSpecialCharacter
    }
    
    public const int MinNicknameLength = 3;
    public const int MaxNicknameLength = 8;

    public ValidateNicknameResult ValidateNickname(string nickname)
    {
        if (DoesNicknameContainProfanity(nickname, out var forbiddenWord))
        {
            Debug.Log($"[ValidateNickname] nickname '{nickname}' contains forbidden word '{forbiddenWord}'");
            return ValidateNicknameResult.ContainsForbiddenWord;
        }

        var length = nickname.Length;
        if (length < MinNicknameLength)
        {
            return ValidateNicknameResult.TooShort;
        }

        if (length > MaxNicknameLength)
        {
            return ValidateNicknameResult.TooLong;
        }

        if (DoesNicknameContainSpecialChar(nickname))
        {
            return ValidateNicknameResult.ContainsSpecialCharacter;
        }

        return ValidateNicknameResult.Valid;
    }

    private bool DoesNicknameContainSpecialChar(string nickname)
    {
        foreach (var c in nickname)
        {
            if (IsSpecialCharacter(c))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsSpecialCharacter(char c)
    {
        //Filter out symbols, numbers, and whitespace immediately
        if (!char.IsLetter(c))
        {
            return true;
        }

        //this is latin character
        if (c <= 0x00FF)
        {
            return false;
        }

        //this is korean character
        if ((c >= 0xAC00 && c <= 0xD7A3) || (c >= 0x1100 && c <= 0x11FF))
        {
            return false;
        }

        //this is chinese character
        if (c >= 0x4E00 && c <= 0x9FFF)
        {
            return false;
        }

        return true;
    }
}
