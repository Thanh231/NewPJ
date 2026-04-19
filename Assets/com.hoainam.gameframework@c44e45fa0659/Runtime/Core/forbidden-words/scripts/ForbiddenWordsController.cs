
using System;

public partial class ForbiddenWordsController : SingletonMonoBehaviour<ForbiddenWordsController>
{
    private ForbiddenWordsProcessor chatProcessor;
    private ForbiddenWordsProcessor nicknameProcessor;

    public void Init(bool useChatFilter, bool useNicknameFilter)
    {
        if (useChatFilter)
        {
            chatProcessor = new ForbiddenWordsProcessor("chat");
        }

        if (useNicknameFilter)
        {
            nicknameProcessor = new ForbiddenWordsProcessor("nickname");
        }
    }
    
    public bool DoesChatContainProfanity(string content, out string forbiddenWord)
    {
        if (chatProcessor == null)
        {
            throw new Exception("use chat filter but not configure to use it");
        }
        
        return !chatProcessor.IsValid(content, out forbiddenWord);
    }
    
    public bool DoesNicknameContainProfanity(string content, out string forbiddenWord)
    {
        if (nicknameProcessor == null)
        {
            throw new Exception("use nickname filter but not configure to use it");
        }
        
        return !nicknameProcessor.IsValid(content, out forbiddenWord);
    }
}
