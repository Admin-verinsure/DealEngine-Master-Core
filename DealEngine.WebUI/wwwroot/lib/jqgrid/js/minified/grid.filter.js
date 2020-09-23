/**
*
* @license Guriddo jqGrid JS - v5.5.0 
* Copyright(c) 2008, Tony Tomov, tony@trirand.com
* 
* License: http://guriddo.net/?page_id=103334
*/
!function(a){"use strict";"function"==typeof define&&define.amd?define(["jquery","./grid.base","./grid.common"],a):a(jQuery)}(function(a){"use strict";a.fn.jqFilter=function(b){if("string"==typeof b){var c=a.fn.jqFilter[b];if(!c)throw"jqFilter - No such method: "+b;var d=a.makeArray(arguments).slice(1);return c.apply(this,d)}var e=a.extend(!0,{filter:null,columns:[],sortStrategy:null,onChange:null,afterRedraw:null,checkValues:null,error:!1,errmsg:"",errorcheck:!0,showQuery:!0,sopt:null,ops:[],operands:null,numopts:["eq","ne","lt","le","gt","ge","nu","nn","in","ni"],stropts:["eq","ne","bw","bn","ew","en","cn","nc","nu","nn","in","ni"],strarr:["text","string","blob"],groupOps:[{op:"AND",text:"AND"},{op:"OR",text:"OR"}],groupButton:!0,ruleButtons:!0,uniqueSearchFields:!1,direction:"ltr",addsubgrup:"Add subgroup",addrule:"Add rule",delgroup:"Delete group",delrule:"Delete rule",autoencode:!1,unaryOperations:[]},a.jgrid.filter,b||{});return this.each(function(){if(!this.filter){this.p=e,null!==this.p.filter&&void 0!==this.p.filter||(this.p.filter={groupOp:this.p.groupOps[0].op,rules:[],groups:[]}),null!=this.p.sortStrategy&&a.isFunction(this.p.sortStrategy)&&this.p.columns.sort(this.p.sortStrategy);var b,c,d=this.p.columns.length,f=/msie/i.test(navigator.userAgent)&&!window.opera;if(this.p.initFilter=a.extend(!0,{},this.p.filter),d){for(b=0;b<d;b++)c=this.p.columns[b],c.stype?c.inputtype=c.stype:c.inputtype||(c.inputtype="text"),c.sorttype?c.searchtype=c.sorttype:c.searchtype||(c.searchtype="string"),void 0===c.hidden&&(c.hidden=!1),c.label||(c.label=c.name),c.index&&(c.name=c.index),c.hasOwnProperty("searchoptions")||(c.searchoptions={}),c.hasOwnProperty("searchrules")||(c.searchrules={}),void 0===c.search?c.inlist=!0:c.inlist=c.search;var g=function(){return a("#"+a.jgrid.jqID(e.id))[0]||null},h=g(),i=a.jgrid.styleUI[h.p.styleUI||"jQueryUI"].filter,j=a.jgrid.styleUI[h.p.styleUI||"jQueryUI"].common;this.p.showQuery&&a(this).append("<table class='queryresult "+i.table_widget+"' style='display:block;max-width:440px;border:0px none;' dir='"+this.p.direction+"'><tbody><tr><td class='query'></td></tr></tbody></table>");var k=function(b,c){var d=[!0,""],f=g();if(a.isFunction(c.searchrules))d=c.searchrules.call(f,b,c);else if(a.jgrid&&a.jgrid.checkValues)try{d=a.jgrid.checkValues.call(f,b,-1,c.searchrules,c.label)}catch(a){}d&&d.length&&!1===d[0]&&(e.error=!d[0],e.errmsg=d[1])};this.onchange=function(){return this.p.error=!1,this.p.errmsg="",!!a.isFunction(this.p.onChange)&&this.p.onChange.call(this,this.p)},this.reDraw=function(){a("table.group:first",this).remove();var b=this.createTableForGroup(e.filter,null);a(this).append(b),a.isFunction(this.p.afterRedraw)&&this.p.afterRedraw.call(this,this.p)},this.createTableForGroup=function(b,c){var d,f=this,g=a("<table class='group "+i.table_widget+" ui-search-table' style='border:0px none;'><tbody></tbody></table>"),h="left";"rtl"===this.p.direction&&(h="right",g.attr("dir","rtl")),null===c&&g.append("<tr class='error' style='display:none;'><th colspan='5' class='"+j.error+"' align='"+h+"'></th></tr>");var k=a("<tr></tr>");g.append(k);var l=a("<th colspan='5' align='"+h+"'></th>");if(k.append(l),!0===this.p.ruleButtons){var m=a("<select size='1' class='opsel "+i.srSelect+"'></select>");l.append(m);var n,o="";for(d=0;d<e.groupOps.length;d++)n=b.groupOp===f.p.groupOps[d].op?" selected='selected'":"",o+="<option value='"+f.p.groupOps[d].op+"'"+n+">"+f.p.groupOps[d].text+"</option>";m.append(o).on("change",function(){b.groupOp=a(m).val(),f.onchange()})}var p="<span></span>";if(this.p.groupButton&&(p=a("<input type='button' value='+ {}' title='"+f.p.subgroup+"' class='add-group "+j.button+"'/>"),p.on("click",function(){return void 0===b.groups&&(b.groups=[]),b.groups.push({groupOp:e.groupOps[0].op,rules:[],groups:[]}),f.reDraw(),f.onchange(),!1})),l.append(p),!0===this.p.ruleButtons){var q,r=a("<input type='button' value='+' title='"+f.p.addrule+"' class='add-rule ui-add "+j.button+"'/>");r.on("click",function(){for(void 0===b.rules&&(b.rules=[]),d=0;d<f.p.columns.length;d++){var c=void 0===f.p.columns[d].search||f.p.columns[d].search,e=!0===f.p.columns[d].hidden;if(!0===f.p.columns[d].searchoptions.searchhidden&&c||c&&!e){q=f.p.columns[d];break}}if(!q)return!1;var g;return g=q.searchoptions.sopt?q.searchoptions.sopt:f.p.sopt?f.p.sopt:-1!==a.inArray(q.searchtype,f.p.strarr)?f.p.stropts:f.p.numopts,b.rules.push({field:q.name,op:g[0],data:""}),f.reDraw(),!1}),l.append(r)}if(null!==c){var s=a("<input type='button' value='-' title='"+f.p.delgroup+"' class='delete-group "+j.button+"'/>");l.append(s),s.on("click",function(){for(d=0;d<c.groups.length;d++)if(c.groups[d]===b){c.groups.splice(d,1);break}return f.reDraw(),f.onchange(),!1})}if(void 0!==b.groups)for(d=0;d<b.groups.length;d++){var t=a("<tr></tr>");g.append(t);var u=a("<td class='first'></td>");t.append(u);var v=a("<td colspan='4'></td>");v.append(this.createTableForGroup(b.groups[d],b)),t.append(v)}void 0===b.groupOp&&(b.groupOp=f.p.groupOps[0].op);var w,x=f.p.ruleButtons&&f.p.uniqueSearchFields;if(x)for(w=0;w<f.p.columns.length;w++)f.p.columns[w].inlist&&(f.p.columns[w].search=!0);if(void 0!==b.rules)for(d=0;d<b.rules.length;d++)if(g.append(this.createTableRowForRule(b.rules[d],b)),x){var y=b.rules[d].field;for(w=0;w<f.p.columns.length;w++)if(y===f.p.columns[w].name){f.p.columns[w].search=!1;break}}return g},this.createTableRowForRule=function(b,c){var d,h,k,l,m,n=this,o=g(),p=a("<tr></tr>"),q="";p.append("<td class='first'></td>");var r=a("<td class='columns'></td>");p.append(r);var s,t=a("<select size='1' class='"+i.srSelect+"'></select>"),u=[];r.append(t),t.on("change",function(){if(n.p.ruleButtons&&n.p.uniqueSearchFields){var c=parseInt(a(this).data("curr"),10),e=this.selectedIndex;c>=0&&(n.p.columns[c].search=!0,a(this).data("curr",e),n.p.columns[e].search=!1)}for(b.field=a(t).val(),k=a(this).parents("tr:first"),a(".data",k).empty(),d=0;d<n.p.columns.length;d++)if(n.p.columns[d].name===b.field){l=n.p.columns[d];break}if(l){l.searchoptions.id=a.jgrid.randId(),l.searchoptions.name=b.field,l.searchoptions.oper="filter",f&&"text"===l.inputtype&&(l.searchoptions.size||(l.searchoptions.size=10));var g=a.jgrid.createEl.call(o,l.inputtype,l.searchoptions,"",!0,n.p.ajaxSelectOptions||{},!0);a(g).addClass("input-elm "+i.srInput),h=l.searchoptions.sopt?l.searchoptions.sopt:n.p.sopt?n.p.sopt:-1!==a.inArray(l.searchtype,n.p.strarr)?n.p.stropts:n.p.numopts;var j="",m=0;for(u=[],a.each(n.p.ops,function(){u.push(this.oper)}),d=0;d<h.length;d++)-1!==(s=a.inArray(h[d],u))&&(0===m&&(b.op=n.p.ops[s].oper),j+="<option value='"+n.p.ops[s].oper+"'>"+n.p.ops[s].text+"</option>",m++);if(a(".selectopts",k).empty().append(j),a(".selectopts",k)[0].selectedIndex=0,a.jgrid.msie()&&a.jgrid.msiever()<9){var p=parseInt(a("select.selectopts",k)[0].offsetWidth,10)+1;a(".selectopts",k).width(p),a(".selectopts",k).css("width","auto")}a(".data",k).append(g),a.jgrid.bindEv.call(o,g,l.searchoptions),a(".input-elm",k).on("change",function(c){var d=c.target;"custom"===l.inputtype&&a.isFunction(l.searchoptions.custom_value)?b.data=l.searchoptions.custom_value.call(o,a(".customelement",this),"get"):b.data=a(d).val(),"select"===l.inputtype&&l.searchoptions.multiple&&(b.data=b.data.join(",")),n.onchange()}),setTimeout(function(){b.data=a(g).val(),("nu"===b.op||"nn"===b.op||a.inArray(b.op,n.p.unaryOperations)>=0)&&(a(g).attr("readonly","true"),a(g).attr("disabled","true")),"select"===l.inputtype&&l.searchoptions.multiple&&a.isArray(b.data)&&(b.data=b.data.join(",")),n.onchange()},0)}});var v=0;for(d=0;d<n.p.columns.length;d++){var w=void 0===n.p.columns[d].search||n.p.columns[d].search,x=!0===n.p.columns[d].hidden;(!0===n.p.columns[d].searchoptions.searchhidden&&w||w&&!x)&&(m="",b.field===n.p.columns[d].name&&(m=" selected='selected'",v=d),q+="<option value='"+n.p.columns[d].name+"'"+m+">"+n.p.columns[d].label+"</option>")}t.append(q),t.data("curr",v);var y=a("<td class='operators'></td>");p.append(y),l=e.columns[v],l.searchoptions.id=a.jgrid.randId(),f&&"text"===l.inputtype&&(l.searchoptions.size||(l.searchoptions.size=10)),l.searchoptions.name=b.field,l.searchoptions.oper="filter";var z=a.jgrid.createEl.call(o,l.inputtype,l.searchoptions,b.data,!0,n.p.ajaxSelectOptions||{},!0);("nu"===b.op||"nn"===b.op||a.inArray(b.op,n.p.unaryOperations)>=0)&&(a(z).attr("readonly","true"),a(z).attr("disabled","true"));var A=a("<select size='1' class='selectopts "+i.srSelect+"'></select>");for(y.append(A),A.on("change",function(){b.op=a(A).val(),k=a(this).parents("tr:first");var c=a(".input-elm",k)[0];"nu"===b.op||"nn"===b.op||a.inArray(b.op,n.p.unaryOperations)>=0?(b.data="","SELECT"!==c.tagName.toUpperCase()&&(c.value=""),c.setAttribute("readonly","true"),c.setAttribute("disabled","true")):("SELECT"===c.tagName.toUpperCase()&&(b.data=c.value),c.removeAttribute("readonly"),c.removeAttribute("disabled")),n.onchange()}),h=l.searchoptions.sopt?l.searchoptions.sopt:n.p.sopt?n.p.sopt:-1!==a.inArray(l.searchtype,n.p.strarr)?n.p.stropts:n.p.numopts,q="",a.each(n.p.ops,function(){u.push(this.oper)}),d=0;d<h.length;d++)-1!==(s=a.inArray(h[d],u))&&(m=b.op===n.p.ops[s].oper?" selected='selected'":"",q+="<option value='"+n.p.ops[s].oper+"'"+m+">"+n.p.ops[s].text+"</option>");A.append(q);var B=a("<td class='data'></td>");p.append(B),B.append(z),a.jgrid.bindEv.call(o,z,l.searchoptions),a(z).addClass("input-elm "+i.srInput).on("change",function(){b.data="custom"===l.inputtype?l.searchoptions.custom_value.call(o,a(".customelement",this),"get"):a(this).val(),n.onchange()});var C=a("<td></td>");if(p.append(C),!0===this.p.ruleButtons){var D=a("<input type='button' value='-' title='"+n.p.delrule+"' class='delete-rule ui-del "+j.button+"'/>");C.append(D),D.on("click",function(){for(d=0;d<c.rules.length;d++)if(c.rules[d]===b){c.rules.splice(d,1);break}return n.reDraw(),n.onchange(),!1})}return p},this.getStringForGroup=function(a){var b,c="(";if(void 0!==a.groups)for(b=0;b<a.groups.length;b++){c.length>1&&(c+=" "+a.groupOp+" ");try{c+=this.getStringForGroup(a.groups[b])}catch(a){alert(a)}}if(void 0!==a.rules)try{for(b=0;b<a.rules.length;b++)c.length>1&&(c+=" "+a.groupOp+" "),c+=this.getStringForRule(a.rules[b])}catch(a){alert(a)}return c+=")","()"===c?"":c},this.getStringForRule=function(b){var c,d,f,g="",h="",i=["int","integer","float","number","currency"];for(c=0;c<this.p.ops.length;c++)if(this.p.ops[c].oper===b.op){g=this.p.operands.hasOwnProperty(b.op)?this.p.operands[b.op]:"",h=this.p.ops[c].oper;break}for(c=0;c<this.p.columns.length;c++)if(this.p.columns[c].name===b.field){d=this.p.columns[c];break}return void 0===d?"":(f=this.p.autoencode?a.jgrid.htmlEncode(b.data):b.data,"bw"!==h&&"bn"!==h||(f+="%"),"ew"!==h&&"en"!==h||(f="%"+f),"cn"!==h&&"nc"!==h||(f="%"+f+"%"),"in"!==h&&"ni"!==h||(f=" ("+f+")"),e.errorcheck&&k(b.data,d),-1!==a.inArray(d.searchtype,i)||"nn"===h||"nu"===h||a.inArray(b.op,this.p.unaryOperations)>=0?b.field+" "+g+" "+f:b.field+" "+g+' "'+f+'"')},this.resetFilter=function(){this.p.filter=a.extend(!0,{},this.p.initFilter),this.reDraw(),this.onchange()},this.hideError=function(){a("th."+j.error,this).html(""),a("tr.error",this).hide()},this.showError=function(){a("th."+j.error,this).html(this.p.errmsg),a("tr.error",this).show()},this.toUserFriendlyString=function(){return this.getStringForGroup(e.filter)},this.toString=function(){function a(a){if(c.p.errorcheck){var b,d;for(b=0;b<c.p.columns.length;b++)if(c.p.columns[b].name===a.field){d=c.p.columns[b];break}d&&k(a.data,d)}return a.op+"(item."+a.field+",'"+a.data+"')"}function b(c){var d,e="(";if(void 0!==c.groups)for(d=0;d<c.groups.length;d++)e.length>1&&("OR"===c.groupOp?e+=" || ":e+=" && "),e+=b(c.groups[d]);if(void 0!==c.rules)for(d=0;d<c.rules.length;d++)e.length>1&&("OR"===c.groupOp?e+=" || ":e+=" && "),e+=a(c.rules[d]);return e+=")","()"===e?"":e}var c=this;return b(this.p.filter)},this.reDraw(),this.p.showQuery&&this.onchange(),this.filter=!0}}})},a.extend(a.fn.jqFilter,{toSQLString:function(){var a="";return this.each(function(){a=this.toUserFriendlyString()}),a},filterData:function(){var a;return this.each(function(){a=this.p.filter}),a},getParameter:function(a){var b=null;return void 0!==a&&this.each(function(c,d){d.p.hasOwnProperty(a)&&(b=d.p[a])}),b||this[0].p},resetFilter:function(){return this.each(function(){this.resetFilter()})},addFilter:function(b){"string"==typeof b&&(b=a.jgrid.parse(b)),this.each(function(){this.p.filter=b,this.reDraw(),this.onchange()})}}),a.extend(a.jgrid,{filterRefactor:function(b){var c,d,e,f,g,h={};try{if(h="string"==typeof b.ruleGroup?a.jgrid.parse(b.ruleGroup):b.ruleGroup,h.rules&&h.rules.length)for(c=h.rules,d=0;d<c.length;d++)e=c[d],a.inArray(e.filed,b.ssfield)&&(f=e.data.split(b.splitSelect),f.length>1&&(void 0===h.groups&&(h.groups=[]),g={groupOp:b.groupOpSelect,groups:[],rules:[]},h.groups.push(g),a.each(f,function(a){f[a]&&g.rules.push({data:f[a],op:e.op,field:e.field})}),c.splice(d,1),d--))}catch(a){}return h}}),a.jgrid.extend({filterToolbar:function(b){var c=a.jgrid.getRegional(this[0],"search");return b=a.extend({autosearch:!0,autosearchDelay:500,searchOnEnter:!0,beforeSearch:null,afterSearch:null,beforeClear:null,afterClear:null,onClearSearchValue:null,url:"",stringResult:!1,groupOp:"AND",defaultSearch:"bw",searchOperators:!1,resetIcon:"x",splitSelect:",",groupOpSelect:"OR",errorcheck:!0,operands:{eq:"==",ne:"!",lt:"<",le:"<=",gt:">",ge:">=",bw:"^",bn:"!^",in:"=",ni:"!=",ew:"|",en:"!@",cn:"~",nc:"!~",nu:"#",nn:"!#",bt:"..."}},c,b||{}),this.each(function(){var d=this,e=[];if(!d.p.filterToolbar){if(a(d).data("filterToolbar")||a(d).data("filterToolbar",b),d.p.force_regional&&(b=a.extend(b,c)),void 0!==d.p.customFilterDef)for(var f in d.p.customFilterDef)d.p.customFilterDef.hasOwnProperty(f)&&!b.operands.hasOwnProperty(f)&&(b.odata.push({oper:f,text:d.p.customFilterDef[f].text}),b.operands[f]=d.p.customFilterDef[f].operand,!0===d.p.customFilterDef[f].unary&&e.push(f));var g,h,i,j=a.jgrid.styleUI[d.p.styleUI||"jQueryUI"].filter,k=a.jgrid.styleUI[d.p.styleUI||"jQueryUI"].common,l=a.jgrid.styleUI[d.p.styleUI||"jQueryUI"].base,m=function(){var c,f,g,h,i={},j=0,k={},l=!1,m=[],n=!1,o=[!0,"",""],p=!1;if(a.each(d.p.colModel,function(){var q=a("#gs_"+d.p.idPrefix+a.jgrid.jqID(this.name),!0===this.frozen&&!0===d.p.frozenColumns?d.grid.fhDiv:d.grid.hDiv);if(f=this.index||this.name,h=this.searchoptions||{},g=b.searchOperators&&h.searchOperMenu?q.parent().prev().children("a").attr("soper")||b.defaultSearch:h.sopt?h.sopt[0]:"select"===this.stype?"eq":b.defaultSearch,c="custom"===this.stype&&a.isFunction(h.custom_value)&&q.length>0?h.custom_value.call(d,q,"get"):q.val(),"select"===this.stype&&h.multiple&&a.isArray(c)&&(c.length>0?(l=!0,m.push(f),c=1===c.length?c[0]:c):c=""),this.searchrules&&b.errorcheck&&(a.isFunction(this.searchrules)?o=this.searchrules.call(d,c,this):a.jgrid&&a.jgrid.checkValues&&(o=a.jgrid.checkValues.call(d,c,-1,this.searchrules,this.label||this.name)),o&&o.length&&!1===o[0]))return this.searchrules.hasOwnProperty("validationError")&&(p=this.searchrules.validationError),!1;if("bt"===g&&(n=!0),c||"nu"===g||"nn"===g||a.inArray(g,e)>=0)i[f]=c,k[f]=g,j++;else try{delete d.p.postData[f]}catch(a){}}),!1!==o[0]){var q=j>0;if(!0===b.stringResult||"local"===d.p.datatype||!0===b.searchOperators){var r='{"groupOp":"'+b.groupOp+'","rules":[',s=0;a.each(i,function(a,b){s>0&&(r+=","),r+='{"field":"'+a+'",',r+='"op":"'+k[a]+'",',b+="",r+='"data":"'+b.replace(/\\/g,"\\\\").replace(/\"/g,'\\"')+'"}',s++}),r+="]}";var t,u,v,w,x,y,z;if(l&&(t=a.jgrid.filterRefactor({ruleGroup:r,ssfield:m,splitSelect:b.splitSelect,groupOpSelect:b.groupOpSelect}),r=JSON.stringify(t)),n&&(a.isPlainObject(t)||(t=a.jgrid.parse(r)),t.rules&&t.rules.length))for(u=t.rules,v=0;v<u.length;v++)x=u[v],"bt"===x.op&&(y=x.data.split("..."),y.length>1&&(void 0===t.groups&&(t.groups=[]),z={groupOp:"AND",groups:[],rules:[]},t.groups.push(z),a.each(y,function(a){var b=0===a?"ge":"le";(w=y[a])&&z.rules.push({data:y[a],op:b,field:x.field})}),u.splice(v,1),v--));(n||l)&&(r=JSON.stringify(t)),a.extend(d.p.postData,{filters:r}),a.each(["searchField","searchString","searchOper"],function(a,b){d.p.postData.hasOwnProperty(b)&&delete d.p.postData[b]})}else a.extend(d.p.postData,i);var A;b.url&&(A=d.p.url,a(d).jqGrid("setGridParam",{url:b.url}));var B="stop"===a(d).triggerHandler("jqGridToolbarBeforeSearch");!B&&a.isFunction(b.beforeSearch)&&(B=b.beforeSearch.call(d)),B||a(d).jqGrid("setGridParam",{search:q}).trigger("reloadGrid",[{page:1}]),A&&a(d).jqGrid("setGridParam",{url:A}),a(d).triggerHandler("jqGridToolbarAfterSearch"),a.isFunction(b.afterSearch)&&b.afterSearch.call(d)}else if(a.isFunction(p))p.call(d,o[1]);else{var C=a.jgrid.getRegional(d,"errors");a.jgrid.info_dialog(C.errcap,o[1],"",{styleUI:d.p.styleUI})}},n=function(c){var e,f={},g=0;c="boolean"!=typeof c||c,a.each(d.p.colModel,function(){var b,c=a("#gs_"+d.p.idPrefix+a.jgrid.jqID(this.name),!0===this.frozen&&!0===d.p.frozenColumns?d.grid.fhDiv:d.grid.hDiv);switch(this.searchoptions&&void 0!==this.searchoptions.defaultValue&&(b=this.searchoptions.defaultValue),e=this.index||this.name,this.stype){case"select":if(c.find("option").each(function(c){if(0===c&&(this.selected=!0),a(this).val()===b)return this.selected=!0,!1}),void 0!==b)f[e]=b,g++;else try{delete d.p.postData[e]}catch(a){}break;case"text":if(c.val(b||""),void 0!==b)f[e]=b,g++;else try{delete d.p.postData[e]}catch(a){}break;case"custom":a.isFunction(this.searchoptions.custom_value)&&c.length>0&&this.searchoptions.custom_value.call(d,c,"set",b||"")}});var h=g>0;if(d.p.resetsearch=!0,!0===b.stringResult||"local"===d.p.datatype){var i='{"groupOp":"'+b.groupOp+'","rules":[',j=0;a.each(f,function(a,b){j>0&&(i+=","),i+='{"field":"'+a+'",',i+='"op":"eq",',b+="",i+='"data":"'+b.replace(/\\/g,"\\\\").replace(/\"/g,'\\"')+'"}',j++}),i+="]}",a.extend(d.p.postData,{filters:i}),a.each(["searchField","searchString","searchOper"],function(a,b){d.p.postData.hasOwnProperty(b)&&delete d.p.postData[b]})}else a.extend(d.p.postData,f);var k;b.url&&(k=d.p.url,a(d).jqGrid("setGridParam",{url:b.url}));var l="stop"===a(d).triggerHandler("jqGridToolbarBeforeClear");!l&&a.isFunction(b.beforeClear)&&(l=b.beforeClear.call(d)),l||c&&a(d).jqGrid("setGridParam",{search:h}).trigger("reloadGrid",[{page:1}]),k&&a(d).jqGrid("setGridParam",{url:k}),a(d).triggerHandler("jqGridToolbarAfterClear"),a.isFunction(b.afterClear)&&b.afterClear()},o=function(){var b=a("tr.ui-search-toolbar",d.grid.hDiv);!0===d.p.frozenColumns&&a(d).jqGrid("destroyFrozenColumns"),"none"===b.css("display")?b.show():b.hide(),!0===d.p.frozenColumns&&a(d).jqGrid("setFrozenColumns")},p=function(c,f,g){a("#sopt_menu").remove(),f=parseInt(f,10),g=parseInt(g,10)+18;for(var h,i,l=a(".ui-jqgrid").css("font-size")||"11px",n='<ul id="sopt_menu" class="ui-search-menu modal-content" role="menu" tabindex="0" style="font-size:'+l+";left:"+f+"px;top:"+g+'px;">',o=a(c).attr("soper"),p=[],q=0,r=a(c).attr("colname"),s=d.p.colModel.length;q<s&&d.p.colModel[q].name!==r;)q++;var t=d.p.colModel[q],u=a.extend({},t.searchoptions);for(u.sopt||(u.sopt=[],u.sopt[0]="select"===t.stype?"eq":b.defaultSearch),a.each(b.odata,function(){p.push(this.oper)}),q=0;q<u.sopt.length;q++)-1!==(i=a.inArray(u.sopt[q],p))&&(h=o===b.odata[i].oper?k.highlight:"",n+='<li class="ui-menu-item '+h+'" role="presentation"><a class="'+k.cornerall+' g-menu-item" tabindex="0" role="menuitem" value="'+b.odata[i].oper+'" oper="'+b.operands[b.odata[i].oper]+'"><table class="ui-common-table"><tr><td width="25px">'+b.operands[b.odata[i].oper]+"</td><td>"+b.odata[i].text+"</td></tr></table></a></li>");n+="</ul>",a("body").append(n),a("#sopt_menu").addClass("ui-menu "+j.menu_widget),a("#sopt_menu > li > a").hover(function(){a(this).addClass(k.hover)},function(){a(this).removeClass(k.hover)}).click(function(){var f=a(this).attr("value"),g=a(this).attr("oper");if(a(d).triggerHandler("jqGridToolbarSelectOper",[f,g,c]),a("#sopt_menu").hide(),a(c).text(g).attr("soper",f),!0===b.autosearch){var h=a(c).parent().next().children()[0];(a(h).val()||"nu"===f||"nn"===f||a.inArray(f,e)>=0)&&m()}})},q=a("<tr class='ui-search-toolbar' role='row'></tr>");b.restoreFromFilters&&(i=d.p.postData.filters)&&("string"==typeof i&&(i=a.jgrid.parse(i)),h=!!i.rules.length&&i.rules),a.each(d.p.colModel,function(c){var e,f,i,k,n,o,p,r,s=this,t="",u="=",v=a("<th role='columnheader' class='"+l.headerBox+" ui-th-"+d.p.direction+"' id='gsh_"+d.p.id+"_"+s.name+"' ></th>"),w=a("<div></div>"),x=a("<table class='ui-search-table' cellspacing='0'><tr><td class='ui-search-oper' headers=''></td><td class='ui-search-input' headers=''></td><td class='ui-search-clear' headers=''></td></tr></table>");if(!0===this.hidden&&a(v).css("display","none"),this.search=!1!==this.search,void 0===this.stype&&(this.stype="text"),this.searchoptions=this.searchoptions||{},void 0===this.searchoptions.searchOperMenu&&(this.searchoptions.searchOperMenu=!0),e=a.extend({},this.searchoptions,{name:s.index||s.name,id:"gs_"+d.p.idPrefix+s.name,oper:"search"}),this.search){if(b.restoreFromFilters&&h){r=!1;for(var y=0;y<h.length;y++)if(h[y].field){var z=s.index||s.name;if(z===h[y].field){r=h[y];break}}}if(b.searchOperators){for(f=e.sopt?e.sopt[0]:"select"===s.stype?"eq":b.defaultSearch,b.restoreFromFilters&&r&&(f=r.op),i=0;i<b.odata.length;i++)if(b.odata[i].oper===f){u=b.operands[f]||"";break}k=null!=e.searchtitle?e.searchtitle:b.operandTitle,t=this.searchoptions.searchOperMenu?"<a title='"+k+"' style='padding-right: 0.5em;' soper='"+f+"' class='soptclass' colname='"+this.name+"'>"+u+"</a>":""}switch(a("td:eq(0)",x).attr("columname",s.name).append(t),void 0===e.clearSearch&&(e.clearSearch=!0),e.clearSearch?(n=b.resetTitle||"Clear Search Value",a("td:eq(2)",x).append("<a title='"+n+"' style='padding-right: 0.3em;padding-left: 0.3em;' class='clearsearchclass'>"+b.resetIcon+"</a>")):a("td:eq(2)",x).hide(),this.surl&&(e.dataUrl=this.surl),o="",e.defaultValue&&(o=a.isFunction(e.defaultValue)?e.defaultValue.call(d):e.defaultValue),b.restoreFromFilters&&r&&(o=r.data),p=a.jgrid.createEl.call(d,this.stype,e,o,!1,a.extend({},a.jgrid.ajaxOptions,d.p.ajaxSelectOptions||{})),a(p).addClass(j.srInput),a("td:eq(1)",x).append(p),a(w).append(x),null==e.dataEvents&&(e.dataEvents=[]),this.stype){case"select":!0===b.autosearch&&e.dataEvents.push({type:"change",fn:function(){return m(),!1}});break;case"text":!0===b.autosearch&&(b.searchOnEnter?e.dataEvents.push({type:"keypress",fn:function(a){return 13===(a.charCode||a.keyCode||0)?(m(),!1):this}}):e.dataEvents.push({type:"keydown",fn:function(a){switch(a.which){case 13:return!1;case 9:case 16:case 37:case 38:case 39:case 40:case 27:break;default:g&&clearTimeout(g),g=setTimeout(function(){m()},b.autosearchDelay)}}}))}a.jgrid.bindEv.call(d,p,e)}a(v).append(w),a(q).append(v),b.searchOperators&&""!==t||a("td:eq(0)",x).hide()}),a("table thead",d.grid.hDiv).append(q),b.searchOperators&&(a(".soptclass",q).click(function(b){var c=a(this).offset(),d=c.left,e=c.top;p(this,d,e),b.stopPropagation()}),a("body").on("click",function(b){"soptclass"!==b.target.className&&a("#sopt_menu").remove()})),a(".clearsearchclass",q).click(function(){for(var c,f,g=a(this).parents("tr:first"),h=a("td.ui-search-oper",g).attr("columname"),i=0,j=d.p.colModel.length,k=a("td.ui-search-oper a",g).attr("soper");i<j;){if(d.p.colModel[i].name===h){c=d.p.colModel[i];break}i++}var l,n=a.extend({},d.p.colModel[i].searchoptions||{}),o=n.defaultValue?n.defaultValue:"";if("select"===d.p.colModel[i].stype?(l=a("td.ui-search-input select",g),o?l.val(o):l[0].selectedIndex=0):(l=a("td.ui-search-input input",g),l.val(o)),a(d).triggerHandler("jqGridToolbarClearVal",[l[0],i,n,o]),a.isFunction(b.onClearSearchValue)&&b.onClearSearchValue.call(d,l[0],i,n,o),"nu"===k||"nn"===k||a.inArray(k,e)>=0){f=n.sopt?n.sopt[0]:"select"===c.stype?"eq":b.defaultSearch;var p=null!=d.p.customFilterDef&&null!=d.p.customFilterDef[f]?d.p.customFilterDef[f].operand:b.operands[f]||"";f===k?a("td.ui-search-oper a",g).attr("soper","dummy").text(p):a("td.ui-search-oper a",g).attr("soper",f).text(p)}!0===b.autosearch&&(m(),f===k&&a("td.ui-search-oper a",g).attr("soper",f).text(p))}),a(d.grid.hDiv).on("scroll",function(a){d.grid.bDiv.scrollLeft=d.grid.hDiv.scrollLeft}),this.p.filterToolbar=!0,this.triggerToolbar=m,this.clearToolbar=n,this.toggleToolbar=o}})},destroyFilterToolbar:function(){return this.each(function(){this.p.filterToolbar&&(this.triggerToolbar=null,this.clearToolbar=null,this.toggleToolbar=null,this.p.filterToolbar=!1,a(this.grid.hDiv).find("table thead tr.ui-search-toolbar").remove())})},refreshFilterToolbar:function(b){return b=a.extend(!0,{filters:"",onClearVal:null,onSetVal:null},b||{}),this.each(function(){function c(g){if(g&&g.rules){for(h=g.rules,m=h.length,d=0;d<m;d++)if(i=h[d],-1!==(j=a.inArray(i.field,n))&&(f=a("#gs_"+k.p.idPrefix+a.jgrid.jqID(l[j].name)),f.length>0&&("select"===l[j].stype?f.find("option[value='"+a.jgrid.jqID(i.data)+"']").prop("selected",!0):"text"===l[j].stype&&f.val(i.data),a.isFunction(b.onSetVal)&&b.onSetVal.call(k,f,l[j].name),e&&e.searchOperators))){var o=f.parent().prev();o.hasClass("ui-search-oper")&&(a(".soptclass",o).attr("soper",i.op),e.operands.hasOwnProperty(i.op)&&a(".soptclass",o).html(e.operands[i.op]))}if(g.groups)for(var p=0;p<g.groups.length;p++)c(g.groups[p])}}var d,e,f,g,h,i,j,k=this,l=k.p.colModel,m=k.p.colModel.length,n=[];if(k.p.filterToolbar){for(e=a(k).data("filterToolbar"),d=0;d<m;d++){switch(n.push(l[d].name),f=a("#gs_"+k.p.idPrefix+a.jgrid.jqID(l[d].name)),l[d].stype){case"select":case"text":f.val("")}a.isFunction(b.onClearVal)&&b.onClearVal.call(k,f,l[d].name)}"string"==typeof b.filters&&(b.filters.length?g=b.filters:k.p.postData.hasOwnProperty("filters")&&(g=k.p.postData.filters),g=a.jgrid.parse(g)),a.isPlainObject(g)&&c(g)}})},searchGrid:function(b){var c=a.jgrid.getRegional(this[0],"search");return b=a.extend(!0,{recreateFilter:!1,drag:!0,sField:"searchField",sValue:"searchString",sOper:"searchOper",sFilter:"filters",loadDefaults:!0,beforeShowSearch:null,afterShowSearch:null,onInitializeSearch:null,afterRedraw:null,afterChange:null,sortStrategy:null,closeAfterSearch:!1,closeAfterReset:!1,closeOnEscape:!1,searchOnEnter:!1,multipleSearch:!1,multipleGroup:!1,top:0,left:0,jqModal:!0,modal:!1,resize:!0,width:450,height:"auto",dataheight:"auto",showQuery:!1,errorcheck:!0,sopt:null,stringResult:void 0,onClose:null,onSearch:null,onReset:null,toTop:!0,overlay:30,columns:[],tmplNames:null,tmplFilters:null,tmplLabel:" Template: ",showOnLoad:!1,layer:null,splitSelect:",",groupOpSelect:"OR",operands:{eq:"=",ne:"<>",lt:"<",le:"<=",gt:">",ge:">=",bw:"LIKE",bn:"NOT LIKE",in:"IN",ni:"NOT IN",ew:"LIKE",en:"NOT LIKE",cn:"LIKE",nc:"NOT LIKE",nu:"IS NULL",nn:"ISNOT NULL"},buttons:[]},c,b||{}),this.each(function(){function c(c){g=a(d).triggerHandler("jqGridFilterBeforeShow",[c]),void 0===g&&(g=!0),g&&a.isFunction(b.beforeShowSearch)&&(g=b.beforeShowSearch.call(d,c)),g&&(a.jgrid.viewModal("#"+a.jgrid.jqID(i.themodal),{gbox:"#gbox_"+a.jgrid.jqID(d.p.id),jqm:b.jqModal,modal:b.modal,overlay:b.overlay,toTop:b.toTop}),a(d).triggerHandler("jqGridFilterAfterShow",[c]),a.isFunction(b.afterShowSearch)&&b.afterShowSearch.call(d,c))}var d=this;if(d.grid){var e,f="fbox_"+d.p.id,g=!0,h=!0,i={themodal:"searchmod"+f,modalhead:"searchhd"+f,modalcontent:"searchcnt"+f,scrollelm:f},j=a.isPlainObject(d.p_savedFilter)&&!a.isEmptyObject(d.p_savedFilter)?d.p_savedFilter:d.p.postData[b.sFilter],k=[],l=a.jgrid.styleUI[d.p.styleUI||"jQueryUI"].filter,m=a.jgrid.styleUI[d.p.styleUI||"jQueryUI"].common;if(b.styleUI=d.p.styleUI,"string"==typeof j&&(j=a.jgrid.parse(j)),!0===b.recreateFilter&&a("#"+a.jgrid.jqID(i.themodal)).remove(),void 0!==a("#"+a.jgrid.jqID(i.themodal))[0])c(a("#fbox_"+a.jgrid.jqID(d.p.id)));else{var n=a("<div><div id='"+f+"' class='searchFilter' style='overflow:auto'></div></div>").insertBefore("#gview_"+a.jgrid.jqID(d.p.id)),o="left",p="";"rtl"===d.p.direction&&(o="right",p=" style='text-align:left'",n.attr("dir","rtl"));var q,r,s=a.extend([],d.p.colModel),t="<a id='"+f+"_search' class='fm-button "+m.button+" fm-button-icon-right ui-search'><span class='"+m.icon_base+" "+l.icon_search+"'></span>"+b.Find+"</a>",u="<a id='"+f+"_reset' class='fm-button "+m.button+" fm-button-icon-left ui-reset'><span class='"+m.icon_base+" "+l.icon_reset+"'></span>"+b.Reset+"</a>",v="",w="",x=!1,y=-1,z=!1,A=[];b.showQuery&&(v="<a id='"+f+"_query' class='fm-button "+m.button+" fm-button-icon-left'><span class='"+m.icon_base+" "+l.icon_query+"'></span>Query</a>");var B=a.jgrid.buildButtons(b.buttons,v+t,m),C=null;if(d.p.groupHeader&&d.p.groupHeader.length>0){var D=a("table.ui-jqgrid-htable",d.grid.hDiv),E=D.find(".jqg-second-row-header"),F=d.p.groupHeader.length;void 0!==E[0]&&(C=d.p.groupHeader[F-1])}var G=function(a,b){var c,d=b.length;for(c=0;c<d;c++)if(b[c].startColumnName===a)return c;return-1};if(b.columns.length)s=b.columns,y=0,q=s[0].index||s[0].name;else{if(null!==C)for(var H=0;H<s.length;H++){var I=G(s[H].name,C.groupHeaders);if(I>=0){s[H].label=C.groupHeaders[I].titleText+"::"+d.p.colNames[H];for(var J=1;J<=C.groupHeaders[I].numberOfColumns-1;J++)s[H+J].label=C.groupHeaders[I].titleText+"::"+d.p.colNames[H+J];H=H+C.groupHeaders[I].numberOfColumns-1}}a.each(s,function(a,b){if(b.label||(b.label=d.p.colNames[a]),!x){var c=void 0===b.search||b.search,e=!0===b.hidden;(b.searchoptions&&!0===b.searchoptions.searchhidden&&c||c&&!e)&&(x=!0,q=b.index||b.name,y=a)}"select"===b.stype&&b.searchoptions&&b.searchoptions.multiple&&(z=!0,A.push(b.index||b.name))})}if(!j&&q||!1===b.multipleSearch){var K="eq";y>=0&&s[y].searchoptions&&s[y].searchoptions.sopt?K=s[y].searchoptions.sopt[0]:b.sopt&&b.sopt.length&&(K=b.sopt[0]),j={groupOp:"AND",rules:[{field:q,op:K,data:""}]}}if(x=!1,b.tmplNames&&b.tmplNames.length&&(x=!0,w="<tr><td class='ui-search-label'>"+b.tmplLabel+"</td>",w+="<td><select size='1' class='ui-template "+l.srSelect+"'>",w+="<option value='default'>Default</option>",a.each(b.tmplNames,function(a,b){w+="<option value='"+a+"'>"+b+"</option>"}),w+="</select></td></tr>"),void 0!==d.p.customFilterDef)for(var L in d.p.customFilterDef)d.p.customFilterDef.hasOwnProperty(L)&&!b.operands.hasOwnProperty(L)&&(b.odata.push({oper:L,text:d.p.customFilterDef[L].text}),b.operands[L]=d.p.customFilterDef[L].operand,!0===d.p.customFilterDef[L].unary&&k.push(L));r="<table class='EditTable' style='border:0px none;margin-top:5px' id='"+f+"_2'><tbody><tr><td colspan='2'><hr class='"+m.content+"' style='margin:1px'/></td></tr>"+w+"<tr><td class='EditButton' style='text-align:"+o+"'>"+u+"</td><td class='EditButton' "+p+">"+B+"</td></tr></tbody></table>",f=a.jgrid.jqID(f),a("#"+f).jqFilter({columns:s,sortStrategy:b.sortStrategy,filter:b.loadDefaults?j:null,showQuery:b.showQuery,errorcheck:b.errorcheck,sopt:b.sopt,groupButton:b.multipleGroup,ruleButtons:b.multipleSearch,uniqueSearchFields:b.uniqueSearchFields,afterRedraw:b.afterRedraw,ops:b.odata,operands:b.operands,ajaxSelectOptions:d.p.ajaxSelectOptions,groupOps:b.groupOps,addsubgrup:b.addsubgrup,addrule:b.addrule,delgroup:b.delgroup,delrule:b.delrule,autoencode:d.p.autoencode,unaryOperations:k,onChange:function(){this.p.showQuery&&a(".query",this).html(this.toUserFriendlyString()),a.isFunction(b.afterChange)&&b.afterChange.call(d,a("#"+f),b)},direction:d.p.direction,id:d.p.id}),n.append(r),a("#"+f+"_2").find("[data-index]").each(function(){var c=parseInt(a(this).attr("data-index"),10);c>=0&&a(this).on("click",function(e){b.buttons[c].click.call(d,a("#"+f),b,e)})}),x&&b.tmplFilters&&b.tmplFilters.length&&a(".ui-template",n).on("change",function(){var c=a(this).val();return"default"===c?a("#"+f).jqFilter("addFilter",j):a("#"+f).jqFilter("addFilter",b.tmplFilters[parseInt(c,10)]),!1}),!0===b.multipleGroup&&(b.multipleSearch=!0),a(d).triggerHandler("jqGridFilterInitialize",[a("#"+f)]),a.isFunction(b.onInitializeSearch)&&b.onInitializeSearch.call(d,a("#"+f)),b.gbox="#gbox_"+f
;var M=a(".ui-jqgrid").css("font-size")||"11px";b.layer?a.jgrid.createModal(i,n,b,"#gview_"+a.jgrid.jqID(d.p.id),a("#gbox_"+a.jgrid.jqID(d.p.id))[0],"string"==typeof b.layer?"#"+a.jgrid.jqID(b.layer):b.layer,"string"==typeof b.layer?{position:"relative","font-size":M}:{"font-size":M}):a.jgrid.createModal(i,n,b,"#gview_"+a.jgrid.jqID(d.p.id),a("#gbox_"+a.jgrid.jqID(d.p.id))[0],null,{"font-size":M}),(b.searchOnEnter||b.closeOnEscape)&&a("#"+a.jgrid.jqID(i.themodal)).keydown(function(c){var d=a(c.target);return!b.searchOnEnter||13!==c.which||d.hasClass("add-group")||d.hasClass("add-rule")||d.hasClass("delete-group")||d.hasClass("delete-rule")||d.hasClass("fm-button")&&d.is("[id$=_query]")?b.closeOnEscape&&27===c.which?(a("#"+a.jgrid.jqID(i.modalhead)).find(".ui-jqdialog-titlebar-close").click(),!1):void 0:(a("#"+f+"_search").click(),!1)}),v&&a("#"+f+"_query").on("click",function(){return a(".queryresult",n).toggle(),!1}),void 0===b.stringResult&&(b.stringResult=b.multipleSearch),a("#"+f+"_search").on("click",function(){var c,g,j={};if(e=a("#"+f),e.find(".input-elm:focus").change(),z&&b.multipleSearch?(d.p_savedFilter={},g=a.jgrid.filterRefactor({ruleGroup:a.extend(!0,{},e.jqFilter("filterData")),ssfield:A,splitSelect:b.splitSelect,groupOpSelect:b.groupOpSelect}),d.p_savedFilter=a.extend(!0,{},e.jqFilter("filterData"))):g=e.jqFilter("filterData"),b.errorcheck&&(e[0].hideError(),b.showQuery||e.jqFilter("toSQLString"),e[0].p.error))return e[0].showError(),!1;if(b.stringResult){try{c=JSON.stringify(g)}catch(a){}"string"==typeof c&&(j[b.sFilter]=c,a.each([b.sField,b.sValue,b.sOper],function(){j[this]=""}))}else b.multipleSearch?(j[b.sFilter]=g,a.each([b.sField,b.sValue,b.sOper],function(){j[this]=""})):(j[b.sField]=g.rules[0].field,j[b.sValue]=g.rules[0].data,j[b.sOper]=g.rules[0].op,j[b.sFilter]="");return d.p.search=!0,a.extend(d.p.postData,j),h=a(d).triggerHandler("jqGridFilterSearch"),void 0===h&&(h=!0),h&&a.isFunction(b.onSearch)&&(h=b.onSearch.call(d,d.p.filters)),!1!==h&&a(d).trigger("reloadGrid",[{page:1}]),b.closeAfterSearch&&a.jgrid.hideModal("#"+a.jgrid.jqID(i.themodal),{gb:"#gbox_"+a.jgrid.jqID(d.p.id),jqm:b.jqModal,onClose:b.onClose}),!1}),a("#"+f+"_reset").on("click",function(){var c={},e=a("#"+f);return d.p.search=!1,d.p.resetsearch=!0,!1===b.multipleSearch?c[b.sField]=c[b.sValue]=c[b.sOper]="":c[b.sFilter]="",e[0].resetFilter(),x&&a(".ui-template",n).val("default"),a.extend(d.p.postData,c),h=a(d).triggerHandler("jqGridFilterReset"),void 0===h&&(h=!0),h&&a.isFunction(b.onReset)&&(h=b.onReset.call(d)),!1!==h&&a(d).trigger("reloadGrid",[{page:1}]),b.closeAfterReset&&a.jgrid.hideModal("#"+a.jgrid.jqID(i.themodal),{gb:"#gbox_"+a.jgrid.jqID(d.p.id),jqm:b.jqModal,onClose:b.onClose}),!1}),c(a("#"+f)),a(".fm-button:not(."+m.disabled+")",n).hover(function(){a(this).addClass(m.hover)},function(){a(this).removeClass(m.hover)})}}})},filterInput:function(b,c){return c=a.extend(!0,{defaultSearch:"cn",groupOp:"OR",searchAll:!1,beforeSearch:null,afterSearch:null},c||{}),this.each(function(){var d=this;if(d.grid){var e,f,g,h='{"groupOp":"'+c.groupOp+'","rules":[',i=0;if(b+="","local"===d.p.datatype){a.each(d.p.colModel,function(){e=this.index||this.name,f=this.searchoptions||{},g=c.defaultSearch?c.defaultSearch:f.sopt?f.sopt[0]:c.defaultSearch,this.search=!1!==this.search,(this.search||c.searchAll)&&(i>0&&(h+=","),h+='{"field":"'+e+'",',h+='"op":"'+g+'",',h+='"data":"'+b.replace(/\\/g,"\\\\").replace(/\"/g,'\\"')+'"}',i++)}),h+="]}",a.extend(d.p.postData,{filters:h}),a.each(["searchField","searchString","searchOper"],function(a,b){d.p.postData.hasOwnProperty(b)&&delete d.p.postData[b]});var j="stop"===a(d).triggerHandler("jqGridFilterInputBeforeSearch");!j&&a.isFunction(c.beforeSearch)&&(j=c.beforeSearch.call(d)),j||a(d).jqGrid("setGridParam",{search:!0}).trigger("reloadGrid",[{page:1}]),a(d).triggerHandler("jqGridFilterInputAfterSearch"),a.isFunction(c.afterSearch)&&c.afterSearch.call(d)}}})},autoSelect:function(b){return b=a.extend(!0,{field:"",direction:"asc",src_date:"Y-m-d",allValues:"All",count_item:!0,create_value:!0},b||{}),this.each(function(){var c,d=this,e="";if(b.field&&d.p.data&&a.isArray(d.p.data)){var f,g,h,i,j,k,l,m=[];try{f=a.jgrid.from.call(d,d.p.data),k=f.groupBy(b.field,b.direction,"text",b.src_date),l=k.length}catch(a){}if(k&&k.length){for(g=a("#gsh_"+d.p.id+"_"+b.field).find("td.ui-search-input > select"),l=k.length,b.allValues&&(e="<option value=''>"+b.allValues+"</option>",m.push(":"+b.allValues));l--;)c=k[l],h=b.count_item?" ("+c.items.length+")":"",e+="<option value='"+c.unique+"'>"+c.unique+h+"</option>",m.push(c.unique+":"+c.unique+h);if(g.append(e),g.on("change",function(){d.triggerToolbar()}),b.create_value){for(l=0,j=d.p.colModel.length;l<j;l++)if(d.p.colModel[l].name===b.field){i=d.p.colModel[l];break}i&&(i.searchoptions?a.extend(i.searchoptions,{value:m.join(";")}):(i.searchoptions={},i.searchoptions.value=m.join(";")))}}}})}})});