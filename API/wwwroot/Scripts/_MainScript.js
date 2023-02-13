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
    let formDto = {};
    let formData = new FormData(form);
    for (let key of formData.keys()) {
        formDto[key] = formData.get(key);
    }
    return formDto;
};

let onLoad = [];

//Execute all onLoad functions
window.onload = () => onLoad.forEach(f => f());