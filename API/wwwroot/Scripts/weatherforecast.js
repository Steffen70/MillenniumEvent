onLoad.push(() => {
    const user = getUser();
    const token = user.token;

    console.log(token);

    window.fetch("api/WeatherForecast/", {
        method: "GET",
        headers: { "Authorization": "Bearer " + token }
    }).then(res => {
        console.log(res);

        if (res.ok)
            res.json().then(body => {
                const output = document.getElementById("output");

                console.log(output);

                output.innerHTML = body.map(o =>
                    `<span>${o.date}</span> <span>${o.summary}</span>`
                ).join("</br>");

                console.log(body);
            });
    });
});