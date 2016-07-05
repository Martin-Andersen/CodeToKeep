using System;
using System.Collections.Generic;
using AmazedSaint.Elastic.Lib;

namespace Lbf.Driftstoette.WPFModules.ØkonomiModule
{
    public class DesignTimeData
    {
        public static List<ElasticObject> Betalingsfrigivelser
        {
            get
            {
                var list = new List<ElasticObject>();
                for (int i = 0; i < 4; i++)
                {
                    dynamic obj = new ElasticObject();
                    obj.IsSelected = false;
                    obj.OrdreNr = "123456";
                    obj.FakturaNr = "5689";
                    obj.SumTotalMedKrediteringFortegn = 7896;
                    obj.Status.DisplayName = "Ikke frigivet";
                    obj.Sag.JournalNr = 12345;
                    list.Add(obj);
                }
                return list;
            }
        }

        public static List<ElasticObject> Kvartaler
        {
            get
            {
                var list = new List<ElasticObject>();
                for (int i = 0; i < 4; i++)
                {
                    dynamic obj = new ElasticObject();
                    obj.SidsteTilsagnsDato = DateTime.Today;
                    obj.OrdrekørselsDato = DateTime.Today;
                    obj.VejledendeBogføringsDato = DateTime.Today;
                    obj.ForfaldsDato = DateTime.Today;
                    obj.Status.DisplayName = "Afventer ordreoprettelse";
                    list.Add(obj);
                }
                return list;
            }
        }

    }


}