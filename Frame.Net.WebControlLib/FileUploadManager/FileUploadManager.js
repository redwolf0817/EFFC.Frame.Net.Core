Type.registerNamespace("EFFC.Frame.Net.WebControlLib");

EFFC.Frame.Net.WebControlLib.UpdatePanelIFrameExecutor = function(sourceElement)
{
	EFFC.Frame.Net.WebControlLib.UpdatePanelIFrameExecutor.initializeBase(this);

	// for properties
	this._started = false;
	this._responseAvailable = false;
	this._timedOut = false;
	this._aborted = false;
	this._responseData = null;
	this._statusCode = null;
	
	// the element initiated the async postback
	this._sourceElement = sourceElement;
	// the form in the page.
	this._form = Sys.WebForms.PageRequestManager.getInstance()._form;
	// the handler to execute when the page in iframe loaded.
	this._iframeLoadCompleteHandler = Function.createDelegate(this, this._iframeLoadComplete);
}
EFFC.Frame.Net.WebControlLib.UpdatePanelIFrameExecutor.prototype =
{
	get_started : function()
	{
		return this._started
	},
	
	get_responseAvailable : function()
	{
		return this._responseAvailable;
	},

	get_timedOut : function()
	{
		return this._timedOut;
	},
	
	get_aborted : function()
	{
		return this._aborted;
	},
	
	get_responseData : function()
	{
		return this._responseData;
	},
	
	get_statusCode : function()
	{
		return this._statusCode;
	},

	executeRequest : function()
	{
		// create an hidden iframe
		this._iframe = this._createIFrame();
		
		// all the additional hidden input elements
		this._addAdditionalHiddenElements();
		
		// point the form's target to the iframe
		this._form.target = this._iframe.id;
		this._form.encType = "multipart/form-data";		
		
		// set up the timeout counter.
		var timeout = this._webRequest.get_timeout();
		if (timeout > 0)
		{
			this._timer = window.setTimeout(Function.createDelegate(this, this._onTimeout), timeout);
		}
		
		this._started = true;
		
		// restore the status of the element after submitting the form
		setTimeout(Function.createDelegate(this, this._restoreElements), 0);
		// sumbit the form
		this._form.submit();
	},
	
	abort : function()
	{
		this._aborted = true;
		this._clearTimer();
	},
	
	_createIFrame : function()
	{
		var id = "f" + new String(Math.floor(9999 * Math.random())); 
	
		var iframe = null;
		
		if (window.ActiveXObject)
		{
			iframe = document.createElement("<iframe name=\"" + id + "\" id=\"" + id + "\" />");
		}
		else
		{
			iframe = document.createElement("iframe");
			iframe.id = id;
			iframe.name = id;
		}
		
		if (!this._hideContainer)
		{
			this._hideContainer = document.createElement("div");
			this._hideContainer.style.display = "none";
			document.body.appendChild(this._hideContainer);
		}
		this._hideContainer.appendChild(iframe);
		
		$addHandler(iframe, "load", this._iframeLoadCompleteHandler);
		return iframe;
	},
	
	_restoreElements : function()
	{
		var form = this._form;
		form.target = "";
		form.encType = "application/x-www-form-urlencoded";
		
		this._removeAdditionalHiddenElements();
	},

	_iframeLoadComplete : function()
	{
		var iframe = this._iframe;
		delete this._iframe;
		
		var responseText = null;
		try
		{	
			var f = iframe.contentWindow.__f__;
			var responseData = f ? this._parseScriptText(f.toString()) : 
				this._parsePreNode(iframe.contentWindow.document.body.firstChild);
				
			if (responseData.indexOf("\r\n") < 0 && responseData.indexOf("\n") > 0)
			{
				responseData = responseData.replace(/\n/g, "\r\n");
			}
				
			this._responseData = responseData;
			this._statusCode = 200;
			this._responseAvailable = true;
		}
		catch (e)
		{
			this._statusCode = 500;
			this._responseAvailable = false;
		}
		
		$removeHandler(iframe, "load", this._iframeLoadCompleteHandler);
		iframe.parentNode.removeChild(iframe);
		this._clearTimer();
		this.get_webRequest().completed(Sys.EventArgs.Empty);
	},
	
	_parseScriptText : function(scriptText)
	{
		var indexBegin = scriptText.indexOf("/*") + 2;
		var indexEnd = scriptText.lastIndexOf("*/");
		var encodedText = scriptText.substring(indexBegin, indexEnd);
		return encodedText.replace(/\*\/\/\*/g, "*/").replace(/<\/scriptt/g, "</script");
    },
	
	_parsePreNode : function(preNode)
	{
		if (preNode.tagName.toUpperCase() !== "PRE") throw new Error();
		return this._parseScriptText(preNode.textContent || preNode.innerText);
	},
	
	_addAdditionalHiddenElements : function()
	{
		var prm = Sys.WebForms.PageRequestManager.getInstance();
		
		this._hiddens = [];
		
		this._addHiddenElement(prm._scriptManagerID, prm._postBackSettings.panelID);
		this._addHiddenElement("__AjaxFileUploading__", "__IsInAjaxFileUploading__");
		
		var additionalInput = null;
		var element = this._sourceElement;
		
		if (element.name)
		{
			var requestBody = this.get_webRequest().get_body();
			var index = -1;
			
			if (element.tagName === 'INPUT')
			{
				var type = element.type;
				if (type === 'submit')
				{
					index = requestBody.lastIndexOf("&" + element.name + "=");
				}
				else if (type === 'image')
				{
					index = requestBody.lastIndexOf("&" + element.name + ".x=");
				}
			}
			else if ((element.tagName === 'BUTTON') && (element.name.length !== 0) && (element.type === 'submit'))
			{
				index = requestBody.lastIndexOf("&" + element.name + "=");
			}
			
			if (index > 0)
			{
				additionalInput = requestBody.substring(index + 1);
			}
		}
		
		if (additionalInput)
		{
			var inputArray = additionalInput.split("&");
			for (var i = 0; i < inputArray.length; i++)
			{
				var nameValue = inputArray[i].split("=");
				this._addHiddenElement(nameValue[0], decodeURIComponent(nameValue[1]));
			}
		}
	},
	
	_removeAdditionalHiddenElements : function()
	{
		var hiddens = this._hiddens;
		delete this._hiddens;
		
		for (var i = 0; i < hiddens.length; i++)
		{
			hiddens[i].parentNode.removeChild(hiddens[i]);
		}
		
		hiddens.length = 0;
	},
	
	_addHiddenElement : function(name, value)
	{
		var hidden = document.createElement("input");
		hidden.type = "hidden";
		hidden.name = name;
		hidden.value = value;
		this._form.appendChild(hidden);
		Array.add(this._hiddens, hidden);
	},
	
	_onTimeout : function()
	{
		this._timedOut = true;
		
		var iframe = this._iframe;
		delete this._iframe;
		$removeHandler(iframe, "load", this._iframeLoadCompleteHandler);
		iframe.parentNode.removeChild(iframe);
	},
	
	_clearTimer : function()
	{
		if (this._timer != null)
		{
			window.clearTimeout(this._timer);
			delete this._timer;
		}
	}
}
EFFC.Frame.Net.WebControlLib.UpdatePanelIFrameExecutor.registerClass("EFFC.Frame.Net.WebControlLib.UpdatePanelIFrameExecutor", Sys.Net.WebRequestExecutor);
EFFC.Frame.Net.WebControlLib.UpdatePanelIFrameExecutor._getHiddenContainer = function()
{
	if (!EFFC.Frame.Net.WebControlLib.UpdatePanelIFrameExecutor._hideContainer)
	{
		var hideContainer = EFFC.Frame.Net.WebControlLib.UpdatePanelIFrameExecutor._hideContainer = document.createElement("div");
		hideContainer.style.display = "none";
		document.body.appendChild(hideContainer);
	}
	
	return EFFC.Frame.Net.WebControlLib.UpdatePanelIFrameExecutor._hideContainer;
}

EFFC.Frame.Net.WebControlLib.UpdatePanelIFrameExecutor._beginRequestHandler = function(sender, e)
{
	var inputList = document.getElementsByTagName("input");
	for (var i = 0; i < inputList.length; i++)
	{
		var type = inputList[i].type;
		if (type && type.toUpperCase() == "FILE")
		{
			e.get_request().set_executor(new EFFC.Frame.Net.WebControlLib.UpdatePanelIFrameExecutor(e.get_postBackElement()));
			return;
		}
	}
}

Sys.Application.add_init(function()
{
	Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(
		EFFC.Frame.Net.WebControlLib.UpdatePanelIFrameExecutor._beginRequestHandler);
});