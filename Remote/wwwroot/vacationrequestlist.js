'use strict';

// component instantiation
mdc.topAppBar.MDCTopAppBar.attachTo(document.querySelector(".mdc-top-app-bar"));
const menue = mdc.menu.MDCMenu.attachTo(document.querySelector(".mdc-menu"));
const snackbar = mdc.snackbar.MDCSnackbar.attachTo(document.querySelector(".mdc-snackbar"));
let selectedListItem = document.querySelector(".mdc-list-item");

// event listeners
function handleMenuClick(state) {
    const r = new XMLHttpRequest();
    r.addEventListener("load", function () {
        if (r.status === 200 || r.status === 204) {
            selectedListItem.dataset.state = state;
            updateStateIcon(selectedListItem);
        } else {
            snackbar.labelText = "Request failed. Server returned " + r.status;
            snackbar.open();
        }
    });
    r.addEventListener("error", function () {
        snackbar.labelText = "Request failed. Please try again in 5 seconds.";
        snackbar.open();
    });
    r.open("PATCH", selectedListItem.getElementsByTagName("a")[0].href);
    r.setRequestHeader("Content-Type", "application/merge-patch+json");
    r.send(JSON.stringify({
        state: state
    }));
}

document.getElementById("menu_accept").addEventListener("click", function(){
    handleMenuClick("accepted" );
});

document.getElementById("menu_reject").addEventListener("click", function(){
    handleMenuClick("rejected");
});

document.getElementById("menu_cancel").addEventListener("click", function (){
    handleMenuClick("cancelled");
});

const moreIconBtns = document.querySelectorAll(".mdc-icon-button");
for (let i = 0; i < moreIconBtns.length; i++) {
    const elmRipple = mdc.ripple.MDCRipple.attachTo(moreIconBtns[i]);
    elmRipple.unbounded = true;

    moreIconBtns[i].addEventListener("click", function (e) {
        e.stopPropagation();
        menue.open = !menue.open;
        selectedListItem = e.currentTarget.parentElement;
        const btnElementRect = e.currentTarget.getBoundingClientRect();
        menue.setAbsolutePosition(btnElementRect.left, btnElementRect.top);
    });
}

function updateStateIcon (element){
    switch (element.dataset.state) {
        case "new":
            element.querySelector(".state-icon").innerHTML = "";
            break;
        case "accepted":
            element.querySelector(".state-icon").innerHTML = "check_circle";
            break;
        case "rejected":
            element.querySelector(".state-icon").innerHTML = "block";
            break;
        case "cancelled":
            element.querySelector(".state-icon").innerHTML = "cancel";
            break;
    }
}

const listItems = document.querySelectorAll(".dmc-list-item");
for (let i = 0; i < listItems.length; i++) {
    updateStateIcon(listItems[i]);
}


