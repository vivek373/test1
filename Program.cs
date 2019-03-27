using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using WebSupergoo.ABCpdf8;
using System.Runtime.InteropServices;




namespace KFSLCRMLETTERPDF
{
    
    class Program
    {

        SqlConnection con = new SqlConnection(ConfigurationSettings.AppSettings["con"]);
        string letter = ConfigurationSettings.AppSettings["letters"];
        int pdfcount = 0;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);
       
        public static void Main(string[] args)
        {
            Program p = new Program();
            p.GetAllData();
        }
        
        public void GetAllData()
        {
            try
            {
                string pdfpath = Convert.ToString(ConfigurationSettings.AppSettings["pdfpath"]);
                DataTable dt, dt1, dt2,dispatch_data;
                //dt = ReadData("COL_LANWisePDFDownload");

                dt = ReadData("COL_MarginPdfDetails");
                dt1 = ReadData("COL_GETPDFBRANCHES");
                dt2 = ReadData("COL_MarginTypeLetters");
               

                CreateBranchDirectory(dt1, pdfpath);
                CreatePDF(dt, dt2);
                Logger.WriteLog(Convert.ToString(pdfcount) + " :PDFs Generated SuccessFully");
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public DataTable ReadData(string procedure)
        {

          
            SqlCommand cmd = new SqlCommand(procedure, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 360;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
       
        public void CreateBranchDirectory(DataTable dt, string path)
        {
            try
            {
                Logger.WriteLog("Function: CreateBranchDirectory:: Creating Directory");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    string path2 = path + dr.ItemArray[0];
                    bool flag = !Directory.Exists(path2);
                    if (flag)
                    {
                        Directory.CreateDirectory(path2);
                        string path3 = Convert.ToString(String.Concat(path2, "\\Downloadfiles"));
                        Directory.CreateDirectory(path3);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void CreatePDF(DataTable dt, DataTable dt2)
        {
            try
            {
                Logger.WriteLog("Function: CreatePDF:: Creating PDF.......");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    
                    #region "DL"
                    if (Convert.ToString(dr.ItemArray[12]) == Convert.ToString(dt2.Rows[1][0]))
                    {
                        bool flag;
                        string pdfpath = Convert.ToString(ConfigurationSettings.AppSettings["pdfpath"]);
                        Doc pdfDoc = new Doc();
                       
                        pdfDoc.Rect.Pin = XRect.Corner.TopLeft;
                        pdfDoc.FontSize = 9;
                        pdfDoc.Rect.Magnify(0.44, 1.0);
                        pdfDoc.Read(letter + dr.ItemArray[11] + "_30DPD.pdf");
                        pdfDoc.Font = pdfDoc.AddFont("HelSSvetica-Bold");
                        pdfDoc.TopDown = true;
                        if (Convert.ToString(dr.ItemArray[11]) == "Telugu")
                        {
                            DL_Telugu(pdfDoc, dr, dt2);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Marathi")
                        {
                            DL_Marathi(pdfDoc, dr, dt2);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Gujarati")
                        {
                            DL_Gujarati(pdfDoc, dr, dt2);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Hindi")
                        {
                            DL_Hindi(pdfDoc, dr, dt2);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Kannada")
                        {
                            DL_Kannada(pdfDoc, dr, dt2);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Tamil")
                        {
                            DL_Tamil(pdfDoc, dr, dt2);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Punjabi")
                        {
                            DL_Punjabi(pdfDoc, dr, dt2);
                        }

                        else if (Convert.ToString(dr.ItemArray[11]) == "Assami")
                        {
                            
                            DL_Assami(pdfDoc, dr, dt2);
                        }

                        else if (Convert.ToString(dr.ItemArray[11]) == "Bengali")
                        {

                            DL_Bengali(pdfDoc, dr, dt2);
                        }

                        //    flag = !Directory.Exists(string.Concat(new string[]
                        //{
                        //    pdfpath,"\\",Convert.ToString(dr.ItemArray[3]),"\\",DateTime.Now.ToString("dd-MM-yyyy")
                        //}));
                        //    if (flag)
                        //    {
                        //        Directory.CreateDirectory(string.Concat(new string[]
                        //    {
                        //        pdfpath,"\\",Convert.ToString(dr.ItemArray[3]),"\\",DateTime.Now.ToString("dd-MM-yyyy")
                        //    }));
                        //    }
                        flag = !Directory.Exists(string.Concat(new string[]
					{
						pdfpath,
                        //"\\",
                        //Convert.ToString(dr.ItemArray[3]),
                        "\\",
                        DateTime.Now.ToString("dd-MM-yyyy"),
                        "\\",
                        dt2.Rows[1][1].ToString()
					}));
					if (flag)
					{
						Directory.CreateDirectory(string.Concat(new string[]
						{
							pdfpath,
                            //"\\",
                            //Convert.ToString(dr.ItemArray[3]),
                            "\\",
                            DateTime.Now.ToString("dd-MM-yyyy"),
                            "\\",
                            dt2.Rows[1][1].ToString()
						}));
					}
					string str2 = string.Concat(new string[]
					{
						pdfpath,
                        //"\\",
                        //Convert.ToString(dr.ItemArray[3]),
                        "\\",
                        DateTime.Now.ToString("dd-MM-yyyy"),
                        "\\",
                        dt2.Rows[1][1].ToString()
					});
					string str3 = Convert.ToString(dr.ItemArray[14]);
					string text2 = str2 + "\\" + str3;
                    flag = !File.Exists(text2);
                    if (flag) 
                    {
                        pdfDoc.Save(text2);
                        pdfDoc.Clear();
                        pdfcount++;
                        CountGenPDF((int)dr.ItemArray[0], Convert.ToString(dr.ItemArray[6]), Convert.ToString(dr.ItemArray[3]), Convert.ToString(dr.ItemArray[12]), Convert.ToString(dr.ItemArray[5]), Convert.ToString(dr.ItemArray[14]), (int)(dr.ItemArray[9]), text2, "Collection", Convert.ToString(dr.ItemArray[1]), Convert.ToString(dr.ItemArray[2]), (int)(dr.ItemArray[13]), Convert.ToString(dr.ItemArray[19]),pdfcount);
                    }
					
                    }
                    #endregion

                    #region "LRAN"
                    else if (Convert.ToString(dr.ItemArray[12]) == Convert.ToString(dt2.Rows[2][0]))
                    {
                        bool flag;
                        string pdfpath = Convert.ToString(ConfigurationSettings.AppSettings["pdfpath"]);
                        Doc pdfDoc = new Doc();
                        pdfDoc.Rect.Pin = XRect.Corner.TopLeft;
                        pdfDoc.FontSize = 8;
                        pdfDoc.Rect.Magnify(0.44, 1.0);
                        pdfDoc.Read(letter + dr.ItemArray[11] + "_LRAN.pdf");
                        pdfDoc.Font = pdfDoc.AddFont("HelSSvetica-Bold");
                        pdfDoc.TopDown = true;
                        DataTable dispatch_data = ReadData_dispatch("pdf_dispatch", dt.Rows[i][5].ToString());

                        string dl_dispatch = dispatch_data.Rows[0][0].ToString();

                        if (Convert.ToString(dr.ItemArray[11]) == "Telugu")
                        {
                            LRAN_Telugu(pdfDoc, dr, dt2, dl_dispatch);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Marathi")
                        {
                            LRAN_Marathi(pdfDoc, dr, dt2, dl_dispatch);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Gujarati")
                        {
                            LRAN_Gujarati(pdfDoc, dr, dt2, dl_dispatch);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Hindi")
                        {
                            LRAN_Hindi(pdfDoc, dr, dt2, dl_dispatch);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Kannada")
                        {
                            LRAN_Kannada(pdfDoc, dr, dt2, dl_dispatch);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Tamil")
                        {
                            LRAN_Tamil(pdfDoc, dr, dt2, dl_dispatch);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Punjabi")
                        {
                            LRAN_Punjabi(pdfDoc, dr, dt2, dl_dispatch);
                        }
                        //add by vivek 4-dec-18
                        else if (Convert.ToString(dr.ItemArray[11]) == "Assami")
                        {
                            LRAN_Assami(pdfDoc, dr, dt2, dl_dispatch);
                        }

                        else if (Convert.ToString(dr.ItemArray[11]) == "Bengali")
                        {
                            LRAN_Bengali(pdfDoc, dr, dt2, dl_dispatch);
                        }

                        //end by vivek 4-dec-18

                        //flag = !Directory.Exists(string.Concat(new string[]
                        //{
                        //    pdfpath,
                        //    "\\",
                        //    Convert.ToString(dr.ItemArray[3]),
                        //    "\\",
                        //    DateTime.Now.ToString("dd-MM-yyyy")
                        //}));
                        //if (flag)
                        //{
                        //    Directory.CreateDirectory(string.Concat(new string[]
                        //    {
                        //        pdfpath,
                        //        "\\",
                        //        Convert.ToString(dr.ItemArray[3]),
                        //        "\\",
                        //        DateTime.Now.ToString("dd-MM-yyyy")
                        //    }));
                        //}
                        flag = !Directory.Exists(string.Concat(new string[]
						{
							pdfpath,
                            //"\\",
                            //Convert.ToString(dr.ItemArray[3]),
							"\\",
							DateTime.Now.ToString("dd-MM-yyyy"),
							"\\",
							dt2.Rows[2][1].ToString()
						}));
						if (flag)
						{
							Directory.CreateDirectory(string.Concat(new string[]
							{
								pdfpath,
                                //"\\",
                                //Convert.ToString(dr.ItemArray[3]),
								"\\",
								DateTime.Now.ToString("dd-MM-yyyy"),
								"\\",
								dt2.Rows[2][1].ToString()
							}));
						}
						string str4 = string.Concat(new string[]
						{
							pdfpath,
                            //"\\",
                            //Convert.ToString(dr.ItemArray[3]),
							"\\",
							DateTime.Now.ToString("dd-MM-yyyy"),
							"\\",
							dt2.Rows[2][1].ToString()
						});
						string str5 = Convert.ToString(dr.ItemArray[14]);
						string text3 = str4 + "\\" + str5;
                        flag = !File.Exists(text3);
                        if (flag) 
                        {
                            pdfDoc.Save(text3);
                            pdfDoc.Clear();
                            pdfcount++;
                            CountGenPDF((int)dr.ItemArray[0], Convert.ToString(dr.ItemArray[6]), Convert.ToString(dr.ItemArray[3]), Convert.ToString(dr.ItemArray[12]), Convert.ToString(dr.ItemArray[5]), Convert.ToString(dr.ItemArray[14]), (int)(dr.ItemArray[9]), text3, "Collection", Convert.ToString(dr.ItemArray[1]), Convert.ToString(dr.ItemArray[2]), (int)(dr.ItemArray[13]), Convert.ToString(dr.ItemArray[19]),pdfcount);
                        }
						
                    }
                    #endregion

                   

                    #region "MCAN"
                    else if (Convert.ToString(dr.ItemArray[12]) == Convert.ToString(dt2.Rows[3][0]))
                    {
                        bool flag;
                        string pdfpath = Convert.ToString(ConfigurationSettings.AppSettings["pdfpath"]);
                        Doc pdfDoc = new Doc();
                        pdfDoc.Rect.Pin = XRect.Corner.TopLeft;
                        pdfDoc.FontSize = 7;
                        pdfDoc.Rect.Magnify(0.44, 1.0);
                        pdfDoc.Read(letter + dr.ItemArray[11] + "_MCAN.pdf");
                        pdfDoc.Font = pdfDoc.AddFont("HelSSvetica-Bold");
                        pdfDoc.TopDown = true;

                        if (Convert.ToString(dr.ItemArray[11]) == "Telugu")
                        {
                            MCAN_Telugu(pdfDoc, dr, dt2);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Marathi")
                        {
                             MCAN_Marathi(pdfDoc, dr, dt2);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Gujarati")
                        {
                              MCAN_Gujarati(pdfDoc, dr, dt2);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Hindi")
                        {
                             MCAN_Hindi(pdfDoc, dr, dt2);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Kannada")
                        {
                            MCAN_Kannada(pdfDoc, dr, dt2);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Tamil")
                        {
                            MCAN_Tamil(pdfDoc, dr, dt2);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Punjabi")
                        {
                            MCAN_Punjabi(pdfDoc, dr, dt2);
                        }

                        else if (Convert.ToString(dr.ItemArray[11]) == "Assami")
                        {
                            MCAN_Assami(pdfDoc, dr, dt2);
                        }



                        else if (Convert.ToString(dr.ItemArray[11]) == "Bengali")
                        {
                            MCAN_Bengali(pdfDoc, dr, dt2);
                        }

                        //flag = !Directory.Exists(string.Concat(new string[]
                        //    {
                        //        pdfpath,
                        //        "\\",
                        //        Convert.ToString(dr.ItemArray[3]),
                        //        "\\",
                        //        DateTime.Now.ToString("dd-MM-yyyy")
                        //    }));
                        //    if (flag)
                        //    {
                        //        Directory.CreateDirectory(string.Concat(new string[]
                        //        {
                        //            pdfpath,
                        //            "\\",
                        //            Convert.ToString(dr.ItemArray[3]),
                        //            "\\",
                        //            DateTime.Now.ToString("dd-MM-yyyy")
                        //        }));
                        //    }
                        flag = !Directory.Exists(string.Concat(new string[]
							{
								pdfpath,
                                //"\\",
                                //Convert.ToString(dr.ItemArray[3]),
								"\\",
								DateTime.Now.ToString("dd-MM-yyyy"),
								"\\",
								dt2.Rows[3][1].ToString()
							}));
							if (flag)
							{
								Directory.CreateDirectory(string.Concat(new string[]
								{
									pdfpath,
                                    //"\\",
                                    //Convert.ToString(dr.ItemArray[3]),
									"\\",
									DateTime.Now.ToString("dd-MM-yyyy"),
									"\\",
									dt2.Rows[3][1].ToString()
								}));
							}
							string str6 = string.Concat(new string[]
							{
								pdfpath,
                                //"\\",
                                //Convert.ToString(dr.ItemArray[3]),
								"\\",
								DateTime.Now.ToString("dd-MM-yyyy"),
								"\\",
								dt2.Rows[3][1].ToString()
							});
							string str7 = Convert.ToString(dr.ItemArray[14]);
							string text4 = str6 + "\\" + str7;
                            flag = !File.Exists(text4);
                            if (flag)
                            {
                                pdfDoc.Save(text4);
                                pdfDoc.Clear();
                                pdfcount++;
                                CountGenPDF((int)dr.ItemArray[0], Convert.ToString(dr.ItemArray[6]), Convert.ToString(dr.ItemArray[3]), Convert.ToString(dr.ItemArray[12]), Convert.ToString(dr.ItemArray[5]), Convert.ToString(dr.ItemArray[14]), (int)(dr.ItemArray[9]), text4, "Collection", Convert.ToString(dr.ItemArray[1]), Convert.ToString(dr.ItemArray[2]), (int)(dr.ItemArray[13]), Convert.ToString(dr.ItemArray[19]),pdfcount);
                            }
                    }
                    #endregion

                    #region "Auction"

                    else if (Convert.ToString(dr.ItemArray[12]) == Convert.ToString(dt2.Rows[0][0]))
                    {
                        DataTable  dispatch_data = ReadData_dispatch("pdf_dispatch",dt.Rows[i][5].ToString());
                        bool flag;
                        string pdfpath = Convert.ToString(ConfigurationSettings.AppSettings["pdfpath"]);
                        Doc pdfDoc = new Doc();
                        pdfDoc.Rect.Pin = XRect.Corner.TopLeft;
                        pdfDoc.FontSize = 8;
                        pdfDoc.Rect.Magnify(0.44, 1.0);
                        pdfDoc.Read(letter + dr.ItemArray[11] + "_AUCTION.pdf");
                        pdfDoc.Font = pdfDoc.AddFont("HelSSvetica-Bold");
                        pdfDoc.TopDown = true;
                        string dl_dispatch=dispatch_data.Rows[0][0].ToString();
                        string lran_dispatch=dispatch_data.Rows[0][1].ToString();

                        if (Convert.ToString(dr.ItemArray[11]) == "Telugu")
                        {
                            AUCTION_Telugu(pdfDoc, dr, dt2,dl_dispatch, lran_dispatch);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Marathi")
                        {
                            AUCTION_Marathi(pdfDoc, dr, dt2, dl_dispatch, lran_dispatch);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Gujarati")
                        {
                            AUCTION_Gujarati(pdfDoc, dr, dt2, dl_dispatch, lran_dispatch);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Hindi")
                        {
                            AUCTION_Hindi(pdfDoc, dr, dt2, dl_dispatch, lran_dispatch);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Kannada")
                        {
                            AUCTION_Kannada(pdfDoc, dr, dt2, dl_dispatch, lran_dispatch);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Tamil")
                        {
                            AUCTION_Tamil(pdfDoc, dr, dt2, dl_dispatch, lran_dispatch);
                        }
                        else if (Convert.ToString(dr.ItemArray[11]) == "Punjabi")
                        {
                            AUCTION_Punjabi(pdfDoc, dr, dt2, dl_dispatch, lran_dispatch);
                        }

                        else if (Convert.ToString(dr.ItemArray[11]) == "Assami")
                        {
                            AUCTION_Assami(pdfDoc, dr, dt2, dl_dispatch, lran_dispatch);
                            
                        }

                        else if (Convert.ToString(dr.ItemArray[11]) == "Bengali")
                        {
                            AUCTION_Bengali(pdfDoc, dr, dt2, dl_dispatch, lran_dispatch);

                        }

                        //flag = !Directory.Exists(string.Concat(new string[]
                        //    {
                        //        pdfpath,
                        //        "\\",
                        //        Convert.ToString(dr.ItemArray[3]),
                        //        "\\",
                        //        DateTime.Now.ToString("dd-MM-yyyy")
                        //    }));
                        //if (flag)
                        //{
                        //    Directory.CreateDirectory(string.Concat(new string[]
                        //        {
                        //            pdfpath,
                        //            "\\",
                        //            Convert.ToString(dr.ItemArray[3]),
                        //            "\\",
                        //            DateTime.Now.ToString("dd-MM-yyyy")
                        //        }));
                        //}
                        flag = !Directory.Exists(string.Concat(new string[]
							{
								pdfpath,
                                //"\\",
                                //Convert.ToString(dr.ItemArray[3]),
								"\\",
								DateTime.Now.ToString("dd-MM-yyyy"),
								"\\",
								dt2.Rows[0][1].ToString()
							}));
                        if (flag)
                        {
                            Directory.CreateDirectory(string.Concat(new string[]
								{
									pdfpath,
                                    //"\\",
                                    //Convert.ToString(dr.ItemArray[3]),
									"\\",
									DateTime.Now.ToString("dd-MM-yyyy"),
									"\\",
									dt2.Rows[0][1].ToString()
								}));
                        }
                        string str6 = string.Concat(new string[]
							{
								pdfpath,
                                //"\\",
                                //Convert.ToString(dr.ItemArray[3]),
								"\\",
								DateTime.Now.ToString("dd-MM-yyyy"),
								"\\",
								dt2.Rows[0][1].ToString()
							});
                        string str7 = Convert.ToString(dr.ItemArray[14]);
                        string text4 = str6 + "\\" + str7;
                        flag = !File.Exists(text4);
                        if (flag)
                        {
                            pdfDoc.Save(text4);
                            pdfDoc.Clear();
                            pdfcount++;
                            CountGenPDF((int)dr.ItemArray[0], Convert.ToString(dr.ItemArray[6]), Convert.ToString(dr.ItemArray[3]), Convert.ToString(dr.ItemArray[12]), Convert.ToString(dr.ItemArray[5]), Convert.ToString(dr.ItemArray[14]), (int)(dr.ItemArray[9]), text4, "Collection", Convert.ToString(dr.ItemArray[1]), Convert.ToString(dr.ItemArray[2]), (int)(dr.ItemArray[13]), Convert.ToString(dr.ItemArray[19]), pdfcount);
                        }


                    }
                    #endregion

                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public DataTable ReadData_dispatch(string procedure,string LAN)
        {
            SqlCommand cmd = new SqlCommand(procedure, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@P_LAN",LAN);
            cmd.CommandTimeout = 360;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public void DL_Telugu(Doc pdfDoc, DataRow dr, DataTable dt)
        {
            try
            {
                        pdfDoc.Rect.Inset(100.0, 193.0);
						pdfDoc.Rect.Position(167.0, 80.0);
                        pdfDoc.Rect.Width = 200.0;
                        pdfDoc.Rect.Height = 10.0;
						pdfDoc.AddText(Convert.ToString(dr.ItemArray[3]));
						pdfDoc.Rect.Inset(100.0, 160.0);
						pdfDoc.Rect.Position(167.0, 95.0);
                        pdfDoc.Rect.Width = 80.0;
                        pdfDoc.Rect.Height = 10.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
						pdfDoc.Rect.Inset(100.0, 200.0);
                        pdfDoc.Rect.Position(421.0, 159.0);
                        pdfDoc.Rect.Width = 80.0;
                        pdfDoc.Rect.Height = 10.0;
						pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
						pdfDoc.Rect.Inset(100.0, 200.0);
                        pdfDoc.Rect.Position(473.0, 186.0);
                        pdfDoc.Rect.Width = 80.0;
                        pdfDoc.Rect.Height = 10.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
						pdfDoc.Rect.Inset(100.0, 200.0);
						pdfDoc.Rect.Position(136.0, 159.0);
						pdfDoc.Rect.Width = 170.0;
                        pdfDoc.Rect.Height = 10.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[6]));
						pdfDoc.Rect.Inset(100.0, 200.0);
                        pdfDoc.Rect.Position(448.0, 225.0);
                        pdfDoc.Rect.Width = 80.0;
                        pdfDoc.Rect.Height = 10.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
						pdfDoc.Rect.Inset(100.0, 200.0);
						pdfDoc.Rect.Position(71.0, 188.0);
                        pdfDoc.Rect.Width = 303.0;
                        pdfDoc.Rect.Height = 63.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));

                        //test code by vivek for mobile no

                        pdfDoc.Rect.Inset(100.0, 200.0);
                        pdfDoc.Rect.Position(180.0, 176.0);
                        pdfDoc.Rect.Width = 260.0;
                        pdfDoc.Rect.Height = 63.0;
                        pdfDoc.AddText( Convert.ToString(dr.ItemArray[8]));

                        //end test code by vivek 
						pdfDoc.Rect.Inset(400.0, 200.0);
						pdfDoc.Rect.Position(375.0, 344.0);
                        pdfDoc.Rect.Width = 80.0;
                        pdfDoc.Rect.Height = 10.0;
						pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
						pdfDoc.Rect.Inset(100.0, 200.0);
						pdfDoc.Rect.Position(110.0, 362.0);
                        pdfDoc.Rect.Width = 80.0;
                        pdfDoc.Rect.Height = 10.0;
						pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
						pdfDoc.Rect.Inset(100.0, 200.0);
                        pdfDoc.Rect.Position(207.0, 449.0);
                        pdfDoc.Rect.Width = 80.0;
                        pdfDoc.Rect.Height = 10.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
						pdfDoc.Rect.Inset(100.0, 200.0);
                        pdfDoc.Rect.Position(355.0, 449.0);
                        pdfDoc.Rect.Width = 80.0;
                        pdfDoc.Rect.Height = 10.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
						pdfDoc.Rect.Inset(100.0, 200.0);
                        pdfDoc.Rect.Position(136.0, 524.0);
                        pdfDoc.Rect.Width = 80.0;
                        pdfDoc.Rect.Height = 10.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
						pdfDoc.Rect.Inset(100.0, 200.0);
                        pdfDoc.Rect.Position(225.0, 524.0);
                        pdfDoc.Rect.Width = 80.0;
                        pdfDoc.Rect.Height = 10.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
						pdfDoc.Rect.Inset(50.0, 75.0);
						pdfDoc.Rect.Position(506.0, 627.0);
                        pdfDoc.Rect.Width = 200.0;
                        pdfDoc.Rect.Height = 10.0;
						pdfDoc.AddText(Convert.ToString(dr.ItemArray[0]));
            }
            catch(Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void DL_Marathi(Doc pdfDoc, DataRow dr, DataTable dt)
        {
            try
            {
                           

                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(167.0, 80.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(167.0, 95.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(421.0, 159.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(473.0, 186.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(136.0, 159.0);
                pdfDoc.Rect.Width = 170.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[6]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(448.0, 225.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(71.0, 188.0);
                pdfDoc.Rect.Width = 303.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));


                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 176.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.Rect.Inset(400.0, 200.0);
                pdfDoc.Rect.Position(375.0, 344.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(110.0, 361.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(207.0, 447.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(355.0, 447.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(162.0, 519.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(370.0, 519.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(50.0, 75.0);
                pdfDoc.Rect.Position(506.0, 648.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[0]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void DL_Gujarati(Doc pdfDoc, DataRow dr, DataTable dt)
        {
            try
            {
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(167.0, 80.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(167.0, 95.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(421.0, 159.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(473.0, 186.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(136.0, 159.0);
                pdfDoc.Rect.Width = 170.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[6]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(448.0, 225.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(71.0, 188.0);
                pdfDoc.Rect.Width = 303.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));

                //test code by vivek for mobile no


                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 176.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.Rect.Inset(400.0, 200.0);
                pdfDoc.Rect.Position(375.0, 344.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(110.0, 361.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(207.0, 447.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(355.0, 447.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(145.0, 522.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(207.0, 522.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(50.0, 75.0);
                pdfDoc.Rect.Position(506.0, 643.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[0]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void DL_Hindi(Doc pdfDoc, DataRow dr, DataTable dt){
            try
                    {
                        pdfDoc.Rect.Inset(100.0, 193.0);
                        pdfDoc.Rect.Position(167.0, 80.0);
                        pdfDoc.Rect.Width = 200.0;
                        pdfDoc.Rect.Height = 10.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[3]));
                        pdfDoc.Rect.Inset(100.0, 160.0);
                        pdfDoc.Rect.Position(167.0, 95.0);
                        pdfDoc.Rect.Width = 80.0;
                        pdfDoc.Rect.Height = 10.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
                        pdfDoc.Rect.Inset(100.0, 200.0);
                        pdfDoc.Rect.Position(421.0, 159.0);
                        pdfDoc.Rect.Width = 80.0;
                        pdfDoc.Rect.Height = 10.0;
                        pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                        pdfDoc.Rect.Inset(100.0, 200.0);
                        pdfDoc.Rect.Position(473.0, 186.0);
                        pdfDoc.Rect.Width = 80.0;
                        pdfDoc.Rect.Height = 10.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                        pdfDoc.Rect.Inset(100.0, 200.0);
                        pdfDoc.Rect.Position(136.0, 159.0);
                        pdfDoc.Rect.Width = 170.0;
                        pdfDoc.Rect.Height = 10.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[6]));
                        pdfDoc.Rect.Inset(100.0, 200.0);
                        pdfDoc.Rect.Position(448.0, 225.0);
                        pdfDoc.Rect.Width = 80.0;
                        pdfDoc.Rect.Height = 10.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                        pdfDoc.Rect.Inset(100.0, 200.0);
                        pdfDoc.Rect.Position(71.0, 188.0);
                        pdfDoc.Rect.Width = 303.0;
                        pdfDoc.Rect.Height = 63.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));

                        //test code by vivek for mobile no

                        pdfDoc.Rect.Inset(100.0, 200.0);
                        pdfDoc.Rect.Position(180.0, 176.0);
                        pdfDoc.Rect.Width = 260.0;
                        pdfDoc.Rect.Height = 63.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                        //end test code by vivek 
                        pdfDoc.Rect.Inset(400.0, 200.0);
                        pdfDoc.Rect.Position(375.0, 341.0);
                        pdfDoc.Rect.Width = 80.0;
                        pdfDoc.Rect.Height = 10.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                        pdfDoc.Rect.Inset(100.0, 200.0);
                        pdfDoc.Rect.Position(110.0, 358.0);
                        pdfDoc.Rect.Width = 80.0;
                        pdfDoc.Rect.Height = 10.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                        pdfDoc.Rect.Inset(100.0, 200.0);
                        pdfDoc.Rect.Position(207.0, 446.0);
                        pdfDoc.Rect.Width = 80.0;
                        pdfDoc.Rect.Height = 10.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                        pdfDoc.Rect.Inset(100.0, 200.0);
                        pdfDoc.Rect.Position(355.0, 445.0);
                        pdfDoc.Rect.Width = 80.0;
                        pdfDoc.Rect.Height = 10.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                       
                        pdfDoc.Rect.Inset(50.0, 75.0);
                        pdfDoc.Rect.Position(506.0, 611.0);
                        pdfDoc.Rect.Width = 200.0;
                        pdfDoc.Rect.Height = 10.0;
                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[0]));               
                
               

            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void DL_Kannada(Doc pdfDoc, DataRow dr, DataTable dt){
            try{

                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(167.0, 80.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(167.0, 95.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(421.0, 159.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(473.0, 186.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(136.0, 159.0);
                pdfDoc.Rect.Width = 170.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[6]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(448.0, 225.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(71.0, 188.0);
                pdfDoc.Rect.Width = 303.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]) );

                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 176.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.Rect.Inset(400.0, 200.0);
                pdfDoc.Rect.Position(375.0, 344.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(110.0, 361.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(207.0, 447.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(355.0, 447.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(185.0, 520.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(290.0, 520.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(50.0, 75.0);
                pdfDoc.Rect.Position(506.0, 667.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[0]));
                
               
            }
            catch (Exception ex)
                 {
                Logger.WriteLog(Convert.ToString(ex.Message));
                }
        }

        public void DL_Tamil(Doc pdfDoc, DataRow dr, DataTable dt){
            try
            {
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(167.0, 80.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(167.0, 95.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(421.0, 159.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(473.0, 186.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(136.0, 159.0);
                pdfDoc.Rect.Width = 170.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[6]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(448.0, 225.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(71.0, 188.0);
                pdfDoc.Rect.Width = 303.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));

                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 176.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 

                pdfDoc.Rect.Inset(400.0, 200.0);
                pdfDoc.Rect.Position(375.0, 344.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(110.0, 361.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(207.0, 447.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(355.0, 447.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(146.0, 519.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(343.0, 518.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(50.0, 75.0);
                pdfDoc.Rect.Position(506.0, 661.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[0]));


               
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void DL_Punjabi(Doc pdfDoc, DataRow dr, DataTable dt){
            try{

                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(167.0, 80.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(167.0, 95.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(421.0, 159.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(473.0, 186.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(136.0, 159.0);
                pdfDoc.Rect.Width = 170.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[6]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(448.0, 225.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(71.0, 188.0);
                pdfDoc.Rect.Width = 303.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));
                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 176.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.Rect.Inset(400.0, 200.0);
                pdfDoc.Rect.Position(375.0, 344.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(110.0, 361.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(207.0, 447.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(355.0, 447.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(252.0, 519.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(363.0, 519.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(50.0, 75.0);
                pdfDoc.Rect.Position(506.0, 648.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[0]));


               
            }
            catch(Exception ex){
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }
        public void DL_Assami(Doc pdfDoc, DataRow dr, DataTable dt)
        {
            try
            {

                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(167.0, 83.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(167.0, 95.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(380.0, 124.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd MMM yyyy"));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(433.0, 145.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(136.0, 125.0);
                //159.0
                pdfDoc.Rect.Width = 170.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[6]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(401.0, 168.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 150.0);
                pdfDoc.Rect.Position(71.0, 154.0);
                pdfDoc.Rect.Width = 250.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));
                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 142.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.FontSize = 8;
                pdfDoc.Rect.Inset(400.0, 200.0);
                pdfDoc.Rect.Position(270.0, 287.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(365.0, 287.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(165.0, 337.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(445.0, 337.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));


                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(252.0, 519.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(363.0, 519.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                //pdfDoc.Rect.Inset(50.0, 75.0);
                //pdfDoc.Rect.Position(506.0, 648.0);
                //pdfDoc.Rect.Width = 200.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(Convert.ToString(dr.ItemArray[0]));



            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }
        public void DL_Bengali(Doc pdfDoc, DataRow dr, DataTable dt)
        {
            try
            {

                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(167.0, 83.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(167.0, 95.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(380.0, 124.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd MMM yyyy"));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(433.0, 145.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(136.0, 125.0);
                //159.0
                pdfDoc.Rect.Width = 170.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[6]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(401.0, 168.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 150.0);
                pdfDoc.Rect.Position(71.0, 154.0);
                pdfDoc.Rect.Width = 250.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));
                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 142.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.FontSize = 7;
                pdfDoc.Rect.Inset(400.0, 200.0);
                pdfDoc.Rect.Position(275.0, 287.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(365.0, 287.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(275.0, 337.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(495.0, 337.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));


                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(252.0, 519.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(363.0, 519.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                //pdfDoc.Rect.Inset(50.0, 75.0);
                //pdfDoc.Rect.Position(506.0, 648.0);
                //pdfDoc.Rect.Width = 200.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(Convert.ToString(dr.ItemArray[0]));



            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void LRAN_Telugu(Doc pdfDoc, DataRow dr, DataTable dt, string dl_dispatch)
        {
            try
            {
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 74.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(354.0, 74.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(407.0, 95.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(70.0, 105.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));
                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 94.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 115.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 127.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[15]));
               				
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(302.0, 199.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(429.0, 198.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(226.0, 209.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dl_dispatch));
               
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(193.0, 371.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(318.0, 371.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(239.0, 381.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dl_dispatch));
                
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(144.0, 537.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(230.0, 537.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(170.0, 551.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dl_dispatch));
               
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(147, 728.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(365.0, 728.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }

            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void LRAN_Marathi(Doc pdfDoc, DataRow dr, DataTable dt, string dl_dispatch)
        {
            try
            {
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 78.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(354.0, 78.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(407.0, 99.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(69.0, 109.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));
                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 98.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 117.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 131.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[15]));
               				
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(287.0, 198.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(422.0, 198.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(195.0, 209.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dl_dispatch));
           
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(192.0, 362.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(318.0, 362.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(222.0, 374.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dl_dispatch));
               
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(145.0, 511.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(340.0, 511.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(90.0, 533.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dl_dispatch));
               
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(147, 756.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(365.0, 755.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void LRAN_Gujarati(Doc pdfDoc, DataRow dr, DataTable dt, string dl_dispatch)
        {
            try
            {
                pdfDoc.Rect.Inset(100.0, 193.0);
									pdfDoc.Rect.Position(133.0, 70.0);
									pdfDoc.Rect.Width= 200.0;
									pdfDoc.Rect.Height = 10.0;
									pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
									pdfDoc.Rect.Inset(100.0, 200.0);
									pdfDoc.Rect.Position(354.0, 70.0);
									pdfDoc.Rect.Width = 80.0;
									pdfDoc.Rect.Height = 10.0;
									pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
									pdfDoc.Rect.Inset(100.0, 160.0);
									pdfDoc.Rect.Position(407.0, 91.0);
									pdfDoc.Rect.Width = 80.0;
									pdfDoc.Rect.Height = 10.0;
									pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
									pdfDoc.Rect.Inset(100.0, 200.0);
									pdfDoc.Rect.Position(66.0, 101.0);
									pdfDoc.Rect.Width = 260.0;
                                    pdfDoc.Rect.Height = 63.0;
									pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]) );
                          

                                    //test code by vivek for mobile no

                                    pdfDoc.Rect.Inset(100.0, 200.0);
                                    pdfDoc.Rect.Position(180.0, 90.0);
                                    pdfDoc.Rect.Width = 260.0;
                                    pdfDoc.Rect.Height = 63.0;
                                    pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                                    //end test code by vivek 

                                    //end test code by vivek 
									pdfDoc.Rect.Inset(100.0, 200.0);
									pdfDoc.Rect.Position(413.0, 109.0);
									pdfDoc.Rect.Width = 80.0;
									pdfDoc.Rect.Height = 10.0;
									pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
									pdfDoc.Rect.Inset(100.0, 200.0);
									pdfDoc.Rect.Position(413.0, 124.0);
									pdfDoc.Rect.Width = 80.0;
									pdfDoc.Rect.Height = 10.0;
									pdfDoc.AddText(Convert.ToString(dr.ItemArray[15]));
                                   					
                                    pdfDoc.Rect.Inset(100.0, 200.0);
                                    pdfDoc.Rect.Position(302.0, 184.0);
                                    pdfDoc.Rect.Width = 80.0;
                                    pdfDoc.Rect.Height = 10.0;
                                    pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                                    pdfDoc.Rect.Inset(100.0, 200.0);
                                    pdfDoc.Rect.Position(443.0, 183.0);
                                    pdfDoc.Rect.Width = 80.0;
                                    pdfDoc.Rect.Height = 10.0;
                                    pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                                    pdfDoc.Rect.Inset(100.0, 200.0);
                                    pdfDoc.Rect.Position(235.0, 196.0);
                                    pdfDoc.Rect.Width = 80.0;
                                    pdfDoc.Rect.Height = 10.0;
                                    pdfDoc.AddText(Convert.ToString(dl_dispatch));
                                  
                                    pdfDoc.Rect.Inset(100.0, 200.0);
                                    pdfDoc.Rect.Position(182.0, 378.0);
                                    pdfDoc.Rect.Width = 80.0;
                                    pdfDoc.Rect.Height = 10.0;
                                    pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
								
									pdfDoc.Rect.Inset(100.0, 200.0);
                                    pdfDoc.Rect.Position(273.0, 377.0);
									pdfDoc.Rect.Width = 80.0;
									pdfDoc.Rect.Height = 10.0;
									pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
				                    pdfDoc.Rect.Inset(100.0, 200.0);
                                    pdfDoc.Rect.Position(172.0, 388.0);
                                    pdfDoc.Rect.Width = 80.0;
                                    pdfDoc.Rect.Height = 10.0;
                                    pdfDoc.AddText(Convert.ToString(dl_dispatch));					
                                   
                                    pdfDoc.Rect.Inset(100.0, 200.0);
									pdfDoc.Rect.Position(140.0, 533.0);
									pdfDoc.Rect.Width = 80.0;
									pdfDoc.Rect.Height = 10.0;
									pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                                    pdfDoc.Rect.Inset(100.0, 200.0);
                                    pdfDoc.Rect.Position(200.0, 533.0);
                                    pdfDoc.Rect.Width = 80.0;
                                    pdfDoc.Rect.Height = 10.0;
                                    pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                                    pdfDoc.Rect.Inset(100.0, 200.0);
                                    pdfDoc.Rect.Position(475.0, 533.0);
                                    pdfDoc.Rect.Width = 80.0;
                                    pdfDoc.Rect.Height = 10.0;
                                    pdfDoc.AddText(Convert.ToString(dl_dispatch));
                                   
                                    pdfDoc.Rect.Inset(100.0, 193.0);
                                    pdfDoc.Rect.Position(147, 742);
                                    pdfDoc.Rect.Width = 200.0;
                                    pdfDoc.Rect.Height = 10.0;
                                    pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                                    pdfDoc.Rect.Inset(100.0, 193.0);
                                    pdfDoc.Rect.Position(368.0, 742);
                                    pdfDoc.Rect.Width = 200.0;
                                    pdfDoc.Rect.Height = 10.0;
                                    pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void LRAN_Hindi(Doc pdfDoc, DataRow dr, DataTable dt, string dl_dispatch)
        {
            try
            {
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 72.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(354.0, 72.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(407.0, 93.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(66.0, 103.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));
                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 92.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
										pdfDoc.Rect.Inset(100.0, 200.0);
										pdfDoc.Rect.Position(413.0, 112.0);
										pdfDoc.Rect.Width = 80.0;
										pdfDoc.Rect.Height = 10.0;
										pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
										pdfDoc.Rect.Inset(100.0, 200.0);
										pdfDoc.Rect.Position(413.0, 125.0);
										pdfDoc.Rect.Width = 80.0;
										pdfDoc.Rect.Height = 10.0;
										pdfDoc.AddText(Convert.ToString(dr.ItemArray[15]));
                                        pdfDoc.Rect.Inset(100.0, 160.0);
                                        pdfDoc.Rect.Position(292.0, 228.0);
										pdfDoc.Rect.Width = 80.0;
										pdfDoc.Rect.Height = 10.0;
										pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
										pdfDoc.Rect.Inset(100.0, 200.0);
                                        pdfDoc.Rect.Position(415.0, 228.0);
										pdfDoc.Rect.Width = 80.0;
										pdfDoc.Rect.Height = 10.0;
										pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
										pdfDoc.Rect.Inset(100.0, 200.0);
                                        pdfDoc.Rect.Position(196.0, 239.0);
										pdfDoc.Rect.Width= 80.0;
										pdfDoc.Rect.Height = 10.0;
                                        pdfDoc.AddText(Convert.ToString(dl_dispatch));
                                      
                                     
                //start test code
                                        pdfDoc.Rect.Inset(100.0, 200.0);
                                        pdfDoc.Rect.Position(200.0, 478.0);
                                        pdfDoc.Rect.Width = 80.0;
                                        pdfDoc.Rect.Height = 10.0;
                                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                //end test code
										pdfDoc.Rect.Inset(100.0, 200.0);
                                        pdfDoc.Rect.Position(317.0, 478.0);
										pdfDoc.Rect.Width = 80.0;
										pdfDoc.Rect.Height = 10.0;
										pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                                        pdfDoc.Rect.Inset(100.0, 200.0);
                                        pdfDoc.Rect.Position(230.0, 489);
                                        pdfDoc.Rect.Width = 80.0;
                                        pdfDoc.Rect.Height = 10.0;
                                        pdfDoc.AddText(Convert.ToString(dl_dispatch));
                                       

                                        pdfDoc.Rect.Inset(100.0, 193.0);
                                        pdfDoc.Rect.Position(140, 763.0);
										pdfDoc.Rect.Width= 200.0;
										pdfDoc.Rect.Height = 10.0;
										pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
										pdfDoc.Rect.Inset(100.0, 193.0);
                                        pdfDoc.Rect.Position(365.0, 763.0);
										pdfDoc.Rect.Width= 200.0;
										pdfDoc.Rect.Height = 10.0;
										pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void LRAN_Kannada(Doc pdfDoc, DataRow dr, DataTable dt, string dl_dispatch)
        {
            try
            {
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 76.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(354.0, 76.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(407.0, 96.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(69.0, 105.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]) );
                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 95.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 115.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 129.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[15]));
              				
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(302.0, 201.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(436.0, 201.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(229.0, 215.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dl_dispatch));
                
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(194.0, 394.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(321.0, 394.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(232.0, 404.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dl_dispatch));
               
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(173.0, 554.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(285.0, 554.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(270.0, 565.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dl_dispatch));
          
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(147, 758.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(365.0, 758.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));


            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void LRAN_Tamil(Doc pdfDoc, DataRow dr, DataTable dt, string dl_dispatch)
        {
            try
            {
               

                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 72.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(354.0, 73.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(407.0, 86.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(68.0, 95.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]) );
                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 85.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 105.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 117.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[15]));
               				
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(290.0, 179.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(418.0, 178.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(192.0, 189.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dl_dispatch));
               
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(183.0, 341.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(307.0, 341.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(183.0, 350.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dl_dispatch));
              
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(160.0, 478.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(380.0, 477.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(418.0, 490.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dl_dispatch));
                
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(147, 734.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(350.0, 733.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void LRAN_Punjabi(Doc pdfDoc, DataRow dr, DataTable dt, string dl_dispatch)
        {
            try
            {
               

                 pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 78.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(354.0, 78.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(407.0, 98.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(67.0, 108.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]) );
                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 97.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 117.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 131.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[15]));
            					
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(305.0, 197.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(434.0, 197.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(229.0, 210.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dl_dispatch));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(194.0, 391.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(312.0, 391.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(219.0, 403.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dl_dispatch));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(262.0, 542.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(360.0, 542.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(178.0, 554.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dl_dispatch));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(147, 747.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(365.0, 747.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void LRAN_Assami(Doc pdfDoc, DataRow dr, DataTable dt, string dl_dispatch)
        {
            try
            {


                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 76.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(390.0, 76.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(427.0, 97.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(67.0, 108.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));
                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 97.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(433.0, 120.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(433.0, 133.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[15]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(250.0, 223.0);
                //
                pdfDoc.FontSize = 8;
                //
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(335.0, 223.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(65.0, 235.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dl_dispatch));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(155.0, 388.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(418.0, 388.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(160.0, 401.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dl_dispatch));


                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(160.0, 539.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(433.0, 539.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(178.0, 554.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(Convert.ToString(dl_dispatch));



                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(147, 647.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(365.0, 647.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));

            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }



        public void LRAN_Bengali(Doc pdfDoc, DataRow dr, DataTable dt, string dl_dispatch)
        {
            try
            {


                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 76.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(390.0, 76.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(427.0, 97.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(67.0, 108.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));
                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 97.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(433.0, 120.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(433.0, 133.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[15]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(250.0, 225.0);
                //
                pdfDoc.FontSize = 8;
                //
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(339.0, 225.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(65.0, 237.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dl_dispatch));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(275.0, 392.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(465.0, 392.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(91.0, 405.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dl_dispatch));


                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(160.0, 539.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(433.0, 539.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(178.0, 554.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(Convert.ToString(dl_dispatch));



                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(147, 659.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(365.0, 659.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));

            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void MCAN_Telugu(Doc pdfDoc, DataRow dr, DataTable dt)
        {
            try
            {
                          

                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 76.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(354.0, 76.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(407.0, 95.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(65.0, 106.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));
                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 94.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(335.0, 188.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(410.0, 188.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(195.0, 308.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(236.0, 373.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(313.0, 373.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(133.0, 514.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(205.0, 514.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(145.0, 764.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(368.0, 764.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void MCAN_Marathi(Doc pdfDoc, DataRow dr, DataTable dt)
        {
            try
            {
              
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 77.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(354.0, 77.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(407.0, 94.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(68.0, 104.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));
                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 94.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(365.0, 192.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(450.0, 192.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(195.0, 314.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(236.0, 380.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(313.0, 379.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(171.0, 519.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(268.0, 519.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(145.0, 755.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(368.0, 755.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void MCAN_Gujarati(Doc pdfDoc, DataRow dr, DataTable dt)
        {
            try
            {
                pdfDoc.Rect.Inset(100.0, 193.0);
										pdfDoc.Rect.Position(133.0, 69.0);
										pdfDoc.Rect.Width= 200.0;
										pdfDoc.Rect.Height = 10.0;
										pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
										pdfDoc.Rect.Inset(100.0, 200.0);
										pdfDoc.Rect.Position(354.0, 69.0);
										pdfDoc.Rect.Width = 80.0;
										pdfDoc.Rect.Height = 10.0;
										pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
										pdfDoc.Rect.Inset(100.0, 160.0);
										pdfDoc.Rect.Position(407.0, 87.0);
										pdfDoc.Rect.Width = 80.0;
										pdfDoc.Rect.Height = 10.0;
										pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
										pdfDoc.Rect.Inset(100.0, 200.0);
										pdfDoc.Rect.Position(68.0, 98.0);
										pdfDoc.Rect.Width = 260.0;
									pdfDoc.Rect.Height = 63.0;
										pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]) );
                                        //test code by vivek for mobile no

                                        pdfDoc.Rect.Inset(100.0, 200.0);
                                        pdfDoc.Rect.Position(180.0, 86.0);
                                        pdfDoc.Rect.Width = 260.0;
                                        pdfDoc.Rect.Height = 63.0;
                                        pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                                        //end test code by vivek
										pdfDoc.Rect.Inset(100.0, 160.0);
										pdfDoc.Rect.Position(365.0, 173.0);
										pdfDoc.Rect.Width = 80.0;
										pdfDoc.Rect.Height = 10.0;
										pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
										pdfDoc.Rect.Inset(100.0, 200.0);
                                        pdfDoc.Rect.Position(450.0, 172.0);
										pdfDoc.Rect.Width = 80.0;
										pdfDoc.Rect.Height = 10.0;
										pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
										pdfDoc.Rect.Inset(100.0, 193.0);
										pdfDoc.Rect.Position(195.0, 304.0);
										pdfDoc.Rect.Width= 200.0;
										pdfDoc.Rect.Height = 10.0;
										pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
										pdfDoc.Rect.Inset(100.0, 200.0);
										pdfDoc.Rect.Position(236.0, 362.0);
										pdfDoc.Rect.Width = 80.0;
										pdfDoc.Rect.Height = 10.0;
										pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
										pdfDoc.Rect.Inset(100.0, 200.0);
                                        pdfDoc.Rect.Position(313.0, 362.0);
										pdfDoc.Rect.Width = 80.0;
										pdfDoc.Rect.Height = 10.0;
										pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
										pdfDoc.Rect.Inset(100.0, 200.0);
										pdfDoc.Rect.Position(155.0, 496.0);
										pdfDoc.Rect.Width = 80.0;
										pdfDoc.Rect.Height = 10.0;
										pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
										pdfDoc.Rect.Inset(100.0, 200.0);
										pdfDoc.Rect.Position(255.0, 496.0);
										pdfDoc.Rect.Width = 80.0;
										pdfDoc.Rect.Height = 10.0;
										pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
										pdfDoc.Rect.Inset(100.0, 193.0);
										pdfDoc.Rect.Position(136.0, 758.0);
										pdfDoc.Rect.Width= 200.0;
										pdfDoc.Rect.Height = 10.0;
										pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
										pdfDoc.Rect.Inset(100.0, 193.0);
										pdfDoc.Rect.Position(365.0, 757.0);
										pdfDoc.Rect.Width= 200.0;
										pdfDoc.Rect.Height = 10.0;
										pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void MCAN_Hindi(Doc pdfDoc, DataRow dr, DataTable dt)
        {
            try
            {
                pdfDoc.Rect.Inset(100.0, 193.0);
											pdfDoc.Rect.Position(133.0, 73.0);
											pdfDoc.Rect.Width= 200.0;
											pdfDoc.Rect.Height = 10.0;
											pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
											pdfDoc.Rect.Inset(100.0, 200.0);
											pdfDoc.Rect.Position(354.0, 74.0);
											pdfDoc.Rect.Width = 80.0;
											pdfDoc.Rect.Height = 10.0;
											pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
											pdfDoc.Rect.Inset(100.0, 160.0);
											pdfDoc.Rect.Position(407.0, 93.0);
											pdfDoc.Rect.Width = 80.0;
											pdfDoc.Rect.Height = 10.0;
											pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
											pdfDoc.Rect.Inset(100.0, 200.0);
											pdfDoc.Rect.Position(65.0, 102.0);
											pdfDoc.Rect.Width = 260.0;
										pdfDoc.Rect.Height = 63.0;
											pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));

                                            //test code by vivek for mobile no

                                            pdfDoc.Rect.Inset(100.0, 200.0);
                                            pdfDoc.Rect.Position(180.0, 92.0);
                                            pdfDoc.Rect.Width = 260.0;
                                            pdfDoc.Rect.Height = 63.0;
                                            pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                                            //end test code by vivek
											pdfDoc.Rect.Inset(100.0, 200.0);
											pdfDoc.Rect.Position(370.0, 227.0);
											pdfDoc.Rect.Width = 80.0;
											pdfDoc.Rect.Height = 10.0;
											pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
											pdfDoc.Rect.Inset(100.0, 200.0);
                                            pdfDoc.Rect.Position(450.0, 227.0);
											pdfDoc.Rect.Width = 80.0;
											pdfDoc.Rect.Height = 10.0;
											pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
											pdfDoc.Rect.Inset(100.0, 200.0);
											pdfDoc.Rect.Position(195.0, 398.0);
											pdfDoc.Rect.Width= 200.0;
											pdfDoc.Rect.Height = 10.0;
											pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
											pdfDoc.Rect.Inset(100.0, 200.0);
											pdfDoc.Rect.Position(227.0, 492.0);
											pdfDoc.Rect.Width = 80.0;
											pdfDoc.Rect.Height = 10.0;
											pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
											pdfDoc.Rect.Inset(100.0, 200.0);
                                            pdfDoc.Rect.Position(314.0, 492.0);
											pdfDoc.Rect.Width = 80.0;
											pdfDoc.Rect.Height = 10.0;
											pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
											pdfDoc.Rect.Inset(100.0, 200.0);
                                            pdfDoc.Rect.Position(139.0, 768.0);
											pdfDoc.Rect.Width= 200.0;
											pdfDoc.Rect.Height = 10.0;
											pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
											pdfDoc.Rect.Inset(100.0, 200.0);
											pdfDoc.Rect.Position(376.0, 768.0);
											pdfDoc.Rect.Width= 200.0;
											pdfDoc.Rect.Height = 10.0;
											pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void MCAN_Kannada(Doc pdfDoc, DataRow dr, DataTable dt)
        {
            try
            {
                                                
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 76.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(354.0, 77.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(407.0, 96.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(68.0, 106.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]) );

                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 95.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(365.0, 225.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(450.0, 225.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(195.0, 358.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(236.0, 415.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(316.0, 415.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(163.0, 553.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(245.0, 553.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(155.0, 764.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(368.0, 764.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void MCAN_Tamil(Doc pdfDoc, DataRow dr, DataTable dt)
        {
            try
            {
              
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 76.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(354.0, 76.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(407.0, 95.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(68.0, 105.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]) );
                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 94.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(365.0, 206.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(450.0, 206.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(195.0, 338.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(239.0, 386.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(314.0, 386.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(192.0, 515.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(255.0, 515.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(155.0, 765.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(368.0, 765.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void MCAN_Punjabi(Doc pdfDoc, DataRow dr, DataTable dt)
        {
            try
            {
                

                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 74.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(354.0, 74.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(407.0, 93.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(68.0, 103.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));
                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 92.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(368.0, 198.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(446.0, 198.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(195.0, 332.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(236.0, 390.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(314.0, 390.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(175.0, 529.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(265.0, 529.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(158.0, 763.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(368.0, 763.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void MCAN_Assami(Doc pdfDoc, DataRow dr, DataTable dt)
        {
            try
            {


                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 78.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(354.0, 78.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(407.0, 97.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(68.0, 107.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));
                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 96.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.FontSize = 8;
                pdfDoc.Rect.Position(360.0, 242.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(425.0, 242.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                //pdfDoc.Rect.Inset(100.0, 193.0);
                //pdfDoc.Rect.Position(195.0, 332.0);
                //pdfDoc.Rect.Width = 200.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 437.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(309.0, 437.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(175.0, 529.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(265.0, 529.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(158.0, 697.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(368.0, 697.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }


        public void MCAN_Bengali(Doc pdfDoc, DataRow dr, DataTable dt)
        {
            try
            {


                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 78.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(354.0, 78.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(407.0, 97.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(68.0, 107.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));
                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 96.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.FontSize = 7;
                pdfDoc.Rect.Position(360.0, 241.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(425.0, 241.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                //pdfDoc.Rect.Inset(100.0, 193.0);
                //pdfDoc.Rect.Position(195.0, 332.0);
                //pdfDoc.Rect.Width = 200.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(175.0, 425.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(355.0, 425.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(175.0, 529.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(265.0, 529.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(158.0, 718.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(368.0, 718.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }


        public void AUCTION_Telugu(Doc pdfDoc, DataRow dr, DataTable dt,string dl_dd,string lran_dd)
        {
            try
            {
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 77.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(354.0, 77.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(407.0, 98.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(69.0, 106.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));

                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 97.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 116.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 130.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[15]));
             					
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(275.0, 198.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(397.0, 198.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(144.0, 209.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.226-0;
                pdfDoc.AddText(dl_dd);

                //test code
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(322.0, 209.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
                //emd test code

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(195.0, 346.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(312.0, 346.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(211.0, 357.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);
                //test code
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(397.0, 357.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
                //emd test code
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(147.0, 505.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(239.0, 505.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(170.0, 524.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);
                //test code
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(397.0, 524.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
                //emd test code
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(147, 761.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(365.0, 761.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }

            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void AUCTION_Marathi(Doc pdfDoc, DataRow dr, DataTable dt, string dl_dd, string lran_dd)
        {
            try
            {
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 77.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(354.0, 77.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(407.0, 98.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(69.0, 108.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));
                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 97.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 116.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 130.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[15]));
              				
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(287.0, 223.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(419.0, 223.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(190.0, 235.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);

               
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(401.0, 235.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
              

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(192.0, 381.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(318.0, 381.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(218.0, 392.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);
              
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(411.0, 392.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
              
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(145.0, 533.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(340.0, 533.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(172.0, 546.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);
               
              
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(88.0, 557.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
                
               
               
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(147, 757.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(365.0, 757.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void AUCTION_Gujarati(Doc pdfDoc, DataRow dr, DataTable dt, string dl_dd, string lran_dd)
        {
            try
            {
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 70.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(354.0, 70.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(407.0, 90.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(69.0, 102.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]) );

                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 89.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 109.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 124.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[15]));
              				
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(302.0, 209.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(442.0, 208.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(298.0, 220.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);

         
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(93.0, 234.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
               
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(185.0, 388.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(287.0, 388.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(178.0, 398.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);

             
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(337.0, 398.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
               

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(138.0, 530.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(205.0, 530.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(475.0, 530.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);
             
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(175.0, 540.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
               
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(147, 769.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(368.0, 769.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void AUCTION_Hindi(Doc pdfDoc, DataRow dr, DataTable dt, string dl_dd, string lran_dd)
        {
            try
            {
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 77.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(354.0, 77.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(407.0, 95.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(66.0, 105.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]) );
                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 94.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 115.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 129.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[15]));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(304.0, 221.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(430.0, 221.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(297.0, 233.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);
              
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(93.0, 245.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
               
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(200.0, 472.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
               
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(317.0, 472.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(230.0, 483);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);
               
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(425.0, 483);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
               
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(140, 768.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(365.0, 766.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void AUCTION_Kannada(Doc pdfDoc, DataRow dr, DataTable dt, string dl_dd, string lran_dd)
        {
            try
            {
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 80.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(354.0, 80.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(407.0, 100.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(69.0, 109.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));

                //test code by vivek for mobile no


                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 98.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 119.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 133.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[15]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(302.0, 205.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(436.0, 203.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(292.0, 217.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);
               
              
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(93.0, 227.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
              
              
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(194.0, 383.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(330.0, 381.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(215.0, 393.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);
               

               
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(426.0, 393.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(173.0, 535.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(280.0, 535.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(300.0, 547.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);
               
              
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(90.0, 557.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
              
               
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(147, 759.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(365.0, 759.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));


            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void AUCTION_Tamil(Doc pdfDoc, DataRow dr, DataTable dt, string dl_dd, string lran_dd)
        {
            try
            {
               

                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 76.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(354.0, 76.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(407.0, 97.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(68.0, 105.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]) );

                //test code by vivek for mobile no

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 96.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 115.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 130.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[15]));
               				
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(290.0, 204.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(418.0, 204.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(189.0, 216.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);
               
               
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(400.0, 216.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
               
              
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(181.0, 355.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(295.0, 354.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(153.0, 366.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);
               

               
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(326.0, 366.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
               
              
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(144.0, 486.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(342.0, 486.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(391.0, 497.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);
              
              
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(220.0, 509.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
                
              
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(147, 765.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(350.0, 765.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void AUCTION_Punjabi(Doc pdfDoc, DataRow dr, DataTable dt, string dl_dd, string lran_dd)
        {
            try
            {


                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 77.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(354.0, 77.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(407.0, 97.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(67.0, 107.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]) );

                //test code by vivek for mobile no


                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 96.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 116.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(413.0, 130.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[15]));
               					
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(293.0, 213.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(418.0, 212.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(194.0, 224.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);
                //pdfDoc.AddText("23-Aug-2017");
                //test code
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(392.0, 224.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
                //pdfDoc.AddText("18-Sep-2017");
                //emd test code
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(194.0, 366.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(312.0, 366.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(194.0, 377.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);
                //pdfDoc.AddText("23-Aug-2017");
                //test code
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(392.0, 377.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
                //pdfDoc.AddText("18-Sep-2017");
                //emd test code
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(252.0, 518.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(355.0, 518.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(188.0, 528.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);
                //pdfDoc.AddText("23-Aug-2017");
                //for lran dispatch date
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(384.0, 528.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
                //pdfDoc.AddText("18-Sep-2017");
                //end 
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(147, 762.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(365.0, 761.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void AUCTION_Assami(Doc pdfDoc, DataRow dr, DataTable dt, string dl_dd, string lran_dd)
        {
            try
            {


                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 77.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(370.0, 77.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(420.0, 97.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(67.0, 107.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));

                //test code by vivek for mobile no


                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 96.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(430.0, 119.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(430.0, 133.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[15]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(250.0, 236.0);
                pdfDoc.FontSize = 7;
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(333.0, 236.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(520.0, 236.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);
                //pdfDoc.AddText("23-Aug-2017");
                //test code
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(195.0, 246.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
                //pdfDoc.AddText("18-Sep-2017");
                //emd test code
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(145.0, 396.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(300.0, 396.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(93.0, 406.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);
                //pdfDoc.AddText("23-Aug-2017");
                //test code
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(340.0, 406.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
                //pdfDoc.AddText("18-Sep-2017");
                //emd test code
                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(252.0, 518.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(355.0, 518.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(188.0, 528.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(dl_dd);
                ////pdfDoc.AddText("23-Aug-2017");
                ////for lran dispatch date
                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(384.0, 528.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(lran_dd);
                //pdfDoc.AddText("18-Sep-2017");
                //end 
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(147, 639.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(365.0, 639.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }


        public void AUCTION_Bengali(Doc pdfDoc, DataRow dr, DataTable dt, string dl_dd, string lran_dd)
        {
            try
            {


                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(133.0, 77.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(Convert.ToString(dr.ItemArray[6])));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(370.0, 77.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(DateTime.Now.ToString("dd-MM-yyyy"));
                pdfDoc.Rect.Inset(100.0, 160.0);
                pdfDoc.Rect.Position(420.0, 97.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(67.0, 107.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[7]));

                //test code by vivek for mobile no


                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(180.0, 96.0);
                pdfDoc.Rect.Width = 260.0;
                pdfDoc.Rect.Height = 63.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[8]));

                //end test code by vivek 
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(430.0, 119.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(430.0, 133.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[15]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(250.0, 236.0);
                pdfDoc.FontSize = 7;
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(333.0, 236.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(520.0, 236.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);
                //pdfDoc.AddText("23-Aug-2017");
                //test code
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(195.0, 246.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
                //pdfDoc.AddText("18-Sep-2017");
                //emd test code
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(290.0, 397.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));

                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(500.0, 397.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(67.0, 409.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(dl_dd);
                //pdfDoc.AddText("23-Aug-2017");
                //test code
                pdfDoc.Rect.Inset(100.0, 200.0);
                pdfDoc.Rect.Position(450.0, 409.0);
                pdfDoc.Rect.Width = 80.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(lran_dd);
                //pdfDoc.AddText("18-Sep-2017");
                //emd test code
                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(252.0, 518.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(Convert.ToString(dr.ItemArray[5]));
                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(355.0, 518.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(Convert.ToString(dr.ItemArray[10]));
                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(188.0, 528.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(dl_dd);
                ////pdfDoc.AddText("23-Aug-2017");
                ////for lran dispatch date
                //pdfDoc.Rect.Inset(100.0, 200.0);
                //pdfDoc.Rect.Position(384.0, 528.0);
                //pdfDoc.Rect.Width = 80.0;
                //pdfDoc.Rect.Height = 10.0;
                //pdfDoc.AddText(lran_dd);
                //pdfDoc.AddText("18-Sep-2017");
                //end 
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(147, 680.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(" : " + Convert.ToString(dr.ItemArray[3]));
                pdfDoc.Rect.Inset(100.0, 193.0);
                pdfDoc.Rect.Position(365.0, 680.0);
                pdfDoc.Rect.Width = 200.0;
                pdfDoc.Rect.Height = 10.0;
                pdfDoc.AddText(Convert.ToString(dr.ItemArray[4]));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

        public void CountGenPDF(int ReferenceNo, string CustName, string Branch, string Type, string Lan, string PdfName, int Dpd, string pdfpath, string source, string division, string district, int marginpercent, string customercode, int count)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Margin_InsPdfGenDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_RefrenceNo", ReferenceNo);
                cmd.Parameters.AddWithValue("@p_CustomerName", CustName);
                cmd.Parameters.AddWithValue("@p_Branch", Branch);
                cmd.Parameters.AddWithValue("@p_Type", Type);
                cmd.Parameters.AddWithValue("@p_Lan", Lan);
                cmd.Parameters.AddWithValue("@p_PdfName", PdfName);
                cmd.Parameters.AddWithValue("@p_Dpd", Dpd);
                cmd.Parameters.AddWithValue("@p_PdfPath", pdfpath);
                cmd.Parameters.AddWithValue("@p_Source", source);
                cmd.Parameters.AddWithValue("@p_Division", division);
                cmd.Parameters.AddWithValue("@p_District", district);
                cmd.Parameters.AddWithValue("@p_MarginPercent", marginpercent);
                cmd.Parameters.AddWithValue("@p_CustomerCode", customercode);
                cmd.Parameters.AddWithValue("@P_GENPDF", count);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Convert.ToString(ex.Message));
            }
        }

    }
}
public class Logger
{
    public static void WriteLog(string strLog)
    {
        StreamWriter log;
        FileStream fileStream = null;
        DirectoryInfo logDirInfo = null;
        FileInfo logFileInfo;

        string logFilePath = String.Concat(Convert.ToString(ConfigurationSettings.AppSettings["logpath"]), "Log File");
        logFilePath = logFilePath + "\\PDFLog-" + System.DateTime.Today.ToString("dd-MM-yyyy") + "." + "txt";
        logFileInfo = new FileInfo(logFilePath);
        logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
        if (!logDirInfo.Exists) logDirInfo.Create();
        if (!logFileInfo.Exists)
        {
            fileStream = logFileInfo.Create();
        }
        else
        {
            fileStream = new FileStream(logFilePath, FileMode.Append);
        }
        log = new StreamWriter(fileStream);
        log.WriteLine("{0} :: {1}", DateTime.Now.ToLongTimeString(), strLog);
        log.WriteLine("-------------------------------");
        log.Close();
    }   
}
