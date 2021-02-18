using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace careerNet
{
    class Program
    {
        static string connectionString = "server = localhost; uid = sa; pwd = 1111; database = PrivateData;";
        static void Main(string[] args)
        {
            // 마일(Mile) 단위
            //double distanceMile =
            //    distance(37.504198, 127.047967, 37.501025, 127.037701, "");
            //Console.WriteLine(distanceMile);
            // 킬로미터(Kilo Meter) 단위
            //double distanceKiloMeter =
            //    distance(37.504198, 127.047967, 37.501025, 127.037701, "kilometer");
            //Console.WriteLine(distanceKiloMeter);
            // 미터(Meter) 단위

            DataSet subwayDs = selectDS("select * from subway_addr");
            DataSet examDs = selectDS("select * from exam_addr");

            List<exam_addr> list = new List<exam_addr>();
            double minMeter = 9999999999;
            int subwayCnt = 0;
            for (int i = 0; i < examDs.Tables[0].Rows.Count; i++)
            {
                DataRow examDr = examDs.Tables[0].Rows[i];
                for (int j= 0; j < subwayDs.Tables[0].Rows.Count; j++)
                {
                    DataRow subwayDr = subwayDs.Tables[0].Rows[j];
                    double distanceMeter = distance(Convert.ToDouble(examDr["lat"]), Convert.ToDouble(examDr["lng"]), Convert.ToDouble(subwayDr["lat"]), Convert.ToDouble(subwayDr["lng"]), "meter");
                    if (minMeter > distanceMeter)
                    {
                        minMeter = distanceMeter;
                        subwayCnt = j;
                    }
                }

                
                exam_addr model = new exam_addr();
                model.seq_no = examDr["seq_no"].ToString();
                model.code1 = examDr["code1"].ToString();
                model.code2 = examDr["code2"].ToString();
                model.code3 = examDr["code3"].ToString();
                model.name = examDr["name"].ToString();
                model.addr = examDr["addr"].ToString();
                model.lng = examDr["lng"].ToString();
                model.lat = examDr["lat"].ToString();
                model.subway_code1 = subwayDs.Tables[0].Rows[subwayCnt]["code1"].ToString();
                model.subway_code2 = subwayDs.Tables[0].Rows[subwayCnt]["code2"].ToString();
                model.subway_code3 = subwayDs.Tables[0].Rows[subwayCnt]["code3"].ToString();
                model.subway_name = subwayDs.Tables[0].Rows[subwayCnt]["name"].ToString();
                model.subway_dstnc = minMeter.ToString();

                list.Add(model);
                minMeter = 9999999999;
                subwayCnt = 0;
            }

            for (int i = 0; i < list.Count; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(" update exam_addr ");
                sb.Append(" set subway_code1 = '" + list[i].subway_code1.Trim() + "' ");
                sb.Append(" ,subway_code2 = '" + list[i].subway_code2.Trim() + "' ");
                sb.Append(" ,subway_code3 = '" + list[i].subway_code3.Trim() + "' ");
                sb.Append(" ,subway_name = '" + list[i].subway_name.Trim() + "' ");
                sb.Append(" ,subway_dstnc = '" + list[i].subway_dstnc.Trim() + "' ");
                sb.Append(" where seq_no = '" + list[i].seq_no.Trim() + "' ");
                insert(sb.ToString());
            }
        }

        private static double distance(double lat1, double lon1, double lat2, double lon2, String unit)
        {

            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));

            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;

            if (unit == "kilometer")
            {
                dist = dist * 1.609344;
            }
            else if (unit == "meter")
            {
                dist = dist * 1609.344;
            }

            return (dist);
        }

        // This function converts decimal degrees to radians
        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        // This function converts radians to decimal degrees
        private static double rad2deg(double rad)
        {
            return (rad * 180 / Math.PI);
        }

        public static DataSet selectDS(string query)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter();
                _SqlDataAdapter.SelectCommand = new SqlCommand(query, conn);
                _SqlDataAdapter.Fill(ds);

                return ds;
            }
        }

        public static void insert(string query)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
        }
    }

    internal class subway_addr
    {
        public string seq_no { get; set; }
        public string code1 { get; set; }
        public string code2 { get; set; }
        public string code3 { get; set; }
        public string name { get; set; }
        public string addr { get; set; }
        public string lng { get; set; }
        public string lat { get; set; }
    }

    internal class exam_addr
    {
        public string seq_no { get; set; }
        public string code1 { get; set; }
        public string code2 { get; set; }
        public string code3 { get; set; }
        public string name { get; set; }
        public string addr { get; set; }
        public string lng { get; set; }
        public string lat { get; set; }
        public string subway_code1 { get; set; }
        public string subway_code2 { get; set; }
        public string subway_code3 { get; set; }
        public string subway_name { get; set; }
        public string subway_dstnc { get; set; }
    }
}
