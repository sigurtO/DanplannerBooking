// wwwroot/js/map.js
window.CampsiteMap = (function () {
    // --- Pan & Zoom via SVG viewBox (reliable) ---
    function initPanZoom(containerSel, svgSel) {
        const container = document.querySelector(containerSel);
        const svg = container ? container.querySelector(svgSel) : null;
        if (!container || !svg) { console.warn("initPanZoom: container/svg not found"); return; }

        const vbStr = svg.getAttribute("viewBox") || "0 0 100 100";
        let [vx, vy, vw, vh] = vbStr.split(/\s+/).map(Number);
        if (![vx, vy, vw, vh].every(Number.isFinite)) { vx = 0; vy = 0; vw = 100; vh = 100; }

        let isPanning = false;
        let panStart = { x: 0, y: 0 };
        let vbStart = { vx, vy };

        function applyVB() { svg.setAttribute("viewBox", `${vx} ${vy} ${vw} ${vh}`); }
        function clientToSvgUnits(dxPx, dyPx) {
            const rect = svg.getBoundingClientRect();
            return { ux: dxPx * (vw / rect.width), uy: dyPx * (vh / rect.height) };
        }

        svg.addEventListener("mousedown", (e) => {
            if (e.target?.closest?.(".unit")) return; // don't pan when starting on a marker
            isPanning = true;
            panStart = { x: e.clientX, y: e.clientY };
            vbStart = { vx, vy };
        });
        window.addEventListener("mousemove", (e) => {
            if (!isPanning) return;
            const { ux, uy } = clientToSvgUnits(e.clientX - panStart.x, e.clientY - panStart.y);
            vx = vbStart.vx - ux; vy = vbStart.vy - uy; applyVB();
        });
        window.addEventListener("mouseup", () => { isPanning = false; });

        svg.addEventListener("wheel", (e) => {
            e.preventDefault();
            const rect = svg.getBoundingClientRect();
            const mxPx = e.clientX - rect.left, myPx = e.clientY - rect.top;
            const { ux: mx, uy: my } = clientToSvgUnits(mxPx, myPx);
            const absX = vx + mx, absY = vy + my;
            const factor = e.deltaY < 0 ? 0.9 : 1.1;
            const newVw = Math.max(10, Math.min(10000, vw * factor));
            const newVh = Math.max(10, Math.min(10000, vh * factor));
            vx = absX - (mx * factor); vy = absY - (my * factor);
            vw = newVw; vh = newVh; applyVB();
        }, { passive: false });

        applyVB();
        console.log("initPanZoom(viewBox): OK");
    }

    // --- Delegated drag for any <g class="unit" data-id="..."> ---
    function enableDrag(svgSelector, _itemSelector, dotNetRef) {
        const svg = document.querySelector(svgSelector);
        if (!svg) { console.warn("enableDrag: svg not found", svgSelector); return; }

        let dragging = null; // { el, id, startMouse:{x,y}, startPos:{x,y} }

        function clientToSvgPoint(evt) {
            const pt = svg.createSVGPoint(); pt.x = evt.clientX; pt.y = evt.clientY;
            const ctm = svg.getScreenCTM(); if (!ctm) return { x: 0, y: 0 };
            const p = pt.matrixTransform(ctm.inverse()); return { x: p.x, y: p.y };
        }
        function getTranslate(el) {
            const t = el.getAttribute("transform") || "translate(0,0)";
            const m = /translate\(([-\d.]+),\s*([-\d.]+)\)/.exec(t);
            return m ? [parseFloat(m[1]), parseFloat(m[2])] : [0, 0];
        }

        svg.addEventListener("mousedown", (e) => {
            const unitEl = e.target?.closest?.(".unit");
            if (!unitEl) return; // let pan handle it
            e.stopPropagation(); e.preventDefault();
            const id = unitEl.getAttribute("data-id"); if (!id) return;
            const m = clientToSvgPoint(e);
            const [tx, ty] = getTranslate(unitEl);
            dragging = { el: unitEl, id, startMouse: m, startPos: { x: tx, y: ty } };
        });

        window.addEventListener("mousemove", (e) => {
            if (!dragging) return;
            const m = clientToSvgPoint(e);
            const dx = m.x - dragging.startMouse.x;
            const dy = m.y - dragging.startMouse.y;
            const nx = Math.round(dragging.startPos.x + dx);
            const ny = Math.round(dragging.startPos.y + dy);
            dragging.el.setAttribute("transform", `translate(${nx},${ny})`);
        });

        window.addEventListener("mouseup", async (e) => {
            if (!dragging) return;
            const m = clientToSvgPoint(e);
            const dx = m.x - dragging.startMouse.x;
            const dy = m.y - dragging.startMouse.y;
            const nx = Math.round(dragging.startPos.x + dx);
            const ny = Math.round(dragging.startPos.y + dy);
            try { await dotNetRef?.invokeMethodAsync?.("OnMarkerMoved", dragging.id, nx, ny); } catch { }
            dragging = null;
        });

        console.log("enableDrag(delegated): ready");
    }

    function downloadText(filename, text) {
        const blob = new Blob([text], { type: "application/json" });
        const url = URL.createObjectURL(blob);
        const a = document.createElement("a"); a.href = url; a.download = filename; a.click();
        setTimeout(() => URL.revokeObjectURL(url), 800);
    }

    console.log("CampsiteMap loaded (viewBox + delegated drag)");
    return { initPanZoom, enableDrag, downloadText };
})();
