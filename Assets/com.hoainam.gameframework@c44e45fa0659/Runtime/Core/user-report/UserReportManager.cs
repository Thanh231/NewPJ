
#if USE_USER_REPORT

using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Unity.Services.Core;
using Unity.Services.UserReporting;
using UnityEngine;

public class UserReportManager : SingletonMonoBehaviour<UserReportManager>
{
	public class AttachmentInfo
	{
		public string title;
		public string content;
	}

	public class ReportInfo
	{
		public string summary;
		public Dictionary<string, string> dimensions = new Dictionary<string, string>();
		public string description;
		public List<AttachmentInfo> attachment = new List<AttachmentInfo>();
	}

	private Queue<ReportInfo> waitingReports = new Queue<ReportInfo>();
	private bool isProcessingReport;
	private bool isInitialzed;

	async UniTask Start()
	{
		await Initialize();
		isInitialzed = true;

		if (waitingReports.Count > 0)
		{
			SendReport(waitingReports.Dequeue());
		}
	}

	private async UniTask Initialize()
	{
		var done = false;
		UnityServices.InitializeAsync().ContinueWith(task =>
		{
			done = true;
		}).AsUniTask().Forget();
		await UniTask.WaitUntil(() => done);
	}

	public void SendReport(ReportInfo reportInfo)
	{
		if (isProcessingReport || !isInitialzed)
		{
			waitingReports.Enqueue(reportInfo);
			return;
		}

		isProcessingReport = true;

		//configure report
		foreach (var i in reportInfo.attachment)
		{
			UserReportingService.Instance.AddAttachmentToReport(i.title, i.title,
				Encoding.UTF8.GetBytes(i.content));
		}

		//create report
		UserReportingService.Instance.CreateNewUserReport();

		//configure report
		if (!string.IsNullOrEmpty(reportInfo.summary))
		{
			UserReportingService.Instance.SetReportSummary(reportInfo.summary);
		}

		foreach (var i in reportInfo.dimensions)
		{
			UserReportingService.Instance.AddDimensionValue(i.Key, i.Value);
		}

		if (!string.IsNullOrEmpty(reportInfo.description))
		{
			UserReportingService.Instance.SetReportDescription(reportInfo.description);
		}

		//send report
		UserReportingService.Instance.SendUserReport(_ => { }, success =>
		{
			Debug.LogError($"[UserReport] sending report {reportInfo.summary} with result={success}");
			UserReportingService.Instance.ClearOngoingReport();
			isProcessingReport = false;
			if (waitingReports.Count > 0)
			{
				SendReport(waitingReports.Dequeue());
			}
		});
	}
}

#endif