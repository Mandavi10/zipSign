

function animateRainbowText() {
    //
    const textElement = document.querySelector('.message');
    //const colors = ['#f06', '#c0f'];
    //let currentColorIndex = 0;
    function changeColor() {
        //textElement.style.color = colors[currentColorIndex];
        //currentColorIndex = (currentColorIndex + 1) % colors.length;
    }

    setInterval(500);
}
function redirectAfterDelay() {
    
    var hiddenFieldValue = $("#myHiddenField").val();
    const redirectMessage = document.querySelector('.redirect-message');
    if (!redirectMessage) {
        console.error("Element with class 'redirect-message' not found.");
        return;
    }

    let seconds = 5;
    redirectMessage.textContent = `Redirecting in ${seconds} seconds.`;

    const interval = setInterval(() => {
        seconds--;
        redirectMessage.textContent = `Redirecting in ${seconds} seconds.`;

        if (seconds <= 0) {

            clearInterval(interval);
            const parentWindow = window.parent;
            if (parentWindow && parentWindow.$) {
                const parentModal = parentWindow.$('#signaturetype');
                if (parentModal.length && hiddenFieldValue != "") {
                    parentModal.modal('hide');
                    //parentModal = null;
                    var URL = "https://uataadharsign.zipsign.in/zipSign/SigningRequest?FilePath=" + hiddenFieldValue;
                    //var URL = "http://localhost:50460/zipSign/SigningRequest?FilePath=" + hiddenFieldValue
                    parentWindow.location.href = URL;
                }
                else if (parentModal.length)
                {
                    parentModal.find('.modal-content').html('');
                    parentModal.modal('hide');
                }
                else {
                    console.error("Parent modal element not found.");
                }
            } else {
                console.error("Parent window or jQuery not available.");
            }
        }
    }, 1000);
}


// Call the function when needed, e.g., after page load.
// redirectAfterDelay();

window.onload = function () {
    //animateRainbowText();
    redirectAfterDelay();
};