
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public interface IServer_playerData
{
    UniTask<string> PlayerData_getText(string userId, string fileName);
    UniTask PlayerData_get(string userId, UnityAction<Stream> callback);
    UniTask PlayerData_set(string userId, Stream modelContent);
}
