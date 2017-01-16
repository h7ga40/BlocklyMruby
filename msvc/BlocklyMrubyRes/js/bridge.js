var Bridge = {
	instance: null
};
Bridge.NewFunc = function(handler) {
	var ret = function() {
		var a = Bridge.instance.NewArray();
		for(i in arguments) {
			Bridge.instance.Push(a, arguments[i]);
		}
		return Bridge.instance.InvokeHandler(handler, a);
	};
	ret.handler = handler;
	return ret;
}
Bridge.New = function(name, args) {
	var ctor = eval(name);
	var obj = Object.create(ctor.prototype);
	ctor.apply(obj, args);
	return obj;
}
Bridge.ParseInt = function(value, radix) {
	return parseInt(value, radix);
}
Bridge.ParseFloat = function(value) {
	return parseFloat(value);
}
Bridge.IsNaN = function(num) {
	return isNaN(num);
}
Bridge.Get = function(scope, name) {
	return scope[name];
}
Bridge.Set = function(scope, name, value)
{
	scope[name] = value;
}
Bridge.Replace = function(str, pattern, replacement) {
	return str.replace(pattern, replacement);
}
Bridge.Match = function(str, pattern) {
	return str.match(pattern);
}
Bridge.CreateElement = function(tagname) {
	return document.createElement(tagname);
}
Bridge.CreateTextNode = function(text) {
	return document.createTextNode(text);
}
Bridge.Stringify = function(value) { return JSON.stringify(value); }
Bridge.Parse = function(text) { return JSON.parse(text); }
Bridge.EncodeURI = function(url) { return encodeURI(url); }
Bridge.DecodeURI = function(url) { return decodeURI(url); }
Bridge.NewRegExp = function(patern, flag) { return new RegExp(patern, flag); }
Bridge.goog_getMsg = function(str, opt_values) {
	return goog.getMsg(str, opt_values);
}
Bridge.goog_dom_createDom = function(v, o, t) {
	return goog.dom.createDom(v, o, t);
}
Bridge.goog_array_equals = function(a, b) {
	return goog.array.equals(a, b);
}
Bridge.goog_debug_getStacktrace = function() {
	return goog.debug.getStacktrace();
}
Bridge.show_stack = false;
Bridge.SetBlocks = function(instance)
{
	var i;
	var template = {
		template: instance,
		instance: null,
		type: instance.type,
		init: function() {
			this.instance = Bridge.instance.NewBlockInstance(this);
			if (Bridge.show_stack) {
				Bridge.show_stack = false;
				alert(Bridge.goog_debug_getStacktrace());
			}
		},
		mutationToDom: function(opt_paramIds) {
			return Bridge.instance.mutationToDom(this.instance, opt_paramIds);
		},
		domToMutation: function(xmlElement) {
			Bridge.instance.domToMutation(this.instance, xmlElement);
		},
		decompose: function(workspace) {
			return Bridge.instance.decompose(this.instance, workspace);
		},
		compose: function(containerBlock) {
			Bridge.instance.compose(this.instance, containerBlock);
		},
		saveConnections: function(containerBlock) {
			Bridge.instance.saveConnections(this.instance, containerBlock);
		},
		onchange: function(e) {
			Bridge.instance.onchange(this.instance, e);
		},
		customContextMenu: function(options) {
			Bridge.instance.customContextMenu(this.instance, options);
		},
		getProcedureCall: function() {
			return Bridge.instance.getProcedureCall(this.instance);
		},
		getProcedureDef: function() {
			return Bridge.instance.getProcedureDef(this.instance);
		},
		getVars: function() {
			return Bridge.instance.getVars(this.template);
		},
		renameProcedure: function(oldName, newName) {
			Bridge.instance.renameProcedure(this.instance, oldName, newName);
		},
		renameVar: function(oldName, newName) {
			Bridge.instance.renameVar(this.instance, oldName, newName);
		},
	};
	if (!Bridge.instance.isDefined(instance, "mutationToDom")) {
		delete template.mutationToDom;
	}
	if (!Bridge.instance.isDefined(instance, "domToMutation")) {
		delete template.domToMutation;
	}
	if (!Bridge.instance.isDefined(instance, "decompose")) {
		delete template.decompose;
	}
	if (!Bridge.instance.isDefined(instance, "compose")) {
		delete template.compose;
	}
	if (!Bridge.instance.isDefined(instance, "saveConnections")) {
		delete template.saveConnections;
	}
	if (!Bridge.instance.isDefined(instance, "onchange")) {
		delete template.onchange;
	}
	if (!Bridge.instance.isDefined(instance, "customContextMenu")) {
		delete template.customContextMenu;
	}
	if (!Bridge.instance.isDefined(instance, "getProcedureCall")) {
		delete template.getProcedureCall;
	}
	if (!Bridge.instance.isDefined(instance, "getProcedureDef")) {
		delete template.getProcedureDef;
	}
	if (!Bridge.instance.isDefined(instance, "getVars")) {
		delete template.getVars;
	}
	if (!Bridge.instance.isDefined(instance, "renameProcedure")) {
		delete template.renameProcedure;
	}
	if (!Bridge.instance.isDefined(instance, "renameVar")) {
		delete template.renameVar;
	}
	Blockly.Blocks[template.type] = template;
	return template;
}
Blockly.Names = {
	equals: function(name1, name2) {
		return Bridge.instance.names_equals(name1, name2);
	},
}
Blockly.Procedures = {
	flyoutCategory: function(workspace) {
		return Bridge.instance.procedures_flyoutCategory(workspace);
	},
}
Blockly.Variables = {
	flyoutCategory: function(workspace) {
		return Bridge.instance.variables_flyoutCategory(workspace);
	},
	allUsedVariables: function(root) {
		var blocks;
		if (root instanceof Blockly.Block) {
			// Root is Block.
			return Bridge.instance.variables_allUsedVariablesBlock(root);
		} else if (root.getAllBlocks) {
			// Root is Workspace.
			return Bridge.instance.variables_allUsedVariablesWorkspace(root);
		} else {
			throw 'Not Block or Workspace: ' + root;
		}
	},
	generateUniqueName: function(workspace) {
		return Bridge.instance.variables_generateUniqueName(workspace);
	},
	promptName: function(promptText, defaultText, callback) {
		Bridge.instance.variables_promptName(promptText, defaultText, callback);
	},
}
