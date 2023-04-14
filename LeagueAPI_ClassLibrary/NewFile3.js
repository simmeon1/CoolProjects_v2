// ==UserScript==
// @name         Insert Timestamps
// @namespace    http://tampermonkey.net/
// @version      0.1
// @description  try to take over the world!
// @author       You
// @match        https://www.youtube.com/watch?v=*
// @icon         https://www.google.com/s2/favicons?sz=64&domain=youtube.com
// @grant        none
// ==/UserScript==

window.onload = main;

function main() {
    const body = document.querySelector('body');
    const observer = new MutationObserver(main2);
    observer.observe(body, {attributes: false, childList: true, subtree: true});
}

function main2(els, obs) {
    const actions = document.querySelector("#actions");
    if (!actions) {
        return;
    }

    obs.disconnect();
    const button = document.createElement("button");
    button.innerHTML = "Get Timestamps";
    button.onclick = main3;
    actions.insertAdjacentElement("afterbegin", button);
}

function main3() {
    const rightSideContainer = document.querySelector("#secondary-inner");
    const timestampTableContainerName = 'timestampTableContainer';
    const timestampTableName = 'timestampTable';
    let tableContainer = document.querySelector("#" + timestampTableContainerName);

    if (!tableContainer) {
        rightSideContainer.insertAdjacentHTML("afterbegin", "<div id = " + timestampTableContainerName + "><table id='" + timestampTableName + "'></table></div>");
        tableContainer = document.querySelector("#" + timestampTableContainerName);
        const playerHeight = document.querySelector("#player").clientHeight;
        tableContainer.style.cssText = "max-height: " + playerHeight + "px; overflow: auto; width: 100%; color: white;";
    }

    const tableElement = document.createElement("table");
    const timesAndDescs = new Set();
    for (const timestamp of document.querySelectorAll("[href*='&t=']")) {
        if (!timestamp.offsetParent) {
            continue;
        }

        const time = timestamp.textContent;
        let fullText = timestamp.offsetParent.innerText;
        const lines = fullText.split("\n");
        const desc = lines.find(l => l.includes(time));

        const timeAndDesc = time + " - " + desc;
        if (timesAndDescs.has(timeAndDesc)) {
            continue;
        }

        timesAndDescs.add(timeAndDesc);
        const row = tableElement.insertRow();
        row.onclick = () => {
            timestamp.click();
        }
        row.style.cssText = "cursor:pointer;text-decoration:underline;font-size:1.4rem;";
        row.title = time + " - " + fullText;
        
        const cell1 = row.insertCell();
        cell1.innerHTML = time;
        const cell2 = row.insertCell();
        cell2.innerHTML = desc;
    }

    tableContainer.innerHTML = "";
    tableContainer.insertAdjacentElement("afterbegin", tableElement);
}