const alertBox = document.querySelector(".alert-warning");

const successBox = document.querySelector(".alert-success.d-none");

function create(username) {
    const user = getUser();
    const token = user.token;

    console.log(token);


    window.fetch("/Api/Account/create?username=" + username, {
        method: "POST",
        headers: { "Authorization": "Bearer " + token }
    }).then(res => {
        console.log(res);

        if (res.status === 400) {
            alertBox.innerHTML = "Username is <strong>invalid</strong>!";
            alertBox.classList.remove("d-none");
        }

        else if (res.status === 208) {
            alertBox.innerHTML = "User <strong>already</strong> added!";
            alertBox.classList.remove("d-none");
        }

        else if (res.ok) {
            clearInput();

            const node = successBox.cloneNode(true);

            node.classList.remove("d-none");

            node.innerHTML = `<strong>${username}</strong> added!${node.innerHTML}`;

            successBox.parentNode.insertBefore(node, successBox.nextSibling);
            reloadTable();
        }

        else {
            alertBox.innerHTML = res.statusText;
            alertBox.classList.remove("d-none");
        }
    });
}

function clearInput() {
    const input = document.querySelector("input[type=\"text\"]");

    input.value = "";
    input.focus();

    alertBox.classList.add("d-none");
}

function reloadTable() {
    document.querySelectorAll(".table-row:not(.d-none)")?.forEach(e => e.remove());

    const user = getUser();
    const token = user.token;

    window.fetch("/Api/Account/List",
        {
            method: "GET",
            headers: { "Authorization": "Bearer " + token }
        }).then(res => {
        console.log(res);

        if (res.ok)
            res.json().then(ticketList => {
                const tableRow = document.querySelector(".table-row");
                const parentNode = tableRow.parentNode;

                let rowCount = 0;

                ticketList.forEach(t => {
                    const trNew = tableRow.cloneNode(true);

                    trNew.classList.remove("d-none");

                    rowCount++;

                    trNew.querySelector(".username").innerText = t.username;
                    trNew.querySelector(".user-role").innerText = t.userRole;
                    trNew.querySelector(".row-count").innerText = rowCount;

                    parentNode.appendChild(trNew);
                });

                document.querySelector("#users-table").classList.remove("d-none");
            });
    });
}

onLoad.push(() => {
    redirectIfRoleIsMissing(["Admin", "Employee"]);

    //Get form element
    const form = document.getElementById("form-send-user");

    function submitForm(event) {
        //Preventing page refresh
        event.preventDefault();
    }

    //Calling a function during form submission.
    form.addEventListener("submit", submitForm);

    const createBtn = document.querySelector("#create_btn");

    if (createBtn)
        createBtn.addEventListener("click", () => {
            create(serializeForm(form).username);
        });

    reloadTable();
});