const alertBox = document.querySelector(".alert-warning");

function setCurrentUser(user) {
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    setUser(user);
}

function login(userDto) {
    window.fetch("/api/account/login", {
        method: "POST",
        headers: { 'Content-Type': "application/json" },
        body: JSON.stringify(userDto)
    }).then(res => {
        console.log(res);

        if (res.ok)
            res.json().then(body => {
                setCurrentUser(body);
                console.log(getUser());

                const url = window.location.href.split("?")[0] + "?view=Tickets";
                window.location.href = url;
            });

        else {
            alertBox.classList.remove("d-none");
        }
    });
}

onLoad.push(() => {
    //Get form element
    const form = document.getElementById("form-signin");

    function submitForm(event) {
        //Preventing page refresh
        event.preventDefault();
    }

    //Calling a function during form submission.
    form.addEventListener("submit", submitForm);

    const loginBtn = document.querySelector("#login_btn");

    if (loginBtn)
        loginBtn.addEventListener("click", () => {
            login(serializeForm(form));
        });

    console.log("successful push onload");
});