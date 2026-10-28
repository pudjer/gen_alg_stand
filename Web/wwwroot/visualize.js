(function () {
    'use strict';

    const holder = document.getElementById('visualization');
    const canvas = document.getElementById('visualization-canvas');
    const dataBlock = document.getElementById('visualization-data');
    if (!holder || !canvas || !dataBlock) {
        return;
    }

    let data = null;
    try {
        data = JSON.parse(dataBlock.textContent || 'null');
    } catch (error) {
        return;
    }

    if (!data) {
        return;
    }

    const kind = holder.getAttribute('data-kind');
    const font = '12px system-ui, -apple-system, "Segoe UI", sans-serif';
    const ink = '#1e293b';
    const muted = '#64748b';
    const grid = '#e2e8f0';
    const accent = '#2f6feb';
    const bestColor = '#e4572e';

    function setup() {
        const dpr = window.devicePixelRatio || 1;
        const width = Math.max(320, canvas.clientWidth || 640);
        const height = 320;

        canvas.width = Math.round(width * dpr);
        canvas.height = Math.round(height * dpr);

        const ctx = canvas.getContext('2d');
        ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
        ctx.clearRect(0, 0, width, height);
        ctx.font = font;

        return { ctx: ctx, width: width, height: height };
    }

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

        return magnitude >= 100 ? value.toFixed(0) : value.toFixed(2);
    }

    function palette(t) {
        const stops = [
            [47, 111, 235],
            [56, 178, 172],
            [250, 204, 21],
            [228, 87, 46],
        ];

        const scaled = Math.min(0.999, Math.max(0, t)) * (stops.length - 1);
        const index = Math.floor(scaled);
        const fraction = scaled - index;
        const first = stops[index];
        const second = stops[index + 1] || stops[index];

        return [
            Math.round(first[0] + (second[0] - first[0]) * fraction),
            Math.round(first[1] + (second[1] - first[1]) * fraction),
            Math.round(first[2] + (second[2] - first[2]) * fraction),
        ];
    }

    function drawBits() {
        const state = setup();
        const ctx = state.ctx;
        const width = state.width;
        const height = state.height;
        const bits = data.bits || [];

        if (bits.length === 0) {
            return;
        }

        const pad = 16;
        const available = width - pad * 2;
        const perRow = Math.max(1, Math.min(bits.length, Math.floor(available / 8)));
        const rows = Math.ceil(bits.length / perRow);
        const cellWidth = Math.min(14, available / perRow);
        const rowHeight = Math.max(2, Math.min(16, (height - 58) / rows));
        const offsetX = pad + (available - perRow * cellWidth) / 2;

        for (let index = 0; index < bits.length; index++) {
            const x = offsetX + (index % perRow) * cellWidth;
            const y = pad + Math.floor(index / perRow) * rowHeight;

            ctx.fillStyle = bits[index] ? accent : grid;
            ctx.fillRect(x, y, Math.max(1, cellWidth - 1), Math.max(2, rowHeight - 2));
        }

        ctx.fillStyle = muted;
        ctx.textAlign = 'left';
        ctx.textBaseline = 'bottom';
        ctx.fillText('единиц: ' + data.ones + ' из ' + data.count, pad, height - 4);
    }

    function drawHeatmap() {
        const state = setup();
        const ctx = state.ctx;
        const width = state.width;
        const height = state.height;

        const nx = data.nx;
        const ny = data.ny;
        const values = data.values || [];
        if (!nx || !ny || values.length !== nx * ny) {
            return;
        }

        let min = Infinity;
        let max = -Infinity;

        for (const value of values) {
            if (Number.isFinite(value)) {
                min = Math.min(min, value);
                max = Math.max(max, value);
            }
        }

        if (!Number.isFinite(min) || !Number.isFinite(max)) {
            return;
        }

        if (max - min < 1e-12) {
            max = min + 1e-12;
        }

        const offscreen = document.createElement('canvas');
        offscreen.width = nx;
        offscreen.height = ny;
        const offscreenContext = offscreen.getContext('2d');
        const image = offscreenContext.createImageData(nx, ny);

        for (let index = 0; index < values.length; index++) {
            const value = values[index];
            const t = Number.isFinite(value) ? (value - min) / (max - min) : 0;
            const color = palette(t);
            const offset = index * 4;

            image.data[offset] = color[0];
            image.data[offset + 1] = color[1];
            image.data[offset + 2] = color[2];
            image.data[offset + 3] = 255;
        }

        offscreenContext.putImageData(image, 0, 0);

        const pad = { left: 62, right: 18, top: 16, bottom: 42 };
        const plotWidth = Math.max(10, width - pad.left - pad.right);
        const plotHeight = Math.max(10, height - pad.top - pad.bottom);

        ctx.imageSmoothingEnabled = true;
        ctx.drawImage(offscreen, pad.left, pad.top, plotWidth, plotHeight);

        ctx.strokeStyle = grid;
        ctx.lineWidth = 1;
        ctx.strokeRect(pad.left, pad.top, plotWidth, plotHeight);

        const bestX = pad.left + ((data.best[0] - data.x0) / (data.x1 - data.x0)) * plotWidth;
        const bestY = pad.top + ((data.y1 - data.best[1]) / (data.y1 - data.y0)) * plotHeight;

        ctx.beginPath();
        ctx.arc(bestX, bestY, 5, 0, Math.PI * 2);
        ctx.fillStyle = '#ffffff';
        ctx.fill();
        ctx.lineWidth = 2;
        ctx.strokeStyle = bestColor;
        ctx.stroke();

        ctx.fillStyle = muted;
        ctx.textAlign = 'center';
        ctx.textBaseline = 'top';
        ctx.fillText(format(data.x0), pad.left, pad.top + plotHeight + 8);
        ctx.fillText(format(data.x1), pad.left + plotWidth, pad.top + plotHeight + 8);
        ctx.fillText('ген 1', pad.left + plotWidth / 2, pad.top + plotHeight + 24);

        ctx.textAlign = 'right';
        ctx.textBaseline = 'middle';
        ctx.fillText(format(data.y1), pad.left - 8, pad.top);
        ctx.fillText(format(data.y0), pad.left - 8, pad.top + plotHeight);

        ctx.save();
        ctx.translate(16, pad.top + plotHeight / 2);
        ctx.rotate(-Math.PI / 2);
        ctx.textAlign = 'center';
        ctx.fillText('ген 2', 0, 0);
        ctx.restore();
    }

    function drawCurve() {
        const state = setup();
        const ctx = state.ctx;
        const width = state.width;
        const height = state.height;
        const xs = data.xs || [];
        const ys = data.ys || [];

        if (xs.length === 0 || xs.length !== ys.length) {
            return;
        }

        let min = Infinity;
        let max = -Infinity;

        for (const value of ys) {
            if (Number.isFinite(value)) {
                min = Math.min(min, value);
                max = Math.max(max, value);
            }
        }

        if (!Number.isFinite(min) || !Number.isFinite(max)) {
            return;
        }

        if (max - min < 1e-12) {
            const delta = Math.abs(max) > 0 ? Math.abs(max) * 0.1 : 1;
            min -= delta;
            max += delta;
        }

        const pad = { left: 64, right: 18, top: 16, bottom: 42 };
        const plotWidth = Math.max(10, width - pad.left - pad.right);
        const plotHeight = Math.max(10, height - pad.top - pad.bottom);
        const x0 = xs[0];
        const x1 = xs[xs.length - 1];

        function xOf(value) {
            return pad.left + ((value - x0) / (x1 - x0 || 1)) * plotWidth;
        }

        function yOf(value) {
            return pad.top + (1 - (value - min) / (max - min)) * plotHeight;
        }

        for (let index = 0; index <= 5; index++) {
            const fraction = index / 5;
            const y = pad.top + fraction * plotHeight;

            ctx.strokeStyle = grid;
            ctx.lineWidth = 1;
            ctx.beginPath();
            ctx.moveTo(pad.left, y);
            ctx.lineTo(pad.left + plotWidth, y);
            ctx.stroke();

            ctx.fillStyle = muted;
            ctx.textAlign = 'right';
            ctx.textBaseline = 'middle';
            ctx.fillText(format(max - fraction * (max - min)), pad.left - 9, y);
        }

        ctx.beginPath();
        let started = false;

        for (let index = 0; index < xs.length; index++) {
            const value = ys[index];
            if (!Number.isFinite(value)) {
                continue;
            }

            const x = xOf(xs[index]);
            const y = yOf(value);

            if (!started) {
                ctx.moveTo(x, y);
                started = true;
            } else {
                ctx.lineTo(x, y);
            }
        }

        ctx.strokeStyle = accent;
        ctx.lineWidth = 2;
        ctx.lineJoin = 'round';
        ctx.stroke();

        if (data.best) {
            ctx.beginPath();
            ctx.arc(xOf(data.best[0]), yOf(data.best[1]), 5, 0, Math.PI * 2);
            ctx.fillStyle = '#ffffff';
            ctx.fill();
            ctx.lineWidth = 2;
            ctx.strokeStyle = bestColor;
            ctx.stroke();
        }

        ctx.fillStyle = muted;
        ctx.textAlign = 'center';
        ctx.textBaseline = 'top';
        ctx.fillText(format(x0), pad.left, pad.top + plotHeight + 8);
        ctx.fillText(format(x1), pad.left + plotWidth, pad.top + plotHeight + 8);
        ctx.fillText('ген', pad.left + plotWidth / 2, pad.top + plotHeight + 24);
    }

    function drawTspMap() {
        const state = setup();
        const ctx = state.ctx;
        const width = state.width;
        const height = state.height;
        const cities = data.cities || [];
        const order = data.order || [];

        if (cities.length < 2 || order.length !== cities.length) {
            return;
        }

        let minX = Infinity;
        let maxX = -Infinity;
        let minY = Infinity;
        let maxY = -Infinity;

        for (const city of cities) {
            minX = Math.min(minX, city[0]);
            maxX = Math.max(maxX, city[0]);
            minY = Math.min(minY, city[1]);
            maxY = Math.max(maxY, city[1]);
        }

        if (maxX - minX < 1e-9) {
            minX -= 1;
            maxX += 1;
        }

        if (maxY - minY < 1e-9) {
            minY -= 1;
            maxY += 1;
        }

        const pad = { left: 22, right: 22, top: 18, bottom: 34 };
        const plotWidth = Math.max(10, width - pad.left - pad.right);
        const plotHeight = Math.max(10, height - pad.top - pad.bottom);
        const spanX = maxX - minX;
        const spanY = maxY - minY;
        const scale = Math.min(plotWidth / spanX, plotHeight / spanY);
        const offsetX = pad.left + ((plotWidth - spanX * scale) / 2);
        const offsetY = pad.top + ((plotHeight - spanY * scale) / 2);

        function xOf(value) {
            return offsetX + (value - minX) * scale;
        }

        function yOf(value) {
            return offsetY + ((maxY - value) * scale);
        }

        ctx.fillStyle = '#f8fafc';
        ctx.fillRect(offsetX, offsetY, spanX * scale, spanY * scale);

        ctx.strokeStyle = '#cbd5e1';
        ctx.lineWidth = 1;
        ctx.strokeRect(offsetX, offsetY, spanX * scale, spanY * scale);

        const n = order.length;
        const dotRadius = n <= 40 ? 4 : n <= 120 ? 3 : 2;

        ctx.globalAlpha = 0.45;
        ctx.strokeStyle = accent;
        ctx.lineWidth = n <= 120 ? 1.6 : 1.1;
        ctx.lineJoin = 'round';
        ctx.beginPath();

        for (let index = 0; index < n; index++) {
            const from = cities[order[index]];
            const to = cities[order[(index + 1) % n]];

            if (!from || !to) {
                continue;
            }

            ctx.moveTo(xOf(from[0]), yOf(from[1]));
            ctx.lineTo(xOf(to[0]), yOf(to[1]));
        }

        ctx.stroke();
        ctx.globalAlpha = 1;

        for (let index = 0; index < n; index++) {
            const city = cities[index];
            if (!city) {
                continue;
            }

            const isStart = index === order[0];

            ctx.beginPath();
            ctx.arc(xOf(city[0]), yOf(city[1]), isStart ? dotRadius + 1.5 : dotRadius, 0, Math.PI * 2);
            ctx.fillStyle = isStart ? bestColor : ink;
            ctx.fill();
        }

        if (n <= 30) {
            ctx.fillStyle = muted;
            ctx.font = '11px system-ui, -apple-system, "Segoe UI", sans-serif';
            ctx.textAlign = 'center';
            ctx.textBaseline = 'middle';

            for (let index = 0; index < n; index++) {
                const city = cities[index];
                if (!city) {
                    continue;
                }

                ctx.fillText(String(index), xOf(city[0]), yOf(city[1]) - 11);
            }
        }

        ctx.fillStyle = muted;
        ctx.textAlign = 'left';
        ctx.textBaseline = 'top';
        ctx.fillText('карта городов · старт тура выделен', pad.left, height - 20);
    }

    function drawQueens() {
        const state = setup();
        const ctx = state.ctx;
        const width = state.width;
        const height = state.height;
        const queens = data.queens || [];
        const conflicted = data.conflicted || [];
        const size = queens.length;

        if (size === 0) {
            return;
        }

        const board = Math.min(width, height) - 36;
        const cell = board / size;
        const originX = (width - board) / 2;
        const originY = (height - board) / 2;

        for (let row = 0; row < size; row++) {
            for (let column = 0; column < size; column++) {
                ctx.fillStyle = (row + column) % 2 === 0 ? '#f8fafc' : '#e8edf5';
                ctx.fillRect(originX + column * cell, originY + row * cell, cell, cell);
            }
        }

        ctx.strokeStyle = '#cbd5e1';
        ctx.lineWidth = 1;
        ctx.strokeRect(originX, originY, board, board);

        for (let row = 0; row < size; row++) {
            const column = queens[row];
            const centerX = originX + (column + 0.5) * cell;
            const centerY = originY + (row + 0.5) * cell;

            ctx.beginPath();
            ctx.arc(centerX, centerY, Math.max(3, cell * 0.31), 0, Math.PI * 2);
            ctx.fillStyle = conflicted[row] ? '#dc2626' : ink;
            ctx.fill();
        }
    }

    function draw() {
        if (kind === 'bits') {
            drawBits();
        } else if (kind === 'heatmap') {
            drawHeatmap();
        } else if (kind === 'curve') {
            drawCurve();
        } else if (kind === 'tsp-map') {
            drawTspMap();
        } else if (kind === 'nqueens') {
            drawQueens();
        }
    }

    let frame = 0;
    window.addEventListener('resize', function () {
        cancelAnimationFrame(frame);
        frame = requestAnimationFrame(draw);
    });

    draw();
})();