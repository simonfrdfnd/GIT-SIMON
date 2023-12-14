var lastCommand = (new Date()).getTime();

//attach event listener from popup
chrome.runtime.onMessage.addListener(function (request, sender, sendResponse) {
    if ((new Date()).getTime() - lastCommand < 500) {
        //dont trigger twice
    }

    if (request.wnsnippet) {
        insertTextAtCursor(request.wnsnippet);
    }
    else if (request.decsnippet) {
        replaceSelectedText(request.decsnippet);
    }
    else if (request.decsnippet7) {
        console.log(request.decsnippet7);
        replaceSelectedText7(request.decsnippet7);
    }
    else if (request.decsnippet8) {
        replaceSelectedText8(request.decsnippet8);
    }
    else if (request.opengroup) {
        opengroup();
    }
    else if (request.openall) {
        openall();
    }

    lastCommand = (new Date()).getTime();
});

function insertTextAtCursor(text) {
    var el = document.activeElement;
    var val = el.value;
    var endIndex;
    var range;
    var doc = el.ownerDocument;
    if (typeof el.selectionStart === 'number' &&
        typeof el.selectionEnd === 'number') {
        endIndex = el.selectionEnd;
        el.value = val.slice(0, endIndex) + text + val.slice(endIndex);
        el.selectionStart = el.selectionEnd = endIndex + text.length;
    } else if (doc.selection !== 'undefined' && doc.selection.createRange) {
        el.focus();
        range = doc.selection.createRange();
        range.collapse(false);
        range.text = text;
        range.select();
    }
}

function replaceSelectedText(snippet) {
    var el = document.activeElement;
    var selectedText = el.value.slice(el.selectionStart, el.selectionEnd);
    snippet = snippet.replace('%s', selectedText);
    var fullText = el.value;
    fullText = fullText.slice(0, el.selectionStart) + snippet + fullText.slice(el.selectionEnd, el.length);
    el.value = fullText;
}

function replaceSelectedText7() {
    var el = document.activeElement;
    var selectedText = el.value.slice(el.selectionStart, el.selectionEnd);
    var items = selectedText.split("\n");
    var result = "";
    for (var i = 0; i < items.length; i++) {
        result += "<li>" + items[i] + "</li>";
    }

    var text = "[code]<ul>" + result + "</ul>[/code]";
    var fullText = el.value;
    fullText = fullText.slice(0, el.selectionStart) + text + fullText.slice(el.selectionEnd, el.length);
    el.value = fullText;
}

function replaceSelectedText8() {
    var el = document.activeElement;
    var selectedText = el.value.slice(el.selectionStart, el.selectionEnd);
    var items = selectedText.split("\n");
    var result = "";
    for (var i = 0; i < items.length; i++) {
        result += "<li>" + items[i] + "</li>";
    }

    var text = "[code]<ol>" + result + "</ol>[/code]";
    var fullText = el.value;
    fullText = fullText.slice(0, el.selectionStart) + text + fullText.slice(el.selectionEnd, el.length);
    el.value = fullText;
}

function opengroup() {
    console.log("opengroup");
    var list_incident = document.activeElement.parentElement.parentElement.parentElement;
    var list_group = document.activeElement.parentElement.parentElement;
    var list_group_index = Array.from(list_group.parentNode.children).indexOf(list_group);
    var compteur = 0;
    for (let i = list_group_index + 1; i < list_incident.childNodes.length; i++) {
        console.log(compteur);
        if(list_incident.childNodes[i].className.includes("list_group")) {
          return;
        }

        if (list_incident.childNodes[i].className.includes("list_row ")) {
            if (compteur == 19) {
                const response = confirm("20 tabs are already open. Would you like to continue ?");
                if (!response) {
                  return;
                }
            }
            var link = list_incident.childNodes[i].childNodes[2].childNodes[0];
            window.open(link.href);
            compteur+= 1;
        }
    }
}

function openall() {
    console.log("openall");
    var incident_table = document.getElementById("incident_table"); 
    if (incident_table != null) {
        var list_incident = incident_table.childNodes[2];
        var compteur = 0;
        for (let i = 0; i < list_incident.childNodes.length; i++) {
            console.log(list_incident.childNodes[i]);
            if (list_incident.childNodes[i].className.includes("list_row ")) {
                if (compteur == 19) {
                    const response = confirm("20 tabs are already open. Would you like to continue ?");
                    if (!response) {
                      return;
                    }
                }
                var link = list_incident.childNodes[i].childNodes[2].childNodes[0];
                window.open(link.href);
                compteur+= 1;
            }
        }
    }
    
    var change_request_table = document.getElementById("change_request_table"); 
    if (change_request_table != null) {
        var list_change = change_request_table.childNodes[2];
        var compteur = 0;
        for (let i = 0; i < list_change.childNodes.length; i++) {
            console.log(list_change.childNodes[i]);
            if (list_change.childNodes[i].className.includes("list_row ")) {
                if (compteur == 19) {
                    const response = confirm("20 tabs are already open. Would you like to continue ?");
                    if (!response) {
                      return;
                    }
                }
                var link = list_change.childNodes[i].childNodes[2].childNodes[0];
                window.open(link.href);
                compteur+= 1;
            }
        }
    }
}

