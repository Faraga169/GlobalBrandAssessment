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


