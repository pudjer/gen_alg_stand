(function () {
    'use strict';

    const canvas = document.getElementById('chart');
    const dataBlock = document.getElementById('chart-data');
    if (!canvas || !dataBlock) {
        return;
    }

    let samples = [];
    try {
        samples = JSON.parse(dataBlock.textContent || '[]');
    } catch (error) {
        return;
    }

    if (!Array.isArray(samples) || samples.length === 0) {
        return;
    }

    const logToggle = document.getElementById('log-scale');
    const avgColor = '#94a3b8';
    const bestColor = '#e4572e';
    const gridColor = '#e2e8f0';
    const textColor = '#475569';

    function format(value) {
        if (!Number.isFinite(value)) {
            return '—';
        }

        const magnitude = Math.abs(value);
        if (magnitude === 0) {
            return '0';
        }

        if (magnitude >= 1e5 || magnitude < 1e-3) {
            return value.toExponential(1);
        }

        if (magnitude >= 100) {
            return value.toFixed(0);
        }

        return magnitude >= 1 ? value.toFixed(2) : value.toFixed(3);
    }

    function draw() {
        const dpr = window.devicePixelRatio || 1;
        const width = Math.max(320, canvas.clientWidth || 640);
        const height = 340;

        canvas.width = Math.round(width * dpr);
        canvas.height = Math.round(height * dpr);

        const ctx = canvas.getContext('2d');
        if (!ctx) {
            return;
        }

        ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
        ctx.clearRect(0, 0, width, height);

        const pad = { left: 76, right: 16, top: 30, bottom: 44 };
        const plotWidth = Math.max(10, width - pad.left - pad.right);
        const plotHeight = Math.max(10, height - pad.top - pad.bottom);

        const useLog = Boolean(logToggle && logToggle.checked);

        let xMin = Number(samples[0][0]);
        let xMax = Number(samples[samples.length - 1][0]);
        if (!Number.isFinite(xMin)) {
            xMin = 0;
        }
        if (!Number.isFinite(xMax) || xMax <= xMin) {
            xMax = xMin + 1;
        }

        const values = [];
        for (const row of samples) {
            values.push(Number(row[1]), Number(row[2]));
        }

        const finite = values.filter(Number.isFinite);
        if (finite.length === 0) {
            return;
        }

        let transform;
        if (useLog) {
            const positives = finite.filter(function (value) { return value > 0; });
            const floor = positives.length > 0 ? Math.min(1, Math.min.apply(null, positives)) : 1e-9;
            transform = function (value) { return Math.log10(Math.max(value, floor)); };
        } else {
            transform = function (value) { return value; };
        }

        let tickMin = Infinity;
        let tickMax = -Infinity;
        for (const value of finite) {
            const transformed = transform(value);
            if (transformed < tickMin) {
                tickMin = transformed;
            }
            if (transformed > tickMax) {
                tickMax = transformed;
            }
        }

        if (!Number.isFinite(tickMin) || !Number.isFinite(tickMax)) {
            return;
        }

        if (tickMax - tickMin < 1e-12) {
            const delta = Math.abs(tickMax) > 0 ? Math.abs(tickMax) * 0.1 : 1;
            tickMin -= delta;
            tickMax += delta;
        }

        const verticalPadding = (tickMax - tickMin) * 0.06;
        tickMin -= verticalPadding;
        tickMax += verticalPadding;

        function xOf(generation) {
            return pad.left + ((Number(generation) - xMin) / (xMax - xMin)) * plotWidth;
        }

        function yOf(value) {
            return pad.top + (1 - (transform(value) - tickMin) / (tickMax - tickMin)) * plotHeight;
        }

        ctx.font = '12px system-ui, -apple-system, "Segoe UI", sans-serif';

        const yTicks = 5;
        for (let index = 0; index <= yTicks; index++) {
            const fraction = index / yTicks;
            const y = pad.top + fraction * plotHeight;

            ctx.strokeStyle = gridColor;
            ctx.lineWidth = 1;
            ctx.beginPath();
            ctx.moveTo(pad.left, y);
            ctx.lineTo(pad.left + plotWidth, y);
            ctx.stroke();

            const transformed = tickMax - fraction * (tickMax - tickMin);
            const value = useLog ? Math.pow(10, transformed) : transformed;

            ctx.fillStyle = textColor;
            ctx.textAlign = 'right';
            ctx.textBaseline = 'middle';
            ctx.fillText(format(value), pad.left - 9, y);
        }

        const xTicks = Math.min(6, Math.max(2, Math.floor(plotWidth / 130)));
        for (let index = 0; index <= xTicks; index++) {
            const fraction = index / xTicks;
            const generation = xMin + fraction * (xMax - xMin);
            const x = pad.left + fraction * plotWidth;

            if (index > 0 && index < xTicks) {
                ctx.strokeStyle = gridColor;
                ctx.beginPath();
                ctx.moveTo(x, pad.top);
                ctx.lineTo(x, pad.top + plotHeight);
                ctx.stroke();
            }

            ctx.fillStyle = textColor;
            ctx.textAlign = 'center';
            ctx.textBaseline = 'top';
            ctx.fillText(String(Math.round(generation)), x, pad.top + plotHeight + 10);
        }

        ctx.textAlign = 'center';
        ctx.textBaseline = 'alphabetic';
        ctx.fillStyle = textColor;
        ctx.fillText('поколение', pad.left + plotWidth / 2, height - 8);

        ctx.save();
        ctx.translate(16, pad.top + plotHeight / 2);
        ctx.rotate(-Math.PI / 2);
        ctx.textAlign = 'center';
        ctx.fillText('значение функции потерь', 0, 0);
        ctx.restore();

        const series = [
            { index: 1, color: avgColor, width: 1.5, label: 'среднее' },
            { index: 2, color: bestColor, width: 2.2, label: 'лучший' },
        ];

        for (const entry of series) {
            ctx.beginPath();
            let started = false;

            for (const row of samples) {
                const value = Number(row[entry.index]);
                if (!Number.isFinite(value)) {
                    continue;
                }

                const x = xOf(row[0]);
                const y = yOf(value);

                if (!started) {
                    ctx.moveTo(x, y);
                    started = true;
                } else {
                    ctx.lineTo(x, y);
                }
            }

            ctx.strokeStyle = entry.color;
            ctx.lineWidth = entry.width;
            ctx.lineJoin = 'round';
            ctx.lineCap = 'round';
            ctx.stroke();
        }

        let legendX = pad.left;
        const legendY = 14;

        for (const entry of series) {
            ctx.fillStyle = entry.color;
            ctx.fillRect(legendX, legendY - 4, 16, 3);

            ctx.fillStyle = textColor;
            ctx.textAlign = 'left';
            ctx.textBaseline = 'middle';
            ctx.fillText(entry.label, legendX + 22, legendY);
            legendX += 24 + ctx.measureText(entry.label).width + 18;
        }
    }

    if (logToggle) {
        logToggle.addEventListener('change', draw);
    }

    let frame = 0;
    window.addEventListener('resize', function () {
        cancelAnimationFrame(frame);
        frame = requestAnimationFrame(draw);
    });

    draw();
})();