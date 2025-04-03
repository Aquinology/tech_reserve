document.addEventListener("DOMContentLoaded", function () {
    let timerElement = document.getElementById("timer");
    let totalSeconds = parseInt(timerElement.getAttribute("data-time"), 10);

    function updateTimer() {
        if (totalSeconds <= 0) {
            timerElement.textContent = "Time is up.";
            clearInterval(timerInterval);
            return;
        }

        let minutes = Math.floor(totalSeconds / 60);
        let seconds = totalSeconds % 60;
        timerElement.textContent = `${minutes} min. ${seconds} sec.`;

        totalSeconds--;
    }

    let timerInterval = setInterval(updateTimer, 1000);
    updateTimer();
});
