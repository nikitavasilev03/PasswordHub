// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function CategoryChangeCountField() {
    let count = document.getElementById("category-count-field").value;
    if (count < 1 || count > 10)
        return;
    let block = document.getElementById("category-fields-block");
    let blockItems = document.getElementsByClassName("content__create-category-item");
    let copyItem = blockItems[0].cloneNode(true);
    let field = copyItem.getElementsByClassName("content__input-field")[0];
    field.value = "";

    let n = blockItems.length;
    if (count > n) {
        for (let i = 0; i < count - n; i++) {
            block.appendChild(copyItem.cloneNode(true));
        }
    } else {
        while (n - count > 0) {
            block.removeChild(blockItems[blockItems.length - 1]);
            n -= 1;
        }
    }
}

function EditCategory(id) {

    let messageBlock = document.getElementById("edit-category-message-result");
    messageBlock.innerHTML = "";

    let name = document.getElementById("category-name-input-field").value;
    let values = Array.from(document.getElementsByClassName("input-field-item")).map(elem => elem.value);
    let types = Array.from(document.getElementsByClassName("input-type-item")).map(elem => elem.value);
    let sizes = Array.from(document.getElementsByClassName("input-size-item")).map(elem => parseInt(elem.value));
   
    let data = {
        id: id,
        name: name,
        values: values,
        types: types,
        sizes: sizes
    };
    $.ajax({
        type: "POST",
        url: "/Hub/EditCategory",
        data: data,
        success: function (data) {
            
            if (data.isSuccess) {
                messageBlock.innerHTML = `    <div  class="content__message-result message-result-true">
                                                  <p class="content__message-result-text">` + data.message + `</p>
                                              </div>`;
                GetCategoryList();
            } else {
                messageBlock.innerHTML = `    <div  class="content__message-result message-result-false">
                                                  <p class="content__message-result-text">` + data.message + `</p>
                                              </div>`;
            }
        },
        error: function (data) {
            messageBlock.innerHTML = `    <div  class="content__message-result message-result-false">
                                                  <p class="content__message-result-text">Упс, что-то пошло не так...</p>
                                              </div>`;
        }
    });
}

function RemoveCategory(id) {
    let messageBlock = document.getElementById("edit-category-message-result");

    let data = {
        id: id
    };
    $.ajax({
        type: "POST",
        url: "/Hub/RemoveCategory",
        data: data,
        success: function (data) {

            if (data.isSuccess) {
                messageBlock.innerHTML = `    <div  class="content__message-result message-result-true">
                                                  <p class="content__message-result-text">` + data.message + `</p>
                                              </div>`;
                document.getElementById("category-filter-" + id).remove();
            } else {
                messageBlock.innerHTML = `    <div  class="content__message-result message-result-false">
                                                  <p class="content__message-result-text">` + data.message + `</p>
                                              </div>`;
            }
        },
        error: function (data) {
            messageBlock.innerHTML = `    <div  class="content__message-result message-result-false">
                                                  <p class="content__message-result-text">Упс, что-то пошло не так...</p>
                                              </div>`;
        }
    });
}

function GetCategoryList() {
    $.ajax({
        type: "POST",
        url: "/Hub/GetCategoryList",
        data: null,
        success: function (data) {
            let html = "";
            let settingImg = document.getElementById("img-sitting").innerHTML;
            data.forEach(elem => {
                html += `<li id=category-filter-` + elem.id + ` class="categories__user-filter-item">
                            <p class="categories__user-filter-link clickable-block" onclick="GetCardList('` + elem.id + `')">` + elem.name + `</p>
                            <a href="/Hub/EditCategory?id=` + elem.id + `">` + settingImg + `</a>
                         </li>`;
            });
            
            let block = document.getElementById("categories-user-filter-list");
            block.innerHTML = html;
        },
        dataType: "json"
    });
}

function GetCardList(filter) {
    $.ajax({
        type: "POST",
        url: "/Hub/GetCardList",
        data: {
            filter: filter
        },
        success: function (data) {
            let html = "";
            data.forEach(elem => {
                html += `<a id=card-item-` + elem.id + ` href="/Hub/EditCard?id=` + elem.id + `" class="clickable-block">
                            <li class="cards__item">
                                <div class="cards__item-block">
                                    <p class="cards__item-text">` + elem.name + `</p>
                                    <p class="cards__item-text">` + ((elem.firstField === "") ? "-" : elem.firstField) + `</p>
                                    <div class="cards__item-signature">
                                        <p class="cards__item-text-signature">` + elem.categoryName + `</p>
                                        <p class="cards__item-text-signature">` + elem.dateTimeCreate + `</p>
                                    </div>
                                </div>
                            </li>
                         </a>`;
            });
            let block = document.getElementById("card-user-list");
            block.innerHTML = html;
        },
        dataType: "json"
    });
}

function GetFieldForCard() {
    let messageBlock = document.getElementById("card-message-result");
    messageBlock.innerHTML = "";

    let category = document.getElementById("category-for-card").value;
    $.ajax({
        type: "POST",
        url: "/Hub/GetFieldForCard",
        data: {
            categoryId: category
        },
        success: function (data) {
            let html = "";
            if (data.isSuccess) {
                data.data.forEach(elem => {
                    html += `<div class="content__input">
					            <p class="content__input-title">` + elem.name + `</p>
					            <input id=` + elem.id + ` type="text" class="input-field-item content__input-field input-field">
				            </div>`;
                });
                let block = document.getElementById("card-fields-block");
                block.innerHTML = html;
            } else {
                messageBlock.innerHTML = `    <div  class="content__message-result message-result-false">
                                                  <p class="content__message-result-text">` + data.message + `</p>
                                              </div>`;
            }
        },
        error: function(data) {
            messageBlock.innerHTML = `    <div  class="content__message-result message-result-false">
                                                  <p class="content__message-result-text">Упс, что-то пошло не так...</p>
                                              </div>`;
        }
    });
}

function EditCard(id) {
    let messageBlock = document.getElementById("card-message-result");
    messageBlock.innerHTML = "";

    let name = document.getElementById("card-name-input-field").value;
    let category = document.getElementById("category-for-card").value;
    let values = Array.from(document.getElementsByClassName("input-field-item")).map(elem => elem.value);
    let templatesId = Array.from(document.getElementsByClassName("input-field-item")).map(elem => elem.id);

    let data = {
        id: id,
        name: name,
        categoryId: category,
        values: values,
        templatesId: templatesId
    };
    $.ajax({
        type: "POST",
        url: "/Hub/EditCard",
        data: data,
        success: function (data) {

            if (data.isSuccess) {
                messageBlock.innerHTML = `    <div  class="content__message-result message-result-true">
                                                  <p class="content__message-result-text">` + data.message + `</p>
                                              </div>`;
                GetCardList();
            } else {
                messageBlock.innerHTML = `    <div  class="content__message-result message-result-false">
                                                  <p class="content__message-result-text">` + data.message + `</p>
                                              </div>`;
            }
        }
    });
}

function RemoveCard(id) {
    let messageBlock = document.getElementById("card-message-result");
    //messageBlock.innerHTML = "";

    let data = {
        id: id
    };
    $.ajax({
        type: "POST",
        url: "/Hub/RemoveCard",
        data: data,
        success: function (data) {

            if (data.isSuccess) {
                messageBlock.innerHTML = `    <div  class="content__message-result message-result-true">
                                                  <p class="content__message-result-text">` + data.message + `</p>
                                              </div>`;
                document.getElementById("card-item-" + id).remove();
            } else {
                messageBlock.innerHTML = `    <div  class="content__message-result message-result-false">
                                                  <p class="content__message-result-text">` + data.message + `</p>
                                              </div>`;
            }

        }
    });
}

function ChangeCardItem(id) {
    $.ajax({
        type: "POST",
        url: "/Hub/GetCardJson",
        data: { id: id },
        success: function (data) {
            let cardBlock = document.getElementById("card-item-" + id);
            cardBlock.innerHTML = `<li class="cards__item">
                                        <div class="cards__item-block">
                                            <p class="cards__item-text">` + data.name + `</p>
                                            <p class="cards__item-text">` + ((data.firstField === "") ? "-" : data.firstField) + `</p>
                                            <div class="cards__item-signature">
                                                <p class="cards__item-text-signature">` + data.categoryName + `</p>
                                                <p class="cards__item-text-signature">` + data.dateTimeCreate + `</p>
                                            </div>
                                        </div>
                                    </li>`;
        }
    });
}

GetCategoryList();
GetCardList(null);


