﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using XXCWEBAPI.Models;
using XXCWEBAPI.Utils;

namespace XXCWEBAPI.Controllers
{   
    [RoutePrefix("api/visitors")]
    public class VisitorsController : ApiController
    {
        public string AppId = ConfigurationManager.AppSettings["AppId"];
        public string AppSecret = ConfigurationManager.AppSettings["AppSecret"];
        [HttpGet, Route("testLink")]
        public string TestLink()
        {
            string result = SQLHelper.LinkSqlDatabase();
            return ConvertHelper.resultJson(1, result);
            //return mssqlserver;
        }
        [HttpPost, Route("getAccess_token")]
        public string GetAccess_token(XXYXT d)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + d.appid + "&secret=" + d.secret;

            HttpHelper2 http = new HttpHelper2();

            string json = http.GetResponseString(HttpHelper.CreateGetHttpResponse(url));

            JavaScriptSerializer js = new JavaScriptSerializer();   //实例化一个能够序列化数据的类
            AccessClass list = js.Deserialize<AccessClass>(json);    //将json数据转化为对象类型并赋值给list
            //textBox1.Text = list.access_token;
            return list.access_token;
        }
        [HttpPost, Route("getOpenId")]
        public string GetOpenId(XXYXT d)
        {
            string url = "https://api.weixin.qq.com/sns/jscode2session?appid=" + d.appid + "&secret=" + d.secret + "&js_code=" + d.js_code + "&grant_type=authorization_code";

            HttpHelper2 http = new HttpHelper2();

            string json = http.GetResponseString(HttpHelper.CreateGetHttpResponse(url));

            JavaScriptSerializer js = new JavaScriptSerializer();   //实例化一个能够序列化数据的类
            OpenIdClass list = js.Deserialize<OpenIdClass>(json);    //将json数据转化为对象类型并赋值给list
            //textBox1.Text = list.access_token;
            return list.openid;
        }
        public struct AccessClass
        {
            public string access_token { get; set; }
        }
        public struct OpenIdClass
        {
            public string openid { get; set; }
        }
        [HttpPost, Route("sendMessage")]
        public string SendMessage(WXUserInfo d)
        {
            #region 微信服务消息发送
            string access_token = d.access_token;//需要从数据库中获取
            string touser = d.openid;//需要从数据库中获取
            string template_id = d.template_id;
            string form_id = d.form_id;//需要从数据库中获取
            //textBox1.Text = "_form_id:" + form_id;
            //string page = "pages/user/index?SActualNo=" + strSActualNo;
            string page = "pages/firstPage/firstPage";
            //var data = new TemplateModel("智慧幼儿园", "SName", DateTime.Now.ToString("yyy-MM-dd"));

            var keyword1 = new
            {
                value = d.name,
                color = "#173177"
            };
            var keyword2 = new
            {
                value = d.SMPhone,
                color = "#173177"
            };
            var keyword3 = new
            {
                value = d.SDDetailName,
                color = "#173177"
            };
            var keyword4 = new
            {
                value = d.ordercode,
                color = "#173177"
            };
            var data = new
            {
                keyword1 = keyword1,
                keyword2 = keyword2,
                keyword3 = keyword3,
                keyword4 = keyword4
            };
            WeChat wechat = new WeChat();
            string result = wechat.SendTemplete(access_token, template_id, touser, form_id, page, data);
            return result;
            //if (result!="")
            //{
            //    return ConvertHelper.resultJson(1, "消息推送成功");
            //}
            //else
            //{
            //    return ConvertHelper.resultJson(0, "消息推送失败");
            //}
            #endregion
        }
        [HttpPost, Route("sendMessage2")]
        public string SendMessage2(WXUserInfo d)
        {
            #region 微信服务消息发送
            string access_token = d.access_token;//需要从数据库中获取
            string touser = d.openid;//需要从数据库中获取
            string template_id = d.template_id;
            string form_id = d.form_id;//需要从数据库中获取
            //textBox1.Text = "_form_id:" + form_id;
            //string page = "pages/user/index?SActualNo=" + strSActualNo;
            string page = "pages/firstPage/firstPage";
            //var data = new TemplateModel("智慧幼儿园", "SName", DateTime.Now.ToString("yyy-MM-dd"));

            var keyword1 = new
            {
                value = d.name,
                color = "#173177"
            };
            var keyword2 = new
            {
                value = d.CreateTime,
                color = "#173177"
            };
            var keyword3 = new
            {
                value = d.NowTime,
                color = "#173177"
            };
            var keyword4 = new
            {
                value = d.sname,
                color = "#173177"
            };
            var keyword5 = new
            {
                value = d.checkStatus,
                color = "#173177"
            };
            var keyword6 = new
            {
                value = d.RefuseReason,
                color = "#173177"
            };
            var data = new
            {
                keyword1 = keyword1,
                keyword2 = keyword2,
                keyword3 = keyword3,
                keyword4 = keyword4,
                keyword5 = keyword5,
                keyword6 = keyword6
            };
            WeChat wechat = new WeChat();
            string result = wechat.SendTemplete(access_token, template_id, touser, form_id, page, data);
            return result;
            //if (result!="")
            //{
            //    return ConvertHelper.resultJson(1, "消息推送成功");
            //}
            //else
            //{
            //    return ConvertHelper.resultJson(0, "消息推送失败");
            //}
            #endregion
        }
        [HttpPost, Route("addWXUserInfo")]
        public string AddWXUserInfo(WXUserInfo v)
        {
            string sql = "select count(*) from XXCLOUDVisitor.dbo.Table_WXUserInfo where OpenId = @OpenId";
            object obj;
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@OpenId",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.OpenId)}
            };
            try
            {
                obj = SQLHelper.ExecuteScalar(sql, System.Data.CommandType.Text, pms);
                if (Convert.ToInt32(obj) == 1)
                {
                    return ConvertHelper.resultJson(1, "数据库中已经存在此数据");
                }
                else if (Convert.ToInt32(obj) == 0)
                {
                    if (string.IsNullOrEmpty(v.Phone))
                    {
                        return ConvertHelper.resultJson(0, "请先填写常用手机号");
                    }
                    //return ConvertHelper.resultJson(0, "数据库中不存在此数据");
                    string sql2 = "insert into XXCLOUDVisitor.dbo.Table_WXUserInfo(NickName, Gender, City, Province, AvatarUrl, OpenId, CreateTime, Phone)" +
                "values(@NickName, @Gender, @City, @Province, @AvatarUrl, @OpenId, @CreateTime, @Phone)";
                    DateTime dt = DateTime.Now;
                    SqlParameter[] pms2 = new SqlParameter[]{
                        new SqlParameter("@NickName",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.NickName)},
                        new SqlParameter("@Gender",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Gender)},
                        new SqlParameter("@City",SqlDbType.NVarChar){Value= DataHelper.IsNullReturnLine(v.City)},
                        new SqlParameter("@Province",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Province)},
                        new SqlParameter("@AvatarUrl",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.AvatarUrl)},
                        new SqlParameter("@OpenId",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.OpenId)},
                        new SqlParameter("@CreateTime",SqlDbType.NVarChar){Value=dt.ToString("yyyy-MM-dd hh:mm:ss")},
                        new SqlParameter("@Phone",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Phone)}
                    };
                    try
                    {
                        int result = SQLHelper.ExecuteNonQuery(sql2, System.Data.CommandType.Text, pms2);
                        return ConvertHelper.IntToJson(result);
                    }
                    catch (Exception e)
                    {
                        //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                        var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                        {
                            Content = new StringContent(e.ToString()),
                            ReasonPhrase = "error"
                        };
                        throw new HttpResponseException(resp);
                    }
                }
                return ConvertHelper.resultJson(0, "系统出错了");
            }
            catch (Exception e)
            {
                //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString()),
                    ReasonPhrase = "error"
                };
                throw new HttpResponseException(resp);
            }
        }
        [HttpPost, Route("haveUserInfo")]
        public string HaveUserInfo(WXUserInfo v)
        {
            string sql = "select count(*) from XXCLOUDVisitor.dbo.Table_WXUserInfo where OpenId = @OpenId";
            object obj;
            SqlParameter[] pms = new SqlParameter[]{
                    new SqlParameter("@OpenId",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.OpenId)}
                };
            try
            {
                obj = SQLHelper.ExecuteScalar(sql, System.Data.CommandType.Text, pms);
                if (Convert.ToInt32(obj) == 1)
                {
                    return ConvertHelper.resultJson(1, "数据库中已经存在此数据");
                }
                else {
                    return ConvertHelper.resultJson(0, "数据库中不存在此数据");
                }
            }
            catch (Exception e)
            {
                //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString()),
                    ReasonPhrase = "error"
                };
                throw new HttpResponseException(resp);
            }
        }
        [HttpPost, Route("getOpenIdByCode")]
        public string GetOpenIdByCode(VisitorsSearchModel v)
        {
            if (v.js_code != "" && v.js_code != null)
            {
                //string url = "https://api.weixin.qq.com/sns/jscode2session?appid=" + AppId + "&secret=" + AppSecret + "&js_code=" + v.Code + "&grant_type=authorization_code";
                string url = "https://api.weixin.qq.com/sns/jscode2session";
                string p="";
                p = "appid=" + v.appid + "&secret=" + v.secret + "&js_code=" + v.js_code + "&grant_type=" + v.grant_type;
                string result = HttpHelper.HttpPost(url, p);
                return ConvertHelper.resultJson(0, result);
            }
            return ConvertHelper.resultJson(0, "系统出错了");
        }
        [HttpPost, Route("check")]
        public string Check(Visitors v)
        {
            string sql = "";
            SqlParameter[] pms = null;
            DateTime dt = DateTime.Now;
            int RandKey=1000;

            //new SqlParameter("@Phone",SqlDbType.NVarChar){Value = (v.Phone)},
            //string sql_str = 
            if (v.CheckStatus == "1")//CheckStatus:1,审核通过,则给对方分配预约码
            {
                bool is_ec_ok = false;
                while (!is_ec_ok) {
                    Random ran = new Random();
                    RandKey = ran.Next(1000, 9999);

                    string sqlIsExistEC = "select count(*) from XXCLOUDVisitor.dbo.Table_Visitors where EnterCode=@EnterCode";
                        SqlParameter[] pms4EC = new SqlParameter[]{
                        new SqlParameter("@EnterCode",SqlDbType.NVarChar){Value = RandKey.ToString()}
                    };
                    object obj = SQLHelper.ExecuteScalar(sqlIsExistEC, System.Data.CommandType.Text, pms4EC);
                    if (Convert.ToInt32(obj) == 0)
                    { //说明此EnterCode可以使用
                        is_ec_ok = true;
                    }
                }
                pms = new SqlParameter[]{
                    new SqlParameter("@Id",SqlDbType.Int){Value = (v.Id)},
                    new SqlParameter("@CheckStatus",SqlDbType.NVarChar){Value = (v.CheckStatus)},
                    new SqlParameter("@Checker",SqlDbType.NVarChar){Value = (v.Checker)},
                    new SqlParameter("@CheckDate",SqlDbType.NVarChar){Value = dt.ToString("yyyy-MM-dd")},
                    new SqlParameter("@EnterCode",SqlDbType.NVarChar){Value = RandKey.ToString()}
                };
                sql = "update XXCLOUDVisitor.dbo.Table_Visitors set CheckStatus=@CheckStatus,Checker=@Checker,CheckDate=@CheckDate,EnterCode=@EnterCode where Id=@Id";
            }
            else {
                pms = new SqlParameter[]{
                    new SqlParameter("@Id",SqlDbType.Int){Value = (v.Id)},
                    new SqlParameter("@CheckStatus",SqlDbType.NVarChar){Value = (v.CheckStatus)},
                    new SqlParameter("@Checker",SqlDbType.NVarChar){Value = (v.Checker)},
                    new SqlParameter("@CheckDate",SqlDbType.NVarChar){Value = dt.ToString("yyyy-MM-dd")},
                    new SqlParameter("@RefuseReason",SqlDbType.NVarChar){Value = DataHelper.IsNullReturnLine(v.RefuseReason)}
                };
                sql = "update XXCLOUDVisitor.dbo.Table_Visitors set CheckStatus=@CheckStatus,Checker=@Checker,CheckDate=@CheckDate,RefuseReason=@RefuseReason where Id=@Id";
            }
            
            try
            {
                int result = SQLHelper.ExecuteNonQuery(sql, System.Data.CommandType.Text, pms);
                return ConvertHelper.IntToJson(result);
            }
            catch (Exception e)
            {
                //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString()),
                    ReasonPhrase = "error"
                };
                throw new HttpResponseException(resp);
            }
        }
        [HttpPost, Route("updatePassword")]
        public string UpdatePassword(VisitorsSearchModel v)
        {
            string sql = "";
            SqlParameter[] pms = null;
            int result;
            object obj;
            pms = new SqlParameter[]{
                new SqlParameter("@Id",SqlDbType.Int){Value = (v.Id)},
                new SqlParameter("@SMPhone",SqlDbType.NVarChar){Value = (v.SMPhone)},
                new SqlParameter("@SInitialPassword",SqlDbType.NVarChar){Value = (v.SInitialPassword)},
                new SqlParameter("@NewPassword",SqlDbType.NVarChar){Value = (v.NewPassword)}
            };
            //核实密码
            sql = "select count(*) from XXCLOUD.dbo.T_StaffInf where Id=@Id and SMPhone=@SMPhone and SInitialPassword=@SInitialPassword";
            obj = SQLHelper.ExecuteScalar(sql, System.Data.CommandType.Text, pms);
            if (Convert.ToInt32(obj) == 1)
            {
                SqlParameter[] pms2 = new SqlParameter[]{
                    new SqlParameter("@Id",SqlDbType.Int){Value = (v.Id)},
                    new SqlParameter("@NewPassword",SqlDbType.NVarChar){Value = (v.NewPassword)}
                };
                // 修改密码
                string sql2 = "update XXCLOUD.dbo.T_StaffInf set SInitialPassword=@NewPassword where Id=@Id";
                try
                {
                    result = SQLHelper.ExecuteNonQuery(sql2, System.Data.CommandType.Text, pms2);
                    return ConvertHelper.IntToJson(result);
                }
                catch (Exception e)
                {
                    //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                    var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(e.ToString()),
                        ReasonPhrase = "error"
                    };
                    throw new HttpResponseException(resp);
                }
            }
            else {
                return "{\"code\":0,\"msg\":" + "旧密码错误" + "}";
            }
        }
        [HttpGet, Route("login")]
        public string Login(string SMPhone, string SInitialPassword,string OpenId4In)
        {
            string sql = "";
            SqlParameter[] pms = null;
            pms = new SqlParameter[]{
                new SqlParameter("@SMPhone",SqlDbType.NVarChar){Value = (SMPhone)},
                new SqlParameter("@SInitialPassword",SqlDbType.NVarChar){Value = (SInitialPassword)}
            };

            //new SqlParameter("@Phone",SqlDbType.NVarChar){Value = (v.Phone)},
            //string sql_str = 
            sql = "select count(*) from XXCLOUD.dbo.T_StaffInf where SMPhone=@SMPhone and SInitialPassword=@SInitialPassword";
            DataTable dt;
            object obj;
            try
            {
                // 先用count(*)
                obj = SQLHelper.ExecuteScalar(sql, CommandType.Text, pms);
                if (Convert.ToInt32(obj) == 1) {
                    //if count(*)==1 在获取userinfo
                    string sql2 = "select * from XXCLOUD.dbo.T_StaffInf where SMPhone=@SMPhone and SInitialPassword=@SInitialPassword";
                    SqlParameter[] pms2 = null;
                    pms2 = new SqlParameter[]{
                        new SqlParameter("@SMPhone",SqlDbType.NVarChar){Value = (SMPhone)},
                        new SqlParameter("@SInitialPassword",SqlDbType.NVarChar){Value = (SInitialPassword)}
                    };
                    dt = SQLHelper.ExecuteDataTable(sql2, CommandType.Text, pms2);
                    string sql3 = "update XXCLOUD.dbo.T_StaffInf set OpenId4In=@OpenId4In where SMPhone=@SMPhone and SInitialPassword=@SInitialPassword";
                    SqlParameter[] pms3 = null;
                    pms3 = new SqlParameter[]{
                        new SqlParameter("@SMPhone",SqlDbType.NVarChar){Value = (SMPhone)},
                        new SqlParameter("@SInitialPassword",SqlDbType.NVarChar){Value = (SInitialPassword)},
                        new SqlParameter("@OpenId4In",SqlDbType.NVarChar){Value = (OpenId4In)}
                    };
                    int i = SQLHelper.ExecuteNonQuery(sql3, CommandType.Text, pms3);
                    if (i == 1)
                    {
                        return "{\"code\":1,\"count\":" + dt.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt) + "}";
                    }
                    else {
                        return ConvertHelper.resultJson(0, "系统出错");
                    }
                    
                }
                else {
                    return ConvertHelper.resultJson(0, "账号或密码错误");
                }
                
            }
            catch (Exception e)
            {
                //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString()),
                    ReasonPhrase = "error"
                };
                throw new HttpResponseException(resp);
            }
        }
        [HttpPost, Route("getInfoByEnterCode")]
        public string GetInfoByEnterCode(Visitors v)
        {
            string sql = "";
            SqlParameter[] pms = null;
            pms = new SqlParameter[]{
                new SqlParameter("@EnterCode",SqlDbType.NVarChar){Value = (v.EnterCode)}
            };
            sql += " select * from XXCLOUDVisitor.dbo.Table_Visitors V";
            sql += " left join XXCLOUD.dbo.T_StaffInf S";
            sql += " on V.SNo = S.SNo";
            sql += " where V.EnterCode = @EnterCode and V.IsUseEC = 0";


            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, CommandType.Text, pms);
                return "{\"code\":1,\"count\":" + dt.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt) + "}";
            }
            catch (Exception e)
            {
                //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString()),
                    ReasonPhrase = "error"
                };
                throw new HttpResponseException(resp);
            }
        }
        [HttpGet, Route("getStaffInfoByName")]
        public string GetStaffInfoByName(string SName)
        {
            string sql = "";
            SqlParameter[] pms = null;
            pms = new SqlParameter[]{
                new SqlParameter("@SName",SqlDbType.NVarChar){Value = (SName)}
            };

            //new SqlParameter("@Phone",SqlDbType.NVarChar){Value = (v.Phone)},
            //string sql_str = 
            sql = "select * from XXCLOUD.dbo.T_StaffInf where SName = @SName";
            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, CommandType.Text, pms);
                return "{\"code\":1,\"count\":" + dt.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt) + "}";
            }
            catch (Exception e)
            {
                //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString()),
                    ReasonPhrase = "error"
                };
                throw new HttpResponseException(resp);
            }
        }
        [HttpGet, Route("getStaffInfoByPhone")]
        public string GetStaffInfoByPhone(string SMPhone)
        {
            string sql = "";
            SqlParameter[] pms = null;
            pms = new SqlParameter[]{
                new SqlParameter("@SMPhone",SqlDbType.NVarChar){Value = (SMPhone)}
            };

            //new SqlParameter("@Phone",SqlDbType.NVarChar){Value = (v.Phone)},
            //string sql_str = 
            sql = "select * from XXCLOUD.dbo.T_StaffInf where SMPhone = @SMPhone";
            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, CommandType.Text, pms);
                return "{\"code\":1,\"count\":" + dt.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt) + "}";
            }
            catch (Exception e)
            {
                //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString()),
                    ReasonPhrase = "error"
                };
                throw new HttpResponseException(resp);
            }
        }
        [HttpPost, Route("getListByNameOrPhone")]
        /// <summary>
        /// 
        /// </summary>
        public string GetListByNameOrPhone(VisitorsSearchModel v)
        {
            string sql = "";
            SqlParameter[] pms = null;

            if (v.Type == "check")
            {
                if (!string.IsNullOrEmpty(v.Name) && string.IsNullOrEmpty(v.Phone))
                {
                    pms = new SqlParameter[]{
                        new SqlParameter("@Name",SqlDbType.NVarChar){Value = (v.Name)},
                        new SqlParameter("@CheckStatus",SqlDbType.NVarChar){Value = (v.CheckStatus)}
                    };
                    sql += " select * from XXCLOUDVisitor.dbo.Table_Visitors V";
                    sql += " left join XXCLOUD.dbo.T_StaffInf S";
                    sql += " on V.SNo = S.SNo";
                    sql += " where Name = @Name and CheckStatus=@CheckStatus";
                }
                else if (string.IsNullOrEmpty(v.Name) && !string.IsNullOrEmpty(v.Phone))
                {
                    pms = new SqlParameter[]{
                        new SqlParameter("@Phone",SqlDbType.NVarChar){Value = (v.Phone)},
                        new SqlParameter("@CheckStatus",SqlDbType.NVarChar){Value = (v.CheckStatus)}
                    };
                    sql += " select * from XXCLOUDVisitor.dbo.Table_Visitors V";
                    sql += " left join XXCLOUD.dbo.T_StaffInf S";
                    sql += " on V.SNo = S.SNo";
                    sql += " where Phone = @Phone and CheckStatus=@CheckStatus";
                }
                else if (!string.IsNullOrEmpty(v.Name) && !string.IsNullOrEmpty(v.Phone))
                {
                    pms = new SqlParameter[]{
                        new SqlParameter("@Name",SqlDbType.NVarChar){Value = (v.Name)},
                        new SqlParameter("@Phone",SqlDbType.NVarChar){Value = (v.Phone)},
                        new SqlParameter("@CheckStatus",SqlDbType.NVarChar){Value = (v.CheckStatus)}
                    };
                    sql += " select * from XXCLOUDVisitor.dbo.Table_Visitors V";
                    sql += " left join XXCLOUD.dbo.T_StaffInf S";
                    sql += " on V.SNo = S.SNo";
                    sql += " where Name = @Name and Phone = @Phone and CheckStatus=@CheckStatus";
                }
                else {
                    pms = new SqlParameter[]{
                        new SqlParameter("@CheckStatus",SqlDbType.NVarChar){Value = (v.CheckStatus)}
                    };
                    sql += " select * from XXCLOUDVisitor.dbo.Table_Visitors V";
                    sql += " left join XXCLOUD.dbo.T_StaffInf S";
                    sql += " on V.SNo = S.SNo";
                    sql += " where CheckStatus=@CheckStatus";
                }
            }

            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, CommandType.Text, pms);
                return "{\"code\":1,\"count\":" + dt.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt) + "}";
            }
            catch (Exception e)
            {
                //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString()),
                    ReasonPhrase = "error"
                };
                throw new HttpResponseException(resp);
            }
        }
        [HttpPost, Route("getListByNameOrPhone4Checked")]
        /// <summary>
        /// 
        /// </summary>
        public string GetListByNameOrPhone4Checked(VisitorsSearchModel v)
        {
            string sql = "";
            SqlParameter[] pms = null;
            string timeStart = v.StartDate;
            string timeEnd = v.EndDate;
            if (v.Type == "check")
            {
                if (!string.IsNullOrEmpty(v.Name) && string.IsNullOrEmpty(v.Phone))
                {
                    pms = new SqlParameter[]{
                        new SqlParameter("@timeStart",SqlDbType.NVarChar){Value = (timeStart)},
                        new SqlParameter("@timeEnd",SqlDbType.NVarChar){Value = (timeEnd)},
                        new SqlParameter("@Name",SqlDbType.NVarChar){Value = (v.Name)},
                        new SqlParameter("@CheckStatus",SqlDbType.NVarChar){Value = (v.CheckStatus)}
                    };
                    sql += " select * from XXCLOUDVisitor.dbo.Table_Visitors V";
                    sql += " left join XXCLOUD.dbo.T_StaffInf S";
                    sql += " on V.SNo = S.SNo";
                    sql += " where Name = @Name and CheckStatus in ('1','-1') and CheckDate between @timeStart and @timeEnd";
                }
                else if (string.IsNullOrEmpty(v.Name) && !string.IsNullOrEmpty(v.Phone))
                {
                    pms = new SqlParameter[]{
                        new SqlParameter("@timeStart",SqlDbType.NVarChar){Value = (timeStart)},
                        new SqlParameter("@timeEnd",SqlDbType.NVarChar){Value = (timeEnd)},
                        new SqlParameter("@Phone",SqlDbType.NVarChar){Value = (v.Phone)},
                        new SqlParameter("@CheckStatus",SqlDbType.NVarChar){Value = (v.CheckStatus)}
                    };
                    sql += " select * from XXCLOUDVisitor.dbo.Table_Visitors V";
                    sql += " left join XXCLOUD.dbo.T_StaffInf S";
                    sql += " on V.SNo = S.SNo";
                    sql += " where Phone = @Phone and CheckStatus in ('1','-1') and CheckDate between @timeStart and @timeEnd";
                }
                else if (!string.IsNullOrEmpty(v.Name) && !string.IsNullOrEmpty(v.Phone))
                {
                    pms = new SqlParameter[]{
                        new SqlParameter("@timeStart",SqlDbType.NVarChar){Value = (timeStart)},
                        new SqlParameter("@timeEnd",SqlDbType.NVarChar){Value = (timeEnd)},
                        new SqlParameter("@Name",SqlDbType.NVarChar){Value = (v.Name)},
                        new SqlParameter("@Phone",SqlDbType.NVarChar){Value = (v.Phone)},
                        new SqlParameter("@CheckStatus",SqlDbType.NVarChar){Value = (v.CheckStatus)}
                    };
                    sql += " select * from XXCLOUDVisitor.dbo.Table_Visitors V";
                    sql += " left join XXCLOUD.dbo.T_StaffInf S";
                    sql += " on V.SNo = S.SNo";
                    sql += " where Name = @Name and Phone = @Phone and CheckStatus in ('1','-1') and CheckDate between @timeStart and @timeEnd";
                }
                else
                {
                    pms = new SqlParameter[]{
                        new SqlParameter("@timeStart",SqlDbType.NVarChar){Value = (timeStart)},
                        new SqlParameter("@timeEnd",SqlDbType.NVarChar){Value = (timeEnd)},
                        new SqlParameter("@CheckStatus",SqlDbType.NVarChar){Value = (v.CheckStatus)}
                    };
                    sql += " select * from XXCLOUDVisitor.dbo.Table_Visitors V";
                    sql += " left join XXCLOUD.dbo.T_StaffInf S";
                    sql += " on V.SNo = S.SNo";
                    sql += " where CheckStatus in ('1','-1') and CheckDate between @timeStart and @timeEnd";
                }
            }

            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, CommandType.Text, pms);
                return "{\"code\":1,\"count\":" + dt.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt) + "}";
            }
            catch (Exception e)
            {
                //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString()),
                    ReasonPhrase = "error"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpPost, Route("getListByTime")]
        /// <summary>
        /// 
        /// </summary>
        public string GetListByTime(VisitorsSearchModel v)
        {
            //string Flag = "Phone";
            //if (number.Length == 11)
            //{
            //    Flag = "Phone";
            //}
            //else {
            //    Flag = "SActualNo";
            //}
            string sql = "";
            SqlParameter[] pms = null;
            string timeStart = v.StartDate + " 00:00:01";
            string timeEnd = v.EndDate + " 23:59:59";

            if (v.Type == "in")
            {
                pms = new SqlParameter[]{
                    new SqlParameter("@timeStart",SqlDbType.NVarChar){Value = (timeStart)},
                    new SqlParameter("@timeEnd",SqlDbType.NVarChar){Value = (timeEnd)},
                    new SqlParameter("@SMPhone",SqlDbType.NVarChar){Value = (v.Phone)}
                };
                sql += " select * from XXCLOUDVisitor.dbo.Table_Visitors V";
                sql += " left join XXCLOUD.dbo.T_StaffInf S";
                sql += " on V.SNo = S.SNo";
                sql += " where CreateTime between @timeStart and @timeEnd and SMPhone = @Phone";
            }
            else if (v.Type == "out")
            {
                pms = new SqlParameter[]{
                    new SqlParameter("@timeStart",SqlDbType.NVarChar){Value = (timeStart)},
                    new SqlParameter("@timeEnd",SqlDbType.NVarChar){Value = (timeEnd)},
                    new SqlParameter("@Phone",SqlDbType.NVarChar){Value = (v.Phone)}
                };
                sql += " select * from XXCLOUDVisitor.dbo.Table_Visitors V";
                sql += " left join XXCLOUD.dbo.T_StaffInf S";
                sql += " on V.SNo = S.SNo";
                sql += " where CreateTime between @timeStart and @timeEnd and Phone = @Phone";
            }

            //pms = new SqlParameter[]{
            //    new SqlParameter("@timeStart",SqlDbType.NVarChar){Value = (timeStart)},
            //    new SqlParameter("@timeEnd",SqlDbType.NVarChar){Value = (timeEnd)}
            //};
            
            //sql += " select * from XXCLOUDVisitor.dbo.Table_Visitors V";
            //sql += " left join XXCLOUD.dbo.T_StaffInf S";
            //sql += " on V.SNo = S.SNo";
            //sql += " where CreateTime between @timeStart and @timeEnd";
            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, CommandType.Text, pms);
                return "{\"code\":1,\"count\":" + dt.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt) + "}";
            }
            catch (Exception e)
            {
                //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString()),
                    ReasonPhrase = "error"
                };
                 throw new HttpResponseException(resp);
            }
        }
        [HttpPost, Route("getListByTimeAndOpenId4In")]
        /// <summary>
        /// flag (time:按照时间查询 top:按照次数查询)
        /// </summary>
        public string GetListByTimeAndOpenId4In(VisitorsSearchModel v)
        {
            string sql = "";
            SqlParameter[] pms = null;
            string timeStart = v.StartDate + " 00:00:01";
            string timeEnd = v.EndDate + " 23:59:59";
            pms = new SqlParameter[]{
                new SqlParameter("@timeStart",SqlDbType.NVarChar){Value = (timeStart)},
                new SqlParameter("@timeEnd",SqlDbType.NVarChar){Value = (timeEnd)},
                new SqlParameter("@OpenId4In",SqlDbType.NVarChar){Value = (v.OpenId4In)}
            };
            sql += " select * from XXCLOUDVisitor.dbo.Table_Visitors V";
            sql += " left join XXCLOUD.dbo.T_StaffInf S";
            sql += " on V.SNo = S.SNo";
            sql += " where CreateTime between @timeStart and @timeEnd and V.OpenId4In = @OpenId4In";


            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, CommandType.Text, pms);
                return "{\"code\":1,\"count\":" + dt.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt) + "}";
            }
            catch (Exception e)
            {
                //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString()),
                    ReasonPhrase = "error"
                };
                throw new HttpResponseException(resp);
            }
        }
        [HttpPost, Route("getListByTimeAndOpenId4Out")]
        /// <summary>
        /// flag (time:按照时间查询 top:按照次数查询)
        /// </summary>
        public string GetListByTimeAndOpenId4Out(VisitorsSearchModel v)
        {
            string sql = "";
            SqlParameter[] pms = null;
            string timeStart = v.StartDate + " 00:00:01";
            string timeEnd = v.EndDate + " 23:59:59";
            pms = new SqlParameter[]{
                new SqlParameter("@timeStart",SqlDbType.NVarChar){Value = (timeStart)},
                new SqlParameter("@timeEnd",SqlDbType.NVarChar){Value = (timeEnd)},
                new SqlParameter("@OpenId4Out",SqlDbType.NVarChar){Value = (v.OpenId4Out)}
            };
            sql += " select * from XXCLOUDVisitor.dbo.Table_Visitors V";
            sql += " left join XXCLOUD.dbo.T_StaffInf S";
            sql += " on V.SNo = S.SNo";
            sql += " where CreateTime between @timeStart and @timeEnd and V.OpenId4Out = @OpenId4Out";


            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, CommandType.Text, pms);
                return "{\"code\":1,\"count\":" + dt.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt) + "}";
            }
            catch (Exception e)
            {
                //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString()),
                    ReasonPhrase = "error"
                };
                throw new HttpResponseException(resp);
            }
        }
        [HttpPost, Route("getListByTimeAndOpenId")]
        /// <summary>
        /// flag (time:按照时间查询 top:按照次数查询)
        /// </summary>
        public string GetListByTimeAndOpenId(VisitorsSearchModel v)
        {
            string sql = "";
            SqlParameter[] pms = null;
            string timeStart = v.StartDate + " 00:00:01";
            string timeEnd = v.EndDate + " 23:59:59";
            pms = new SqlParameter[]{
                new SqlParameter("@timeStart",SqlDbType.NVarChar){Value = (timeStart)},
                new SqlParameter("@timeEnd",SqlDbType.NVarChar){Value = (timeEnd)},
                new SqlParameter("@OpenId",SqlDbType.NVarChar){Value = (v.OpenId)}
            };
            sql += " select * from XXCLOUDVisitor.dbo.Table_Visitors V";
            sql += " left join XXCLOUD.dbo.T_StaffInf S";
            sql += " on V.SNo = S.SNo";
            sql += " where CreateTime between @timeStart and @timeEnd and V.OpenId = @OpenId";
            

            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, CommandType.Text, pms);
                return "{\"code\":1,\"count\":" + dt.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt) + "}";
            }
            catch (Exception e)
            {
                //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString()),
                    ReasonPhrase = "error"
                };
                throw new HttpResponseException(resp);
            }
        }
        [HttpPost, Route("getListByTimeAndPhone")]
        /// <summary>
        /// flag (time:按照时间查询 top:按照次数查询)
        /// </summary>
        public string getListByTimeAndPhone(VisitorsSearchModel v)
        {
            //string Flag = "Phone";
            //if (number.Length == 11)
            //{
            //    Flag = "Phone";
            //}
            //else {
            //    Flag = "SActualNo";
            //}
            string sql = "";
            SqlParameter[] pms = null;
            string timeStart = v.StartDate + " 00:00:01";
            string timeEnd = v.EndDate + " 23:59:59";
            if (v.Type == "in")
            {
                pms = new SqlParameter[]{
                    new SqlParameter("@timeStart",SqlDbType.NVarChar){Value = (timeStart)},
                    new SqlParameter("@timeEnd",SqlDbType.NVarChar){Value = (timeEnd)},
                    new SqlParameter("@SMPhone",SqlDbType.NVarChar){Value = (v.Phone)}
                };
                sql += " select * from XXCLOUDVisitor.dbo.Table_Visitors V";
                sql += " left join XXCLOUD.dbo.T_StaffInf S";
                sql += " on V.SNo = S.SNo";
                sql += " where CreateTime between @timeStart and @timeEnd and SMPhone = @SMPhone";
            }
            else if(v.Type=="out"){
                pms = new SqlParameter[]{
                    new SqlParameter("@timeStart",SqlDbType.NVarChar){Value = (timeStart)},
                    new SqlParameter("@timeEnd",SqlDbType.NVarChar){Value = (timeEnd)},
                    new SqlParameter("@Phone",SqlDbType.NVarChar){Value = (v.Phone)}
                };
                sql += " select * from XXCLOUDVisitor.dbo.Table_Visitors V";
                sql += " left join XXCLOUD.dbo.T_StaffInf S";
                sql += " on V.SNo = S.SNo";
                sql += " where CreateTime between @timeStart and @timeEnd and Phone = @Phone";
            }
            

            //new SqlParameter("@Phone",SqlDbType.NVarChar){Value = (v.Phone)},
            //string sql_str = 
            //sql = "select * from XXCLOUDVisitor.dbo.Table_Visitors where CreateTime between @timeStart and @timeEnd and Phone = @Phone";
            
            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, CommandType.Text, pms);
                return "{\"code\":1,\"count\":" + dt.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt) + "}";
            }
            catch (Exception e)
            {
                //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString()),
                    ReasonPhrase = "error"
                };
                throw new HttpResponseException(resp);
            }
        }
        [HttpPost, Route("add4Out")]
        public string Add4Out(Visitors v)
        {
            string wramStr = "";
            if (string.IsNullOrEmpty(v.Name))
            {
                wramStr = "访客姓名不能为空";
                return ConvertHelper.resultJson(0, wramStr);
            }
            if (string.IsNullOrEmpty(v.Phone))
            {
                wramStr = "联系电话不能为空";
                return ConvertHelper.resultJson(0, wramStr);
            }
            if (string.IsNullOrEmpty(v.IdentityNumber))
            {
                wramStr = "证件号不能为空";
                return ConvertHelper.resultJson(0, wramStr);
            }
            string sql = "insert into XXCLOUDVisitor.dbo.Table_Visitors(Name, Sex, Phone, IdentityNumber, Reason, Number, PlateNumber, Unit, Date, StartTime, EndTime, Remark, Type, CreateTime, SNo, OpenId4Out, OpenId4In)" +
                "values(@Name, @Sex, @Phone, @IdentityNumber, @Reason, @Number, @PlateNumber, @Unit, @Date, @StartTime,@EndTime, @Remark, @Type, @CreateTime, @SNo, @OpenId4Out, @OpenId4In)";
                DateTime dt = DateTime.Now;
                SqlParameter[] pms = new SqlParameter[]{
                    new SqlParameter("@Name",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Name)},
                    new SqlParameter("@Sex",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Sex)},
                    new SqlParameter("@Phone",SqlDbType.NVarChar){Value= DataHelper.IsNullReturnLine(v.Phone)},
                    new SqlParameter("@IdentityNumber",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.IdentityNumber)},
                    new SqlParameter("@Reason",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Reason)},
                    new SqlParameter("@Number",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Number)},
                    new SqlParameter("@PlateNumber",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.PlateNumber)},
                    new SqlParameter("@Unit",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Unit)},
                    new SqlParameter("@Date",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Date)},
                    new SqlParameter("@StartTime",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.StartTime)},
                    new SqlParameter("@EndTime",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.EndTime)},
                    new SqlParameter("@Remark",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Remark)},
                    new SqlParameter("@Type",SqlDbType.NVarChar){Value="out"},
                    new SqlParameter("@CreateTime",SqlDbType.NVarChar){Value=dt.ToString("yyyy-MM-dd hh:mm:ss")},
                    new SqlParameter("@SNo",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.SNo)},
                    new SqlParameter("@OpenId4Out",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.OpenId4Out)},
                    new SqlParameter("@OpenId4In",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.OpenId4In)}
                };
                try
                {
                    int result = SQLHelper.ExecuteNonQuery(sql, System.Data.CommandType.Text, pms);
                    return ConvertHelper.IntToJson(result);
                }
                catch (Exception e)
                {
                    //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                    var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(e.ToString()),
                        ReasonPhrase = "error"
                    };
                    throw new HttpResponseException(resp);
                }
        }
        // 待完善，先查找，有就select，无就插入并提示要主要通知对方
        [HttpPost, Route("add4In")]
        public string Add4In(Visitors v)
        {
            string wramStr = "";
            if (string.IsNullOrEmpty(v.Name))
            {
                wramStr = "访客姓名不能为空";
                return ConvertHelper.resultJson(0, wramStr);
            }
            if (string.IsNullOrEmpty(v.Phone))
            {
                wramStr = "联系电话不能为空";
                return ConvertHelper.resultJson(0, wramStr);
            }
            int RandKey = 1000;
            bool is_ec_ok = false;
            while (!is_ec_ok)
            {
                Random ran = new Random();
                RandKey = ran.Next(1000, 9999);

                string sqlIsExistEC = "select count(*) from XXCLOUDVisitor.dbo.Table_Visitors where EnterCode=@EnterCode";
                SqlParameter[] pms4EC = new SqlParameter[]{
                        new SqlParameter("@EnterCode",SqlDbType.NVarChar){Value = RandKey.ToString()}
                    };
                object obj = SQLHelper.ExecuteScalar(sqlIsExistEC, System.Data.CommandType.Text, pms4EC);
                if (Convert.ToInt32(obj) == 0)
                { //说明此EnterCode可以使用
                    is_ec_ok = true;
                }
            }
            //查找数据库中存在此访客
            string sql2 = "select * from XXCLOUDVisitor.dbo.Table_WXUserInfo where Phone=@Phone";
            SqlParameter[] pms2 = new SqlParameter[]{
                new SqlParameter("@Phone",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Phone)}
            };
            DataTable dt2 = SQLHelper.ExecuteDataTable(sql2, System.Data.CommandType.Text, pms2);
            string OpenId4Out;
            string sql;
            SqlParameter[] pms;
            if (dt2.Rows.Count == 1)
            {
                OpenId4Out = dt2.Rows[0]["OpenId"].ToString();
                sql = "insert into XXCLOUDVisitor.dbo.Table_Visitors(Name, Sex, Phone, IdentityNumber, Reason, Number, PlateNumber, Unit, Date, StartTime, EndTime, Remark, Type, CreateTime, Checker, CheckDate, CheckStatus,SNo,OpenId4Out,OpenId4In,EnterCode)" +
                    "values(@Name, @Sex, @Phone, @IdentityNumber, @Reason, @Number, @PlateNumber, @Unit, @Date, @StartTime,@EndTime, @Remark, @Type, @CreateTime, @Checker, @CheckDate, @CheckStatus, @SNo, @OpenId4Out, @OpenId4In, @EnterCode)";
                DateTime dt = DateTime.Now;
                pms = new SqlParameter[]{
                        new SqlParameter("@Name",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Name)},
                        new SqlParameter("@Sex",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Sex)},
                        new SqlParameter("@Phone",SqlDbType.NVarChar){Value= DataHelper.IsNullReturnLine(v.Phone)},
                        new SqlParameter("@IdentityNumber",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.IdentityNumber)},
                        new SqlParameter("@Reason",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Reason)},
                        new SqlParameter("@Number",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Number)},
                        new SqlParameter("@PlateNumber",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.PlateNumber)},
                        new SqlParameter("@Unit",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Unit)},
                        new SqlParameter("@Date",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Date)},
                        new SqlParameter("@StartTime",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.StartTime)},
                        new SqlParameter("@EndTime",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.EndTime)},
                        new SqlParameter("@Remark",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Remark)},
                        new SqlParameter("@Type",SqlDbType.NVarChar){Value="in"},
                        new SqlParameter("@CreateTime",SqlDbType.NVarChar){Value=dt.ToString("yyyy-MM-dd hh:mm:ss")},
                        new SqlParameter("@Checker",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Checker)},
                        new SqlParameter("@CheckDate",SqlDbType.NVarChar){Value=dt.ToString("yyyy-MM-dd")},
                        new SqlParameter("@CheckStatus",SqlDbType.NVarChar){Value=1},
                        new SqlParameter("@SNo",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.SNo)},
                        new SqlParameter("@OpenId4Out",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(OpenId4Out)},
                        new SqlParameter("@OpenId4In",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.OpenId4In)},
                        new SqlParameter("@EnterCode",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(RandKey.ToString())}
                    };
                try
                {
                    int result = SQLHelper.ExecuteNonQuery(sql, System.Data.CommandType.Text, pms);
                    if (result == 1)
                    {
                        return "{\"code\":\"1\",\"openid\":\"" + OpenId4Out + "\",\"ordercode\":\"" + RandKey.ToString() + "\"}";
                    }
                    else {
                        return ConvertHelper.resultJson(0, "系统出错了");
                    }
                    
                }
                catch (Exception e)
                {
                    //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                    var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(e.ToString()),
                        ReasonPhrase = "error"
                    };
                    throw new HttpResponseException(resp);
                }
            }
            else {
                sql = "insert into XXCLOUDVisitor.dbo.Table_Visitors(Name, Sex, Phone, IdentityNumber, Reason, Number, PlateNumber, Unit, Date, StartTime, EndTime, Remark, Type, CreateTime, Checker, CheckDate, CheckStatus,SNo,OpenId4In,EnterCode)" +
                    " values(@Name, @Sex, @Phone, @IdentityNumber, @Reason, @Number, @PlateNumber, @Unit, @Date, @StartTime,@EndTime, @Remark, @Type, @CreateTime, @Checker, @CheckDate, @CheckStatus, @SNo,@OpenId4In,@EnterCode)";
                DateTime dt = DateTime.Now;
                pms = new SqlParameter[]{
                        new SqlParameter("@Name",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Name)},
                        new SqlParameter("@Sex",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Sex)},
                        new SqlParameter("@Phone",SqlDbType.NVarChar){Value= DataHelper.IsNullReturnLine(v.Phone)},
                        new SqlParameter("@IdentityNumber",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.IdentityNumber)},
                        new SqlParameter("@Reason",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Reason)},
                        new SqlParameter("@Number",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Number)},
                        new SqlParameter("@PlateNumber",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.PlateNumber)},
                        new SqlParameter("@Unit",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Unit)},
                        new SqlParameter("@Date",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Date)},
                        new SqlParameter("@StartTime",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.StartTime)},
                        new SqlParameter("@EndTime",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.EndTime)},
                        new SqlParameter("@Remark",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Remark)},
                        new SqlParameter("@Type",SqlDbType.NVarChar){Value="in"},
                        new SqlParameter("@CreateTime",SqlDbType.NVarChar){Value=dt.ToString("yyyy-MM-dd hh:mm:ss")},
                        new SqlParameter("@Checker",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Checker)},
                        new SqlParameter("@CheckDate",SqlDbType.NVarChar){Value=dt.ToString("yyyy-MM-dd")},
                        new SqlParameter("@CheckStatus",SqlDbType.NVarChar){Value=1},
                        new SqlParameter("@SNo",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.SNo)},
                        new SqlParameter("@OpenId4In",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.OpenId4In)},
                        new SqlParameter("@EnterCode",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(RandKey.ToString())}
                    };
                try
                {
                    int result = SQLHelper.ExecuteNonQuery(sql, System.Data.CommandType.Text, pms);
                    if (result == 1)
                    {
                        string msg = "此访客尚未在此系统开通微信服务消息收发权限，请主动联系对方并把此次预约码（" + RandKey.ToString() + "）告诉对方";
                        return "{\"code\":\"101\",\"msg\":\"" + msg + "\",\"ordercode\":\"" + RandKey.ToString() + "\"}";
                    }
                    else {
                        return ConvertHelper.resultJson(0, "系统出错了");    
                    }
                    
                    //上次改到这里
                }
                catch (Exception e)
                {
                    //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                    var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(e.ToString()),
                        ReasonPhrase = "error"
                    };
                    throw new HttpResponseException(resp);
                }
            }
        }
    }
}
