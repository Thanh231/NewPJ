
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public interface IServer_database
{
    UniTask Database_overwriteDoc(string collectionName, string documentKey, Dictionary<string, object> documentValue);
    UniTask Database_updateFieldDoc(string collectionName, string documentKey,
        Dictionary<string, object> documentValue);
    UniTask<bool> Database_checkDocExist(string collectionName, string documentKey);
}
