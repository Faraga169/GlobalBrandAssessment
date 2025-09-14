// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).ready(function () {
    var toastElement = $('#myToast');
    toastElement.toast('show'); // Show toast automatically
    setTimeout(function () {
        toastElement.fadeOut(); // Hide the toast after 5 seconds
    }, 5000);
});


$(document).ready(function () {
    // check saved mode in localStorage
    if (localStorage.getItem("dark-mode") === "enabled") {
        $("body").addClass("dark-mode");
        $(".navbar-brand span").removeClass("text-dark").addClass("text-light");
    }

    $("#darkModeToggle").click(function () {
        $("body").toggleClass("dark-mode");

        if ($("body").hasClass("dark-mode")) {
            $(".navbar-brand span").removeClass("text-dark").addClass("text-light");
            localStorage.setItem("dark-mode", "enabled");
        } else {
            $(".navbar-brand span").removeClass("text-light").addClass("text-dark");
            localStorage.setItem("dark-mode", "disabled");
        }
    });
});

    function toggleExtra() {
        let status = document.getElementById("statusSelect").value;
    let extra = document.getElementById("extraFields");
    if (status === "In Progress") {
        extra.style.display = "block";
            } else {
        extra.style.display = "none";
            }
        }

    document.getElementById("statusSelect").addEventListener("change", toggleExtra);
    window.onload = toggleExtra;