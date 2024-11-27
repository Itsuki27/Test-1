const subMenu = document.querySelector('.sub-menu');
const toggleBtn = document.querySelector('#toggle-btn')
const sideBar = document.querySelector('#sidebar')
let shown = false;
const ddIcon = document.querySelector('.ddIcon');

sideBar.style.border = "black 5px solid";

if (shown == false) {
    window.onload = function () {
        subMenu.setAttribute("class", "disappear");
    }
}

function toggleSub() {
    if (shown == true) {
        subMenu.setAttribute("class", "disappear");
        ddIcon.setAttribute("class", ".revert")
        shown = false;
        console.log(shown);
    } else {
        subMenu.setAttribute("class", "sub-menu");
        shown = true;
        ddIcon.setAttribute("class", ".rotate")
        console.log(shown);
    }
    
}