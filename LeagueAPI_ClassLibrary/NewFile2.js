const radioName = arguments[0];
const maxSongs = parseInt(arguments[1]);
const callback = arguments[arguments.length - 1];

const obs = new MutationObserver(listHasChanged);

const cookieButton = document.querySelector("[mode=primary]");
if (cookieButton) {
    cookieButton.click()
}

setTimeout(() => {
    main(obs);
}, 2000)

function main(obs) {
    
    obs.tracks = [];
    obs.collectedTracks = new Set();
    const config = {attributes: false, childList: true, subtree: true};
    const songList = getSongList();
    obs.observe(songList, config);
    
    const radioDropdown = document.querySelector("[name='rid']");
    radioDropdown.value = getValueFromRadioName(radioName, radioDropdown);
    radioDropdown.dispatchEvent(new Event("change"))

    const searchButton = document.querySelector("button");
    searchButton.click();
}

function listHasChanged(els, obs) {
    const tracks = document.querySelectorAll("[itemprop='track']");
    tracks.forEach(t => {
        let artist = t.querySelector("[itemprop='byArtist']").textContent;
        let track = t.querySelector("[itemprop='name']").textContent;
        const artistAndTrack = artist + track;

        if (!obs.collectedTracks.has(artistAndTrack)) {
            obs.tracks.push(
                {
                    artist: artist,
                    track: track,
                }
            );
            obs.collectedTracks.add(artistAndTrack);
        }
    });

    if (maxSongs !== 0 && obs.tracks.length >= maxSongs) {
        callback(obs.tracks);
        return;
    }

    let lastPageWasActive = false;
    const pageCounter = document.querySelector(".arcPagerCont");
    const pages = pageCounter.querySelectorAll("a");
    for (const p of pages) {
        if (p.className.includes("active")) {
            lastPageWasActive = true;
        } else if (lastPageWasActive) {
            p.click();
            return;
        }
    }
    callback(obs.tracks);
}

function getValueFromRadioName(radioName, radioDropdown) {
    const options = radioDropdown.querySelectorAll("option");
    for (const o of options) {
        if (o.textContent === radioName) {
            return o.value;
        }
    }
    return null;
}

function getSongList() {
    return document.querySelector(".js-songListC");
}

//debug(main);
main(obs);