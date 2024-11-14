class HioDotnetInterop {
    constructor() {
    }

    downloadText(data, filename) {
        var text = data;
        //text = text.replace(/\n/g, "\r\n"); // To retain the Line breaks.
        var blob = new Blob([text], { type: "text/plain" });
        var anchor = document.createElement("a");
        anchor.download = filename;
        anchor.href = window.URL.createObjectURL(blob);
        anchor.target = "_blank";
        anchor.style.display = "none"; // just to be safe!
        document.body.appendChild(anchor);
        anchor.click();
        document.body.removeChild(anchor);
    }

    copyToClipboard(text) {
        navigator.clipboard.writeText(text);
    }

    readFromClipboard() {
        return navigator.clipboard.readText();
    }

    focusElement(element) {
        try {
            element.focus();
        }
        catch {

        }
    }

    downloadSvgAsFile(svgElementId, filename){
        const svgElement = document.getElementById(svgElementId);
        if (!svgElement) return;

        const svgData = new XMLSerializer().serializeToString(svgElement);
        const blob = new Blob([svgData], { type: "image/svg+xml;charset=utf-8" });
        const url = URL.createObjectURL(blob);

        const downloadLink = document.createElement("a");
        downloadLink.href = url;
        downloadLink.download = filename;
        document.body.appendChild(downloadLink);
        downloadLink.click();
        document.body.removeChild(downloadLink);
        URL.revokeObjectURL(url);
    }

    async convertImageToBase64(imageUrl){
        const response = await fetch(imageUrl);
        const blob = await response.blob();
        return new Promise((resolve) => {
            const reader = new FileReader();
            reader.onloadend = () => resolve(reader.result.split(",")[1]);
            reader.readAsDataURL(blob);
        });
    }
}

class HioHeatmapInterop {
    initializeTooltip(){
        const tooltip = document.createElement("div");
        tooltip.style.position = "absolute";
        tooltip.style.backgroundColor = "rgba(255, 255, 255, 0.8)";
        tooltip.style.padding = "5px";
        tooltip.style.border = "1px solid black";
        tooltip.style.fontSize = "12px";
        tooltip.style.display = "none";
        tooltip.style.pointerEvents = "none";
        document.body.appendChild(tooltip);

        return tooltip;
    }

    setUpMouseTracking(points, zoom, hoverRange){
        const tooltip = this.initializeTooltip();
        let svgelement = document.getElementById("heatmapSvg");

        svgelement.addEventListener("mousemove", (event) => {
            const offsetX = event.offsetX / zoom;
            const offsetY = event.offsetY / zoom;
            let found = false;

            points.forEach(point => {
                if (Math.abs(point.x - offsetX) <= hoverRange && Math.abs(point.y - offsetY) <= hoverRange) {
                    tooltip.style.left = event.clientX + 10 + "px";
                    tooltip.style.top = event.clientY + 10 + "px";
                    tooltip.innerHTML = `<strong>${point.name}</strong><br>X: ${point.x}, Y: ${point.y}<br>Value: ${point.value} ${point.unit}`;
                    tooltip.style.display = "block";
                    found = true;
                }
            });

            if (!found) {
                tooltip.style.display = "none";
            }
        });
    }
}

window.hiodotnet = new HioDotnetInterop()
window.hioheatmap = new HioHeatmapInterop()
