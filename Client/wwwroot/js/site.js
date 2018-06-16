$(document).ready(function () {
	M.AutoInit();
	$(".hidden").toggle();

	const { ipcRenderer } = require("electron");
	const fs = require('fs');

	document.getElementById("save-dialog")
		.addEventListener("click", () => {
			ipcRenderer.send("save-dialog");
		});

	ipcRenderer.on("reload", (sender) => {
		window.location.reload(true); //true = reload from server
	});
});

function toggleO(tableId) {
	$("#" + tableId).toggle();
}