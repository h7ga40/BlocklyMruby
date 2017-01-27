if (typeof external.CreateXMLHttpRequest != "undefined") {
	window.console = {
		log: function (text) { external.console_log(text); },
		warn: function (text) { external.console_warn(text); }
	}
	window.onerror = function (msg, src, line, column, _exc) {
		var exc = external.new_object();
		for (m in _exc) {
			external.object_add(exc, m, _exc[m]);
		}
		external.onerror(msg, src, line, column, exc);
	}
	external.Error = window.Error;
	window.Error = function () {
		var nullPtr = null;
		nullPtr(); // スタックトレースを表示するためnull参照
		return external.new_error();
	}
	window.XMLHttpRequest = function () {
		var ret = {
			instance: null,
			abort: function (text) { this.instance.abort(text); },
			getAllResponseHeaders: function () { return this.instance.getAllResponseHeaders(); },
			onabort: null,
			onload: null,
			onreadystatechange: null,
			open: function (type, url, async, username, password) { this.instance.open(type, url, async, username, password); },
			overrideMimeType: function (mimeType) { this.instance.overrideMimeType(mimeType); },
			readyState: null,
			response: null,
			responseText: null,
			responseType: null,
			send: function (header) { this.instance.send(header); },
			setRequestHeader: function (name, value) { this.instance.setRequestHeader(name, value); },
			status: null,
			statusText: null,
		}
		ret.instance = window.external.CreateXMLHttpRequest();
		ret.instance.instance = ret;
		return ret;
	}
}
if (typeof external.CreateWebSocket != "undefined") {
	global.WebSocket = function (url, protocol) {
		return external.CreateWebSocket(url, protocol);
	}
}
else if (typeof NiseWebSocket != "undefined") {
	global.WebSocket = function (url, protocol) {
		return new NiseWebSocket(url, protocol);
	}
}
var Bridge = {
	instance: null
};
Bridge.NewFunc = function (handler) {
	var ret = function () {
		var a = Bridge.instance.new_array();
		for (i in arguments) {
			Bridge.instance.array_add(a, arguments[i]);
		}
		return Bridge.instance.InvokeHandler(handler, a);
	};
	ret.instance = handler;
	return ret;
}
Bridge.New = function (name, args) {
	var ctor = eval(name);
	var obj = Object.create(ctor.prototype);
	ctor.apply(obj, args);
	return obj;
}
Bridge.ParseInt = function (value, radix) {
	return parseInt(value, radix);
}
Bridge.ParseFloat = function (value) {
	return parseFloat(value);
}
Bridge.IsNaN = function (num) {
	return isNaN(num);
}
Bridge.Get = function (scope, name) {
	return scope[name];
}
Bridge.Set = function (scope, name, value) {
	scope[name] = value;
}
Bridge.Replace = function (str, pattern, replacement) {
	return str.replace(pattern, replacement);
}
Bridge.Split = function (str, pattern) {
	return str.split(pattern);
}
Bridge.Match = function (str, pattern) {
	return str.match(pattern);
}
Bridge.CreateElement = function (tagname) {
	return document.createElement(tagname);
}
Bridge.CreateTextNode = function (text) {
	return document.createTextNode(text);
}
Bridge.GetElementById = function (id) {
	return document.getElementById(id);
}
Bridge.PreventDefault = function (ev) {
	ev.preventDefault();
}
Bridge.StopPropagation = function (ev) {
	ev.stopPropagation();
}
Bridge.Stringify = function (value) { return JSON.stringify(value); }
Bridge.Parse = function (text) { return JSON.parse(text); }
Bridge.EncodeURI = function (url) { return encodeURI(url); }
Bridge.DecodeURI = function (url) { return decodeURI(url); }
Bridge.NewRegExp = function (patern, flag) { return new RegExp(patern, flag); }
Bridge.RegExpEscape = function (s) { return RegExp.escape(s); }
Bridge.ParseXML = function (data) {
	return (new window.DOMParser()).parseFromString(data, "text/xml");
}
Bridge.jqNew = function (v) {
	return $(v);
}
Bridge.Select = function (selector) {
	return $(selector);
}
Bridge.Select2 = function (selector, parent) {
	return $(selector, parent);
}
Bridge.Attr = function (obj, attr) {
	return obj.attr(attr);
}
Bridge.Attr2 = function (obj, attr, val) {
	obj.attr(attr, val);
}
Bridge.ReplaceWith = function (obj, str) {
	obj.replaceWith(str);
}
Bridge.Val = function (obj) {
	return obj.val();
}
Bridge.Val1 = function (obj, val) {
	obj.val(val);
}
Bridge.jqGet = function (obj) {
	return obj.get();
}
Bridge.jqGet2 = function (obj, val) {
	return obj.get(val);
}
Bridge.Parent = function (obj) {
	return obj.parent();
}
Bridge.AppendChild = function (obj, ele) {
	obj.appendChild(ele);
}
Bridge.Text = function (obj) {
	return obj.text();
}
Bridge.Text2 = function (obj, text) {
	obj.text(text);
}
Bridge.Html = function (obj) {
	return obj.html();
}
Bridge.Html2 = function (obj, html) {
	obj.html(html);
}
Bridge.RemoveAttr = function (obj, attr) {
	obj.removeAttr(attr);
}
Bridge.Click = function (obj, state, callback) {
	obj.click(state, callback);
}
Bridge.ButtonToggle = function (obj) {
	return obj.button('toggle');
}
Bridge.Is = function (obj, val) {
	return obj.is(val);
}
Bridge.Children = function (obj) {
	return obj.children();
}
Bridge.Find = function (obj, selector) {
	return obj.find(selector);
}
Bridge.Remove = function (obj) {
	return obj.remove();
}
Bridge.Append = function (obj, content) {
	return obj.append(content);
}
Bridge.Show = function (obj) {
	return obj.show();
}
Bridge.Hide = function (obj) {
	return obj.hide();
}
Bridge.On = function (obj, events, handler) {
	return obj.on(events, handler);
}
external.bridge = Bridge;
