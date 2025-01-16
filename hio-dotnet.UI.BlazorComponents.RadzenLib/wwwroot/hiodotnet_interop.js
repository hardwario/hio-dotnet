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

    scrollToBottom(elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            element.scrollTop = element.scrollHeight;
        }
    }

    downloadFileFromByteArray(file) {
        const link = document.createElement('a');
        link.download = file.fileName;
        link.href = URL.createObjectURL(
            new Blob([new Uint8Array(file.byteArray)], { type: file.contentType })
        );
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
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

class HioJsonViewerInterop {
    async loadJsonViewer() {
        await import("https://unpkg.com/@alenaksu/json-viewer@2.1.0/dist/json-viewer.bundle.js");
    }

    initializeJsonViewer(elementId, jsonData) {
        const selector = `#${CSS.escape(elementId)}`;
        const jsonViewer = document.querySelector(selector);

        if (jsonViewer) {
            jsonViewer.data = JSON.parse(jsonData);
            this.waitForShadowRoot(jsonViewer);
        } else {
            console.error(`Element s ID '${elementId}' nebyl nalezen.`);
        }
    }

    waitForShadowRoot(jsonViewer) {
        const intervalId = setInterval(() => {
            if (jsonViewer.shadowRoot) {
                clearInterval(intervalId);
                this.observeShadowRoot(jsonViewer.shadowRoot);
            } else {
                console.log(`Čekám na vytvoření shadowRoot pro element s ID '${jsonViewer.id}'`);
            }
        }, 100);
    }

    observeShadowRoot(root) {
        const observer = new MutationObserver((mutationsList) => {
            mutationsList.forEach((mutation) => {
                mutation.addedNodes.forEach((node) => {
                    if (node.nodeType === Node.ELEMENT_NODE) {
                        this.addCopyButtonsToPrimitiveNodes(node);
                    }
                });
            });
        });

        observer.observe(root, { childList: true, subtree: true });
        this.addCopyButtonsToPrimitiveNodes(root);
    }

    addCopyButtonsToPrimitiveNodes(root) {
        const primitiveElements = root.querySelectorAll(`[part^="primitive"]`);

        primitiveElements.forEach((primitiveElement) => {
            if (!primitiveElement.hasAttribute("data-copy-added")) {
                primitiveElement.setAttribute("data-copy-added", "true");

                const copyButton = document.createElement("span");
                copyButton.textContent = "📋"; // copy icon
                copyButton.style.cursor = "pointer";
                copyButton.style.marginLeft = "5px";
                copyButton.style.fontSize = "14px";

                copyButton.onclick = () => {
                    const textToCopy = primitiveElement.cloneNode(true);
                    textToCopy.querySelector("span")?.remove();

                    navigator.clipboard.writeText(textToCopy.textContent.trim())
                        .then(() => {
                            copyButton.textContent = "✔️";
                            setTimeout(() => {
                                copyButton.textContent = "📋";
                            }, 1500);
                        })
                        .catch(err => console.error("Chyba kopírování:", err));
                };

                primitiveElement.appendChild(copyButton);
            }
        });
    }

    expand(elementId, path) {
        const jsonViewer = document.querySelector(`#${CSS.escape(elementId)}`);
        jsonViewer?.expand(path);
    }

    expandAll(elementId) {
        const jsonViewer = document.querySelector(`#${CSS.escape(elementId)}`);
        jsonViewer?.expandAll();
    }

    collapse(elementId, path) {
        const jsonViewer = document.querySelector(`#${CSS.escape(elementId)}`);
        jsonViewer?.collapse(path);
    }

    collapseAll(elementId) {
        const jsonViewer = document.querySelector(`#${CSS.escape(elementId)}`);
        jsonViewer?.collapseAll();
    }

    filter(elementId, filterText) {
        const jsonViewer = document.querySelector(`#${CSS.escape(elementId)}`);
        jsonViewer?.filter(filterText);
    }

    search(elementId, searchText) {
        const jsonViewer = document.querySelector(`#${CSS.escape(elementId)}`);
        jsonViewer?.search(searchText);
    }
}

window.hiodotnet = new HioDotnetInterop()
window.hioheatmap = new HioHeatmapInterop()
window.hiojsonviewer = new HioJsonViewerInterop()
