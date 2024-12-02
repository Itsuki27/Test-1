const editRoute = document.querySelector('.editRoute')
const subMenu = document.querySelector('.sub-menu')
const toggleBtn = document.querySelector('#toggle-btn')
const sideBar = document.querySelector('#sidebar')
const logo = document.querySelector('.logo')
const headLogo = document.querySelector(".headLogoNone")
const btLogout = document.querySelector('.btLogout')
const hideLogout = document.querySelector('.hideLogout')
const btUsers = document.querySelector('.btUsers')
const btAudit = document.querySelector('.btAudit')
const btReports = document.querySelector('.btReports')
const btDepts = document.querySelector('.btDepts')

var shown = false;
var sideShown = true;
var deg0 = true;

const ddIcon = document.querySelector('#ddIcon');

if (shown == false) {
    window.onload = function () {
        subMenu.setAttribute("class", "disappear");
    }
}


function toggleSub() {
    if (shown == true) {
    subMenu.setAttribute("class", "disappear");
    //ddIcon.setAttribute("class", "rotate");
    shown = false;
    ddIcon.removeAttribute("class", "ddIcon180");
    ddIcon.setAttribute("class", "ddIcon0");

    sideShown = false;
    toggleSideBar();

    console.log(shown);

    } else {
    subMenu.setAttribute("class", "show");
    //ddIcon.setAttribute("class", "revert");
    shown = true;
    ddIcon.removeAttribute("class", "ddIcon0");
    ddIcon.setAttribute("class", "ddIcon180");

    sideShown = false;
    toggleSideBar();

    console.log(shown);
    }

}

function ddRotate() {

}


function toggleSideBar() {
    if (sideShown == true) {
    sideShown = false;
    sideBar.removeAttribute("class", "sidebar-full")
    logo.setAttribute("class", "logoNone")
    sideBar.setAttribute("class", "sidebar-small")
    headLogo.setAttribute("class", "headLogo")
    } else {
    sideShown = true;
    sideBar.removeAttribute("class", "sidebar-small")
    logo.setAttribute("class", "logo")
    sideBar.setAttribute("class", "sidebar-full")
    headLogo.setAttribute("class", "headLogoNone")
    }
}

    //sideBar.addEventListener('mouseover', function () {
    //    sideShown = false;
    //    toggleSideBar();
    //});

    //sideBar.addEventListener('mouseout', function () {
    //    sideShown = true;
    //    toggleSideBar();
    //});

btLogout.addEventListener('click', function (e) {
    e.preventDefault();
    var form = $(this).parents('form');
    Swal.fire({
        icon: 'question',
        title: 'Log out?',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Logout',
    }).then((result) => {
        if (result.value) {
            hideLogout.click();
        }
    });
})

function goUsers() {
    btUsers.click();
}

function goAudit() {
    btAudit.click();
}

function goReports() {
    btReports.click();
}

function goDepts() {
    btDepts.click();
}