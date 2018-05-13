
/**
* Number.prototype.format(n, x, s, c)
* 
* param integer n: length of decimal
* param integer x: length of whole part
* param mixed   s: sections delimiter
* param mixed   c: decimal delimiter
**/
Number.prototype.format = function (n, x, s, c) {
    var re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\D' : '$') + ')',
        num = this.toFixed(Math.max(0, ~~n));
    return (c ? num.replace('.', c) : num).replace(new RegExp(re, 'g'), '$&' + (s || ','));
};

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

/**
* Function to format date
*/
function formatDateVN(date) {
    var yyyy = date.getFullYear();
    var dd = date.getDate();
    var mm = date.getMonth() + 1;
    if (dd < 10) {
        dd = "0" + dd;
    }
    if (mm < 10) {
        mm = "0" + mm;
    }
    return dd + "-" + mm + "-" + yyyy;
}

function formatDateTimeVN(date) {
    var yyyy = date.getFullYear();
    var dd = date.getDate();
    var mm = date.getMonth() + 1;
    var hh = date.getHours();
    var mi = date.getMinutes();
    if (dd < 10) {
        dd = "0" + dd;
    }
    if (mm < 10) {
        mm = "0" + mm;
    }
    if (hh < 10) {
        hh = "0" + hh;
    }
    if (mi < 10) {
        mi = "0" + mi;
    }
    return dd + "-" + mm + "-" + yyyy + " " + hh + ":" + mi;
}

function spacer(size) {
    var html = '';
    for (i = 0; i < size; i++) {
        html += '&nbsp;'
    }
    return html;
}

