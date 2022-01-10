// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let actualCards = document.getElementById("actual-wishes-list");
let completedCards = document.getElementById("completed-wishes-list");
let actualButton = document.getElementById("showActualButton");
let completedButton = document.getElementById("showCompletedButton");
ShowActualCards();

function ShowActualCards(){
    actualCards.style.display = 'block';
    completedCards.style.display = 'none';
    actualButton.classList.add("btn-dark");
    actualButton.classList.remove("btn-default");
    completedButton.classList.remove("btn-dark");
    completedButton.classList.add("btn-default");
}

function ShowCompletedCards(){
    completedCards.style.display = 'block';
    actualCards.style.display = 'none';
    actualButton.classList.add("btn-default");
    actualButton.classList.remove("btn-dark");
    completedButton.classList.remove("btn-default");
    completedButton.classList.add("btn-dark");
}