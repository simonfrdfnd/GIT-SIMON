var defaultMenuConf = {
    "documentUrlPatterns": ["https://*.service-now.com/*"]
};

var menuItems = [
    {
        "id": "openall",
        "contexts": ["all"],
        "title": "Open All"
    },
    {
        "id": "opengroup",
        "contexts": ["link"],
        "title": "Open Group"
    },
    {
        "id": "worknotesnippets",
        "contexts": ["editable"],
        "title": "Worknote Snippets"
    },
    {
        "id": "decsnippets",
        "contexts": ["selection"],
        "title": "Decoration Snippets"
    }
];

var wnsnippets = {
    "workenotesnippet1": ["worknotesnippets", "Italic", `[code]<em></em>[/code] `],
    "workenotesnippet2": ["worknotesnippets", "Strikethrough", `[code]<strike></strike>[/code] `],
    "workenotesnippet3": ["worknotesnippets", "Code (inline)", `[code]<code></code>[/code] `],
    "workenotesnippet4": ["worknotesnippets", "Code (block)", `[code]<pre><code></code></pre>[/code] `],
    "workenotesnippet5": ["worknotesnippets", "Blockquote", `[code]<blockquote></blockquote>[/code] `],
    "workenotesnippet6": ["worknotesnippets", "Hyperlink", `[code]<a href="URL" target="_blank"></a>[/code] `],
    "workenotesnippet7": ["worknotesnippets", "Bullet points", `[code][code]<ul><li>Item 1</li><li>Item 2</li><li>Item 3</li></ul>[/code][/code] `],
    "workenotesnippet8": ["worknotesnippets", "Numbered lists", `[code]<ol><li>Item 1</li><li>Item 2</li><li>Item 3</li></ol>[/code] `],
    "workenotesnippet9": ["worknotesnippets", "Header", `[code]<h3></h3>[/code] `],
};

var decsnippets = {
    "decsnippet1": ["decsnippets", "Italic", `[code]<em>%s</em>[/code] `],
    "decsnippet2": ["decsnippets", "Strikethrough", `[code]<strike>%s</strike>[/code] `],
    "decsnippet3": ["decsnippets", "Code (inline)", `[code]<code>%s</code>[/code] `],
    "decsnippet4": ["decsnippets", "Code (block)", `[code]<pre><code>%s</code></pre>[/code] `],
    "decsnippet5": ["decsnippets", "Blockquote", `[code]<blockquote>%s</blockquote>[/code] `],
    "decsnippet6": ["decsnippets", "Hyperlink", `[code]<a href="URL" target="_blank">%s</a>[/code] `],
    "decsnippet7": ["decsnippets", "Bullet points", `[code]<ul><li>%s</li><li>Item 2</li><li>Item 3</li></ul>[/code] `],
    "decsnippet8": ["decsnippets", "Numbered lists", `[code]<ol><li>%s</li><li>Item 2</li><li>Item 3</li></ol>[/code] `],
    "decsnippet9": ["decsnippets", "Header", `[code]<h3>%s</h3>[/code] `],
};

chrome.contextMenus.removeAll(initializeContextMenus);

function initializeContextMenus() {
    for (var itemIdx = 0; itemIdx < menuItems.length; itemIdx++) {
        chrome.contextMenus.create(Object.assign(menuItems[itemIdx], defaultMenuConf));
    }

    for (var snip in wnsnippets) {
        chrome.contextMenus.create(Object.assign({
            "id": snip,
            "parentId": wnsnippets[snip][0],
            "contexts": ["editable"],
            "title": wnsnippets[snip][1]
        },
            defaultMenuConf));
    }

    for (var snip in decsnippets) {
        chrome.contextMenus.create(Object.assign({
            "id": snip,
            "parentId": decsnippets[snip][0],
            "contexts": ["selection"],
            "title": decsnippets[snip][1]
        },
            defaultMenuConf));
    }
}

chrome.runtime.onMessage.addListener(function (message, sender, sendResponse) {
    console.log(message);
    if (message.event == "initializecontextmenus") {
        chrome.contextMenus.removeAll(initializeContextMenus);
    }

    return true;
});

chrome.commands.onCommand.addListener(function (command) {
    chrome.tabs.query({
        active: true,
        currentWindow: true
    }, function (tabs) {
        var clickData = { menuItemId: "" };
        switch (command) {
            case 'italic':
                clickData.menuItemId = "decsnippet1";
                break;
            case 'strikethrough':
                clickData.menuItemId = "decsnippet2";
                break;
            case 'codeinline':
                clickData.menuItemId = "decsnippet3";
                break;
            case 'codeblock':
                clickData.menuItemId = "decsnippet4";
                break;
            case 'blockquote':
                clickData.menuItemId = "decsnippet5";
                break;
            case 'hyperlink':
                clickData.menuItemId = "decsnippet6";
                break;
            case 'bullet':
                clickData.menuItemId = "decsnippet7";
                break;
            case 'numbered':
                clickData.menuItemId = "decsnippet8";
                break;
            case 'header':
                clickData.menuItemId = "decsnippet9";
                break;
            default:
                console.log(`Command not implemented...`);
                return;
        }
        insertDecSnippet(clickData);
    });
});

chrome.contextMenus.onClicked.addListener(function (clickData, tab) {
    if (clickData.menuItemId.includes('workenotesnippet'))
        insertWnSnippet(clickData, tab);
    else if (clickData.menuItemId.includes('decsnippet'))
        insertDecSnippet(clickData, tab);
    else if (clickData.menuItemId.includes('opengroup'))
        opengroup(clickData, tab);
    else if (clickData.menuItemId.includes('openall'))
        openall(clickData, tab);
});

function openall(clickData) {
    chrome.tabs.query({
        active: true,
        currentWindow: true
    }, function (tabs) {
        chrome.tabs.sendMessage(tabs[0].id, {
            "openall": "openall"
        });
    });
}

function opengroup(clickData) {
    chrome.tabs.query({
        active: true,
        currentWindow: true
    }, function (tabs) {
        chrome.tabs.sendMessage(tabs[0].id, {
            "opengroup": "opengroup"
        });
    });
}

function insertWnSnippet(clickData) {
    chrome.tabs.query({
        active: true,
        currentWindow: true
    }, function (tabs) {
        chrome.tabs.sendMessage(tabs[0].id, {
            "wnsnippet": wnsnippets[clickData.menuItemId][2]
        });
    });
}

function insertDecSnippet(clickData) {
    chrome.tabs.query({
        active: true,
        currentWindow: true
    }, function (tabs) {
        if (clickData.menuItemId == "decsnippet7") {
            chrome.tabs.sendMessage(tabs[0].id, {
                "decsnippet7": decsnippets[clickData.menuItemId][2]
            });
        }
        else if (clickData.menuItemId == "decsnippet8") {
            chrome.tabs.sendMessage(tabs[0].id, {
                "decsnippet8": decsnippets[clickData.menuItemId][2]
            });
        }
        else {
            chrome.tabs.sendMessage(tabs[0].id, {
                "decsnippet": decsnippets[clickData.menuItemId][2]
            });
        }
    });
}

