using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

public static class GUIDUtility
{

    public static string getUniqueID(bool generateNewIDState = false)
    {
        return generateGUID();
    }

	public static string generateGUID()
	{
		var random = new System.Random();
		DateTime epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
		double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;

		string uniqueID = String.Format("{0:X}", Convert.ToInt32(timestamp))                //Time
						+ "-" + String.Format("{0:X}", random.Next(1000000000))                   //Random Number
						+ "-" + String.Format("{0:X}", random.Next(1000000000))                 //Random Number
						+"-" + String.Format("{0:X}", random.Next(1000000000))                  //Random Number
						+"-" + String.Format("{0:X}", random.Next(1000000000));                  //Random Number

		GD.Print(uniqueID);

		return uniqueID;
	}

}