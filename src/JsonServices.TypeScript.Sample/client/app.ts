import { IReturnVoid } from './IReturn';
import { JsonClient } from './JsonClient';

const start = function () {
    const inc = document.getElementById('incoming') as HTMLPreElement;
    const form = document.getElementById('sendForm') as HTMLFormElement;
    const input = document.getElementById('sendText') as HTMLInputElement;
    const sample1 = document.getElementById('sample1') as HTMLButtonElement;
    const sample2 = document.getElementById('sample2') as HTMLButtonElement;
    const sample3 = document.getElementById('sample3') as HTMLButtonElement;

    inc.innerHTML += new JsonClient().hello + "... connecting to server ..<br/>";

	// create a new websocket and connect
	const ws = new WebSocket('ws://localhost:8765/Default');

	// when data is coming from the server, this metod is called
	ws.onmessage = function (evt) {
		inc.innerHTML += "<font color=red><-- " + evt.data + '</font><br/>';
	};

	// when the connection is established, this method is called
	ws.onopen = function () {
		inc.innerHTML += '.. connection open<br/>';
	};

	// when the connection is closed, this method is called
	ws.onclose = function () {
		inc.innerHTML += '.. connection closed<br/>';
	}

	form.addEventListener('submit', function(e){
		e.preventDefault();
		var val = input.value;
		inc.innerHTML += "--> " + val + '<br/>';
		ws.send(val);
		input.value = "";
	});

	sample1.addEventListener('click', function(e){
		e.preventDefault();
		var msg = "{\"jsonrpc\":\"2.0\",\"method\":\"JsonServices.Tests.Messages.GetVersion\",\"params\":{\"IsInternal\":true},\"id\":\"123\"}";
		inc.innerHTML += "--> " + msg + '<br/>';
		ws.send(msg);
	});

	sample2.addEventListener('click', function(e){
		e.preventDefault();
		var msg = "{\"jsonrpc\":\"2.0\",\"method\":\"JsonServices.Tests.Messages.Calculate\",\"params\":{\"FirstOperand\":3, \"SecondOperand\": 5, \"Operation\": \"+\"},\"id\":\"123\"}";
		inc.innerHTML += "--> " + msg + '<br/>';
		ws.send(msg);
	});

	sample3.addEventListener('click', function(e){
		e.preventDefault();
		var msg = "{\"jsonrpc\":\"2.0\",\"method\":\"JsonServices.Tests.Messages.Calculate\",\"params\":{\"FirstOperand\":123, \"SecondOperand\": 0, \"Operation\": \"/\"},\"id\":\"123\"}";
		inc.innerHTML += "--> " + msg + '<br/>';
		ws.send(msg);
	});
}

window.onload = start;

/*class Greeter {
    element: HTMLElement;
    span: HTMLElement;
    timerToken: number;

    constructor(element: HTMLElement) {
        this.element = element;
        this.element.innerHTML += "The time is: ";
        this.span = document.createElement('span');
        this.element.appendChild(this.span);
        this.span.innerText = new Date().toUTCString();
    }

    start() {
        this.timerToken = setInterval(() => this.span.innerHTML = new Date().toUTCString(), 500);
    }

    stop() {
        clearTimeout(this.timerToken);
    }

}

window.onload = () => {
    var el = document.getElementById('content');
    var greeter = new Greeter(el);
    greeter.start();
};*/
