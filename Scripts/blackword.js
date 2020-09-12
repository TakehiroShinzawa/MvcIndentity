$.validator.addMethod('blackword',
function (value, element, param) {
    value = $.trim(value);
    if (value === '') { return true; }

    var list = param.split(',');
    for (var i = 0, len = list.length; i < len; i++) {
        if (value.indexOf(list[i]) !== -1)
        {
            return false;
        }
    }
    return true;
});

$.validator.unobtrusive.adapters.addSingleVal('blackword', 'opts');
