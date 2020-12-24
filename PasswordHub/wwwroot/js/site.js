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
        }
    });
}

function RemoveCategory(id) {
    let messageBlock = document.getElementById("edit-category-message-result");
    messageBlock.innerHTML = "";
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
                            <a href="#" class="categories__user-filter-link">` + elem.name + `</a>
                            <a href="/Hub/EditCategory?id=` + elem.id + `">` + settingImg + `</a>
                         </li>`;
            });
            
            let block = document.getElementById("categories-user-filter-list");
            block.innerHTML = html;
        },
        dataType: "json"
    });
}

GetCategoryList();