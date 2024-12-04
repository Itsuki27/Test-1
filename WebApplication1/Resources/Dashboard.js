var clock = document.getElementById('clock');
var clockContainer = document.querySelector('.clock-container');

function runningClock() {
    var now = new Date();

    var hh = now.getHours();
    var mm = now.getMinutes();
    var ss = now.getSeconds();

    var meridian;

    if (hh > 12) {
        hh = hh - 12;
        meridian = " PM";
    } else {
        meridian = " AM";
    }

    if (hh < 10) {
        hh = "0" + hh;
    }

    if (mm < 10) {
        mm = "0" + mm;
    }

    if (ss < 10) {
        ss = "0" + ss;
    }

    var clockNow = hh + ":" + mm + ":" + ss + meridian;

    clockContainer.innerHTML = "<h2 class='clockLine'>" + clockNow + "</h2>" + "<h2 class='clockFill'>" + clockNow + "</h2>";
    //clock.text = clockNow;
    console.log(clockNow);


    setTimeout(runningClock, 1000);
}

runningClock();