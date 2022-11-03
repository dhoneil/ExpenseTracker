(function () {
    window.SelectOption = function (optionIdName, valueToSelect) {
        $(`#${optionIdName} option[value=${valueToSelect}]`).prop('selected', true);
    }
})();