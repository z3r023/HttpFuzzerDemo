﻿using LoveCoody.entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LoveCoody.handle
{
    class ExpHandle
    {
      static  Dictionary<String, String> expBodyDics = new Dictionary<string, string>();
        private static String getExpUnHexBody(ExpModule exp)
        {
            String key = exp.Language + "_" + exp.Name;
            if (expBodyDics.ContainsKey(key)) {
                return expBodyDics[key];
            }
           String body = Encoding.UTF8.GetString(hexStringToByte(exp.ExpContext.Body));
           try {
               expBodyDics.Add(key, body);
           }
           catch { }
           return body;
        }

        public static ExpVerificationResult Verification(String url, ExpModule exp)
        {
            String method = exp.ExpContext.Method;
            String body = exp.ExpContext.Body;
            if (exp.ExpContext.IsHex)
            {
                body = getExpUnHexBody(exp);
            }
            LoveCoody.handle.HttpHandle.HttpResult httpEntity = HttpHandle.BaseConn(url, method, body, exp.ExpContext.Encode, exp.ExpContext.Header);
            ExpVerificationResult result = new ExpVerificationResult();
            if (httpEntity == null)
            {
                result.Code = 2;
                result.Result = "连接失败";
                return result;
            }
            result.Html = httpEntity.Header + "\r\n" + httpEntity.Body;
            if (exp.Verification.Type == 0)
            {
                if (httpEntity.Code != Convert.ToInt32(exp.Verification.Context))
                {
                    result.Code = 0;
                    result.Result = "不存在" + exp.Name;
                    return result;
                }
                result.Code = 1;
                result.Result = "存在" + exp.Name;
                result.ExpName = exp.Name;
                return result;
            }
            if (exp.Verification.Type == 1)
            {
                if (exp.Verification.Calc.Equals("等于"))
                {
                    String resultBody = httpEntity.Body;
                    String value = exp.Verification.Context.Trim();
                    if (!resultBody.ToLower().Equals(value.ToLower()))
                    {
                        result.Code = 0;
                        result.Result = "不存在" + exp.Name;
                        return result;
                    }
                    result.Code = 1;
                    result.Result = resultBody;
                    result.ExpName = exp.Name;
                    return result;
                }
                if (exp.Verification.Calc.Equals("包含"))
                {

                    if (!result.Html.ToLower().Contains(exp.Verification.Context.ToLower().Trim()))
                    {
                        result.Code = 0;
                        result.Result = "不存在" + exp.Name;
                        return result;
                    }
                    result.Code = 1;
                    result.Result = exp.Verification.Context;
                    result.ExpName = exp.Name;
                    return result;
                }
                if (exp.Verification.Calc.Equals("匹配"))
                {
                    List<String> list = matchExport(result.Html, new Regex(exp.Verification.Context), new Uri(url).Host);
                    if (list == null || list.Count == 0)
                    {
                        result.Code = 0;
                        result.Result = "不存在" + exp.Name;
                        return result;
                    }
                    result.Code = 1;
                    result.Result = JsonHandle.toJson(list);
                    result.ExpName = exp.Name;
                    return result;
                }

            }
            if (exp.Verification.Type == 2)
            {
                String newUrl = urlFormat(url);
                newUrl = newUrl + exp.Verification.Context;
                LoveCoody.handle.HttpHandle.HttpResult entity = HttpHandle.Get(newUrl, exp.ExpContext.Encode, exp.ExpContext.Header);
                if (entity.Code == 200)
                {
                    result.Code = 0;
                    result.Result = "不存在" + exp.Name;
                    return result;
                }
                result.Code = 1;
                result.Result = JsonHandle.toJson(exp.Verification.Context);
                result.ExpName = exp.Name;
                return result;
            }
            ExpVerificationResult results = new ExpVerificationResult();
            results.Code = 2;
            results.Result = "模块配置有误";
            return results;
        }
        public static bool IsHexadecimal(string str)
        {
            if (str.Contains("%")) {
                return false;
            }
            const string PATTERN = @"[A-Fa-f0-9]+$";
            return System.Text.RegularExpressions.Regex.IsMatch(str, PATTERN);
        }
        public static byte[] hexStringToByte(String hex)
        {
            if (String.IsNullOrEmpty(hex))
            {
                return null;
            }
            hex = hex.ToUpper();
            int length = hex.Length / 2;
            char[] hexChars = hex.ToCharArray();
            byte[] d = new byte[length];
            for (int i = 0; i < length; i++)
            {
                int pos = i * 2;
                d[i] = (byte)(charToByte(hexChars[pos]) << 4 | charToByte(hexChars[pos + 1]));

            }
            return d;
        }
        public static string urlFormat(String url)
        {
            Uri uri = new Uri(url);
            url = uri.Scheme + "://" + uri.Host;
            if (uri.Port != -1 && uri.Port != 80 && uri.Port != 443)
            {
                url += (":") + uri.Port;
            }
            return url;
        }
        private static byte charToByte(char c)
        {
            return (byte)"0123456789ABCDEF".IndexOf(c);
        }
        public static List<String> matchExport(String context, Regex reg,String url)
        {
            try
            {
                MatchCollection result = reg.Matches(context);
                List<String> results = new List<string>();
                int num = 0;
                foreach (Match m in result)
                {
                    if (num > 10000)
                    {
                        StreamWriter sw = File.CreateText(System.Diagnostics.Process.GetCurrentProcess().ProcessName + "/" + url);
                        sw.WriteLine("readLine");
                        sw.Close();
                        break;
                    }
                    num++;
                    if (String.IsNullOrEmpty(m.Value))
                    {
                        continue;
                    }
                    results.Add(m.Value);
                }
                return results;
            }
            catch
            {
                return null;
            }
        }
    }
}
