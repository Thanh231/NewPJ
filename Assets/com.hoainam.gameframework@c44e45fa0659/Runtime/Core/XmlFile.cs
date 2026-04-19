
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine.Events;

public class XmlFile
{
	private string path;
	private XmlDocument doc;
	private XmlElement root;
	private Dictionary<string, string> dicNamespace = new Dictionary<string, string>();

	public XmlElement Root => root;

	public XmlFile(string path)
	{
		this.path = path;

		doc = new XmlDocument();
		doc.Load(path);

		root = doc.DocumentElement;
		foreach (XmlAttribute i in root.Attributes)
		{
			if (i.Prefix.Equals("xmlns"))
			{
				dicNamespace.Add(i.LocalName, i.Value);
			}
		}
	}

	public void Save()
	{
		doc.Save(path);
	}

	#region attribute utils

	private XmlAttribute CreateAttribute(string name, string value, string prefix = null)
	{
		XmlAttribute attribute;
		if (prefix != null)
		{
			attribute = doc.CreateAttribute(prefix, name, dicNamespace[prefix]);
		}
		else
		{
			attribute = doc.CreateAttribute(name);
		}
		attribute.Value = value;
		return attribute;
	}

	public void SetAttribute(XmlElement element, string attributeName, string attributeValue, string attributePrefix = null)
	{
		var attribute = CreateAttribute(attributeName, attributeValue, attributePrefix);
		element.Attributes.SetNamedItem(attribute);
	}

	#endregion

	#region element utils

	public void AddElement(XmlElement parent, string name, UnityAction<XmlElement> callback)
	{
		var element = doc.CreateElement(name);
		callback?.Invoke(element);
		parent.AppendChild(element);
	}

	public XmlElement GetChildElement(XmlElement parent, string name)
	{
		var l = GetListChildrenElement(parent, name);
		if (l.Count == 1)
		{
			return l[0];
		}
		else
		{
			throw new Exception($"there're {l.Count} child element {name} of element {parent.Name}");
		}
	}

	public XmlElement GetChildElementWithAttribute(XmlElement parent, string tagName, string attributeName, string attributeVal)
	{
		var l = GetListChildrenElement(parent, tagName);
		foreach (var tag in l)
		{
			foreach (XmlAttribute attribute in tag.Attributes)
			{
				if (attribute.Name.Equals(attributeName) && attribute.Value.Equals(attributeVal))
				{
					return tag;
				}
			}
		}
		return null;
	}

	public List<XmlElement> GetListChildrenElement(XmlElement parent, string name)
	{
		var l = new List<XmlElement>();
		for (var i = 0; i < parent.ChildNodes.Count; i++)
		{
			var node = parent.ChildNodes[i];
			if (node.Name.Equals(name))
			{
				l.Add((XmlElement)node);
			}
		}
		return l;
	}

	#endregion

	#region namespace utils

	public void AddNamespace(string name, string url)
	{
		if (dicNamespace.ContainsKey(name))
		{
			return;
		}

		var attribute = doc.CreateAttribute($"xmlns:{name}");
		attribute.Value = url;
		root.Attributes.Append(attribute);
		dicNamespace.Add(name, url);
	}

	#endregion
}