﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LoveCoody.entity
{
   public class ExpModule
    {

        private bool formatUrl = false;

        public bool FormatUrl
        {
            get { return formatUrl; }
            set { formatUrl = value; }
        }

        private String name;

        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        private String language;

        public String Language
        {
            get { return language; }
            set { language = value; }
        }

        private ExpVerification verification;

        public ExpVerification Verification
        {
            get { return verification; }
            set { verification = value; }
        }
        /// <summary>
        /// 0禁用  1可用
        /// </summary>
        private Int32 status = 1;

        public Int32 Status
        {
            get { return status; }
            set { status = value; }
        }

        private HttpModule expContext;

        public HttpModule ExpContext
        {
            get { return expContext; }
            set { expContext = value; }
        }



    }
}
