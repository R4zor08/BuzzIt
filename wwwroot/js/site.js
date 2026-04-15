// Bootstrap-style validation for forms marked with .needs-validation
document.addEventListener("DOMContentLoaded", function () {
    const forms = document.querySelectorAll("form.needs-validation");
    forms.forEach(function (form) {
        form.addEventListener("submit", function (event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add("was-validated");
        });
    });
});
