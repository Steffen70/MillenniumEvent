onLoad.push(() => {
    const user = getUser();
    const token = user.token;

    const lgItem = document.querySelector(".list-group-item");

    if (!lgItem)
        return;

    const ul = lgItem.parentElement;

    if (!ul)
        return;

    ul.removeChild(lgItem);

    window.fetch("api/members?pageSize=100", {
        method: "GET",
        headers: { "Authorization": "Bearer " + token }
    }).then(res => {
        console.log(res);

        if (res.ok)
            res.json().then(body => {
                console.log(body);

                body.forEach(u => {
                    if (u.userRole == "Admin")
                        return;

                    console.log(u);
                    var buttontext = u.userRole == "Moderator" ? "Demote" : "Promote";
                    var buttonclass = buttontext == "Promote" ? "btn-primary" : "btn-outline-primary";
                    console.log(buttontext);

                    var clone = lgItem.cloneNode(true);
                    clone.innerHTML = clone.innerHTML
                        .replace("%email%", u.email)
                        .replace("%button%", buttontext)
                        .replace("%buttonclass%", buttonclass);

                    ul.appendChild(clone);
                });

                ul.classList.remove("d-none");
            });
    });
});