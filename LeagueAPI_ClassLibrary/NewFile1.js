function doWork() {
    // const observer = new MutationObserver(logEvents);
    // var eventList = getEventList();
    // observer.observe(eventList, {attributes: false, childList: true, subtree: true});
    // logEvents(null, observer);
    //
    var numberOfEventsSpan = document.querySelector("[data-testid='numberEvents']");
    const observer = new MutationObserver(logEvents);
    // var config = { characterData: true, childList: true, attributes: true };
    var config = {attributes: false, childList: true, subtree: true};
    observer.observe(numberOfEventsSpan.parentElement.parentElement.parentElement, config);

    var today = new Date().toLocaleDateString();
    putInDate(today);
}

function logEvents(elements, observer) {
    var events = getEvents();
    var lastDate = null;
    for (let i = 0; i < events.length; i++) {
        var event = events[i];
        var propsKey = getReactPropsKey(event);
        var eventProps = event[propsKey].children.props;
        var title = getStringFromProp(eventProps.title);
        var subtitle = getStringFromProp(eventProps.subtitle);
        var date = eventProps.dateStart.localDate;
        lastDate = date;
        var str = title + ' - ' + subtitle + ' - ' + date;
        console.log(str);
    }

    if (lastDate) {
        var date = new Date(lastDate);
        date.setDate(date.getDate() + 1);
        putInDate(date.toLocaleDateString());
    }
    // observer.disconnect();
    // var eventList = getEventList();
    // eventList.innerHTML = "";
    // observer.observe(eventList, {attributes: false, childList: true, subtree: true});
    // var moreButton = document.querySelector('[data-testid="pagination-button"]');
    // moreButton.click();
}

function getEvents() {
    return document.querySelectorAll('[data-testid="eventList"] > li');
}

function getEventList() {
    return document.querySelector('[data-testid="eventList"]');
}

function getStringFromProp(prop) {
    return Object.values(prop).toString();
}

function getReactPropsKey(el) {
    var keys = Object.keys(el);
    for (let i = 0; i < keys.length; i++) {
        var key = keys[i];
        if (key.includes('_reactProps')) {
            return key;
        }
    }
    return "";
}

function putInDate(today) {
    var dateInput = document.querySelector('#date');
    var datePropsKey = getReactPropsKey(dateInput);
    dateInput[datePropsKey].onChange({target: {value: today}});
}