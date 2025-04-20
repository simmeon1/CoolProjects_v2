// ==UserScript==
// @name         FFVI guide add buttons
// @namespace    http://tampermonkey.net/
// @version      0.1
// @description  try to take over the world!
// @author       You
// @match        https://steamcommunity.com/sharedfiles/filedetails/?id=2762645480
// @icon         https://www.google.com/s2/favicons?sz=64&domain=steamcommunity.com
// @grant        none
// ==/UserScript==

window.onload = function () {
    addButton("Get list with bestiary and images", true, true);
    addButton("Get list with bestiary", true, false);
    addButton("Get list", false, false);
}

function main(event) {

    const guide = document.querySelector(".guide");
    guide.innerHTML = guide.innerHTML.replaceAll(". Phoenix Cave: 6/9", ". [Phoenix Cave: 6/9");
    const originalHtml = guide.innerHTML;

    let guideHtml = guide.innerHTML;
    const regExp = new RegExp(" \\d+\\/\\d+(]|})", 'g');
    const treasureMatches = [...guideHtml.matchAll(regExp)];

    for (let i = treasureMatches.length - 1; i > 0; i--) {
        const match = treasureMatches[i];
        const matchStartIndex = match.index;
        const startIndex = getPositionOfCharBackwards(matchStartIndex, guideHtml);
        const endIndex = matchStartIndex + match[0].toString().length;
        const wholePhrase = guideHtml.substring(startIndex, endIndex);
        const span = "<span class='chest'>" + wholePhrase + "</span>"
        guideHtml = guideHtml.substring(0, startIndex) + span + guideHtml.substring(endIndex);
    }
    guide.innerHTML = guideHtml;

    const button = event.target;
    const includeBestiary = button.includeBestiary;
    const includeImages = button.includeImages;
    let list = "";
    let selectors = ".chest, u";
    if (includeBestiary || includeImages) {
        selectors += ", .bb_table";
    }

    const collectiblesArray = Array.from(document.querySelectorAll(selectors));
    for (let i = 0; i < collectiblesArray.length; i++) {
        let collectible = collectiblesArray[i];
        if (includeImages || !collectible.querySelector("img")) {
            list += collectible.outerHTML + "<br>";
        }
    }

    const newWin = window.open();
    newWin.document.write(
        "<title>" + button.title + "</title>" +
        "<style>" +
        ".bb_table" +
        "{" +
        "	display: table;" +
        "	font-size: 12px;" +
        "}" +
        "" +
        ".bb_table_th" +
        "{" +
        "	display: table-cell;" +
        "	font-weight: bold;" +
        "	border: 1px solid #4d4d4d;" +
        "	padding: 4px;" +
        "}" +
        "" +
        ".bb_table_tr" +
        "{" +
        "	display: table-row;" +
        "}" +
        "" +
        ".bb_table_td" +
        "{" +
        "	display: table-cell;" +
        "	vertical-align: middle;" +
        "	border: 1px solid #4d4d4d;" +
        "	padding: 4px;" +
        "}" +
        "</style>");
    newWin.document.write(list);
    newWin.document.close()

    guide.innerHTML = originalHtml;
}

function getPositionOfCharBackwards(matchStartIndex, text) {
    let earliestStart = matchStartIndex - 30;
    for (let i = matchStartIndex; i > earliestStart; i--) {
        if (text[i] === '[' || text[i] === '{') {
            return i;
        }
    }
    return earliestStart;
}

function addButton(text, includeBestiary, includeImages) {
    const button = document.createElement("button");
    button.innerHTML = text;
    button.includeBestiary = includeBestiary;
    button.includeImages = includeImages;
    button.onclick = main;
    button.title = text;
    document.querySelector("body").insertAdjacentElement("afterbegin", button);
}