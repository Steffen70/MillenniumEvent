const alertBox = document.querySelector(".alert-warning");

const successBox = document.querySelector(".alert-success.d-none");

function send(email, sendBtn) {
    const user = getUser();
    const token = user.token;

    console.log(token);


    window.fetch("/Api/Ticket/Send?email=" + email, {
        method: "POST",
        headers: {"Authorization": "Bearer " + token}
    }).then(res => {
        console.log(res);

        if (res.status === 400) {
            alertBox.innerHTML = "Email address is <strong>invalid</strong>!";
            alertBox.classList.remove("d-none");
        } else if (res.status === 208) {
            alertBox.innerHTML = "Ticket <strong>already</strong> sent to this address!";
            alertBox.classList.remove("d-none");
        } else if (res.ok) {
            clearInput();

            const node = successBox.cloneNode(true);

            node.classList.remove("d-none");

            const limitL = truncateString(email, 18);

            node.innerHTML = `sent to <strong>${limitL}</strong>  ${node.innerHTML}`;

            successBox.parentNode.insertBefore(node, successBox.nextSibling);
        } else {
            alertBox.innerHTML = res.statusText;
            alertBox.classList.remove("d-none");
        }

        sendBtn.disabled = false;
    });
}

function clearInput() {
    const input = document.querySelector("input[type=\"email\"]");

    input.value = "";
    input.focus();

    alertBox.classList.add("d-none");
}

onLoad.push(() => {
    redirectIfRoleIsMissing(["Promoter", "Admin"]);

    //Get form element
    const form = document.getElementById("form-send-ticket");

    function submitForm(event) {
        //Preventing page refresh
        event.preventDefault();
    }

    //Calling a function during form submission.
    form.addEventListener("submit", submitForm);

    const sendBtn = document.querySelector("#send_btn");

    if (sendBtn)
        sendBtn.addEventListener("click", () => {
            sendBtn.disabled = true;
            send(serializeForm(form).email, sendBtn);
        });
});