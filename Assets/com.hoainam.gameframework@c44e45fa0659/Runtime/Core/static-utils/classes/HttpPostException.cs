
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.Networking;

public class HttpPostException : Exception
{
	public bool isHostAvailable;

	//UnityWebRequestException.Text: the message received from server e.g. your request contain invalid name
	//UnityWebRequestException.Error: the system inform type of error e.g. you encounter 404 error
	//UnityWebRequestException.Message: combine Text + Error
	public HttpPostException(UnityWebRequestException e)
		: base(string.IsNullOrEmpty(e.Text) ? e.Error : e.Text)
	{
		isHostAvailable = e.Result != UnityWebRequest.Result.ConnectionError;
	}
}