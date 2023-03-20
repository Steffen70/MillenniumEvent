import { createApp } from "https://unpkg.com/petite-vue?module"

async function sayHello(name) {

}

function helloApp(props) {
    return {
        get helloMuhammad() {
            return "Hello Muhammad";
        },
        helloWorld: "",
        async sayHelloWorld() {
            const reHello = await fetch("/api/Demo/HelloWorld?name=World");
            if (reHello.ok)
                this.helloWorld = await reHello.text();
        }
    }
}

createApp({
    HelloApp: helloApp
}).mount("#hello-app")