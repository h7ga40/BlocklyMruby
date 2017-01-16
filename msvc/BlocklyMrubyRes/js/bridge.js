if (typeof external.CreateXMLHttpRequest != "undefined") {
	window.XMLHttpRequest = function () {
		return external.CreateXMLHttpRequest();
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
	window.console = {
		log: function (text) { external.console_log(text); },
		warn: function (text) { external.console_warn(text); }
	};
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
	ret.handler = handler;
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

external.bridge = Bridge;
