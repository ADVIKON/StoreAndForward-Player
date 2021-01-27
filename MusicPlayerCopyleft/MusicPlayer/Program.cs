﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data.Sql;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Diagnostics;
using NetFwTypeLib;
namespace StoreAndForwardPlayer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 


        [STAThread]
        static void Main(string[] args)
        {
            try
            {



                gblClass objMainClass = new gblClass();

                StaticClass.constr = new SqlConnection("Data Source=146.0.237.246;database=OnlineDB;uid=sa;password=Jan@Server007;Connect Timeout=5000");
                StaticClass.LocalCon.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\db.mdb;User Id=admin;Password=;";
                string str = "";
                string localCode = "";
                string filename = Application.StartupPath + "\\tid.amp";
                string textline = "";
                Int64 IsBlock = 0;
                Int64 IsSuspend = 0;
                string ExpiryCopyrightStatus = "";
                Int64 LeftCopyrightDays = 0;

                string strOpt = "";
                string proc = Process.GetCurrentProcess().ProcessName;
                Process[] processes = Process.GetProcessesByName("StoreAndForwardPlayer");

                //                MessageBox.Show("First time " + processes.Length);
                if (processes.Length > 1)
                {
                    Application.Exit();
                    return;
                    // MessageBox.Show("Application is already running" );
                    //return;http://www.sikhiwiki.org/index.php/Guru_Granth_Sahib_on_alcohol
                }

                if (File.Exists(filename))
                {
                    System.IO.StreamReader objReader;
                    objReader = new System.IO.StreamReader(filename);
                    do
                    {

                        textline = textline + objReader.ReadLine();
                    } while (objReader.Peek() != -1);
                    objReader.Close();

                    try
                    {
                        string strOpt1 = "select * from tbMisc";
                        DataSet dsOption1 = new DataSet();
                        dsOption1 = objMainClass.fnFillDataSet_Local(strOpt1);
                        if (dsOption1.Tables[0].Rows.Count > 0)
                        {
                            StaticClass.DealerCode = dsOption1.Tables[0].Rows[0]["DealerCode"].ToString();
                            StaticClass.dfClientId = dsOption1.Tables[0].Rows[0]["dfClientId"].ToString();
                            StaticClass.IsStore = Convert.ToBoolean(dsOption1.Tables[0].Rows[0]["IsStore"]);
                            StaticClass.IsAdvt = Convert.ToBoolean(dsOption1.Tables[0].Rows[0]["IsAdvt"]);
                            StaticClass.IsLock = Convert.ToBoolean(dsOption1.Tables[0].Rows[0]["IsLock"]);
                            StaticClass.PlayerVersion = Convert.ToInt32(dsOption1.Tables[0].Rows[0]["PlayerVersion"]);
                            StaticClass.MainwindowMessage = dsOption1.Tables[0].Rows[0]["support"].ToString();
                            StaticClass.ScheduleType = dsOption1.Tables[0].Rows[0]["ScheduleType"].ToString();
                            
                        }
                    }
                    catch (Exception ex) { }

                    if (objMainClass.CheckForInternetConnection() == false)
                    {
                        StaticClass.TokenServiceId = 0;
                        StaticClass.IsAdvtManual = false;
                        StaticClass.IsBlockAdvt = false;
                        StaticClass.TokenUserId = 0;
                        StaticClass.AdvtCityId = 0;
                        StaticClass.IsCopyright = true;
                        StaticClass.TokenId = textline;

                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new CopyrightPlayer());
                        return;
                    }


                    if (StaticClass.constr.State == ConnectionState.Open) StaticClass.constr.Close();
                    StaticClass.constr.Open();

                    strOpt = "select ISNULL(IsCopyright,0) as Copyright, ISNULL(IsFitness,0) as Fitness, isnull(IsStream,0) as Stream,dealerCode, ISNULL(IsAdvt,0) as Advt, ISNULL(IsAdvtManual,0) as AdvtManual, ISNULL(IsBlockAdvt,0) as IsBlockAdvt, isnull(serviceid,0) as serviceid , isnull(cityid,0) as CityId  , isnull(StateId,0) as StateId, isnull(CountryId,0) as CountryId, UserId, isnull(isStore,0) as isStore,isnull(IsStopControl,0) as IsLock , isnull(IsVedioActive,0) as VedioActive, isnull(IsUpdated,0) as PlayerVersion, isnull(PersonName,'') as pname, isnull(AMPlayerTokens.ScheduleType,'0') as ScheduleType from AMPlayerTokens where TokenID=" + textline;
                    DataSet dsOption = new DataSet();
                    dsOption = objMainClass.fnFillDataSet(strOpt);
                    if (dsOption.Tables[0].Rows.Count > 0)
                    {
                        StaticClass.TokenServiceId = Convert.ToInt32(dsOption.Tables[0].Rows[0]["serviceid"]);
                        StaticClass.DealerCode = dsOption.Tables[0].Rows[0]["DealerCode"].ToString();

                        StaticClass.IsAdvtManual = Convert.ToBoolean(dsOption.Tables[0].Rows[0]["AdvtManual"]);
                        StaticClass.IsAdvt = Convert.ToBoolean(dsOption.Tables[0].Rows[0]["Advt"]);
                        StaticClass.IsBlockAdvt = Convert.ToBoolean(dsOption.Tables[0].Rows[0]["IsBlockAdvt"]);
                        StaticClass.TokenUserId = Convert.ToInt32(dsOption.Tables[0].Rows[0]["userid"]);
                        StaticClass.AdvtCityId = Convert.ToInt32(dsOption.Tables[0].Rows[0]["CityId"]);
                        StaticClass.Stateid = Convert.ToInt32(dsOption.Tables[0].Rows[0]["Stateid"]);
                        StaticClass.CountryId = Convert.ToInt32(dsOption.Tables[0].Rows[0]["CountryId"]);
                        StaticClass.IsStore = Convert.ToBoolean(dsOption.Tables[0].Rows[0]["IsStore"]);
                        StaticClass.IsLock = Convert.ToBoolean(dsOption.Tables[0].Rows[0]["IsLock"]);
                        StaticClass.IsVedioActive = Convert.ToBoolean(dsOption.Tables[0].Rows[0]["VedioActive"]);

                        StaticClass.TokenId = textline;
                        StaticClass.PlayerVersion = Convert.ToInt32(dsOption.Tables[0].Rows[0]["PlayerVersion"]);

                        StaticClass.ScheduleType = dsOption.Tables[0].Rows[0]["ScheduleType"].ToString();
                        StaticClass.MainwindowMessage = StaticClass.TokenId.ToString() + "  (" + dsOption.Tables[0].Rows[0]["pname"].ToString() + ")";


                        str = "spGetTokenExpiryStatus_Copyright " + textline + ", " + dsOption.Tables[0].Rows[0]["Copyright"] + ", " + dsOption.Tables[0].Rows[0]["Fitness"] + ", " + dsOption.Tables[0].Rows[0]["Stream"];
                        DataSet dsExpire = new DataSet();
                        dsExpire = objMainClass.fnFillDataSet(str);


                        ExpiryCopyrightStatus = dsExpire.Tables[0].Rows[0]["ExpiryCopyrightStatus"].ToString();
                        LeftCopyrightDays = Convert.ToInt32(dsExpire.Tables[0].Rows[0]["LeftCopyrightDays"]);

                        StaticClass.StreamExpiryMessage = dsExpire.Tables[0].Rows[0]["ExpiryStreamStatus"].ToString();
                        StaticClass.LeftStreamtDays = Convert.ToInt32(dsExpire.Tables[0].Rows[0]["LeftStreamDays"]);

                        if (ExpiryCopyrightStatus == "NoLic")
                        {
                            StaticClass.PlayerExpiryMessage = "Purchase the subscription of music player. Please contact our support team ";
                            Application.SetCompatibleTextRenderingDefault(false);
                            Application.Run(new frmNet());
                            return;
                        }
                        if (ExpiryCopyrightStatus == "Yes")
                        {
                            StaticClass.PlayerExpiryMessage = "Your license has expired. Please contact our support team ";
                            Application.SetCompatibleTextRenderingDefault(false);
                            Application.Run(new frmNet());
                            return;
                        }

                        if (ExpiryCopyrightStatus != "NoLic" && LeftCopyrightDays <= 10)
                        {
                            StaticClass.PlayerExpiryMessage = Convert.ToString(LeftCopyrightDays) + " days left to renewal of subscription. Please contact our support team";
                            StaticClass.IsCopyright = true;
                        }
                        else if (ExpiryCopyrightStatus != "NoLic" && LeftCopyrightDays == 0)
                        {
                            StaticClass.PlayerExpiryMessage = "Last day to renewal of subscription. Please contact our support team ";
                            StaticClass.IsCopyright = true;
                        }
                        else
                        {
                            StaticClass.IsCopyright = true;
                        }
                        if (ExpiryCopyrightStatus == "Yes")
                        {
                            StaticClass.PlayerExpiryMessage = "Your license has expired. Please contact our support team ";
                            Application.SetCompatibleTextRenderingDefault(false);
                            Application.Run(new frmNet());
                            return;
                        }
                         
                        else if (ExpiryCopyrightStatus == "NoLic")
                        {
                            StaticClass.PlayerExpiryMessage = "You do not have license. Please contact our support team ";
                            Application.SetCompatibleTextRenderingDefault(false);
                            Application.Run(new frmNet());
                            return;
                        }

                        // str = "spGetTokenExpiryStatus_Copyleft " + Convert.ToInt32(textline) + ", " + dsOption.Tables[0].Rows[0]["Dam"] + ", " + dsOption.Tables[0].Rows[0]["Sanjivani"] + ", " + dsOption.Tables[0].Rows[0]["Stream"];
                        StaticClass.StreamExpiryMessage = dsExpire.Tables[0].Rows[0]["ExpiryStreamStatus"].ToString();
                        StaticClass.LeftStreamtDays = Convert.ToInt32(dsExpire.Tables[0].Rows[0]["LeftStreamDays"]);
                        str = "select *, ISNULL(IsBlock,0) as Is_Block, ISNULL(IsSuspend,0) as Is_Suspend   from AMPlayerTokens where tokenid=" + textline;
                        DataSet ds = new DataSet();
                        string dbCode = "";

                        ds = objMainClass.fnFillDataSet(str);
                        dbCode = ds.Tables[0].Rows[0]["code"].ToString();
                        try
                        {
                            localCode = GenerateId.getKey(GenerateId._wvpaudi);
                        }
                        catch (Exception ex)
                        {
                            //dbCode = textline;
                           // localCode = textline;
                        }
                        //6B16-4875-D8C6-7142-21D7
                        //6B16-4875-D8C6-7142-21D7

                        if (dbCode == localCode)
                        {
                            StaticClass.dfClientId = ds.Tables[0].Rows[0]["ClientID"].ToString();

                            StaticClass.TokenId = ds.Tables[0].Rows[0]["TokenId"].ToString();

                            IsBlock = Convert.ToInt32(ds.Tables[0].Rows[0]["Is_Block"]);
                            IsSuspend = Convert.ToInt32(ds.Tables[0].Rows[0]["Is_Suspend"]);
                            if (IsBlock == 1)
                            {
                                MessageBox.Show("Your token is blocked by admin");
                                Application.Exit();
                                return;
                            }
                            else if (IsSuspend == 1)
                            {
                                MessageBox.Show("Your token is suspend by admin");
                                return;
                            }
                            //  Application.EnableVisualStyles();
                            Application.SetCompatibleTextRenderingDefault(false);

                            Application.Run(new Clientlogin());
                            return;
                        }
                        else
                        {
                            //   Application.EnableVisualStyles();
                            Application.SetCompatibleTextRenderingDefault(false);
                            Application.Run(new frmStart());
                            return;
                        }
                    }
                    else
                    {
                        File.Delete(Application.StartupPath + "//tid.amp");
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new frmStart());
                        return;
                    }
                }
                else
                {
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new frmStart());
                    return;
                }

            }

            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message);
            }

        }




    }

}
/// 49439906 Hotel 3 Login
