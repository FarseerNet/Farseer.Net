using System.Collections.Generic;
using System.Linq;
using Collections.Pooled;
using FS.Core;
using FS.Utils.Common;

public class UserPOByDataRow : Farseer.Net.DataDemo.UserPO
{
    public static PooledList<Farseer.Net.DataDemo.UserPO> ToList(IEnumerable<MapingData> mapData)
    {
        var firstData = mapData.FirstOrDefault();
        var lst            = new PooledList<Farseer.Net.DataDemo.UserPO>(firstData.DataList.Count);
        for (int i = 0; i < firstData.DataList.Count; i++)
        {
            lst.Add(ToEntity(mapData, i));
        }
        return lst;
    }

    public static Farseer.Net.DataDemo.UserPO ToEntity(IEnumerable<MapingData> mapData, int rowsIndex = 0)
    {
        if (mapData == null || !mapData.Any() || mapData.FirstOrDefault().DataList.Count == 0) { return null; }
        var entity = new Farseer.Net.DataDemo.UserPO();
        foreach (var map in mapData)
        {
            var col = map.DataList[rowsIndex];
            if (col == null) { continue; }
            switch (map.ColumnName.ToUpper())
            {
                case "ID":
                    if (col is System.Int32 i) { entity.Id = i; }
                    else
                    {
                        if (System.Int32.TryParse(col.ToString(), out System.Int32 id_Out)) { entity.Id = id_Out; }
                    }
                    break;
                case "NAME":
                    entity.Name = col.ToString();
                    break;
                case "AGE":
                    if (col is System.Int32) { entity.Age = (System.Int32)col; }
                    else
                    {
                        if (System.Int32.TryParse(col.ToString(), out System.Int32 age_Out)) { entity.Age = age_Out; }
                    }
                    break;
                case "FULLNAME":
                    entity.Fullname = Jsons.ToObject<Farseer.Net.DataDemo.FullNameVO>(col.ToString());
                    break;
                case "SPECIALTY":
                    entity.Specialty = StringHelper.ToList<System.String>(col.ToString()).ToList();
                    break;
                case "ATTRIBUTE":
                    entity.Attribute = Jsons.ToObject<Dictionary<System.String, System.String>>(col.ToString());
                    break;
                case "GENDER":
                    if (typeof(Farseer.Net.DataDemo.GenderType).GetEnumUnderlyingType() == col.GetType()) { entity.Gender = (Farseer.Net.DataDemo.GenderType)col; }
                    else
                    {
                        if (System.Enum.TryParse(col.ToString(), out Farseer.Net.DataDemo.GenderType gender_Out)) { entity.Gender = gender_Out; }
                    }
                    break;

            }
        }
        return entity;
    }
}