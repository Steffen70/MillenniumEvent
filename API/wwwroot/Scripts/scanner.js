const alertBox = document.querySelector(".alert-warning");

const successBox = document.querySelector(".alert-success.d-none");

var scanning = false;

function delayedRelease() {
    new Promise(resolve => setTimeout(resolve, 3000)).then(() => { scanning = false; });
}

function onScanSuccess(decodedText, decodedResult) {
    if (scanning) return;

    scanning = true;
    // Handle on success condition with the decoded text or result.
    console.log(`Scan result: ${decodedText}`, decodedResult);

    const user = getUser();
    const token = user.token;

    window.fetch("/Api/Ticket/Redeem?ticketKey=" + decodedText,
        {
            method: "PUT",
            headers: { "Authorization": "Bearer " + token }
        }).then(res => {
            if (res.status === 404) {
                alertBox.innerHTML = "Ticket <strong>not found</strong>!";
                alertBox.classList.remove("d-none");
            } else if (res.status === 208) {
                alertBox.innerHTML = "Ticket <strong>already</strong> redeemed!";
                alertBox.classList.remove("d-none");
            } else if (res.ok) {
                res.text().then(email => {
                    console.log(email);

                    alertBox.classList.add("d-none");

                    const node = successBox.cloneNode(true);

                    node.classList.remove("d-none");

                    const limitL = truncateString(email, 18);

                    node.innerHTML = `Redeemed <strong>${limitL}</strong> ${node.innerHTML}`;

                    successBox.parentNode.insertBefore(node, successBox.nextSibling);

                    delayedRelease();
                });
                return;
            }

            delayedRelease();
        });
}

onLoad.push(() => {
    redirectIfRoleIsMissing(["Admin"]);

    var html5QrcodeScanner = new Html5QrcodeScanner("reader", { fps: 10, qrbox: 200 });
    html5QrcodeScanner.render(onScanSuccess);
});