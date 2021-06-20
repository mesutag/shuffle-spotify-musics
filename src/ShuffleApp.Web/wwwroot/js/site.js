// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
String.isNullOrEmpty = function (value) {
    return (!value);
}


var searchQueries = [];
function check() {
    fix();
    $(".artist-item").each(function (index) {
        var val = $(this).text().trim();
        if (searchQueries.includes(val)) {
            $(this).removeClass("btn-light").addClass("btn-dark");
        }
        else {
            $(this).addClass("btn-light").removeClass("btn-dark");
        }
    });
    console.log(searchQueries);
}

function fix() {
    var query = $("#searchquery").val().trim();
    searchQueries = query.split(",");
    var d = [];
    searchQueries = searchQueries.filter(function (value, index, arr) {
        if (value.trim() != "") {
            d.push(value.trim());
        }
        return true;
    });
    searchQueries = d;
}
$(document).ready(function () {
    check();
    $("#searchquery").on("keydown", function (e) {
        check();
    })
    $(".artist-item").on("click", function () {
        select(this);

    });
    $(".device-item").on("click", function () {
        var deviceId = $(this).data("device-id");
        $("#selectedDevice").val(deviceId);
        $(".device-item").find("span").remove();
        $(".device-item").removeClass("btn-dark");
        $(this).removeClass("btn-light").addClass("btn-dark");
        $(this).append("<span class=\"badge badge-light\"><svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" fill=\"currentColor\" class=\"bi bi-award-fill\" viewBox=\"0 0 16 16\">"+
                                    "<path d=\"m8 0 1.669.864 1.858.282.842 1.68 1.337 1.32L13.4 6l.306 1.854-1.337 1.32-.842 1.68-1.858.282L8 12l-1.669-.864-1.858-.282-.842-1.68-1.337-1.32L2.6 6l-.306-1.854 1.337-1.32.842-1.68L6.331.864 8 0z\" />"+
                                    "<path d=\"M4 11.794V16l4-1 4 1v-4.206l-2.018.306L8 13.126 6.018 12.1 4 11.794z\" />"+
                                "</svg></span>");
    })
});
function select(item) {
    var val = $(item).text().trim();
    if (!searchQueries.includes(val)) {
        searchQueries.push(val);
        $(item).removeClass("btn-light").addClass("btn-dark");
    }
    else {
        searchQueries = searchQueries.filter(function (value, index, arr) {

            return value.trim() != val.trim();
        });
        $(item).addClass("btn-light").removeClass("btn-dark");
    }

    $("#searchquery").val(searchQueries.join(", "))
}


let deferredPrompt;

window.addEventListener('beforeinstallprompt', (e) => {
    // Prevent the mini-infobar from appearing on mobile
    e.preventDefault();
    // Stash the event so it can be triggered later.
    deferredPrompt = e;
    // Update UI notify the user they can install the PWA
    showInstallPromotion();
});

buttonInstall.addEventListener('click', (e) => {
    // Hide the app provided install promotion
    hideMyInstallPromotion();
    // Show the install prompt
    deferredPrompt.prompt();
    // Wait for the user to respond to the prompt
    deferredPrompt.userChoice.then((choiceResult) => {
        if (choiceResult.outcome === 'accepted') {
            console.log('User accepted the install prompt');
        } else {
            console.log('User dismissed the install prompt');
        }
    });
});


function openWebApp() {
    window.open('https://open.spotify.com/', '_blank');
    setInterval(function () { window.location.reload(); }, 3000)
}
