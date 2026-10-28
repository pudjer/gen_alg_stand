(function () {
    'use strict';

    function sync(select) {
        const axis = select.getAttribute('data-axis-select');
        if (!axis) {
            return;
        }

        document.querySelectorAll('.module-params[data-axis="' + axis + '"]').forEach(function (block) {
            block.classList.toggle('active', block.getAttribute('data-index') === select.value);
        });
    }

    document.querySelectorAll('select[data-axis-select]').forEach(function (select) {
        select.addEventListener('change', function () {
            sync(select);
        });
        sync(select);
    });
})();