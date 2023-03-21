const alertBox = document.querySelector(".alert-warning");

const successBox = document.querySelector(".alert-success.d-none");

function create(bikeDto) {
    const user = getUser();
    const token = user.token;

    console.log(token);

    const bikeJson = JSON.stringify(bikeDto);
    console.log(bikeJson);

    window.fetch("/Api/Bike/create", {
        method: "POST",
        headers: { "Authorization": "Bearer " + token, "Content-Type": "application/json" },
        body: bikeJson
    }).then(res => {
        console.log(res);

        if (res.status === 208) {
            alertBox.innerHTML = "Bike was <strong>already</strong> added!";
            alertBox.classList.remove("d-none");
        }

        else if (res.ok) {
            res.text().then(b => {
                clearInput();

                const node = successBox.cloneNode(true);

                node.classList.remove("d-none");

                const truncatedB = truncateString(b, 20);

                node.innerHTML = `<strong>${truncatedB}</strong>${node.innerHTML}`;

                successBox.parentNode.insertBefore(node, successBox.nextSibling);
                reloadTable();
            });
        }

        else {
            alertBox.innerHTML = res.statusText;
            alertBox.classList.remove("d-none");
        }
    });
}

function clearInput() {
    const inputs = document.querySelectorAll("input[type=\"text\"]");

    inputs.forEach(i => i.value = "");
    inputs[0].focus();

    alertBox.classList.add("d-none");
}

function reloadTable() {
    document.querySelectorAll(".table-row:not(.d-none)")?.forEach(e => e.remove());

    const user = getUser();
    const token = user.token;

    window.fetch("/Api/Bike/List",
        {
            method: "GET",
            headers: { "Authorization": "Bearer " + token }
        }).then(res => {
            console.log(res);
            if (res.status === 204)
                return;
            else if (res.ok)
                res.json().then(ticketList => {
                    const tableRow = document.querySelector(".table-row");
                    const parentNode = tableRow.parentNode;

                    let rowCount = 0;

                    ticketList.forEach(t => {
                        const trNew = tableRow.cloneNode(true);

                        trNew.classList.remove("d-none");

                        rowCount++;

                        trNew.querySelector(".brand").innerText = t.brand;
                        trNew.querySelector(".model").innerText = t.model;
                        trNew.querySelector(".year").innerText = t.year;
                        trNew.querySelector(".category").innerText = t.category;
                        trNew.querySelector(".row-count").innerText = rowCount;

                        parentNode.appendChild(trNew);
                    });

                    document.querySelector("#bikes-table").classList.remove("d-none");
                });
        });
}

onLoad.push(() => {
    redirectIfRoleIsMissing(["Admin", "Employee"]);

    //Get form element
    const form = document.getElementById("form-send-bike");

    function submitForm(event) {
        //Preventing page refresh
        event.preventDefault();
    }

    //Calling a function during form submission.
    form.addEventListener("submit", submitForm);

    const createBtn = document.querySelector("#create_btn");

    if (createBtn)
        createBtn.addEventListener("click", () => {
            create(serializeForm(form));
        });

    reloadTable();
});