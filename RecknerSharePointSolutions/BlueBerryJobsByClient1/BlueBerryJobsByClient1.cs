﻿using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace RecknerSharePointSolutions.BlueBerryJobsByClient1
{
    [ToolboxItemAttribute(false)]
    public class BlueBerryJobsByClient1 : WebPart
    {

        private string _siteUrl = "";
        private string _jobSiteUrl = "";
         private int _jobYear;



        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Settings")]
        [DefaultValue(@"http://dev-home.reckner.com/BlueBerry/ ")]
        public String SiteUrl { get { return _siteUrl; } set { _siteUrl = value; } }



        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Settings")]
        [DefaultValue(@"http://dev-work.reckner.com/jobs/")]
        public String JobSiteUrl { get { return _jobSiteUrl; } set { _jobSiteUrl = value; } }


        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Settings")]
        [DefaultValue("2012")]
        public int JobYear { get { return _jobYear; } set { _jobYear = value; } }

 


        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/RecknerSharePointSolutions/BlueBerryJobsByClient1/BlueBerryJobsByClient1UserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }
    }
}
