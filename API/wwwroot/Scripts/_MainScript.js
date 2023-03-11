function getDecodedToken(token) {
    return JSON.parse(atob(token.split(".")[1]));
}

function setUser(user) {
    window.localStorage.setItem("user", JSON.stringify(user));
}

function getUser() {
    return JSON.parse(window.localStorage.getItem("user"));
}

function logout() {
    localStorage.removeItem("user");
    this.currentUserSource.next(null);
}

function serializeForm(form) {
    const formDto = {};
    const formData = new FormData(form);
    for (let key of formData.keys()) {
        formDto[key] = formData.get(key);
    }
    return formDto;
};

function redirectIfRoleIsMissing(roles) {
    const currentUser = getUser();
    if (currentUser === null || currentUser === undefined || !roles.includes(currentUser.userRole)) {
        const url = window.location.href.split("?")[0] + "?view=Login";
        window.location.href = url;
    } else {
        document.querySelector("#flex_box").classList.remove("d-none");
    }
}

function truncateString(str, num) {
    if (str.length > num) {
        return str.slice(0, num) + "...";
    } else {
        return str;
    }
}

let onLoad = [];

//Execute all onLoad functions
window.onload = () => onLoad.forEach(f => f());

onLoad.push(() => {
    console.log(getUser());
});