﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace BlocklyMruby.Properties {
    using System;
    
    
    /// <summary>
    ///   ローカライズされた文字列などを検索するための、厳密に型指定されたリソース クラスです。
    /// </summary>
    // このクラスは StronglyTypedResourceBuilder クラスが ResGen
    // または Visual Studio のようなツールを使用して自動生成されました。
    // メンバーを追加または削除するには、.ResX ファイルを編集して、/str オプションと共に
    // ResGen を実行し直すか、または VS プロジェクトをビルドし直します。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   このクラスで使用されているキャッシュされた ResourceManager インスタンスを返します。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BlocklyMruby.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   厳密に型指定されたこのリソース クラスを使用して、すべての検索リソースに対し、
        ///   現在のスレッドの CurrentUICulture プロパティをオーバーライドします。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   &lt;!doctype html&gt;
        ///&lt;html lang=&quot;ja-jp&quot;&gt;
        ///&lt;head&gt;
        ///	&lt;meta charset=&quot;utf-8&quot; /&gt;
        ///	&lt;meta http-equiv=&quot;X-UA-Compatible&quot; content=&quot;IE=edge&quot;&gt;
        ///	&lt;meta name=&quot;viewport&quot; content=&quot;width=device-width, initial-scale=1&quot;&gt;
        ///	&lt;title&gt;Ruby Editor&lt;/title&gt;
        ///&lt;/head&gt;
        ///&lt;body&gt;
        ///	&lt;div class=&quot;ace_editor ace-clouds&quot; id=&quot;text-editor&quot;&gt;&lt;textarea style=&quot;opacity: 0;&quot; spellcheck=&quot;false&quot; class=&quot;ace_text-input&quot; nowrap&gt;&lt;/textarea&gt;&lt;div class=&quot;ace_gutter&quot;&gt;&lt;div class=&quot;ace_layer ace_gutter-layer ace_folding-enabled&quot;&gt;&lt;/div&gt;&lt;div class=&quot;ace_gutter-active-l [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ace_html {
            get {
                return ResourceManager.GetString("ace_html", resourceCulture);
            }
        }
        
        /// <summary>
        ///   /* ***** BEGIN LICENSE BLOCK *****
        /// * Distributed under the BSD license:
        /// *
        /// * Copyright (c) 2010, Ajax.org B.V.
        /// * All rights reserved.
        /// *
        /// * Redistribution and use in source and binary forms, with or without
        /// * modification, are permitted provided that the following conditions are met:
        /// *     * Redistributions of source code must retain the above copyright
        /// *       notice, this list of conditions and the following disclaimer.
        /// *     * Redistributions in binary form must reproduce the above copyr [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ace_js {
            get {
                return ResourceManager.GetString("ace_js", resourceCulture);
            }
        }
        
        /// <summary>
        ///   // Do not edit this file; automatically generated by build.py.
        ///&apos;use strict&apos;;
        ///
        ///var COMPILED = !0,
        ///	goog = goog || {};
        ///goog.global = this;
        ///goog.isDef = function (a) {
        ///	return void 0 !== a
        ///};
        ///goog.exportPath_ = function (a, b, c) {
        ///	a = a.split(&quot;.&quot;);
        ///	c = c || goog.global;
        ///	a[0] in c || !c.execScript || c.execScript(&quot;var &quot; + a[0]);
        ///	for (var d; a.length &amp;&amp; (d = a.shift()) ;) !a.length &amp;&amp; goog.isDef(b) ? c[d] = b : c = c[d] ? c[d] : c[d] = {}
        ///};
        ///goog.define = function (a, b) {
        ///	var c = b;
        ///	COMPILED || (goog.glo [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string blockly_js {
            get {
                return ResourceManager.GetString("blockly_js", resourceCulture);
            }
        }
        
        /// <summary>
        ///   /*
        /// *  Fit terminal columns and rows to the dimensions of its
        /// *  DOM element.
        /// *
        /// *  Approach:
        /// *    - Rows: Truncate the division of the terminal parent element height
        /// *            by the terminal row height
        /// *
        /// *    - Columns: Truncate the division of the terminal parent element width by
        /// *               the terminal character width (apply display: inline at the
        /// *               terminal row and truncate its width with the current number
        /// *               of columns)
        /// */
        ///(function (fit) {
        ///  [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string fit_js {
            get {
                return ResourceManager.GetString("fit_js", resourceCulture);
            }
        }
        
        /// <summary>
        ///   型 System.Drawing.Bitmap のローカライズされたリソースを検索します。
        /// </summary>
        internal static System.Drawing.Bitmap icon {
            get {
                object obj = ResourceManager.GetObject("icon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   &lt;!doctype html&gt;
        ///&lt;html lang=&quot;ja-jp&quot;&gt;
        ///&lt;head&gt;
        ///	&lt;meta charset=&quot;utf-8&quot; /&gt;
        ///	&lt;meta http-equiv=&quot;X-UA-Compatible&quot; content=&quot;IE=edge&quot;&gt;
        ///	&lt;meta name=&quot;viewport&quot; content=&quot;width=device-width, initial-scale=1&quot;&gt;
        ///	&lt;title&gt;Blockly Demo: Resizable Blockly (Part 2)&lt;/title&gt;
        ///	&lt;style&gt;
        ///		html, body {
        ///			height: 100%;
        ///			margin: 0;
        ///		}
        ///
        ///		body {
        ///			background-color: #fff;
        ///			font-family: sans-serif;
        ///			overflow: hidden;
        ///		}
        ///
        ///		h1 {
        ///			font-weight: normal;
        ///			font-size: 140%;
        ///		}
        ///
        ///		table {
        ///			height: 100%;
        ///		 [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string index_html {
            get {
                return ResourceManager.GetString("index_html", resourceCulture);
            }
        }
        
        /// <summary>
        ///   define(&quot;ace/mode/ruby_highlight_rules&quot;,[&quot;require&quot;,&quot;exports&quot;,&quot;module&quot;,&quot;ace/lib/oop&quot;,&quot;ace/mode/text_highlight_rules&quot;], function(require, exports, module) {
        ///&quot;use strict&quot;;
        ///
        ///var oop = require(&quot;../lib/oop&quot;);
        ///var TextHighlightRules = require(&quot;./text_highlight_rules&quot;).TextHighlightRules;
        ///var constantOtherSymbol = exports.constantOtherSymbol = {
        ///    token : &quot;constant.other.symbol.ruby&quot;, // symbol
        ///    regex : &quot;[:](?:[A-Za-z_]|[@$](?=[a-zA-Z0-9_]))[a-zA-Z0-9_]*[!=?]?&quot;
        ///};
        ///
        ///var qString = exports.qString = {
        ///  [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string mode_ruby_js {
            get {
                return ResourceManager.GetString("mode_ruby_js", resourceCulture);
            }
        }
        
        /// <summary>
        ///   define(&quot;ace/theme/twilight&quot;,[&quot;require&quot;,&quot;exports&quot;,&quot;module&quot;,&quot;ace/lib/dom&quot;], function(require, exports, module) {
        ///
        ///exports.isDark = true;
        ///exports.cssClass = &quot;ace-twilight&quot;;
        ///exports.cssText = &quot;.ace-twilight .ace_gutter {\
        ///background: #232323;\
        ///color: #E2E2E2\
        ///}\
        ///.ace-twilight .ace_print-margin {\
        ///width: 1px;\
        ///background: #232323\
        ///}\
        ///.ace-twilight {\
        ///background-color: #141414;\
        ///color: #F8F8F8\
        ///}\
        ///.ace-twilight .ace_cursor {\
        ///color: #A7A7A7\
        ///}\
        ///.ace-twilight .ace_marker-layer .ace_selection {\        /// [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string theme_twilight_js {
            get {
                return ResourceManager.GetString("theme_twilight_js", resourceCulture);
            }
        }
        
        /// <summary>
        ///   /**
        /// * xterm.js: xterm, in the browser
        /// * Copyright (c) 2014, sourceLair Limited (www.sourcelair.com (MIT License)
        /// * Copyright (c) 2012-2013, Christopher Jeffrey (MIT License)
        /// * https://github.com/chjj/term.js
        /// *
        /// * Permission is hereby granted, free of charge, to any person obtaining a copy
        /// * of this software and associated documentation files (the &quot;Software&quot;), to deal
        /// * in the Software without restriction, including without limitation the rights
        /// * to use, copy, modify, merge, publish, distri [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string xterm_css {
            get {
                return ResourceManager.GetString("xterm_css", resourceCulture);
            }
        }
        
        /// <summary>
        ///   &lt;!doctype html&gt;
        ///&lt;html lang=&quot;ja-jp&quot;&gt;
        ///&lt;head&gt;
        ///	&lt;meta charset=&quot;utf-8&quot; /&gt;
        ///	&lt;meta http-equiv=&quot;X-UA-Compatible&quot; content=&quot;IE=edge&quot;&gt;
        ///	&lt;meta name=&quot;viewport&quot; content=&quot;width=device-width, initial-scale=1&quot;&gt;
        ///	&lt;title&gt;Terminal&lt;/title&gt;
        ///&lt;/head&gt;
        ///&lt;body&gt;
        ///	&lt;div id=&quot;terminal&quot;&gt; &lt;/div&gt;
        ///	&lt;script&gt;
        ///		function start_xterm() {
        ///			var term = new Terminal({
        ///				cols: 80,
        ///				rows: 24,
        ///				useStyle: true,
        ///				screenKeys: true,
        ///				cursorBlink: true
        ///			});
        ///			term.open(document.getElementById(&apos;terminal&apos;));
        ///			external.te [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string xterm_html {
            get {
                return ResourceManager.GetString("xterm_html", resourceCulture);
            }
        }
        
        /// <summary>
        ///   /**
        /// * xterm.js: xterm, in the browser
        /// * Copyright (c) 2014, sourceLair Limited (www.sourcelair.com (MIT License)
        /// * Copyright (c) 2012-2013, Christopher Jeffrey (MIT License)
        /// * https://github.com/chjj/term.js
        /// *
        /// * Permission is hereby granted, free of charge, to any person obtaining a copy
        /// * of this software and associated documentation files (the &quot;Software&quot;), to deal
        /// * in the Software without restriction, including without limitation the rights
        /// * to use, copy, modify, merge, publish, distri [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string xterm_js {
            get {
                return ResourceManager.GetString("xterm_js", resourceCulture);
            }
        }
    }
}
