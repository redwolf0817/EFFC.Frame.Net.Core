using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;

namespace EFFC.Frame.Net.WebControlLib
{
    /// <summary>
    /// Summary description for DatePicker.
    /// </summary>
    public class DatePicker : Control, INamingContainer
    {
        #region Private Properties
        private string m_closeGif = "cancel.gif";
        private string m_dividerGif = "divider.gif";
        private string m_drop1Gif = "down.gif";
        private string m_drop2Gif = "down.gif";
        private string m_left1Gif = "left.gif";
        private string m_left2Gif = "left.gif";
        private string m_right1Gif = "right.gif";
        private string m_right2Gif = "right.gif";
        private string m_imgDirectory = "";
        private string m_ControlCssClass = "";
        private string m_text = "";
        private string m_Css = "";
        private string m_DateType = "yyyy/mm/dd";
        private string btnStyle = "<input type='button' style='background-color: #F0F0F0;border: 0px ;font-size: 9px;color: #000000;height: 13px;width: auto;font:caption;' value='<<' onclick='javascript:decMonth()' />";
        

        private bool enable = true;

        private int maxlength = 10;

        private bool isRequired = false;
        private string validateName = string.Empty;
        /// <summary>
        /// Set or get enable of control
        /// </summary>
        public bool Enable
        {
            get
            {
                return this.enable;
            }
            set
            {
                this.enable = value;
            }
        }


        /// <summary>
        /// Set focus of control's textbox
        /// </summary>
        public override void Focus()
        {
            base.Focus();
            if (this.FindControl("foo") != null)
            {
                this.FindControl("foo").Focus();
            }
        }
        private int width = 150;

        /// <summary>
        /// Set or get width of control's textbox
        /// </summary>
        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }

        /// <summary>
        /// Set or get length of textbox's max 
        /// </summary>
        public int MaxLength
        {
            get
            {
                return this.maxlength;
            }
            set
            {
                this.maxlength = value;
            }
        }
        //////////////////////////////////////


        #endregion

        #region Public Properties
        /// <summary>
        /// Text
        /// </summary>
        public string Text
        {
            get
            {
                if (this.Controls.Count == 0)
                    return "";
                for (int i = 0; i < this.Controls.Count; i++)
                {
                    if (Controls[i] is System.Web.UI.WebControls.TextBox)
                    {
                        return ((System.Web.UI.WebControls.TextBox)Controls[i]).Text;
                    }
                }
                return "";
            }
            set
            {
                    m_text = value;
                    for (int i = 0; i < this.Controls.Count; i++)
                    {
                        if (Controls[i] is System.Web.UI.WebControls.TextBox)
                        {
                            ((System.Web.UI.WebControls.TextBox)Controls[i]).Text = m_text;
                        }
                    }
            }
        }
        /// <summary>
        /// CSS Class
        /// </summary>
        public string CssClass
        {
            get
            {
                if (this.Controls.Count == 0)
                    return "";
                for (int i = 0; i < this.Controls.Count; i++)
                {
                    if (Controls[i] is System.Web.UI.WebControls.TextBox)
                    {
                        return ((System.Web.UI.WebControls.TextBox)Controls[i]).CssClass;
                    }
                }
                return "";
            }
            set
            {
                if (this.Controls.Count != 0)
                {
                    for (int i = 0; i < this.Controls.Count; i++)
                    {
                        if (Controls[i] is System.Web.UI.WebControls.TextBox)
                        {
                            ((System.Web.UI.WebControls.TextBox)Controls[i]).CssClass = value;
                            break;
                        }
                    }
                    m_Css = "";
                }
                else
                    m_Css = value;
            }
        }
        /// <summary>
        /// imgClose
        /// </summary>
        public string imgClose
        {
            get { return m_closeGif; }
            set { m_closeGif = value; }
        }
        /// <summary>
        /// imgDivider
        /// </summary>
        public string imgDivider
        {
            get { return m_dividerGif; }
            set { m_dividerGif = value; }
        }
        /// <summary>
        /// imgDrop1
        /// </summary>
        public string imgDrop1
        {
            get { return m_drop1Gif; }
            set { m_drop1Gif = value; }
        }
        /// <summary>
        /// imgDrop2
        /// </summary>
        public string imgDrop2
        {
            get { return m_drop2Gif; }
            set { m_drop2Gif = value; }
        }
        /// <summary>
        /// imgLeft1
        /// </summary>
        public string imgLeft1
        {
            get { return m_left1Gif; }
            set { m_left1Gif = value; }
        }
        /// <summary>
        /// imgLeft2
        /// </summary>
        public string imgLeft2
        {
            get { return m_left2Gif; }
            set { m_left2Gif = value; }
        }
        /// <summary>
        /// imgRight1
        /// </summary>
        public string imgRight1
        {
            get { return m_right1Gif; }
            set { m_right1Gif = value; }
        }
        /// <summary>
        /// imgDirectory
        /// </summary>
        public string imgDirectory
        {
            get { return m_imgDirectory; }
            set { m_imgDirectory = value; }
        }
        /// <summary>
        /// ControlCssClass
        /// </summary>
        public string ControlCssClass
        {
            get { return m_ControlCssClass; }
            set { m_ControlCssClass = value; }
        }
        /// <summary>
        /// DateType
        /// </summary>
        public string DateType
        {
            get { return m_DateType; }
            set { m_DateType = value; }
        }

        /// <summary>
        /// Is required
        /// </summary>
        public bool IsRequired
        {
            get
            {
                return isRequired;
            }
            set
            {
                isRequired = value;
            }
        }

        /// <summary>
        /// Validate Name
        /// </summary>
        public string ValidateName
        {

            get
            {
                return validateName;
            }
            set
            {
                validateName = value;
            }

        }

        #endregion

        #region Constructors
        /// <summary>
        /// Date picker
        /// </summary>
        public DatePicker()
        {
            if (System.Web.HttpRuntime.AppDomainAppVirtualPath == "" ||
             System.Web.HttpRuntime.AppDomainAppVirtualPath == string.Empty ||
             System.Web.HttpRuntime.AppDomainAppVirtualPath == null)
            {
                this.m_imgDirectory = "/images/";
            }
            else if (System.Web.HttpRuntime.AppDomainAppVirtualPath == "/")
            {
                this.m_imgDirectory = System.Web.HttpRuntime.AppDomainAppVirtualPath + "images/";
            }
            else
            {
                this.m_imgDirectory = System.Web.HttpRuntime.AppDomainAppVirtualPath + "/images/";
            }
        }
        #endregion

        #region Private Methods/Properties
        /// <summary>
        /// javascript
        /// </summary>
        private void placeJavascript()
        {
            string strBuildUp = "<script language=JavaScript>";
            strBuildUp += "var fixedX = -1   // x position (-1 if to appear below control)\n";
            strBuildUp += "var fixedY = -1   // y position (-1 if to appear below control)\n";
            strBuildUp += "var startAt = 1   // 0 - sunday ; 1 - monday\n";
            strBuildUp += "var showWeekNumber = 1 // 0 - don't show; 1 - show\n";
            strBuildUp += "var showToday = 1  // 0 - don't show; 1 - show\n";
            strBuildUp += @"var imgDir = """ + m_imgDirectory + @"""" + "\n";
            strBuildUp += @"var gotoString = ""getMonth""" + "\n";
            strBuildUp += @"var todayString = ""Today """ + "\n";
            strBuildUp += @"var weekString = ""Week""" + "\n";
            strBuildUp += @"var scrollLeftMessage = ""Select last month，Press on will be up.""" + "\n";
            strBuildUp += @"var scrollRightMessage = "" Select next month,Press on will be down""" + "\n";
            strBuildUp += @"var selectMonthMessage = ""Select month.""" + "\n";
            strBuildUp += @"var selectYearMessage = ""Select year.""" + "\n";
            strBuildUp += @"var selectDateMessage = ""Select [date] ."" // do not replace [date], it will be replaced by date." + "\n";
            strBuildUp += "var crossobj, crossMonthObj, crossYearObj, monthSelected, yearSelected, dateSelected, omonthSelected, oyearSelected, odateSelected, monthConstructed, yearConstructed, intervalID1, intervalID2, timeoutID1, timeoutID2, ctlToPlaceValue, ctlNow, dateFormat, nStartingYear\n\n";
            strBuildUp += "var bPageLoaded=false\n";
            strBuildUp += "var ie=document.all\n";
            strBuildUp += "var dom=document.getElementById\n\n";
            strBuildUp += "var ns4=document.layers\n";
            strBuildUp += "var today = new Date()\n";
            strBuildUp += "var dateNow  = today.getDate()\n";
            strBuildUp += "var monthNow = today.getMonth()\n";
            strBuildUp += "var yearNow  = today.getYear()\n";
            strBuildUp += @"var imgsrc = new Array(""" + m_drop1Gif + @""",""" + m_drop2Gif + @""",""" + m_left1Gif + @""",""" + m_left2Gif + @""",""" + m_right1Gif + @""",""" + m_right2Gif + @""")" + "\n";
            strBuildUp += "var img = new Array()\n\n";
            strBuildUp += "var bShow = false;\n\n";
            strBuildUp += "var textValue = document.all." + this.ClientID + "_foo.value;";
            /*
 
             * Add Clear function
             */
            strBuildUp += "function hideCalendar3()";
            strBuildUp += "{";
            strBuildUp += " hideCalendar();";
            strBuildUp += "}";
            strBuildUp += "/* hides <select> and <applet> objects (for IE only) */\n";
            strBuildUp += "function hideElement( elmID, overDiv )\n";
            strBuildUp += "{\n";
            strBuildUp += "if( ie )\n";
            strBuildUp += "{\n";
            strBuildUp += "for( i = 0; i < document.all.tags( elmID ).length; i++ )\n";
            strBuildUp += "{\n";
            strBuildUp += "obj = document.all.tags( elmID )[i];\n";
            strBuildUp += "if( !obj || !obj.offsetParent )\n";
            strBuildUp += "{\n";
            strBuildUp += "continue;\n";
            strBuildUp += "}\n\n";
            strBuildUp += "// Find the element's offsetTop and offsetLeft relative to the BODY tag.\n";
            strBuildUp += "objLeft   = obj.offsetLeft;\n";
            strBuildUp += "objTop    = obj.offsetTop;\n";
            strBuildUp += "objParent = obj.offsetParent;\n\n";
            strBuildUp += @"while( objParent.tagName.toUpperCase() != ""BODY"" )" + "\n";
            strBuildUp += "{\n";
            strBuildUp += "objLeft  += objParent.offsetLeft;\n";
            strBuildUp += "objTop   += objParent.offsetTop;\n";
            strBuildUp += "objParent = objParent.offsetParent;\n";
            strBuildUp += "}\n\n";
            strBuildUp += "objHeight = obj.offsetHeight;\n";
            strBuildUp += "objWidth = obj.offsetWidth;\n\n";
            strBuildUp += "if(( overDiv.offsetLeft + overDiv.offsetWidth ) <= objLeft );\n";
            strBuildUp += "else if(( overDiv.offsetTop + overDiv.offsetHeight ) <= objTop );\n";
            strBuildUp += "else if( overDiv.offsetTop >= ( objTop + objHeight ));\n";
            strBuildUp += "else if( overDiv.offsetLeft >= ( objLeft + objWidth ));\n";
            strBuildUp += "else\n";
            strBuildUp += "{\n";
            strBuildUp += @"obj.style.visibility = ""hidden"";" + "\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n\n";
            strBuildUp += "/*\n";
            strBuildUp += "* unhides <select> and <applet> objects (for IE only)\n";
            strBuildUp += "*/\n";
            strBuildUp += "function showElement( elmID )\n";
            strBuildUp += "{\n";
            strBuildUp += "if( ie )\n";
            strBuildUp += "{\n";
            strBuildUp += "for( i = 0; i < document.all.tags( elmID ).length; i++ )\n";
            strBuildUp += "{\n";
            strBuildUp += "obj = document.all.tags( elmID )[i];\n\n";
            strBuildUp += "if( !obj || !obj.offsetParent )\n";
            strBuildUp += "{\n";
            strBuildUp += "continue;\n";
            strBuildUp += "}\n\n";
            strBuildUp += @"obj.style.visibility = """";" + "\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n\n";
            strBuildUp += "function HolidayRec (d, m, y, desc)\n";
            strBuildUp += "{\n";
            strBuildUp += "this.d = d\n";
            strBuildUp += "this.m = m\n";
            strBuildUp += "this.y = y\n";
            strBuildUp += "this.desc = desc\n";
            strBuildUp += "}\n\n";
            strBuildUp += "var HolidaysCounter = 0\n";
            strBuildUp += "var Holidays = new Array()\n\n";
            strBuildUp += "function addHoliday (d, m, y, desc)\n";
            strBuildUp += "{\n";
            strBuildUp += "Holidays[HolidaysCounter++] = new HolidayRec ( d, m, y, desc )\n";
            strBuildUp += "}\n\n";
            strBuildUp += "if (dom)\n";
            strBuildUp += "{\n";
            strBuildUp += "for (i=0;i<imgsrc.length;i++)\n";
            strBuildUp += "{\n";
            strBuildUp += "img[i] = new Image\n";
            strBuildUp += "img[i].src = imgDir + imgsrc[i]\n";
            strBuildUp += "}\n";
            strBuildUp += @"document.write (""<div onclick='bShow=true' id='calendar' style='z-index:+999;position:absolute;visibility:hidden;'><table width=""+((showWeekNumber==1)?250:220)+"" style='font-family:宋体;font-size:11px;border-width:1;border-style:solid;border-color:#a0a0a0;font-family:宋体; font-size:11px}' bgcolor='#ffffff'><tr bgcolor='#F0F0F0'><td><table width='""+((showWeekNumber==1)?265:248)+""'><tr><td style='width:210px;padding:2px;font-family:宋体; font-size:11px;'><font color='#ffffff'><B><span id='caption'></span></B></font></td><td align=right><a href='javascript:hideCalendar3()'><font color='#000000'>CLOSE</font><IMG style='display:none' SRC='""+imgDir+""" + m_closeGif + @"' WIDTH='15' HEIGHT='13' BORDER='0' ALT='Close the Calendar'></a></td></tr></table></td></tr><tr><td style='padding:5px' bgcolor=#ffffff><span id='content'></span></td></tr>"")" + "\n\n";
            strBuildUp += "if (showToday==1)\n";
            strBuildUp += "{\n";
            strBuildUp += @"document.write (""<tr bgcolor=#f0f0f0><td style='padding:5px' align=center><span id='lblToday'></span></td></tr>"")" + "\n";
            strBuildUp += "}\n\n";
            strBuildUp += @"document.write (""</table></div><div id='selectMonth' style='z-index:+999;position:absolute;visibility:hidden;'></div><div id='selectYear' style='z-index:+999;position:absolute;visibility:hidden;'></div>"");" + "\n";
            strBuildUp += "}\n\n";
            //strBuildUp += @"var monthName = new Array(""January"",""February"",""March"",""April"",""May"",""June"",""July"",""August"",""September"",""October"",""November"",""December"")" + "\n";
            strBuildUp += @"var monthName = new Array(""JAN"",""FEB"",""MAR"",""APR"",""MAY"",""JUN"",""JUL"",""AUG"",""SEP"",""OCT"",""NOV"",""DEC"")" + "\n";
            strBuildUp += @"var monthName2 = new Array(""JAN"",""FEB"",""MAR"",""APR"",""MAY"",""JUN"",""JUL"",""AUG"",""SEP"",""OCT"",""NOV"",""DEC"")" + "\n";
            strBuildUp += "if (startAt==0)\n";
            strBuildUp += "{\n";
            strBuildUp += @"dayName = new Array (""日"",""一"",""二"",""三"",""四"",""五"",""六"")" + "\n";
            strBuildUp += @"dayName2 = new Array (""星期天"",""星期一"",""星期二"",""星期三"",""星期四"",""星期五"",""星期六"")" + "\n";
            strBuildUp += "}\n";
            strBuildUp += "else\n";
            strBuildUp += "{\n";
            strBuildUp += @"dayName = new Array (""Mon"",""Tue"",""Wed"",""Thr"",""Fri"",""Sat"",""Sun"")" + "\n";
            strBuildUp += @"dayName2 = new Array (""Mon"",""Tue"",""Wed"",""Thr"",""Fri"",""Sat"",""Sun"")" + "\n";
            strBuildUp += "}\n";
            strBuildUp += @"var styleAnchor=""text-decoration:none;color:black;""" + "\n";
            strBuildUp += @"var styleLightBorder=""border-style:solid;border-width:1px;border-color:#a0a0a0;""" + "\n\n";
            strBuildUp += "function swapImage(srcImg, destImg){\n";
            strBuildUp += @"if (ie) { document.all(srcImg,0).setAttribute(""src"",imgDir + destImg) }" + "\n";
            strBuildUp += "}\n\n";
            strBuildUp += "function init() {\n";
            strBuildUp += "if (!ns4)\n";
            strBuildUp += "{\n";
            strBuildUp += "if (!ie) { yearNow += 1900 }\n\n";
            strBuildUp += @"crossobj=(dom)?document.all(""calendar"",0).style : ie? document.all.calendar : document.calendar" + "\n";
            strBuildUp += "hideCalendar()\n\n";
            strBuildUp += @"crossMonthObj=(dom)?document.all(""selectMonth"",0).style : ie? document.all.selectMonth : document.selectMonth" + "\n\n";
            strBuildUp += @"crossYearObj=(dom)?document.all(""selectYear"",0).style : ie? document.all.selectYear : document.selectYear" + "\n\n";
            strBuildUp += "monthConstructed=false;\n";
            strBuildUp += "yearConstructed=false;\n\n";
            strBuildUp += "if (showToday==1)\n";
            strBuildUp += "{\n";
            strBuildUp += @"document.all(""lblToday"",0).innerHTML = todayString + "" <a onmousemove='window.status=\""""+gotoString+""\""' onmouseout='window.status=\""\""' title='""+gotoString+""' style='""+styleAnchor+""' href='javascript:monthSelected=monthNow;yearSelected=yearNow;constructCalendar();'>""+dayName2[(today.getDay()-startAt==-1)?6:(today.getDay()-startAt)]+"", "" + yearNow +"" "" + monthName2[monthNow].substring(0,3)  +""  "" + dateNow  + ""</a>""" + "\n";
            strBuildUp += "}\n\n";
            strBuildUp += @"sHTML1=""<span id='spanLeft'   style='border-style:solid;border-width:1;border-color:#F0F0F0;cursor:pointer' onmouseover='swapImage(\""changeLeft\"",\""left.gif\"");this.style.borderColor=\""#D4D0C8\"";window.status=\""""+scrollLeftMessage+""\""' onclick='javascript:decMonth()' onmouseout='clearInterval(intervalID1);swapImage(\""changeLeft\"",\""left.gif\"");this.style.borderColor=\""#F0F0F0\"";window.status=\""\""' onmousedown='clearTimeout(timeoutID1);timeoutID1=setTimeout(\""StartDecMonth()\"",500)' onmouseup='clearTimeout(timeoutID1);clearInterval(intervalID1)'>&nbsp<input type='button' style='background-color: #F0F0F0;border: 0px ;font-size: 9px;color: #000000;height: 16px;width: 16px;font:caption;' value='<<'  /><IMG style='display:none'  id='changeLeft' SRC='""+imgDir+""left.gif' width=10 height=11 BORDER=0>&nbsp</span>&nbsp;""" + "\n";
            strBuildUp += @"sHTML1+=""<span id='spanRight' style='border-style:solid;border-width:1;border-color:#F0F0F0;cursor:pointer' onmouseover='swapImage(\""changeRight\"",\""right.gif\"");this.style.borderColor=\""#D4D0C8\"";window.status=\""""+scrollRightMessage+""\""' onmouseout='clearInterval(intervalID1);swapImage(\""changeRight\"",\""right.gif\"");this.style.borderColor=\""#F0F0F0\"";window.status=\""\""' onclick='incMonth()' onmousedown='clearTimeout(timeoutID1);timeoutID1=setTimeout(\""StartIncMonth()\"",500)' onmouseup='clearTimeout(timeoutID1);clearInterval(intervalID1)'>&nbsp<input type='button' style='background-color: #F0F0F0;border: 0px ;font-size: 9px;color: #000000;height: 16px;width: 16px;font:caption;' value='>>'  /><IMG style='display:none' id='changeRight' SRC='""+imgDir+""right.gif' width=10 height=11 BORDER=0>&nbsp</span>&nbsp""" + "\n";
            strBuildUp += @"sHTML1+=""<span id='spanMonth' style='border-style:solid;border-width:1;border-color:#F0F0F0;cursor:pointer' onmouseover='swapImage(\""changeMonth\"",\""down.gif\"");this.style.borderColor=\""#D4D0C8\"";window.status=\""""+selectMonthMessage+""\""' onmouseout='swapImage(\""changeMonth\"",\""down.gif\"");this.style.borderColor=\""#F0F0F0\"";window.status=\""\""' onclick='popUpMonth()'></span>&nbsp;""" + "\n";
            strBuildUp += @"sHTML1+=""<span id='spanYear' style='border-style:solid;border-width:1;border-color:#F0F0F0;cursor:pointer' onmouseover='swapImage(\""changeYear\"",\""down.gif\"");this.style.borderColor=\""#D4D0C8\"";window.status=\""""+selectYearMessage+""\""' onmouseout='swapImage(\""changeYear\"",\""down.gif\"");this.style.borderColor=\""#F0F0F0\"";window.status=\""\""' onclick='popUpYear()'></span>&nbsp;""" + "\n\n";
            strBuildUp += @"document.all(""caption"",0).innerHTML  = sHTML1" + "\n\n";
            strBuildUp += "bPageLoaded=true\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n\n";
            strBuildUp += "function hideCalendar() {\n";
            strBuildUp += @"crossobj.visibility=""hidden""" + "\n";
            strBuildUp += @"if (crossMonthObj != null){crossMonthObj.visibility=""hidden""}" + "\n";
            strBuildUp += @"if (crossYearObj != null){crossYearObj.visibility=""hidden""}" + "\n\n";
            strBuildUp += "showElement( 'SELECT' );\n";
            strBuildUp += "showElement( 'APPLET' );\n";
            strBuildUp += "}\n\n";
            strBuildUp += "function padZero(num) {\n";
            strBuildUp += "return (num < 10)? '0' + num : num ;\n";
            strBuildUp += "}\n\n";
            strBuildUp += "function constructDate(d,m,y)\n";
            strBuildUp += "{\n";
            strBuildUp += "sTmp = dateFormat\n";
            strBuildUp += @"sTmp = sTmp.replace (""dd"",""<e>"")" + "\n";
            strBuildUp += @"sTmp = sTmp.replace (""d"",""<d>"")" + "\n";
            strBuildUp += @"sTmp = sTmp.replace (""<e>"",padZero(d))" + "\n";
            strBuildUp += @"sTmp = sTmp.replace (""<d>"",d)" + "\n";
            strBuildUp += @"sTmp = sTmp.replace (""mmm"",""<o>"")" + "\n";
            strBuildUp += @"sTmp = sTmp.replace (""mm"",""<n>"")" + "\n";
            strBuildUp += @"sTmp = sTmp.replace (""m"",""<m>"")" + "\n";
            strBuildUp += @"sTmp = sTmp.replace (""<m>"",m+1)" + "\n";
            strBuildUp += @"sTmp = sTmp.replace (""<n>"",padZero(m+1))" + "\n";
            strBuildUp += @"sTmp = sTmp.replace (""<o>"",monthName[m])" + "\n";
            strBuildUp += @"return sTmp.replace (""yyyy"",y)" + "\n";
            strBuildUp += "}\n\n";
            strBuildUp += "function closeCalendar() {\n";
            strBuildUp += "var sTmp\n\n";
            strBuildUp += "hideCalendar();\n";
            strBuildUp += "ctlToPlaceValue.value = constructDate(dateSelected,monthSelected,yearSelected)\n";
            strBuildUp += "}\n\n";
            strBuildUp += "/*** Month Pulldown ***/\n\n";
            strBuildUp += "function StartDecMonth()\n";
            strBuildUp += "{\n";
            strBuildUp += @"intervalID1=setInterval(""decMonth()"",80)" + "\n";
            strBuildUp += "}\n\n";
            strBuildUp += "function StartIncMonth()\n";
            strBuildUp += "{\n";
            strBuildUp += @"intervalID1=setInterval(""incMonth()"",80)" + "\n";
            strBuildUp += "}\n\n";
            strBuildUp += "function incMonth () {\n";
            strBuildUp += "monthSelected++\n";
            strBuildUp += "if (monthSelected>11) {\n";
            strBuildUp += "monthSelected=0\n";
            strBuildUp += "yearSelected++\n";
            strBuildUp += "}\n";
            strBuildUp += "constructCalendar()\n";
            strBuildUp += "}\n\n";
            strBuildUp += "function decMonth () {\n";
            strBuildUp += "monthSelected--\n";
            strBuildUp += "if (monthSelected<0) {\n";
            strBuildUp += "monthSelected=11\n";
            strBuildUp += "yearSelected--\n";
            strBuildUp += "}\n";
            strBuildUp += "constructCalendar()\n";
            strBuildUp += "}\n\n";
            strBuildUp += "function constructMonth() {\n";
            strBuildUp += "popDownYear()\n";
            strBuildUp += "if (!monthConstructed) {\n";
            strBuildUp += @"sHTML = """"" + "\n";
            strBuildUp += "for (i=0; i<12; i++) {\n";
            strBuildUp += "sName = monthName[i];\n";
            strBuildUp += "if (i==monthSelected){\n";
            strBuildUp += @"sName = ""<B>"" + sName + ""</B>""" + "\n";
            strBuildUp += "}\n";
            strBuildUp += @"sHTML += ""<tr><td id='m"" + i + ""' onmouseover='this.style.backgroundColor=\""#FFCC99\""' onmouseout='this.style.backgroundColor=\""\""' style='cursor:pointer' onclick='monthConstructed=false;monthSelected="" + i + "";constructCalendar();popDownMonth();event.cancelBubble=true'>&nbsp;"" + sName + ""&nbsp;</td></tr>""" + "\n";
            strBuildUp += "}\n\n";
            strBuildUp += @"document.all(""selectMonth"",0).innerHTML = ""<table width=70 style='font-family:宋体; font-size:11px; border-width:1; border-style:solid; border-color:#a0a0a0;' bgcolor='#FFFFDD' cellspacing=0 onmouseover='clearTimeout(timeoutID1)' onmouseout='clearTimeout(timeoutID1);timeoutID1=setTimeout(\""popDownMonth()\"",100);event.cancelBubble=true'>"" + sHTML + ""</table>""" + "\n\n";
            strBuildUp += "monthConstructed=true\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n\n";
            strBuildUp += "function popUpMonth() {\n";
            strBuildUp += "constructMonth()\n";
            strBuildUp += @"crossMonthObj.visibility = (dom||ie)? ""visible"" : ""show""" + "\n";
            strBuildUp += "crossMonthObj.left = parseInt(crossobj.left) + 70\n";
            strBuildUp += "crossMonthObj.top = parseInt(crossobj.top) + 26\n\n";
            strBuildUp += @"hideElement( 'SELECT', document.all(""selectMonth"",0) );" + "\n";
            strBuildUp += @"hideElement( 'APPLET', document.all(""selectMonth""),0 );" + "\n";
            strBuildUp += "}\n\n";
            strBuildUp += "function popDownMonth() {\n";
            strBuildUp += @"crossMonthObj.visibility= ""hidden""" + "\n";
            strBuildUp += "}\n\n";
            strBuildUp += "/*** Year Pulldown ***/\n\n";
            strBuildUp += "function incYear() {\n";
            strBuildUp += "for (i=0; i<7; i++){\n";
            strBuildUp += "newYear = (i+nStartingYear)+1\n";
            strBuildUp += "if (newYear==yearSelected)\n";
            strBuildUp += @"{ txtYear = ""&nbsp;<B>"" + newYear + ""</B>&nbsp;"" }" + "\n";
            strBuildUp += "else\n";
            strBuildUp += @"{ txtYear = ""&nbsp;"" + newYear + ""&nbsp;"" }" + "\n";
            strBuildUp += @"document.all(""y""+i,0).innerHTML = txtYear" + "\n";
            strBuildUp += "}\n";
            strBuildUp += "nStartingYear ++;\n";
            strBuildUp += "bShow=true\n";
            strBuildUp += "}\n\n";
            strBuildUp += "function decYear() {\n";
            strBuildUp += "for (i=0; i<7; i++){\n";
            strBuildUp += "newYear = (i+nStartingYear)-1\n";
            strBuildUp += "if (newYear==yearSelected)\n";
            strBuildUp += @"{ txtYear = ""&nbsp;<B>"" + newYear + ""</B>&nbsp;"" }" + "\n";
            strBuildUp += "else\n";
            strBuildUp += @"{ txtYear = ""&nbsp;"" + newYear + ""&nbsp;"" }" + "\n";
            strBuildUp += @"document.all(""y""+i,0).innerHTML = txtYear" + "\n";
            strBuildUp += "}\n";
            strBuildUp += "nStartingYear --;\n";
            strBuildUp += "bShow=true\n";
            strBuildUp += "}\n\n";
            strBuildUp += "function selectYear(nYear) {\n";
            strBuildUp += "yearSelected=parseInt(nYear+nStartingYear);\n";
            strBuildUp += "yearConstructed=false;\n";
            strBuildUp += "constructCalendar();\n";
            strBuildUp += "popDownYear();\n";
            strBuildUp += "}\n\n";
            strBuildUp += "function constructYear() {\n";
            strBuildUp += "popDownMonth()\n";
            strBuildUp += @"sHTML = """"" + "\n";
            strBuildUp += "if (!yearConstructed) {\n\n";
            strBuildUp += @"sHTML = ""<tr><td align='center' onmouseover='this.style.backgroundColor=\""#FFCC99\""' onmouseout='clearInterval(intervalID1);this.style.backgroundColor=\""\""' style='cursor:pointer' onmousedown='clearInterval(intervalID1);intervalID1=setInterval(\""decYear()\"",30)' onmouseup='clearInterval(intervalID1)'>-</td></tr>""" + "\n\n";
            strBuildUp += "j = 0\n";
            strBuildUp += "nStartingYear = yearSelected-3\n";
            strBuildUp += "for (i=(yearSelected-3); i<=(yearSelected+3); i++) {\n";
            strBuildUp += "sName = i;\n";
            strBuildUp += "if (i==yearSelected){\n";
            strBuildUp += @"sName = ""<B>"" + sName + ""</B>""" + "\n";
            strBuildUp += "}\n\n";
            strBuildUp += @"sHTML += ""<tr><td id='y"" + j + ""' onmouseover='this.style.backgroundColor=\""#FFCC99\""' onmouseout='this.style.backgroundColor=\""\""' style='cursor:pointer' onclick='selectYear(""+j+"");event.cancelBubble=true'>&nbsp;"" + sName + ""&nbsp;</td></tr>""" + "\n";
            strBuildUp += "j ++;\n";
            strBuildUp += "}\n\n";
            strBuildUp += @"sHTML += ""<tr><td align='center' onmouseover='this.style.backgroundColor=\""#FFCC99\""' onmouseout='clearInterval(intervalID2);this.style.backgroundColor=\""\""' style='cursor:pointer' onmousedown='clearInterval(intervalID2);intervalID2=setInterval(\""incYear()\"",30)' onmouseup='clearInterval(intervalID2)'>+</td></tr>""" + "\n\n";
            strBuildUp += @"document.all(""selectYear"",0).innerHTML = ""<table width=44 style='font-family:宋体; font-size:11px; border-width:1; border-style:solid; border-color:#a0a0a0;' bgcolor='#FFFFDD' onmouseover='clearTimeout(timeoutID2)' onmouseout='clearTimeout(timeoutID2);timeoutID2=setTimeout(\""popDownYear()\"",100)' cellspacing=0>"" + sHTML + ""</table>""" + "\n\n";
            strBuildUp += "yearConstructed = true\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n";
            strBuildUp += "function popDownYear() {\n";
            strBuildUp += "clearInterval(intervalID1)\n";
            strBuildUp += "clearTimeout(timeoutID1)\n";
            strBuildUp += "clearInterval(intervalID2)\n";
            strBuildUp += "clearTimeout(timeoutID2)\n";
            strBuildUp += @"crossYearObj.visibility= ""hidden""" + "\n";
            strBuildUp += "}\n\n";
            strBuildUp += "function popUpYear() {\n";
            strBuildUp += "var leftOffset\n\n";
            strBuildUp += "constructYear()\n";
            strBuildUp += @"crossYearObj.visibility = (dom||ie)? ""visible"" : ""show""" + "\n";
            strBuildUp += @"leftOffset = parseInt(crossobj.left) + document.all(""spanYear"",0).offsetLeft" + "\n";
            strBuildUp += "if (ie)\n";
            strBuildUp += "{\n";
            strBuildUp += "leftOffset += 6\n";
            strBuildUp += "}\n";
            strBuildUp += "crossYearObj.left = leftOffset\n";
            strBuildUp += "crossYearObj.top = parseInt(crossobj.top) + 26\n";
            strBuildUp += "}\n\n";
            strBuildUp += "/*** calendar ***/\n";
            strBuildUp += "function WeekNbr(n) {\n";
            strBuildUp += "// Algorithm used:\n";
            strBuildUp += "// From Klaus Tondering's Calendar document (The Authority/Guru)\n";
            strBuildUp += "// hhtp://www.tondering.dk/claus/calendar.html\n";
            strBuildUp += "// a = (14-month) / 12\n";
            strBuildUp += "// y = year + 4800 - a\n";
            strBuildUp += "// m = month + 12a - 3\n";
            strBuildUp += "// J = day + (153m + 2) / 5 + 365y + y / 4 - y / 100 + y / 400 - 32045\n";
            strBuildUp += "// d4 = (J + 31741 - (J mod 7)) mod 146097 mod 36524 mod 1461\n";
            strBuildUp += "// L = d4 / 1460\n";
            strBuildUp += "// d1 = ((d4 - L) mod 365) + L\n";
            strBuildUp += "// WeekNumber = d1 / 7 + 1\n\n";
            strBuildUp += "year = n.getFullYear();\n";
            strBuildUp += "month = n.getMonth() + 1;\n";
            strBuildUp += "if (startAt == 0) {\n";
            strBuildUp += "day = n.getDate() + 1;\n";
            strBuildUp += "}\n";
            strBuildUp += "else {\n";
            strBuildUp += "day = n.getDate();\n";
            strBuildUp += "}\n\n";
            strBuildUp += "a = Math.floor((14-month) / 12);\n";
            strBuildUp += "y = year + 4800 - a;\n";
            strBuildUp += "m = month + 12 * a - 3;\n";
            strBuildUp += "b = Math.floor(y/4) - Math.floor(y/100) + Math.floor(y/400);\n";
            strBuildUp += "J = day + Math.floor((153 * m + 2) / 5) + 365 * y + b - 32045;\n";
            strBuildUp += "d4 = (((J + 31741 - (J % 7)) % 146097) % 36524) % 1461;\n";
            strBuildUp += "L = Math.floor(d4 / 1460);\n";
            strBuildUp += "d1 = ((d4 - L) % 365) + L;\n";
            strBuildUp += "week = Math.floor(d1/7) + 1;\n\n";
            strBuildUp += "return week;\n";
            strBuildUp += "}\n\n";
            strBuildUp += "function constructCalendar () {\n";
            strBuildUp += "var aNumDays = Array (31,0,31,30,31,30,31,31,30,31,30,31)\n\n";
            strBuildUp += "var dateMessage\n";
            strBuildUp += "var startDate = new Date (yearSelected,monthSelected,1)\n";
            strBuildUp += "var endDate\n\n";
            strBuildUp += "if (monthSelected==1)\n";
            strBuildUp += "{\n";
            strBuildUp += "endDate = new Date (yearSelected,monthSelected+1,1);\n";
            strBuildUp += "endDate = new Date (endDate - (24*60*60*1000));\n";
            strBuildUp += "numDaysInMonth = endDate.getDate()\n";
            strBuildUp += "}\n";
            strBuildUp += "else\n";
            strBuildUp += "{\n";
            strBuildUp += "numDaysInMonth = aNumDays[monthSelected];\n";
            strBuildUp += "}\n\n";
            strBuildUp += "datePointer = 0\n";
            strBuildUp += "dayPointer = startDate.getDay() - startAt\n\n";
            strBuildUp += "if (dayPointer<0)\n";
            strBuildUp += "{\n";
            strBuildUp += "dayPointer = 6\n";
            strBuildUp += "}\n\n";
            strBuildUp += @"sHTML = ""<table  border=0 style='font-family:verdana;font-size:13px;'><tr>""" + "\n\n";
            strBuildUp += "if (showWeekNumber==1)\n";
            strBuildUp += "{\n";
            strBuildUp += @"sHTML += ""<td width='12.5%'><b>"" + weekString + ""</b></td><td width='1px' rowspan=7 bgcolor='#d0d0d0' style='padding:0px;width:1px'><img style='width:1px' src='""+imgDir+""" + m_dividerGif + @"' width='1px'></td>""" + "\n";
            strBuildUp += "}\n\n";
            strBuildUp += "for (i=0; i<7; i++) {\n";
            strBuildUp += @"sHTML += ""<td width='12.5%' align='right'><B>""+ dayName[i]+""</B></td>""" + "\n";
            strBuildUp += "}\n";
            strBuildUp += @"sHTML +=""</tr><tr>""" + "\n\n";
            strBuildUp += "if (showWeekNumber==1)\n";
            strBuildUp += "{\n";
            strBuildUp += @"sHTML += ""<td align=right>"" + WeekNbr(startDate) + ""&nbsp;</td>""" + "\n";
            strBuildUp += "}\n\n";
            strBuildUp += "for ( var i=1; i<=dayPointer;i++ )\n";
            strBuildUp += "{\n";
            strBuildUp += @"sHTML += ""<td>&nbsp;</td>""" + "\n";
            strBuildUp += "}\n\n";
            strBuildUp += "for ( datePointer=1; datePointer<=numDaysInMonth; datePointer++ )\n";
            strBuildUp += "{\n";
            strBuildUp += "dayPointer++;\n";
            strBuildUp += @"sHTML += ""<td align=right>""" + "\n";
            strBuildUp += "sStyle=styleAnchor\n";
            strBuildUp += "if ((datePointer==odateSelected) && (monthSelected==omonthSelected) && (yearSelected==oyearSelected))\n";
            strBuildUp += "{ sStyle+=styleLightBorder }\n\n";
            strBuildUp += @"sHint = """"" + "\n";
            strBuildUp += "for (k=0;k<HolidaysCounter;k++)\n";
            strBuildUp += "{\n";
            strBuildUp += "if ((parseInt(Holidays[k].d)==datePointer)&&(parseInt(Holidays[k].m)==(monthSelected+1)))\n";
            strBuildUp += "{\n";
            strBuildUp += "if ((parseInt(Holidays[k].y)==0)||((parseInt(Holidays[k].y)==yearSelected)&&(parseInt(Holidays[k].y)!=0)))\n";
            strBuildUp += "{\n";
            strBuildUp += @"sStyle+=""background-color:#FFDDDD;""" + "\n";
            strBuildUp += @"sHint+=sHint==""""?Holidays[k].desc:""\n""+Holidays[k].desc" + "\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n\n";
            strBuildUp += @"var regexp= /\""/g" + "\n";
            strBuildUp += @"sHint=sHint.replace(regexp,""&quot;"")" + "\n\n";
            strBuildUp += @"dateMessage = ""onmousemove='window.status=\""""+selectDateMessage.replace(""[date]"",constructDate(datePointer,monthSelected,yearSelected))+""\""' onmouseout='window.status=\""\""' """ + "\n\n";
            strBuildUp += "if ((datePointer==dateNow)&&(monthSelected==monthNow)&&(yearSelected==yearNow))\n";
            strBuildUp += @"{ sHTML += ""<b><a ""+dateMessage+"" title=\"""" + sHint + ""\"" style='""+sStyle+""' href='javascript:dateSelected=""+datePointer+"";closeCalendar();'><font color=#ff0000>&nbsp;"" + datePointer + ""</font>&nbsp;</a></b>""}" + "\n";
            strBuildUp += "else if (dayPointer % 7 == (startAt * -1)+1)\n";
            strBuildUp += @"{ sHTML += ""<a ""+dateMessage+"" title=\"""" + sHint + ""\"" style='""+sStyle+""' href='javascript:dateSelected=""+datePointer + "";closeCalendar();'>&nbsp;<font color=#909090>"" + datePointer + ""</font>&nbsp;</a>"" }" + "\n";
            strBuildUp += "else\n";
            strBuildUp += @"{ sHTML += ""<a ""+dateMessage+"" title=\"""" + sHint + ""\"" style='""+sStyle+""' href='javascript:dateSelected=""+datePointer + "";closeCalendar();'>&nbsp;"" + datePointer + ""&nbsp;</a>"" }" + "\n\n";
            strBuildUp += @"sHTML += ""</td>""" + "\n";
            strBuildUp += "if ((dayPointer+startAt) % 7 == startAt) {\n";
            strBuildUp += @"sHTML += ""</tr><tr>""" + "\n";
            strBuildUp += "if ((showWeekNumber==1)&&(datePointer<numDaysInMonth))\n";
            strBuildUp += "{\n";
            strBuildUp += @"sHTML += ""<td align=right>"" + (WeekNbr(new Date(yearSelected,monthSelected,datePointer+1))) + ""&nbsp;</td>""" + "\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n\n";
            strBuildUp += "if((dayPointer+startAt) % 7 != startAt){";
            strBuildUp += @"for(var c=1;c<7;c++){sHTML +=""<td></td>"";";
            strBuildUp += @"if((dayPointer+startAt+c) % 7 == startAt){sHTML +=""</tr>"";break;}}";
            strBuildUp += "}\n";
            strBuildUp += @"document.all(""content"",0).innerHTML   = sHTML" + "\n";
            strBuildUp += @"document.all(""spanMonth"",0).innerHTML = ""&nbsp;<font color=#000000>"" + monthName[monthSelected] + ""</font>&nbsp;<IMG style='display:none' id='changeMonth' SRC='""+imgDir+""down.gif' WIDTH='12' HEIGHT='10' BORDER=0>""" + "\n";
            strBuildUp += @"document.all(""spanYear"",0).innerHTML = ""&nbsp;<font color=#000000>"" + yearSelected + ""</font>&nbsp;<IMG style='display:none' id='changeYear' SRC='""+imgDir+""down.gif' WIDTH='12' HEIGHT='10' BORDER=0>""" + "\n";
            strBuildUp += "}\n\n";
            strBuildUp += @"function isDegit(str){var pattern=/^[0-9]\d*$/; return pattern.test(str); }";
            strBuildUp += "function checkDateTime(obj) {";
            strBuildUp += "var str=obj.value;var strTemp='';";
            strBuildUp += "if (str.trim().length==0){return true;}";
            strBuildUp += "if (str.length==8){str=str.substring(0,4)+'/'+str.substring(4,6)+'/'+str.substring(6,8);}strTemp=str;";
            strBuildUp += "if (str.length!=10){return false;}";
            strBuildUp += "var j =str.indexOf('/');var temp='';";
            strBuildUp += "if (j!=4){return false;}else{temp=str.substring(5,10)}";
            strBuildUp += "j =temp.indexOf('/');";
            strBuildUp += "if (j!=2){return false;}";
            strBuildUp += @"str =str.replace(/(\/)/g,'');";
            strBuildUp += "var newstr='';";
            strBuildUp += "var year,month,day;";
            strBuildUp += "for(var i=0;i<str.length;i++){";
            strBuildUp += "if(isDegit(str.charAt(i))){";
            strBuildUp += "newstr+=str.charAt(i);}}if(newstr.length!=8){";
            strBuildUp += "return false;}";
            strBuildUp += "year = newstr.substring(0,4);month=newstr.substring(4,6);day=newstr.substring(6,8);";
            strBuildUp += "if (year < 1000 || year > 9999){return false;}";
            strBuildUp += "if (month < 1 || month > 12){return false;}";
            strBuildUp += "if (day < 1 || day > 31){return false;}";
            strBuildUp += "if ((month==4 || month==6 || month==9 || month==11) && day==31){return false;}";
            strBuildUp += "if (month==2){var isleap=(year % 4==0 && (year % 100 !=0 || year % 400==0));";
            strBuildUp += "if (day>29){return false;}";
            strBuildUp += "if ((day==29) && (!isleap)){return false;}}";
            strBuildUp += "if(obj.value.length==8)obj.value=strTemp;return true; }\n";
            strBuildUp += "";

            strBuildUp += "function popUpCalendar(ctl, ctl2, format) {\n";
            strBuildUp += "var leftpos=0\n";
            strBuildUp += "var toppos=0\n\n";
            strBuildUp += "if (bPageLoaded)\n";
            strBuildUp += "{\n";
            strBuildUp += @"if ( crossobj.visibility == ""hidden"" ) {" + "\n";
            strBuildUp += "ctlToPlaceValue = ctl2\n";
            strBuildUp += "dateFormat=format;\n\n";
            strBuildUp += @"formatChar = "" """ + "\n";
            strBuildUp += "aFormat = dateFormat.split(formatChar)\n";
            strBuildUp += "if (aFormat.length<3)\n";
            strBuildUp += "{\n";
            strBuildUp += @"formatChar = ""/""" + "\n";
            strBuildUp += "aFormat = dateFormat.split(formatChar)\n";
            strBuildUp += "if (aFormat.length<3)\n";
            strBuildUp += "{\n";
            strBuildUp += @"formatChar = "".""" + "\n";
            strBuildUp += "aFormat = dateFormat.split(formatChar)\n";
            strBuildUp += "if (aFormat.length<3)\n";
            strBuildUp += "{\n";
            strBuildUp += @"formatChar = ""/""" + "\n";
            strBuildUp += "aFormat = dateFormat.split(formatChar)\n";
            strBuildUp += "if (aFormat.length<3)\n";
            strBuildUp += "{\n";
            strBuildUp += "// invalid date format\n";
            strBuildUp += @"formatChar=""""" + "\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n\n";
            strBuildUp += "tokensChanged = 0\n";
            strBuildUp += @"if ( formatChar != """" )" + "\n";
            strBuildUp += "{\n";
            strBuildUp += "// use user's date\n";
            strBuildUp += "aData = ctl2.value.split(formatChar)\n\n";
            strBuildUp += "for (i=0;i<3;i++)\n";
            strBuildUp += "{\n";
            strBuildUp += @"if ((aFormat[i]==""d"") || (aFormat[i]==""dd""))" + "\n";
            strBuildUp += "{\n";
            strBuildUp += "dateSelected = parseInt(aData[i], 10)\n";
            strBuildUp += "tokensChanged ++\n";
            strBuildUp += "}\n";
            strBuildUp += @"else if ((aFormat[i]==""m"") || (aFormat[i]==""mm""))" + "\n";
            strBuildUp += "{\n";
            strBuildUp += "monthSelected = parseInt(aData[i], 10) - 1\n";
            strBuildUp += "tokensChanged ++\n";
            strBuildUp += "}\n";
            strBuildUp += @"else if (aFormat[i]==""yyyy"")" + "\n";
            strBuildUp += "{\n";
            strBuildUp += "yearSelected = parseInt(aData[i], 10)\n";
            strBuildUp += "tokensChanged ++\n";
            strBuildUp += "}\n";
            strBuildUp += @"else if (aFormat[i]==""mmm"")" + "\n";
            strBuildUp += "{\n";
            strBuildUp += "for (j=0; j<12; j++)\n";
            strBuildUp += "{\n";
            strBuildUp += "if (aData[i]==monthName[j])\n";
            strBuildUp += "{\n";
            strBuildUp += "monthSelected=j\n";
            strBuildUp += "tokensChanged ++\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n\n";
            strBuildUp += "if ((tokensChanged!=3)||isNaN(dateSelected)||isNaN(monthSelected)||isNaN(yearSelected))\n";
            strBuildUp += "{\n";
            strBuildUp += "dateSelected = dateNow\n";
            strBuildUp += "monthSelected = monthNow\n";
            strBuildUp += "yearSelected = yearNow\n";
            strBuildUp += "}\n\n";
            strBuildUp += "odateSelected=dateSelected\n";
            strBuildUp += "omonthSelected=monthSelected\n";
            strBuildUp += "oyearSelected=yearSelected\n\n";
            strBuildUp += "aTag = ctl\n";
            strBuildUp += "do {\n";
            strBuildUp += "aTag = aTag.offsetParent;\n";
            strBuildUp += "leftpos += aTag.offsetLeft;\n";
            strBuildUp += "toppos += aTag.offsetTop;\n";
            strBuildUp += @"} while(aTag.tagName!=""BODY"");" + "\n\n";
            strBuildUp += "crossobj.left = fixedX==-1 ? ctl.offsetLeft + leftpos : fixedX\n";
            strBuildUp += "crossobj.top = fixedY==-1 ? ctl.offsetTop + toppos + ctl.offsetHeight + 2 : fixedY\n";
            strBuildUp += "constructCalendar (1, monthSelected, yearSelected);\n";
            strBuildUp += @"crossobj.visibility=(dom||ie)? ""visible"" : ""show""" + "\n\n";
            strBuildUp += @"hideElement( 'SELECT', document.all(""calendar"",0) );" + "\n";
            strBuildUp += @"hideElement( 'APPLET', document.all(""calendar"",0) );" + "\n\n";
            strBuildUp += "bShow = true;\n";
            strBuildUp += "}\n";
            strBuildUp += "else\n";
            strBuildUp += "{\n";
            strBuildUp += "hideCalendar()\n";
            strBuildUp += "if (ctlNow!=ctl) {popUpCalendar(ctl, ctl2, format)}\n";
            strBuildUp += "}\n";
            strBuildUp += "ctlNow = ctl\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n\n";
            strBuildUp += "document.onkeypress = function hidecal1 () {\n";
            strBuildUp += "if (event.keyCode==27)\n";
            strBuildUp += "{\n";
            strBuildUp += "hideCalendar()\n";
            strBuildUp += "}\n";
            strBuildUp += "}\n";
            strBuildUp += "document.onclick = function hidecal2 () {\n";
            strBuildUp += "if (!bShow)\n";
            strBuildUp += "{\n";
            strBuildUp += "hideCalendar()\n";
            strBuildUp += "}\n";
            strBuildUp += "bShow = false\n";
            strBuildUp += "}\n\n";
            strBuildUp += "if(ie)\n";
            strBuildUp += "{\n";
            strBuildUp += "init()\n";
            strBuildUp += "}\n";
            strBuildUp += "else\n";
            strBuildUp += "{\n";
            strBuildUp += "window.onload=init;\n";
            strBuildUp += "}\n";
            strBuildUp += "<";
            strBuildUp += "/";
            strBuildUp += "script>";
            if (!Page.ClientScript.IsStartupScriptRegistered(this.GetType(), "datePicker"))
                Page.ClientScript.RegisterStartupScript(this.GetType(), "datePicker", strBuildUp);
        }
        /// <summary>
        /// Override create childe controls
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            if (this.enable)
                placeJavascript();
            System.Web.UI.WebControls.TextBox txtTextBox = new TextBox();
            txtTextBox.MaxLength = this.maxlength;
            txtTextBox.Width = this.width;
            txtTextBox.Font.Name = "Arial";

            if (m_ControlCssClass.Length > 0)
                txtTextBox.CssClass = m_ControlCssClass;

            if (m_text != "")
                txtTextBox.Text = m_text;
            if (m_Css == "")
                txtTextBox.CssClass = m_Css;
            txtTextBox.ID = "foo";
            if (this.enable)
            {
                txtTextBox.Attributes.Add("onclick", "if(!$('#" + this.ClientID + "_foo').attr('readonly')){popUpCalendar(document.all." + this.ClientID + "_foo, document.all." + this.ClientID + "_foo, '" + m_DateType + "');if(!checkDateTime(document.all." + this.ClientID + "_foo)){document.all." + this.ClientID + "_foo.value=textValue;}textValue=document.all." + this.ClientID + "_foo.value;}");
                txtTextBox.Attributes.Add("onblur", "if(!$('#" + this.ClientID + "_foo').attr('readonly')){if(!checkDateTime(document.all." + this.ClientID + "_foo)){document.all." + this.ClientID + "_foo.value=textValue;}}");
                //txtTextBox.Attributes.Add("onkeydown", "if(!$('#" + this.ClientID + "_foo').attr('readonly')&&event.keyCode==13){if(!checkDateTime(document.all." + this.ClientID + "_foo)){document.all." + this.ClientID + "_foo.value=textValue;hideCalendar();}}");
            }
            else
            {
                txtTextBox.ReadOnly = true;
            }


            this.Controls.Add(txtTextBox);

            if (this.IsRequired)
            {
                Label lblRequired = new Label();
                lblRequired.ID = txtTextBox.ID + "Alarm";
                lblRequired.Text = "*";
                lblRequired.Width = 10;
                lblRequired.Font.Size = txtTextBox.Font.Size;
                lblRequired.Font.Bold = true;
                lblRequired.Style.Add("text-align", HorizontalAlign.Center.ToString());
                lblRequired.ForeColor = System.Drawing.Color.Red;

                this.Controls.Add(lblRequired);
                RequiredFieldValidator validator = new RequiredFieldValidator();
                validator.ID = txtTextBox.ID + "RequiredFieldValidator";
                validator.ErrorMessage = ValidateName + "required";
                validator.Text = "required";
                validator.Display = ValidatorDisplay.Dynamic;
                validator.ControlToValidate = txtTextBox.ID;
                validator.Enabled = this.Enable;

                this.Controls.Add(validator);

            }

        }

        #endregion
    }
}

