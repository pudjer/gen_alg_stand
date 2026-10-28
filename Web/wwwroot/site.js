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

    const problemSelect = document.querySelector('select[data-problem-select]');
    const configureButton = document.getElementById('configure-submit');

    if (problemSelect && configureButton) {
        let configureTimer = 0;

        problemSelect.addEventListener('change', function () {
            window.clearTimeout(configureTimer);
            configureTimer = window.setTimeout(function () {
                // Параметры другой задачи не должны переезжать в новую.
                document.querySelectorAll('input[name^="ProblemParams["]').forEach(function (input) {
                    input.value = '';
                });

                configureButton.click();
            }, 150);
        });
    }
})();