document.addEventListener('mousedownddddd', function (event) {
    //let elem = event.target;
    //let jsonObject =
    //{
    //    Key: 'click',
    //    Value: elem.name || elem.id || elem.tagName || "Unkown"
    //};

    let jsonObject =
    {
        Key: 'mousedown',
        Value:
        {
            X: event.screenX,
            Y: event.screenY
        }
    };

    window.chrome.webview.postMessage(jsonObject);
    event.preventDefault();
});

document.addEventListener('mousedowndddd', function (event) {
    let elem = event.target;
    let jsonObject =
    {
        eventName: 'right-click',
        eventValue: elem.value || "Unknown",
        elemName: elem.name || "Unknown",
        elemId: elem.id || "Unknown",
        elemTagName: elem.tagName || "Unknown",
        elemPixels: event.clientX + "," + event.clientY,
        value: elem.innerText || "Unknown"
    };

    var element = document.elementFromPoint(event.clientX, event.clientY).click();

    window.chrome.webview.postMessage(jsonObject);
});



document.addEventListener('click', function (event) {
    
    let elem = event.target;
    let jsonObject = {
        eventName: 'click',
        eventValue: elem.value || "Unknown",
        elemName: elem.name || "Unknown",
        elemId: elem.id || "Unknown",
        elemTagName: elem.tagName || "Unknown",
        elemPixels: event.clientX + "," + event.clientY,
        value: elem.innerText || "Unknown"
    };

    window.chrome.webview.postMessage(jsonObject);

    //if (e.target?.tagName?.toLowerCase() === 'a') {
    //    e.stopPropagation();
    //    e.preventDefault();
    //    const href = e.target.getAttribute('href');
    //    console.log('prevented');
    //}

}, true);