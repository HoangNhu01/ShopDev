// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {

    $('.btn-link[aria-expanded="true"]').closest('.accordion-item').addClass('active');
    $('.collapse').on('show.bs.collapse', function () {
        $(this).closest('.accordion-item').addClass('active');
    });

    $('.collapse').on('hidden.bs.collapse', function () {
        $(this).closest('.accordion-item').removeClass('active');
    });

    const passwordInput = document.getElementById("password");
    const showHidePassword = document.getElementById("show-hide-password");

    showHidePassword.addEventListener("click", () => {
        if (passwordInput.type === "password") {
            passwordInput.type = "text";
            showHidePassword.innerHTML = '<iconify-icon icon="quill:eye-closed" width="20" height="20"></iconify-icon>';
        } else {
            passwordInput.type = "password";
            showHidePassword.innerHTML = '<iconify-icon icon="clarity:eye-line" width="20" height="20"></iconify-icon>';
        }
    });

});