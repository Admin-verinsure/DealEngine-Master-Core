/**
*
* @license Guriddo jqGrid JS - v5.5.0 
* Copyright(c) 2008, Tony Tomov, tony@trirand.com
* 
* License: http://guriddo.net/?page_id=103334
*/
!function(a){"use strict";"function"==typeof define&&define.amd?define(["jquery","./grid.base","./grid.grouping"],a):a(jQuery)}(function(a){"use strict";function b(a,b){var c,d,e,f=[];if(!this||"function"!=typeof a||a instanceof RegExp)throw new TypeError;for(e=this.length,c=0;c<e;c++)if(this.hasOwnProperty(c)&&(d=this[c],a.call(b,d,c,this))){f.push(d);break}return f}a.assocArraySize=function(a){var b,c=0;for(b in a)a.hasOwnProperty(b)&&c++;return c},a.jgrid.extend({pivotSetup:function(c,d){var e=[],f=[],g=[],h=[],i=[],j={grouping:!0,groupingView:{groupField:[],groupSummary:[],groupSummaryPos:[]}},k=[],l=a.extend({rowTotals:!1,rowTotalsText:"Total",colTotals:!1,groupSummary:!0,groupSummaryPos:"header",frozenStaticCols:!1},d||{});return this.each(function(){function d(a,c,d){var e;return e=b.call(a,c,d),e.length>0?e[0]:null}function m(a,b){var c,d=0,e=!0;for(c in a)if(a.hasOwnProperty(c)){if(a[c]!=this[d]){e=!1;break}if(++d>=this.length)break}return e&&(r=b),e}function n(b,c,d,e,f){var g;if(a.isFunction(b))g=b.call(y,c,d,e);else switch(b){case"sum":g=parseFloat(c||0)+parseFloat(e[d]||0);break;case"count":""!==c&&null!=c||(c=0),g=e.hasOwnProperty(d)?c+1:0;break;case"min":g=""===c||null==c?parseFloat(e[d]||0):Math.min(parseFloat(c),parseFloat(e[d]||0));break;case"max":g=""===c||null==c?parseFloat(e[d]||0):Math.max(parseFloat(c),parseFloat(e[d]||0));break;case"avg":g=(parseFloat(c||0)*(f-1)+parseFloat(e[d]||0))/f}return g}function o(b,c,d,e){var g,j,k,l,m,o,p=c.length,q="",s=[],t=1;for(a.isArray(d)?(l=d.length,s=d):(l=1,s[0]=d),h=[],i=[],h.root=0,k=0;k<l;k++){var u,v=[];for(g=0;g<p;g++){if(m="string"==typeof c[g].aggregator?c[g].aggregator:"cust",null==d)j=a.trim(c[g].member)+"_"+m,u=j,s[0]=c[g].label||m+" "+a.trim(c[g].member);else{u=d[k].replace(/\s+/g,"");try{j=1===p?q+u:q+u+"_"+m+"_"+String(g)}catch(a){}s[k]=d[k]}j=isNaN(parseInt(j,10))?j:j+" ","avg"===c[g].aggregator&&(o=-1===r?f.length+"_"+j:r+"_"+j,F[o]?F[o]++:F[o]=1,t=F[o]),e[j]=v[j]=n(c[g].aggregator,e[j],c[g].member,b,t)}q+=d&&null!=d[k]?d[k].replace(/\s+/g,""):"",h[j]=v,i[j]=s[k]}return e}function p(a){var b,c,d,f,g;for(d in a)if(a.hasOwnProperty(d)){if("object"!=typeof a[d]){if("level"===d){if(void 0===N[a.level]&&(N[a.level]="",a.level>0&&-1===a.text.indexOf("_r_Totals")&&(k[a.level-1]={useColSpanStyle:!1,groupHeaders:[]})),N[a.level]!==a.text&&a.children.length&&-1===a.text.indexOf("_r_Totals")&&a.level>0){k[a.level-1].groupHeaders.push({titleText:a.label,numberOfColumns:0});var h=k[a.level-1].groupHeaders.length-1,i=0===h?P:O;if(a.level-1==(l.rowTotals?1:0)&&h>0){for(var j=0,m=0;m<h;m++)j+=k[a.level-1].groupHeaders[m].numberOfColumns;j&&(i=j+t)}e[i]&&(k[a.level-1].groupHeaders[h].startColumnName=e[i].name,k[a.level-1].groupHeaders[h].numberOfColumns=e.length-i),O=e.length}N[a.level]=a.text}if(a.level===u&&"level"===d&&u>0)if(v>1){var n=1;for(b in a.fields)a.fields.hasOwnProperty(b)&&(1===n&&k[u-1].groupHeaders.push({startColumnName:b,numberOfColumns:1,titleText:a.label||a.text}),n++);k[u-1].groupHeaders[k[u-1].groupHeaders.length-1].numberOfColumns=n-1}else k.splice(u-1,1)}if(null!=a[d]&&"object"==typeof a[d]&&p(a[d]),"level"===d&&a.level>0&&(a.level===(0===u?a.level:u)||-1!==N[a.level].indexOf("_r_Totals"))){c=0;for(b in a.fields)if(a.fields.hasOwnProperty(b)){g={};for(f in l.aggregates[c])if(l.aggregates[c].hasOwnProperty(f))switch(f){case"member":case"label":case"aggregator":break;default:g[f]=l.aggregates[c][f]}v>1?(g.name=b,g.label=l.aggregates[c].label||a.label):(g.name=a.text,g.label="_r_Totals"===a.text?l.rowTotalsText:a.label),e.push(g),c++}}}}var q,r,s,t,u,v,w,x,y=this,z=c.length,A=0;if(l.rowTotals&&l.yDimension.length>0){var B=l.yDimension[0].dataName;l.yDimension.splice(0,0,{dataName:B}),l.yDimension[0].converter=function(){return"_r_Totals"}}if(t=a.isArray(l.xDimension)?l.xDimension.length:0,u=l.yDimension.length,v=a.isArray(l.aggregates)?l.aggregates.length:0,0===t||0===v)throw"xDimension or aggregates optiona are not set!";var C;for(s=0;s<t;s++)C={name:l.xDimension[s].dataName,frozen:l.frozenStaticCols},null==l.xDimension[s].isGroupField&&(l.xDimension[s].isGroupField=!0),C=a.extend(!0,C,l.xDimension[s]),e.push(C);for(var D=t-1,E={},F=[];A<z;){q=c[A];var G=[],H=[];w={},s=0;do{G[s]=a.trim(q[l.xDimension[s].dataName]),w[l.xDimension[s].dataName]=G[s],s++}while(s<t);var I=0;if(r=-1,x=d(f,m,G)){if(r>=0){if(I=0,u>=1){for(I=0;I<u;I++)H[I]=a.trim(q[l.yDimension[I].dataName]),l.yDimension[I].converter&&a.isFunction(l.yDimension[I].converter)&&(H[I]=l.yDimension[I].converter.call(this,H[I],G,H));x=o(q,l.aggregates,H,x)}else 0===u&&(x=o(q,l.aggregates,null,x));f[r]=x}}else{if(I=0,u>=1){for(I=0;I<u;I++)H[I]=a.trim(q[l.yDimension[I].dataName]),l.yDimension[I].converter&&a.isFunction(l.yDimension[I].converter)&&(H[I]=l.yDimension[I].converter.call(this,H[I],G,H));w=o(q,l.aggregates,H,w)}else 0===u&&(w=o(q,l.aggregates,null,w));f.push(w)}var J,K=0,L=null,M=null;for(J in h)if(h.hasOwnProperty(J)){if(0===K)E.children&&void 0!==E.children||(E={text:J,level:0,children:[],label:J}),L=E.children;else{for(M=null,s=0;s<L.length;s++)if(L[s].text===J){M=L[s];break}M?L=M.children:(L.push({children:[],text:J,level:K,fields:h[J],label:i[J]}),L=L[L.length-1].children)}K++}A++}F=null;var N=[],O=e.length,P=O;u>0&&(k[u-1]={useColSpanStyle:!1,groupHeaders:[]}),p(E);var Q;if(l.colTotals)for(var R=f.length;R--;)for(s=t;s<e.length;s++)Q=e[s].name,g[Q]?g[Q]+=parseFloat(f[R][Q]||0):g[Q]=parseFloat(f[R][Q]||0);if(D>0)for(s=0;s<D;s++)e[s].isGroupField&&(j.groupingView.groupField.push(e[s].name),j.groupingView.groupSummary.push(l.groupSummary),j.groupingView.groupSummaryPos.push(l.groupSummaryPos));else j.grouping=!1;j.sortname=e[D].name,j.groupingView.hideFirstGroupCol=!0}),{colModel:e,rows:f,groupOptions:j,groupHeaders:k,summary:g}},jqPivot:function(b,c,d,e){return this.each(function(){function f(b){a.isFunction(c.onInitPivot)&&c.onInitPivot.call(g),a.isArray(b)||(b=[]);var e,f,h,i,j=jQuery(g).jqGrid("pivotSetup",b,c),k=a.assocArraySize(j.summary)>0,l=a.jgrid.from.call(g,j.rows);for(c.ignoreCase&&(l=l.ignoreCase()),e=0;e<j.groupOptions.groupingView.groupField.length;e++)f=c.xDimension[e].sortorder?c.xDimension[e].sortorder:"asc",h=c.xDimension[e].sorttype?c.xDimension[e].sorttype:"text",l.orderBy(j.groupOptions.groupingView.groupField[e],f,h,"",h);if(i=c.xDimension.length,d.sortname){for(f=d.sortorder?d.sortorder:"asc",h="text",e=0;e<i;e++)if(c.xDimension[e].dataName===d.sortname){h=c.xDimension[e].sorttype?c.xDimension[e].sorttype:"text";break}l.orderBy(d.sortname,f,h,"",h)}else j.groupOptions.sortname&&i&&(f=c.xDimension[i-1].sortorder?c.xDimension[i-1].sortorder:"asc",h=c.xDimension[i-1].sorttype?c.xDimension[i-1].sorttype:"text",l.orderBy(j.groupOptions.sortname,f,h,"",h));jQuery(g).jqGrid(a.extend(!0,{datastr:a.extend(l.select(),k?{userdata:j.summary}:{}),datatype:"jsonstring",footerrow:k,userDataOnFooter:k,colModel:j.colModel,viewrecords:!0,formatFooterData:!0===c.colTotals,sortname:c.xDimension[0].dataName},j.groupOptions,d||{}));var m=j.groupHeaders;if(m.length)for(e=0;e<m.length;e++)m[e]&&m[e].groupHeaders.length&&jQuery(g).jqGrid("setGroupHeaders",m[e]);c.frozenStaticCols&&jQuery(g).jqGrid("setFrozenColumns"),a.isFunction(c.onCompletePivot)&&c.onCompletePivot.call(g),c.loadMsg&&a(".loading_pivot").remove()}var g=this,h=d.regional?d.regional:"en";void 0===c.loadMsg&&(c.loadMsg=!0),c.loadMsg&&a("<div class='loading_pivot ui-state-default ui-state-active row'>"+a.jgrid.getRegional(g,"regional."+h+".defaults.loadtext")+"</div>").insertBefore(g).show(),"string"==typeof b?a.ajax(a.extend({url:b,dataType:"json",success:function(b){f(a.jgrid.getAccessor(b,e&&e.reader?e.reader:"rows"))}},e||{})):f(b)})}})});