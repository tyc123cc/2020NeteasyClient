using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

public class IPParse
{
    public XmlNodeList xnl;
    public IPParse(string filename)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(filename);
        XmlNode xn = doc.SelectSingleNode("collection");
        xnl = xn.ChildNodes;
    }

    public string GetIP()
    {
        foreach (XmlNode item in xnl)
        {
            if(item.Name == "connect")
            {
                XmlElement xe = (XmlElement)item;
                XmlNodeList xnl0 = xe.ChildNodes;
                return xnl0.Item(0).InnerText; ;
            }         
        }
        return "";
    }

    public int GetPort()
    {
        foreach (XmlNode item in xnl)
        {
            if (item.Name == "connect")
            {
                XmlElement xe = (XmlElement)item;
                XmlNodeList xnl0 = xe.ChildNodes;
                return int.Parse(xnl0.Item(1).InnerText) ;
            }

        }
        return 0;
    }
}

