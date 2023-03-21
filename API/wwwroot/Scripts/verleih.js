const alertBox = document.querySelector(".alert-warning");

const successBox = document.querySelector(".alert-success.d-none");

function create(reservationDto) {
    const user = getUser();
    const token = user.token;

    console.log(token);

    const reservationJson = JSON.stringify(reservationDto);
    console.log(reservationJson);

    window.fetch("/Api/Reservation/create",
        {
            method: "POST",
            headers: { "Authorization": "Bearer " + token, "Content-Type": "application/json" },
            body: reservationJson
        }).then(res => {
            console.log(res);

            if (res.status === 400) {
                alertBox.innerHTML = "Your reservation is <strong>invalid</strong>!";
                alertBox.classList.remove("d-none");
            } else if (res.status === 208) {
                alertBox.innerHTML = "Bike is <strong>already</strong> reserved!";
                alertBox.classList.remove("d-none");
            } else if (res.ok) {
                res.text().then(name => {
                    console.log(name);
                    clearInput();

                    const node = successBox.cloneNode(true);

                    node.classList.remove("d-none");

                    node.innerHTML =
                        `Reservation: <strong>${name}</strong> ${node.innerHTML}`;

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
    const inputs = document.querySelectorAll("input");

    console.log(inputs);

    inputs.forEach(i => i.value = "");
    inputs[0].focus();

    alertBox.classList.add("d-none");
}

function reloadTable() {
    document.querySelectorAll(".table-row:not(.d-none)")?.forEach(e => e.remove());

    const user = getUser();
    const token = user.token;

    window.fetch("/Api/Reservation/List",
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

                    ticketList.forEach(r => {
                        const trNew = tableRow.cloneNode(true);

                        trNew.classList.remove("d-none");

                        rowCount++;

                        trNew.querySelector(".reservation-name").innerText = r.reservationName;
                        trNew.querySelector(".start-date").innerText = r.startDate;
                        trNew.querySelector(".end-date").innerText = r.endDate;

                        const truncatedB = truncateString(r.bike, 20);

                        trNew.querySelector(".bike").innerText = truncatedB;
                        trNew.querySelector(".row-count").innerText = rowCount;

                        parentNode.appendChild(trNew);
                    });

                    document.querySelector("#reservations-table").classList.remove("d-none");
                });
        });
}

function createOption(id, display, option) {
    const newOption = option.cloneNode();
    newOption.value = id;
    newOption.textContent = display;
    option.parentNode.appendChild(newOption);
}

function loadDropdownOptions() {
    const user = getUser();
    const token = user.token;

    window.fetch("/Api/Reservation/Options",
        {
            method: "GET",
            headers: { "Authorization": "Bearer " + token }
        }).then(res => {
            console.log(res);

            if (res.status === 204)
                return;
            else if (res.ok)
                res.json().then(j => {
                    console.log(j);

                    const userOptionTemplate = document.querySelector(".form-floating [name=\"appUserId\"] option");
                    j.users.forEach(u => createOption(u.id, u.username, userOptionTemplate));
                    userOptionTemplate.remove();

                    const bikeOptionTemplate = document.querySelector(".form-floating [name=\"bikeId\"] option");
                    j.bikes.forEach(b => createOption(b.id, b.toString, bikeOptionTemplate));
                    bikeOptionTemplate.remove();
                });
        });
}

onLoad.push(() => {
    redirectIfRoleIsMissing(["Admin", "Employee"]);

    //Get form element
    const form = document.getElementById("form-send-reservation");

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

    loadDropdownOptions();
    reloadTable();
});