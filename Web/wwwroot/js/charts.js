// Chart.js interop for the Blazor bench. Exposes window.gaCharts.
// Loaded after the Chart.js UMD bundle from the CDN; degrades to a no-op if it is unavailable.
(function () {
    'use strict';

    /** Chart registry: canvasId -> Chart instance. */
    var charts = new Map();

    function readPoints(series) {
        var points = series.points || series.Points || [];
        var result = new Array(points.length);
        for (var i = 0; i < points.length; i++) {
            result[i] = Number(points[i]);
        }
        return result;
    }

    function readLabel(series) {
        return series.label || series.Label || '';
    }

    function readColor(series) {
        return series.color || series.Color || '#0d6efd';
    }

    function buildDatasets(seriesList) {
        return (seriesList || []).map(function (series) {
            var color = readColor(series);
            return {
                label: readLabel(series),
                data: readPoints(series),
                borderColor: color,
                backgroundColor: color,
                borderWidth: 2,
                pointRadius: 0,
                pointHitRadius: 8,
                tension: 0.2,
                fill: false
            };
        });
    }

    function maxLength(seriesList) {
        var length = 0;
        (seriesList || []).forEach(function (series) {
            var points = readPoints(series);
            if (points.length > length) {
                length = points.length;
            }
        });
        return length;
    }

    function buildLabels(seriesList) {
        var count = maxLength(seriesList);
        var labels = new Array(count);
        for (var i = 0; i < count; i++) {
            labels[i] = i;
        }
        return labels;
    }

    function buildConfig(seriesList) {
        return {
            type: 'line',
            data: {
                labels: buildLabels(seriesList),
                datasets: buildDatasets(seriesList)
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                animation: false,
                interaction: { mode: 'index', intersect: false },
                plugins: {
                    legend: { display: true, position: 'top' }
                },
                scales: {
                    x: { title: { display: true, text: 'Generation' }, ticks: { maxTicksLimit: 20 } },
                    y: { title: { display: true, text: 'Loss' } }
                }
            }
        };
    }

    function chartAvailable() {
        if (typeof window.Chart === 'undefined') {
            console.warn('gaCharts: Chart.js is not loaded; chart rendering skipped.');
            return false;
        }
        return true;
    }

    function getCanvas(canvasId) {
        var canvas = document.getElementById(canvasId);
        if (!canvas) {
            console.warn('gaCharts: no canvas with id ' + canvasId + '.');
        }
        return canvas;
    }

    /**
     * Creates (or replaces) a line chart on the given canvas.
     * @param {string} canvasId DOM id of the canvas element.
     * @param {Array<{label: string, color: string, points: number[]}>} seriesList Series to plot.
     */
    function createLineChart(canvasId, seriesList) {
        destroyChart(canvasId);

        if (!chartAvailable()) {
            return;
        }

        var canvas = getCanvas(canvasId);
        if (!canvas) {
            return;
        }

        charts.set(canvasId, new window.Chart(canvas, buildConfig(seriesList)));
    }

    /**
     * Updates the data of an existing chart, recreating it when it is missing.
     * @param {string} canvasId DOM id of the canvas element.
     * @param {Array<{label: string, color: string, points: number[]}>} seriesList Series to plot.
     */
    function updateLineChart(canvasId, seriesList) {
        var existing = charts.get(canvasId);
        if (!existing || !chartAvailable()) {
            createLineChart(canvasId, seriesList);
            return;
        }

        existing.data.labels = buildLabels(seriesList);
        existing.data.datasets = buildDatasets(seriesList);
        existing.update('none');
    }

    /**
     * Destroys the chart bound to a canvas, if any. Safe to call repeatedly.
     * @param {string} canvasId DOM id of the canvas element.
     */
    function destroyChart(canvasId) {
        var existing = charts.get(canvasId);
        if (!existing) {
            return;
        }

        charts.delete(canvasId);
        try {
            existing.destroy();
        } catch (error) {
            console.warn('gaCharts: failed to destroy chart ' + canvasId, error);
        }
    }

    window.gaCharts = {
        createLineChart: createLineChart,
        updateLineChart: updateLineChart,
        destroyChart: destroyChart
    };
})();