const alertBox = document.querySelector(".alert-warning");

const successBox = document.querySelector(".alert-success.d-none");

function create(email) {
    const user = getUser();
    const token = user.token;

    console.log(token);


    window.fetch("/Api/Account/create?email=" + email, {
        method: "POST",
        headers: { "Authorization": "Bearer " + token }
    }).then(res => {
        console.log(res);

        if (res.status === 400) {
            alertBox.innerHTML = "Email address is <strong>invalid</strong>!";
            alertBox.classList.remove("d-none");
        }

        else if (res.status === 208) {
            alertBox.innerHTML = "Promoter <strong>already</strong> added!";
            alertBox.classList.remove("d-none");
        }

        else if (res.ok) {
            res.text().then(pw => {
                console.log(pw);

                clearInput();

                const node = successBox.cloneNode(true);

                node.classList.remove("d-none");

                node.innerHTML = `password: <strong>${pw}</strong>  ${node.innerHTML}`;

                successBox.parentNode.insertBefore(node, successBox.nextSibling);
            });
        }

        else {
            alertBox.innerHTML = res.statusText;
            alertBox.classList.remove("d-none");
        }
    });
}

function clearInput() {
    const input = document.querySelector("input[type=\"email\"]");

    input.value = "";
    input.focus();

    alertBox.classList.add("d-none");
}

onLoad.push(() => {
    redirectIfRoleIsMissing(["Admin"]);

    //Get form element
    const form = document.getElementById("form-send-ticket");

    function submitForm(event) {
        //Preventing page refresh
        event.preventDefault();
    }

    //Calling a function during form submission.
    form.addEventListener("submit", submitForm);

    const createBtn = document.querySelector("#create_btn");

    if (createBtn)
        createBtn.addEventListener("click", () => {
            create(serializeForm(form).email);
        });


    const user = getUser();
    const token = user.token;

    window.fetch("/Api/Ticket/List",
        {
            method: "GET",
            headers: { "Authorization": "Bearer " + token }
        }).then(res => {
            console.log(res);

            if (res.ok)
                res.json().then(ticketList => {
                    const tableRow = document.querySelector(".table-row");
                    const parentNode = tableRow.parentNode;

                    let ticketCount = 0;

                    ticketList.forEach(t => {
                        const trNew = tableRow.cloneNode(true);

                        trNew.classList.remove("d-none");

                        trNew.querySelector(".ticket-mail").innerText = t.ticketEmail;
                        trNew.querySelector(".promoter-mail").innerText = t.promoterEmail;
                        trNew.querySelector(".ticket-count").innerText = t.ticketCount;

                        ticketCount += t.ticketCount;

                        parentNode.insertBefore(trNew, tableRow.nextSibling);
                    });

                    const ticketSum = document.querySelector(".ticket-sum");

                    ticketSum.innerHTML = `<b>${ticketCount}</b>`;

                    document.querySelector("#tickets-table").classList.remove("d-none");
                });
        });
});