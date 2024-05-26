//Чтобы  js код в кеш не отправляло, нужно добавлять рандомное значение при подключении
var currentWarehouseId; // Переменная глобальная для передачи id склада без параметра в любую функицю
//Так прощее:)
function openModal(parameters) {
    const id = parameters.data;
    const url = parameters.url + '?random=' + Math.random();;
    const modal = $("#"+ parameters.modalId); 
  
    if (!id || !url) {
        alert('Упссс.... что-то пошло не так');
        return;
    }
    $.ajax({
        type: 'GET',
        url: url,
        data: { "id": id },
        success: function (response) {
            modal.find(".modal-body").html(response);
            modal.modal('show');
            var context = parameters.context;
            $('#SaveChanBtn').data('context', context);

            // Выполняем соответствующее действие в зависимости от контекста
            currentWarehouseId = parameters.data; 
            if (context === "ListWarehouse") {
               
                initializeWarehouseProductTable(id);

            }
            if (context === "MoveProductWarehouse") {
                MassInputWarehouse(id);
               
               
            }
            if (context == "DebitingProduct") {
                initializeWarehouseDebiting();
            }
            modal.css('z-index', '1010');
            $('.modal-backdrop').css('z-index', '1005');
            // Вызов callback после успешного выполнения запроса
            if (typeof parameters.callback === 'function') {
                parameters.callback();
            }
        },
        failure: function () {
            modal.modal('hide')
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
};

$('#SaveChanBtn').on('click',
    function() {
        // Получаем контекст из кнопки
        var context = $(this).data('context');

        if (context === 'profile') {
            // Выполняем соответствующее действие для профиля
            var id = $('#hiddenId').val();
            var lastName = $('#lastName').val();
            var Name = $('#firstName').val();
            var Surname = $('#patronymic').val();
            var age = $('#age').val();
            var Role = $('#roleSelect').val();
            var Em = $('#Em').val();

            var profileData = {
                Id: id,
                Age: age,
                Surname: Surname,
                LastName: lastName,
                Name: Name,
                RoleId: Role,
                Email: Em

            };
            MassUpdate('/Profile/Save', profileData, 'Обновление профиля', 'Изменение профиля');
        } else if (context === 'provider') {
            // Выполняем соответствующее действие для поставщика
            var providerId = $('#hiden').val();
            var providerName = $('#NameProvider').val();
            MassUpdate('/Provider/UpdateProvider', { ProviderId: providerId, ProviderName: providerName }, 'Обновление поставщика', 'Изменение поставщика');
        } else if (context === 'warehouse') {
            // Выполняем соответствующее действие для склада
            var warehouseId = $('#WhHiden').val();
            var warehouseName = $('#WhName').val();
            MassUpdate('/Warehouse/UpdateWarehouse', { Id: warehouseId, WarehouseName: warehouseName }, 'Обновление склада', 'Изменение склада');

        } 
});




// Функкиця на стороне сервера при передаче данных отправка запроса на сервер
function SaveProfile() {
    Swal.fire({
        title: 'Изменение профиля',
        html: 'Пожалуйста, подождите...',
        timerProgressBar: true,
        showConfirmButton: false,
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
   
    var profileData = {
        id : $('#hiddenId').val(),
        lastName: $('#lastName').val(),
        Name: $('#firstName').val(),
        Surname: $('#patronymic').val(),
        age: $('#age').val()
    };
    //Какие поля в представлении, точно так же обзываешь их и в profiledata, таким образом будет сопоставление


    $.ajax({
        type: 'POST',
        url: 'Save',
        data: profileData , 
        success: function (response) {
            setTimeout(function () {
                Swal.close();
          
                Swal.fire({
                    title: 'Успешное изменение данных',
                    text: response.description,
                    icon: 'success',
                    confirmButtonText: 'Ок'
                }).then((result) => {
                    if (result.isConfirmed) {
                        $('#modal').modal('hide');
                        location.reload(); 
                    }
                });
            }, 1000);
        },
        error: function (response) {
            setTimeout(function () {
                Swal.close();
                Swal.fire({
                    title: 'Ошибка изменения данных',
                    text: response.responseJSON.description,
                    icon: 'error',
                    confirmButtonText: 'Ок'
                }).then((result) => {
                
                    $('#modal').modal('hide');
                });
            }, 1000); 
        }
    });
}
//Обновление конкретно статуса заявки
function UpdateStatus(data) {

    $.ajax({
        type:'POST',
        url: 'UpdateStatusStat',
        data: { id: data.data, NewStatus: data.newStatus },
        success: function (response) { 
            console.log('Запрос успешно выполнен');
            console.log('Ответ сервера:', response);
        },
        error: function (xhr, status, error) { 
            console.error('Произошла ошибка при выполнении запроса:', error);
        }
    });

}

//Глобальное обновление заявки
function UpdateStatment() {
    //Загрузка
    Swal.fire({
        title: 'Отправка ответа',
        html: 'Пожалуйста подождите...',
        timerProgressBar: true,
        showConfirmButton: false,
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    var Data = {
        id: $('#hhID').val(),
        Response: $('#AnID').val(),
    };

    $.ajax({
        type: 'POST',
        url: 'EndStatment',
        data: Data,
        success: function (response) {
            setTimeout(function () {
                Swal.close();
                Swal.fire({
                    title: 'Успешный ответ на заявку',
                    text: response.description,
                    icon: 'success',
                    confirmButtonText: 'Окей'
                }).then((result) => {
                     if (result.isConfirmed) {
                         $('#StateModdal').modal('hide');
                        location.reload(); 
                    }
                   
                });
            }, 1000);
        },
        error: function (response) {
            setTimeout(function () {
                Swal.close(); 
                Swal.fire({
                    title: 'Что-то пошло не так',
                    text: response.responseJSON.description,
                    icon: 'error',
                    confirmButtonText: 'Окей'
                }).then((result) => {

                });
            }, 1000);
        }
    });
}
//Отправка на создание поставщика
function CreateProvider(providerName) {
    
    //Загрузка
    Swal.fire({
        title: 'Создание поставщика',
        html: 'Пожалуйста подождите...',
        timerProgressBar: true,
        showConfirmButton: false,
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: 'POST',
        url: 'Provider/CreateProv',
        data: { ProviderName: providerName },
        success: function(response) {
            setTimeout(function () {
                    Swal.close(); 
                    Swal.fire({
                        title: 'Создание поставщика',
                        text: response.description,
                        icon: 'success',
                        confirmButtonText: 'Окей'

                    }).then((result) => {
                        if (result.isConfirmed) {
                            location.reload();
                        }
                    });
                },
                1000);

        },
        error:function(response) {
            setTimeout(function () {
                Swal.close(); 
                Swal.fire({
                    title: 'Что-то пошло не так..',
                    text: response.responseJSON.description,
                    icon: 'error',
                    confirmButtonText :'Окей'
                });
            },1000);
        }

    });

}
//Создание склада
function CreateWarehouse(WhName) {
    Swal.fire({
        title: 'Создание склада',
        html: 'Пожалуйста подождите...',
        timerProgressBar: true,
        showConfirmButton: false,
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: 'POST',
        url: 'Warehouse/CreateWarehouse',
        data: { WarehouseName: WhName },

        success: function(response) {
            setTimeout(function() {
                Swal.close();
                Swal.fire({
                    title: 'Создание склада',
                    text: response.description,
                    icon: 'success',
                    confirmButtonText: 'Окей'
                });
            },1000);
        },
        error:function(response) {
            setTimeout(function() {
                Swal.close();
                Swal.fire({
                    title: 'Упс, что-то пошло не так',
                    text: response.responseJSON.description,
                    icon: 'error',
                    confirmButtonText:'Окей'
                });
            },1000);
        }
    });

}

//Создам общий метод для массового применения с передачей парамтеров
//Отвечает, что отображается в зазрузочном окне - NameCreating

function MassUpdate(url, data,NameCreating,  ResponseTitle) {

    Swal.fire({
        title: NameCreating,
        html: 'Пожалуйста, подождите..',
        timerProgressBar: true,
        showConfirmButton: false,
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: 'POST',
        url: url,
        data: data,
        success: function(response) {
            setTimeout(function() {
                Swal.close();
                Swal.fire({
                    title: ResponseTitle,
                    text: response.description,
                    icon: 'success',
                    confirmButtonText: 'Окей',

                }).then((result) => {
                    if (result.isConfirmed) {
                        $('#modal').modal('hide');
                        location.reload();
                    }
                });
            },1000);
        },
        error: function(response) {
            setTimeout(function() {
                Swal.close();
                Swal.fire({
                    title: ResponseTitle,
                    text: response.responseJSON.description,
                    icon: 'error',
                    confirmButtonText: 'Окей',
                }).then((result) => {
                    if (result.isConfirmed) {
                        $('#modal').modal('hide');
                    }
                });
            },1000);
        }

    });
}

//Вызывается модальное окно и по кнопке изменений сохраняется профиль



$('#AnswerIdB').click(function() {
    UpdateStatment();
});
$('#BtnBind').click(function () {
    if ($('.row').is(':visible')) {
        // Если элемент видим, то скрываем его плавно
        $('.row').slideUp();
    } else {
        // Если элемент скрыт, то плавно его показываем
        $('.row').slideDown();
       //инициализируем выборку
        initializeSelect2();
    }
});
//Привязка данных со стороны склада
$("#BtnBindWarehouse").click(function () {

 
    var User = $("#UserId").val();
    var Product = $("#ProductID").val();
    var Com = $('#BindingComent').val();
    var Data = {
        UserId : User,
        WarehouseId: currentWarehouseId,
        ProductId: Product,
        Comments:Com
    }

    

    Swal.fire({
        title: 'Создание связи',
        html: '<img src="/myIcon/icons8-cheque.gif" alt="Custom"><p>Пожалуйста, подождите...</p>',
        timerProgressBar: true,
        showConfirmButton: false,
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: 'POST',
        url: '/Warehouse/BindingWarehouseProduct',
        data: Data,
        success: function(response) {
            setTimeout(function() {
                Swal.close();
                Swal.fire({
                    title: 'Привязка товара',
                    text: response.description,
                    icon: 'success',
                    confirmButtonText: 'Окей',

                }).then((result) => {
                    location.reload();
                });
            },2000);
        },
        error: function(response) {
            setTimeout(function() {
                Swal.close();
                Swal.fire({
                    title: 'Упс..что-то пошло не так',
                    text: response.responseJSON.description,
                    icon: 'error',
                    confirmButtonText: 'Окей',
                }).then((result) => {

                });
            },2000);
        }

    });

});
// Привязка данных к пользователю со стороны заявки
function BindProduct(hiddenId, productsId,Com) {
    Swal.fire({
        title: 'Создание связи',
        html: 'Пожалуйста подождите...',
        timerProgressBar: true,
        showConfirmButton: false,
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: 'POST',
        url: '/Product/BindingProduct',
        data: {
            StatId: hiddenId,
            ProductId: productsId,
            Comments : Com
        },
        success: function (response) {
            setTimeout(function () {
                Swal.close();
                Swal.fire({
                    title: 'Привязка товара',
                    text: response.description,
                    icon: 'success',
                    confirmButtonText: 'Окей',

                }).then((result) => {
                   
                });
            }, 1000);
        },
        error: function (response) {
            setTimeout(function () {
                Swal.close();
                Swal.fire({
                    title: 'Привязка товара',
                    text: response.responseJSON.description,
                    icon: 'error',
                    confirmButtonText: 'Окей',
                }).then((result) => {
                    
                });
            }, 1000);
        }
    });
}
//Метод на открепления товара от пользователя, передача в контроллер
function UnbindingProduct(ProfileId, productsToUnbind) {
    Swal.fire({
        title: 'Открепление товара',
        html: 'Пожалуйста, подождите...',
        timerProgressBar: true,
        showConfirmButton: false,
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }

    });
    // Формируем массив объектов в ожидаемом формате
 
    var data = {
        ProfileId: ProfileId,
        model: productsToUnbind
    };

    $.ajax({
        type: 'POST',
        url: '/Product/UnbindingProduct',
       /* contentType: 'application/json',*/
        data: data,
        success: function(response) {
            Swal.close();
            setTimeout(function() {
                    Swal.fire({
                        title: 'Открепление товара',
                        text: response.description,
                        icon: 'success',
                        confirmButtonText: 'Окей',

                    }).then((result) => {
                        location.reload();
                    });
                },
                2000);
        },
        error:function(response) {
            Swal.close();
            setTimeout(function() {
                Swal.fire({
                    title: 'Что-то пошло не так',
                    text: response.responseJSON.description,
                    icon: 'error',
                    confirmButtonText: 'Хорошо',

                });
            },2000);
        }
    });
}
//Тут будет функция обработки клика поиска товара
$('#BtnFindInfoProducts').click(function() {
    // Получаем выбранный ID товара из элемента <select>
    var selectedProductId = $('#ProductId').val();

    FindProduct(selectedProductId);
});

function FindProduct(Id) {
    Swal.fire({
        title: 'Поиск товара',
        html:'<img src="/myIcon/icons8-find.gif" alt="Custom Icon"><p>Поиск товара, пожалуйста подождите...</p>',
        timerProgressBar: true,
        showConfirmButton: false,
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }

    });
    $.ajax({
        type: 'GET',
        url: '/Product/GetAllInfoProduct',
        data: { id: Id },
        success:function(response) {
            setTimeout(function() {
                Swal.close();
                Swal.fire({
                    title: 'Информация о товаре',
                    text: response.description,
                    icon: 'success',
                    confirmButtonText: 'Отлично'
                }).then((result) => {
                    if (result.isConfirmed) {
                        //Тут будет функция заполнения данными
                        const data = response.data;
                        console.log(data);
                        ProductInfo(data);


                    }
                });
            },2000);
        },
        error:function(response) {
            setTimeout(function() {
                Swal.close();
                Swal.fire({
                    title: 'Что-то пошло не так',
                    text: response.responseJSON.description,
                    icon: 'error',
                    confirmButtonText: 'Окей'
                });
            },1000);
        }
    });
}
//Функция для отображения профиля 
function ProductUserFunction(data) {
    const $ProfileContainer = $('#ProfileProduct');
    let message;
    if (!data.usver) {
        let message  = 'Товар ни за кем не закреплён';

        if (data.comments) {
            message += `<br>Комментарий к товару: ${data.comments}`;
        }


        $ProfileContainer.html(message);
    } else {


        let userId = data.usver.userId;

        // Формируем URL-адрес для перехода на страницу профиля

        let profileUrl = `/Profile/Detail/${userId}`;
        let Profile = `
    <div class="card-profile">
        <div class="card-body">
            <h5 class="card-title">Владелец товара: ${data.usver.login}</h5>
            <p class="card-text">Фамилия: ${data.usver.lastName || 'Неизвестно'}</p>
            <p class="card-text">Имя: ${data.usver.name || 'Неизвестно'}</p>
            <p class="card-text">Отчество: ${data.usver.surname || 'Неизвестно'}</p>
        </div>
        <div class="card-footer">
        <div class="d-grid gap-2">
        <a  href="${profileUrl}" class="btn btn-primary">Перейти в профиль </a>
        </div>
   
        </div>

    </div>
`;
     let   msg = `<br>Комментарий к товару: ${data.comments}`;
        Profile += msg;
        $ProfileContainer.html(Profile);
    }
}



// Функция для отображения маршрутизации товара
function GenerateRoute(transfers,debing) {
    const $infoContainer = $('#routeInfoContainer');

    if (!Array.isArray(transfers) || transfers.length === 0) {
        $infoContainer.html('Нет информации о перемещениях товара.');
        return;
    }

    let infoHtml = '<div style="font-weight: bold; font-size: 18px; margin-bottom: 10px;">Все перемещения товара</div>';

    transfers.forEach((transfer, index) => {
        let color = '';

        if (transfers.length === 1) {
            // Если перемещение одно (только начальная и конечная точка)
            color = 'orange'; // Оранжевый цвет для единственного перемещения
        } else {
            // Если перемещений больше одного
            if (index === 0) {
                // Первое перемещение
                color = 'green'; // Зелёный цвет для первого перемещения
            } else if (index === transfers.length - 1) {
                // Последнее перемещение
                color = 'orange'; // Оранжевый цвет для последнего перемещения
            } else {
                // Промежуточные перемещения
                color = 'gray'; 
            }
        }


        let transferInfo = `<div style="border: 4px solid ${color}; padding: 5px; margin: 4px;">`
            + `Перемещение ${index + 1}:`
            + `<br>Отправная точка: ${transfer.sourceWarehouseName || 'Неизвестно'}`
            + `<br>Конечная точка:<strong>${transfer.destinationWarehouseName || 'Неизвестно'}</strong>`
            + `<br>Дата прибытия: ${transfer.dateTimeIncoming || 'Неизвестно'}`
            + `<br> Дата отправления: ${transfer.dateTimeOutgoing || 'Неизвестно'}`;

        // Добавляем стили и текст для последней записи
        if (index === transfers.length - 1 && debing) {
            transferInfo += `<div style="position: relative; display: inline-block; padding: 10px; margin-top: 10px; vertical-align: top;">`;
            transferInfo += `<p class="text-center" style="font-size: 20px; position: relative; left: 220px;  z-index: 2; margin: 0; padding: 2px; transform: rotate(-5deg);"> СПИСАН ${debing} </p>`;
            transferInfo += `<div style="position: absolute; top: 0; left: 220px; width: 100%; height: 100%; border: 3px solid brown; transform: rotate(-5deg); z-index: 1; border-radius: 5px;"></div>`;
            transferInfo += `</div>`;
        }



        transferInfo += `<br>Комментарий при перемещении: ${transfer.comments || 'Нет комментариев'}`;

       
        transferInfo += `</div>`;

        // Добавляем HTML-разметку к общему HTML-коду
        infoHtml += transferInfo;
    });

    $infoContainer.html(infoHtml);
}
//Функция выводящая информация о товаре
function BaseInfo(data) {
    const $infoProduct = $('#BaseProductInfo');

    if (!data.nameProduct || !data.inventoryCode) {
        $infoProduct.html('Недостаточно информации о товаре.');
        return;
    }
    // Определяем текущее местоположение товара
    let currentLocation = data.currentWarehouseName || data.originalWarehouse || 'Неизвестно';

    var InfoProduct = `Наименование товара: ${data.nameProduct} `;
    InfoProduct += ` <br>Инвентарный код: ${data.inventoryCode}`;
    InfoProduct += `<br>Изначальное поступление на склад <strong> ${data.originalWarehouse} </strong>`;
    InfoProduct += `<br> Накладная :<strong> ${data.numberDocument} </strong> `;
    InfoProduct += `<br>Дата : ${data.dateInvouce} `;
    InfoProduct += `<br>Текущее местоположение: ${currentLocation}`;

    if (data.dateDebiting) {
        InfoProduct += `<div style="position: relative; display: inline-block; padding: 10px; margin-top: 10px; margin-bottom: 20px; vertical-align: top;">`;
        InfoProduct += `<p class="text-center" style="font-size: 20px; position: relative; left: 220px; z-index: 2; margin: 0; padding: 2px; transform: rotate(-5deg);"> СПИСАН ${data.dateDebiting} </p>`;
        InfoProduct += `<div style="position: absolute; top: 0; left: 220px; width: 100%; height: 100%; border: 3px solid brown; transform: rotate(-5deg); z-index: 1; border-radius: 5px;"></div>`;
        InfoProduct += `</div>`;
    }
    $infoProduct.html(InfoProduct);
    $infoProduct.css({
        'border': '3px solid #e97a0c',  
        'padding': '10px',             
        'margin': '10px'              
    });
}

//Функция отображения информации

function ProductInfo(data) {
    var textFieldsContainer = $('#ProductRowInfo');

    if (textFieldsContainer.is(':visible')) {
        // Если контейнер видим, просто обновляем информацию внутри него
        textFieldsContainer.slideUp('slow', function () {
            // По завершении анимации скрытия, обновляем информацию и снова отображаем контейнер
            GenerateRoute(data.allTransfersProducts, data.dateDebiting);
            BaseInfo(data);
            ProductUserFunction(data)
            textFieldsContainer.slideDown('slow');
        });

    } else {
        // Если контейнер скрыт, заполняем его новой информацией и показываем с анимацией
        GenerateRoute(data.allTransfersProducts,data.dateDebiting);
        BaseInfo(data);
        ProductUserFunction(data)
        textFieldsContainer.slideDown('slow');
    }
}
//напишу ещё одну функцию поиска ВСЕЙ ДОСТУПНОЙ ИНФОРМАЦИИ О ТОВАРЕ - это функция будет вызываться
//Когда администратору нужно будет найти товар и вытащить всю доступную информаицю о нём

function initialiseFindProduct() {
    $('#ProductId').select2({
        placeholder: "Начните искать товар",
        minimumInputLength: 4,
        allowClear: true,
        tags: true,
        dropdownParent: $('#FindProducts'),
        language: {
            inputTooShort: function(args) {
                var remainingChars = args.minimum - args.input.length;
                return "Пожалуйста, введите не менее " + remainingChars + " символов";
            },
            "noResults": function() {
                return "Результаты не найдены";
            },
            "searching": function() {
                return "Поиск...";
            }
        },

        ajax: {
            type: 'POST',
            url: '/Product/GetAllproducts',
            dataType: 'json',
            data: function (params) {
                return {
                    term: params.term
                };
            },
            processResults: function (response) {
                // Преобразование данных словаря в формат, ожидаемый select2
                var data = $.map(response,
                    function (value, key) {
                        return { id: key, text: value };
                    });

                // Возвращаем результаты для select2
                return {
                    results: data
                };

            }
        }
        
    });
}
//Метод для отчёта с поиском пользователя
//Разница в том, что один в привязке товара находится, а другой в отчётности
function initialiseUserReport() {
    $("#UserID").select2({
        placeholder: "Выберите пользователя",
        minimumInputLength: 5,
        allowClear: true,
        tags: true,
        dropdownParent: $('#userModal'),
        language: {
            inputTooShort: function (args) {
                var remainingChars = args.minimum - args.input.length;
                return "Пожалуйста, введите не менее " + remainingChars + " символов";
            },
            "noResults": function () {
                return "Результаты не найдены";
            },
            "searching": function () {
                return "Поиск...";
            }
        },
        ajax: {
            type: 'POST',
            url: '/Profile/GetUsers',
            dataType: 'json',
            data: function (params) {
                return {
                    term: params.term
                };
            },
            processResults: function (response) {
                var data = $.map(response,
                    function (val, key) {
                        return { id: key, text: val };
                    });
                return {
                    results: data
                };

            }
        }
    });
}

//метод с возможностью поиска по пользователям
function initializeSelectUser2() {
    $("#UserId").select2({
        placeholder: "Выберите пользователя",
        minimumInputLength: 5,
        allowClear: true,
        tags: true,
        dropdownParent: $('#ModalBindingProduct'),
        language: {
            inputTooShort: function (args) {
                var remainingChars = args.minimum - args.input.length;
                return "Пожалуйста, введите не менее " + remainingChars + " символов";
            },
            "noResults": function () {
                return "Результаты не найдены";
            },
            "searching": function () {
                return "Поиск...";
            }
        },
       ajax: {
           type: 'POST',
           url: '/Profile/GetUsers',
           dataType: 'json',
           data: function(params) {
               return {
                   term: params.term
               };
           },
           processResults : function(response) {
               var data = $.map(response,
                   function(val, key) {
                       return { id: key, text: val };
                   });
               return {
                   results: data
               };

           }
       }
    });
}
//Это combox с возможностью поиска товаров
function initializeSelect2() {
    $('#ProductsID').select2({
        placeholder: "Выберите товар",
        minimumInputLength: 4,
        allowClear: true,
        tags: true,
        dropdownParent: $('#StateModdal'),
        language: {
            inputTooShort: function (args) {
                var remainingChars = args.minimum - args.input.length;
                return "Пожалуйста, введите не менее " + remainingChars + " символов";
            },
            "noResults": function () {
                return "Результаты не найдены";
            },
            "searching": function () {
                return "Поиск...";
            }
        },

        ajax: {
            type: 'POST',
            url: '/Product/GetProduct',
            dataType: 'json',
            data: function(params) {
                return {
                    term: params.term
                };
            },
            processResults: function (response) {
                // Преобразование данных словаря в формат, ожидаемый select2
                var data = $.map(response,
                    function (value, key) {
                        return { id: key, text: value };
                    });

                // Возвращаем результаты для select2
                return {
                    results: data
                };

            }
        }
    });
}
//Метод отправки данных на создание накладной и товаров

    function CreateInvoice() {
        var formData = {
            Number: $('#NumberDocumetn').val(),
            ProviderID: $('#Provider').val(),
            WarehouseId: $('#Warehouse').val(),
            Positions: []
        };
        $('#PositionsContainer').find('.position-row').each(function(index, element) {
            var position = {
                NameProduct: $(element).find(`input[name="NameProduct${index + 1}"]`).val(),
                InventoryCode: $(element).find(`input[name="InventoryCode${index + 1}"]`).val(),
                Quantity: $(element).find(`input[name="Quantity${index + 1}"]`).val(),
                ProviderID: $('#Provider').val(), // Добавляем ProviderID
                WarehouseId: $('#Warehouse').val() // Добавляем WarehouseId
            };
            formData.Positions.push(position);
        });
        Swal.fire({
            title: 'Создание накладной',
            html: 'Пожалуйста подождите...',
            timerProgressBar: true,
            showConfirmButton: false,
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
        $.ajax({
            url: '/Invoice/CreateInvoiceProducts', 
            type: 'POST',
            data: formData,
            success: function(response) {
                setTimeout(function() {
                        Swal.close();
                        Swal.fire({
                            title: 'Создание накладной',
                            text: response.message,
                            icon: 'success',
                            confirmButtonText: 'Окей'

                        }).then((result) => {
                            if (result.isConfirmed) {
                                $('#CreateProductsForm')[0].reset();
                                // Скрыть форму
                                $('#formContainer').hide();
                                // Скрыть все позиции
                                $('.position-row').hide();
                                $('.table-container').show();
                                let table = $('#InvoiceTableID').DataTable();
                                table.ajax.reload();
                            }
                        });
                    },
                    1000);
            },
            error: function(response) {
                setTimeout(function() {
                        Swal.close();
                        Swal.fire({
                            title: 'Упс, что-то пошло не так',
                            text: response.responseJSON.description,
                            icon: 'error',
                            confirmButtonText: 'Окей'
                        });
                    },
                    1000);
            }
        });
    }







//Функция для инициализации таблицы профиля
function initializeProfileTable() {
    var table = " #BindingProductId";
    var language = "//cdn.datatables.net/plug-ins/1.10.25/i18n/Russian.json";
    $(table).DataTable({
        searching: true,
        paging: true,
        ordering: true,
        info: true,
        language: {
            url: language
        }
    });
}

//Триггер на отключение кнопки списания

 async function CancellationTrigger() {
    //отправим get запрос и получим словарь с id и датой
    //Что то с этим сделать
    const Response = await $.get('/Warehouse/GetTimeDebeting');

    if (Object.keys(Response).length > 0) {
       
        Object.entries(Response).forEach(function ([productId, timeDebitting]) {
            var timeDebitting = new Date(timeDebitting);
            var currentTime = new Date();
            var timeDifference = currentTime.getTime() - timeDebitting.getTime();
            if (timeDifference > 24 * 60 * 60 * 1000) {
                var unDebitBtn = $("#UnDebitBtn_" + productId + "");

                unDebitBtn.removeClass('btn-warning').addClass('btn-secondary disabled');
                unDebitBtn.text('Отмена списания недоступна');
            
            }
        });


    }
  

}
//Загрузка таблицы списка утилизационных товаров
function initializeWarehouseDebiting() {
    var tableSelector = "#DebitingAccordian .accordion-body table";
    var languageUrl = "//cdn.datatables.net/plug-ins/1.10.25/i18n/Russian.json";
    $(tableSelector).DataTable({
        searching: true,
        paging: true,
        ordering: true,
        info: true,
        language: {
            url: languageUrl
        }

    });
    CancellationTrigger();

    $(tableSelector).find('tbody tr').each(function () {

        var $row = $(this);
        //Ищем кнопку отменить списание
        var $UndoDebiting = $row.find('.btn-warning');
        var BtnProductID = $UndoDebiting.data('product-id');
        $UndoDebiting.on('click', function () {
          
            UndoWriteProduct(BtnProductID);
        });
    });



}

//Метод отмены списания товара 
function UndoWriteProduct(id) {
    Swal.fire({
        title: 'Внимание',
        text: 'Вы действительно хотите отменить списание товара?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Да,продолжить',
        cancelButtonText: 'Нет'
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                title: 'Отмена списания',
                html: '<img src="/myIcon/icons8-truck.gif" alt="Custom Icon"><p>Пожалуйста, подождите...</p>',
                timerProgressBar: true,
                showConfirmButton: false,
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });
            $.ajax({
                type: 'POST',
                url: '/Transfer/DeleteTransfer',
                data: { Id: id },
                success: function (response) {
                    setTimeout(function () {
                        Swal.close();
                        Swal.fire({
                            title: 'Отмена списания',
                            text: response.description,
                            icon: 'success',
                            confirmButtonText: 'Хорошо'
                        }).then((result) => {
                            if (result.isConfirmed) {
                                location.reload();
                            }
                        });
                    },1000)
                },
                error: function (response) {
                    setTimeout(function () {
                        Swal.close();
                        Swal.fire({
                            title: 'Что-то пошло не так',
                            text: response.responseJSON.description,
                            icon: 'error',
                            confirmButtonText: 'Хорошо'
                        });
                    },1000)
                  

                }
            });
        }
    });
}

//Загрузка таблицы списка товаров на складе через ajax


function initializeWarehouseProductTable(warehouseId) {
    var tableSelector = "#productAccordion .accordion-body table";
    var languageUrl = "//cdn.datatables.net/plug-ins/1.10.25/i18n/Russian.json";

    $(tableSelector).DataTable({
        searching: true,
        paging: true,
        ordering: true,
        info: true,
        language: {
            url: languageUrl
        }
      
    });

    $(tableSelector).find('tbody tr').each(function () {
        var $row = $(this);

        // Находим кнопку "Закрепить" в текущей строке и устанавливаем ей нужный id склада
        var $attachBtn = $row.find('button.btn-success');
        var $debitBtn = $row.find('button.btn-danger');
        $attachBtn.attr('data-warehouse-id', warehouseId);
        $debitBtn.attr('data-warehouse-id', warehouseId);

        $(document).on('click', '#attachBtn', function () {
            var $button = $(this); // Получаем ссылку на кнопку "Закрепить", на которую кликнули
            var productId = $button.data('product-id'); // Получаем ID продукта из атрибута data-product-id кнопки
            var SourceWh = $debitBtn.data('warehouse-id');

            var nameProduct = $button.closest('tr').find('td:eq(1)').text();

            var Code = $button.closest('tr').find('td:eq(2)').text();
            //Привязка со стороны склада
            BindingProdWarehouse(productId, nameProduct, Code,SourceWh);
          
        });

        $(document).on('click', '#DebittingBtn', function () {
            var $btn = $(this);
            var prod = $btn.data('product-id');
            var nameProduct = $btn.closest('tr').find('td:eq(1)').text();

            var Code = $btn.closest('tr').find('td:eq(2)').text();
           
            DebitingWarehouse(prod, nameProduct, Code);
        })
    });
}


   


//функиця для заполнения данными mass select 
function MassInputWarehouse(id) {
    $.ajax({
        url: '/Warehouse/GetNotCurrentWarehouse',
        type: 'GET',
        data: { id: id }, // Передаем идентификатор склада как параметр запроса
        success: function (response) {
            if (response && response.data && response.data.length >0) {
                var warehouses = response.data;
                var selectMassWarehouse = $('#SelectMassWarehouse');
                selectMassWarehouse.empty(); // Очищаем список перед добавлением новых элементов
                warehouses.forEach(function (warehouse) {
                    // Добавляем класс "utilization" к складам, которые являются утилизованными
                  
                    var optionClass = warehouse.isService ? 'utilization' : '';
                
                    selectMassWarehouse.append('<option value="' + warehouse.id + '" class= "' + optionClass + '">' + warehouse.name + '</option>');
                });
            } else {
                var selectMassWarehouse = $('#SelectMassWarehouse');
                selectMassWarehouse.append('<option disabled="">Доступные склады не найдены</option>'); // Добавляем сообщение
               
            }
        },
        error: function (xhr, status, error) {
            console.error('Произошла ошибка при выполнении запроса:', error);
        }
    });
}
//Клик события списания
function DebitingWarehouse(ProductId, Name, Code) {

    var nameProduct = Name;
    var codeProduct = Code;
    var Product = ProductId;
   
    $('#PrID').val(Product);
    $('#DebitNameProduct').val(nameProduct);
    $('#CodInv').val(codeProduct);
    $('#ModelDebiting').modal('show');


}

//Вот тут обработает событие списание кнопки
$('#BtnDebiting').click(function () {
    var massMoveProducts = [];
    var Prod = $('#PrID').val();
    var Com = $('#DegitingCom').val();

    if (!Com || Com.length <= 5) {
        Swal.fire({
            title: 'Ошибка списания',
            text: 'Комментарий обязателен для списания товара.',
            icon: 'info',
            confirmButtonText: 'Хорошо'
        });
    }
    else {



        let Destw;
        $.get('/Warehouse/GetDetimingWarehouse', function (response) {
            if (response.data) {
                Destw = response.data;

                var product = {
                    Id: Prod,
                    SourceWarehouseId: null,
                    DestinationWarehouseId: Destw,
                    CountTransfer: 1,
                    CommentDebiting: Com
                };


                massMoveProducts.push(product);
                MassMovedProduct(massMoveProducts, 'Списание товара', 'Списание', Destw);
            }
        });
    }
  

   
});



//Клик события закрепление
function BindingProdWarehouse(ProductId,Name,Code) {
    // Так как клики многократные, остальные убираем, оставляем текущий

    var nameProduct = Name;
    var codeProduct = Code;
    var Product = ProductId;
         $('#ProductID').val(Product);
        $('#nameProduct').val(nameProduct);
    $('#inventoryCode').val(codeProduct);

        $('#ModalBindingProduct').modal('show');
        initializeSelectUser2();
   
}





//Метод массового перемещения товара, будет уходить по тому же url, только сразу списком
//titl - Когда загрузка, res - при ответе
//DestanWh - Для того, чтобы проверить, что за склад, на который товар переносится, отправим его сюда отдельно
function MassMovedProduct(products, titl, res, DestanWh) {
    // Добавляем текущий склад к каждому продукту
    products.forEach(function (product) {
        product.SourceWarehouseId = currentWarehouseId;
    });

    // Делаем запрос на сервер для получения данных о складе
    $.get('/Warehouse/GetMovedWarehouse', { id: DestanWh }, function (response) {
        if (response.data) {
            // Проверяем, не является ли склад сервисным центром
            var isService = response.data.isService;

            if (isService) {
                var count = products.length;
                var messageText = count > 1 ?
                    `Перемещаемые товары ${count} (шт) будут перенесены на склад утилизации. Продолжить?` :
                    'Перемещаемый товар будет перенесён на склад утилизации. Продолжить?';
                var title = count > 1 ? 'Списание товаров' : 'Списание товара';

                // Проверяем валидность комментариев
                let hasInvalidComments = products.some(item => {
                    let commentsValid = true;
                    if (item.Comments === undefined) {
                        commentsValid = item.CommentDebiting && item.CommentDebiting.length >= 5;
                    } else if (item.CommentDebiting === undefined) {
                        commentsValid = item.Comments && item.Comments.length >= 5;
                    }
                    return !commentsValid;
                });


                if (hasInvalidComments) {
                    Swal.fire({
                        title: 'Ошибка перемещения товара на склад утилизации',
                        text: 'При перемещении товара на склад утилизации, комментарий обязателен. (Минимальная длина 5 символов)',
                        icon: 'error',
                        confirmButtonColor: 'green',
                        confirmButtonText: 'Хорошо'
                    });
                } else {
                    Swal.fire({
                        title: 'Внимание',
                        text: messageText,
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: 'Да, продолжить',
                        cancelButtonText: 'Отменить'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            Swal.fire({
                                title: title,
                                html: '<img src="/myIcon/icons8-truck.gif" alt="Custom Icon"><p>Пожалуйста, подождите...</p>',
                                timerProgressBar: true,
                                showConfirmButton: false,
                                allowOutsideClick: false,
                                didOpen: () => {
                                    Swal.showLoading();
                                }
                            });
                            $.ajax({
                                type: 'POST',
                                url: '/Transfer/AddTransfer',
                                data: { model: products },
                                success: function (response) {
                                    setTimeout(function () {
                                        Swal.close();
                                        Swal.fire({
                                            title: 'Списание товаров',
                                            text: response.description,
                                            icon: 'success',
                                            confirmButtonText: 'Окей',
                                        }).then((result) => {
                                            if (result.isConfirmed) {
                                                location.reload();
                                            }
                                        });
                                    }, 1000);
                                },
                                error: function (response) {
                                    setTimeout(function () {
                                        Swal.close();
                                        Swal.fire({
                                            title: 'Упс..что-то пошло не так',
                                            text: response.responseJSON.description,
                                            icon: 'error',
                                            confirmButtonText: 'Понятно',
                                        });
                                    }, 1000);
                                }
                            });
                        }
                    });
                }
            } else {
                // Если склад не является сервисным центром, отправляем AJAX-запрос без дополнительных проверок
                Swal.fire({
                    title: titl,
                    html: '<img src="/myIcon/icons8-truck.gif" alt="Custom Icon"><p>Пожалуйста, подождите...</p>',
                    timerProgressBar: true,
                    showConfirmButton: false,
                    allowOutsideClick: false,
                    didOpen: () => {
                        Swal.showLoading();
                    }
                });
                $.ajax({
                    type: 'POST',
                    url: '/Transfer/AddTransfer',
                    data: { model: products },
                    success: function (response) {
                        setTimeout(function () {
                            Swal.close();
                            Swal.fire({
                                title: res,
                                text: response.description,
                                icon: 'success',
                                confirmButtonText: 'Окей',
                            }).then((result) => {
                                location.reload();
                            });
                        }, 1000);
                    },
                    error: function (response) {
                        setTimeout(function () {
                            Swal.close();
                            Swal.fire({
                                title: 'Упс..что-то пошло не так',
                                text: response.responseJSON.description,
                                icon: 'error',
                                confirmButtonText: 'Понятно',
                            });
                        }, 1000);
                    }
                });
            }
        }
    });
}



function ReportsWarehouse(StartDate,EndDate) {
    Swal.fire({
        title: 'Формирование отчёта по складам',
        html: '<img src="/myIcon/wired-lineal-edit-document.gif" alt="Custom Icon"><p>Пожалуйста, подождите...</p>',
        timerProgressBar: true,
        showConfirmButton: false,
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }

    });

    $.ajax({
        type: 'POST',
        url: '/Reports/ReportWarehouses',
        data: {
            StartTime: StartDate,
            EndTime: EndDate
        },
        success: function(response) {
            setTimeout(function() {
                Swal.close();
                Swal.fire({
                    title: 'Формирование отчёта',
                    text: response.description,
                    icon:'info'
                }).then((result) => {
                    if (result) {
                        var reportHtml = '';
                        reportHtml += '<div style="position: relative;">'; // Обертка для контейнера и кнопки
                        reportHtml += '<button id="CloseReportsBtn" style="position: absolute; top: 10px; right: 6px;" class="btn btn-danger">Закрыть</button>';
                        reportHtml += '<h3 class="text-center">Отчет по складам за ' + response.data.startTime + ' по ' + response.data.endTime + '</h3>';

                       

                        reportHtml += '</div>';
                        // Перебираем данные о складах и их продуктах
                        response.data.warehousesReports.forEach(function (warehouseReport) {
                            reportHtml += '<div class="warehouse-report mb-4">';
                            reportHtml += '<h4 class="text-center"> СКЛАД ' + warehouseReport.warehouseName + '</h4>';
                            reportHtml += '<div class="table-responsive">';
                            reportHtml += '<table class="table table-bordered">';
                            reportHtml += '<thead>';
                            reportHtml += '<tr>';
                            reportHtml += '<th colspan="3" style="display:none" class="text-center">СКЛАД ' + warehouseReport.warehouseName + '</th>';
                            reportHtml += '</tr>';
                            reportHtml += '<tr>';
                            reportHtml += '<th>Наименование товара</th>';
                            reportHtml += ' <th>Инвентарный код </th>';
                            reportHtml += '<th>Количество на складе</th>';
                            reportHtml += '<th>Доступное количество</th>';
                            reportHtml += '</tr>';
                            reportHtml += '</thead>';
                            reportHtml += '<tbody>';
                            if (warehouseReport.productsInfo.length>0) {


                                warehouseReport.productsInfo.forEach(function (productInfo) {
                                    reportHtml += '<tr>';
                                    reportHtml += '<td>' + productInfo.productName + '</td>';
                                    reportHtml += '<td>' + productInfo.inventoryCode + '</td>';
                                    reportHtml += '<td>' + productInfo.quantityOnWarehouse + '</td>';
                                    reportHtml += '<td>' + productInfo.availableQuantity + '</td>';
                                    reportHtml += '</tr>';
                                });
                            }
                            else {
                                // Если нет товаров на складе, добавляем сообщение
                                reportHtml += '<tr>';
                                reportHtml += '<td colspan="4" class="text-center">На складе товаров нет</td>';
                                reportHtml += '</tr>';
                            }
                            reportHtml += '</tbody>';
                            reportHtml += '<tfoot>';
                            reportHtml += '<tr>';
                            reportHtml += '<td colspan="4" class="text-center font-weight-bold"><strong> Итого </strong> товаров на складе: ' + '<strong>' + warehouseReport.totalQuantity + ' </strong> </td>';
                            reportHtml += '</tr>';
                            reportHtml += '</tfoot>';
                            reportHtml += '</table>';
                            reportHtml += '</div>'; // Закрываем div.table-responsive
                            reportHtml += '</div>'; // Закрываем div.warehouse-report
                            
                        });

                        reportHtml += '<div class="footer">' +
                            '<div class="d-grid gap-2">' +
                            '<a class="btn btn-info" id="ExportBtn"><strong>Экспорт</strong></a>' +
                            '</div>' +
                            '</div>';

                       /* $('#reportContainer').css('display', 'block');*/
                        $('#reportContainer').show();
                        // Вставляем HTML-код отчета в контейнер
                        $('#reportContainer').html(reportHtml);

                  

                     
                    }
                    
                });
            },2000);
        },
        error: function (response) {
            setTimeout(function () {
                Swal.close();
                Swal.fire({
                    title: 'Формирование отчёта',
                    text: response.responseJSON.description,
                    icon: 'error'
                });
            }, 1000);
        }
    });

}
//Обработчик закрытия отчёта
$(document).on('click', '#CloseReportsBtn', function () {

    $("#reportContainer").fadeOut();
});
   


//Функция для передачи данных в метод формирования отчёта для юзверя
$("#BtnUserReports").on('click', function() {

    var userId = $("#UserID").val();
    if (userId) {
        UserReports(userId);
    } else {
        Swal.fire({
            title: 'Что-то пошло не так',
            text: 'Пользователь не выбран',
            icon: 'error'
        });
    }
    

});

function UserReports(user) {
    Swal.fire({
        title: 'Формирование отчёта по пользователю',
        html: '<img src="/myIcon/wired-lineal-edit-document.gif" alt="Custom Icon"><p>Пожалуйста, подождите...</p>',
        timerProgressBar: true,
        showConfirmButton: false,
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }

    });


    $.ajax({
        type: 'POST',
        url: '/Reports/ReportUserProducts',
        data: { userid: user },
        success: function(response) {
            setTimeout(function() {
                Swal.close();
                Swal.fire({
                    title: 'Формирование отчёта.',
                    text: response.description,
                    icon: 'info'
                }).then((result) => {
                    if (result) {
                        var reportHtml = '';
                     
                        reportHtml += '<button id="CloseReportsBtn" class="btn btn-danger">Закрыть</button>';
                        if (response.data.length > 0) {
                          
                            reportHtml += '<h3 class="text-center">Отчет по пользователю ' + response.data[0].fullName + '</h3>';

                            // Добавляем таблицу с данными о товарах
                            reportHtml += '<table class="table table-bordered">';
                            reportHtml += '<thead>';
                            reportHtml += '<tr>';
                            reportHtml += '<th colspan="3" style="display:none" class="text-center">Пользователь:' + response.data[0].fullName + '</th>';
                            reportHtml += '</tr>';
                            reportHtml += '<tr>';
                            reportHtml += '<th>Наименование товара</th>';
                            reportHtml += '<th>Инвентарный код</th>';
                            reportHtml += '<th>Количество</th>';
                            reportHtml += '</tr>';
                            reportHtml += '</thead>';
                            reportHtml += '<tbody>';

                            // Добавляем строки с данными о товарах
                            response.data.forEach(function (item) {
                                reportHtml += '<tr>';
                                reportHtml += '<td>' + item.productName + '</td>';
                                reportHtml += '<td>' + item.code + '</td>';
                                reportHtml += '<td>' + item.quantity + '</td>';
                                reportHtml += '</tr>';
                            });
                            reportHtml += '<tfoot>';
                            reportHtml += '<tr>';
                            reportHtml += '<td colspan="4" class="text-center font-weight-bold" style="background-color: #fc6;"> <strong>Итого </strong> прикреплённых товаров: ' + '<strong>' + response.data[0].totalCount + '  </strong> </td>';
                            reportHtml += '</tr>';
                            reportHtml += '</tfoot>';
                            reportHtml += '</tbody>';
                            reportHtml += '</table>';
                            if (response.data.length > 0) {


                                reportHtml += '<div class="footer">' +
                                    '<div class="d-grid gap-2">' +
                                    '<a class="btn btn-info" id="ExportBtn"><strong>Экспорт</strong></a>' +
                                    '</div>' +
                                    '</div>';
                            }

                        } else {
                            // Если данных нет, добавляем сообщение об этом
                            reportHtml += '<p class="text-center font-weight-bold" style="font-size: larger; style="background-color: #fc6;">' + response.description + '</p>';
                        }
                      

                        $('#reportContainer').fadeIn();
                        // Вставляем HTML-код отчета в контейнер
                        $('#reportContainer').html(reportHtml);
                    }

                });
            },1000);
        },
        error: function(response) {
            setTimeout(function() {
                Swal.close();
                Swal.fire({
                    title: 'Что-то пошло не так',
                    text: response.responseJSON.description,
                    icon: 'error'
                });
            },1000);
        }
    });
}
//Функция формирования отчёта по пользователям
function UsersReports() {
    Swal.fire({
        title: 'Формирование отчёта по пользователям',
        html: '<img src="/myIcon/wired-lineal-edit-document.gif" alt="Custom Icon"><p>Пожалуйста, подождите...</p>',
        timerProgressBar: true,
        showConfirmButton: false,
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }

    });
    $.ajax({
        type: 'GET',
        url: '/Reports/ReportUsersProducts',
        dataType: 'json',
        success:function(response) {
            setTimeout(function() {
                Swal.close();
                Swal.fire({
                    title: 'Формирование отчёта по пользователям',
                    text: response.description,
                    icon: 'info',
                    confirmButtonText: 'Отлично'

                }).then((result) => {
                    if (result) {
                        var ReportUser = '';
                        var exportButtonVisible = false; // Флаг для отображения кнопки экспорта
                        ReportUser += '<button id="CloseReportsBtn" class="btn btn-danger">Закрыть</button>';
                        if (response.data.users.length > 0) {

                            response.data.users.forEach(function(user) {
                                ReportUser += '<h3 class="text-center"> Пользователь:' + user.fullName + '('+user.login +')'+ '</h3>';

                                if (user.userProducts.length > 0) {
                                    exportButtonVisible = true;
                                    ReportUser += '<table class="table table-bordered">';
                                    ReportUser += '<thead>';
                                    ReportUser += '<tr>';
                                    ReportUser += '<th colspan="3" style="display:none" class="text-center">Пользователь: ' + user.fullName + '(' + user.login + ')' + '</th>';
                                    ReportUser += '</tr>';
                                    ReportUser += '<tr>';
                                    ReportUser += '<th>Наименование товара</th>';
                                    ReportUser += '<th>Инвентарный код</th>';
                                    ReportUser += '<th>Количество</th>';
                                    ReportUser += '</tr>';
                                    ReportUser += '</thead>';
                                    ReportUser += '<tbody>';

                                    user.userProducts.forEach(function (product) {
                                        ReportUser += '<tr>';
                                        ReportUser += '<td>' + product.nameProduct + '</td>';
                                        ReportUser += '<td>' + product.inventoryCod + '</td>';
                                        ReportUser += '<td>' + product.totalCount + '</td>';
                                        ReportUser += '</tr>';
                                    });

                                    ReportUser += '<tfoot>';
                                    ReportUser += '<tr>';
                                    ReportUser += '<td colspan="3" class="text-center font-weight-bold" style="background-color: #fc6;"> <strong>Итого </strong> прикреплённых товаров: ' + '<strong>' + user.totalProducts + ' </strong>   </td>';
                                    ReportUser += '</tr>';
                                    ReportUser += '</tfoot>';

                                    ReportUser += '</tbody>'; 
                                    ReportUser += '</table>';
                                  
                                   

                                } else {
                                    ReportUser += '<p class="text-center font-weight-bold" style="font-size: larger;">' +user.message+ '</p>';
                                }
                              

                            });
                        } else {
                            ReportUser+= '<p class="text-center font-weight-bold" style="font-size: larger;">Нет пользователей</p>';
                        }
                        if (exportButtonVisible) {


                            ReportUser += '<div class="footer">' +
                                '<div class="d-grid gap-2">' +
                                '<a class="btn btn-info" id="ExportBtn"><strong>Экспорт</strong></a>' +
                                '</div>' +
                                '</div>';
                        }
                        $('#reportContainer').fadeIn();
                        // Вставляем HTML-код отчета в контейнер
                        $('#reportContainer').html(ReportUser);

                    }
                });

            },2000);
        },
        error:function(response) {
            setTimeout(function() {
                    Swal.close();

                    Swal.fire({
                        title: 'Упс,что-то пошло не так',
                        text: response.responseJSON.description,
                        icon: 'error'
                    });
                },
                1000);
        }
    });
}

//Функция формирования отчёта по списанным товарам

function ReportDebbetingProdict() {
    Swal.fire({
        title: 'Формирование отчёта по списанному товару',
        html: '<img src="/myIcon/wired-lineal-edit-document.gif" alt="Custom Icon"><p>Пожалуйста, подождите...</p>',
        timerProgressBar: true,
        showConfirmButton: false,
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    $.ajax({
        type: 'GET',
        url: '/Reports/ReportDebitingProducts',
        dataType: 'json',
        success: function (response) {
            setTimeout(function () {
                Swal.close();
                if (response.data) {
                    var DebitingReport = '';
                    var exportButtonVisible = false;

                    DebitingReport += '<button id="CloseReportsBtn" class="btn btn-danger">Закрыть</button>';

                    if (response.data.whDebitingProduct.length > 0) {
                        exportButtonVisible = true;
                        DebitingReport += `<h3 class="text-center">Отчет по списанному товару, находящемуся на  ${response.data.warehouseName}</h3>`;
                        DebitingReport += '<div class="warehouse-report mb-4">';
                        DebitingReport += '<div class="table-responsive">';
                        DebitingReport += '<table class="table table-bordered">';
                        DebitingReport += '<thead>';
                        DebitingReport += '<tr>';
                        DebitingReport += '<th>Наименование товара</th>';
                        DebitingReport += '<th>Инвентарный код</th>';
                        DebitingReport += '<th>Дата поступления товара</th>';
                        DebitingReport += '<th>Дата списания</th>';
                        DebitingReport += '<th>Склад, на который товар изначально поступал</th>';
                        DebitingReport += '<th>Склад, с которого товар был списан</th>';
                        DebitingReport += '<th>Комментарий по списанному товару</th>';
                        DebitingReport += '</tr>';
                        DebitingReport += '</thead>';
                        DebitingReport += '<tbody>';

                        response.data.whDebitingProduct.forEach(function (item) {
                            DebitingReport += '<tr>';
                            DebitingReport += '<td>' + item.productName + '</td>';
                            DebitingReport += '<td>' + item.inventory + '</td>';
                            DebitingReport += '<td>' + item.dataEntrance + '</td>';
                            DebitingReport += '<td>' + item.dateDebiting + '</td>';
                            DebitingReport += '<td>' + item.originalWarehouse + '</td>';
                            DebitingReport += '<td>' + item.debitingWarehouse + '</td>';
                            DebitingReport += '<td>' + item.commentsDebiting + '</td>';
                            DebitingReport += '</tr>';
                        });

                        DebitingReport += '</tbody>';
                        DebitingReport += '<tfoot>';
                        DebitingReport += '<tr>';
                        DebitingReport += '<td colspan="7" class="text-center font-weight-bold"><strong>Итого товаров на складе: ' + response.data.totalCount + '</strong></td>';
                        DebitingReport += '</tr>';
                        DebitingReport += '</tfoot>';

                        DebitingReport += '</table>';
                        DebitingReport += '</div>'; // Закрываем div.table-responsive
                        DebitingReport += '</div>'; // Закрываем div.warehouse-report
                    } else {
                        DebitingReport += '<p class="text-center">На складе нет списанных товаров</p>';
                    }

                    if (exportButtonVisible) {
                        DebitingReport += '<div class="footer">' +
                            '<div class="d-grid gap-2">' +
                            '<a class="btn btn-info" id="ExportBtn"><strong>Экспорт</strong></a>' +
                            '</div>' +
                            '</div>';
                    }

                    $('#reportContainer').show();
                    $('#reportContainer').html(DebitingReport);
                } else {
                    Swal.fire({
                        title: 'Формирование отчёта',
                        text: 'Отчет не найден',
                        icon: 'error'
                    });
                }
            }, 2000)
        },
        error: function (response) {
            setTimeout(function () {
                Swal.close();
                Swal.fire({
                    title: 'Формирование отчёта',
                    text: response.responseJSON.description,
                    icon: 'error'
                });
            }, 1000);
        }
    });
}

// Функция для создания и стилизации таблицы
$(document).on('click',"#ExportBtn",function() {

    var h3Elements = $('#reportContainer').find('h3');
    var fileName = '';

    if (h3Elements.length === 1) {
        fileName = h3Elements.first().text().trim();
    } else if (h3Elements.length > 1) {
        
        fileName = 'Отчёт по пользователям';
    } else {
        // Если не найдено ни одного элемента <h3>, устанавливаем стандартное имя файла
        fileName = 'Report';
    }
    $('#reportContainer').tableExport({
        type: 'xlsx', 
        escape: 'false', 
        fileName: fileName,
        bootstrap: true
     
    });
});
//Функция заморозки склада или поставщика 
function FreeZing(Url, NameTitle,Id) {
    Swal.fire({
        title: NameTitle,
        html: '<img src="/myIcon/freeze.gif" alt="Custom Icon"><p>Пожалуйста, подождите...</p>',
        timerProgressBar: true,
        showConfirmButton: false,
        allowOutsideClick: false,
        color:'#black',
  
        didOpen: () => {
            
            $('.frozen-background').fadeIn();
            Swal.showLoading();
        },
        willClose: () => {
            $('.frozen-background').fadeOut(1400);
        }

    });

    $.ajax({
        type: 'POST',
        url: Url,
        data: { Id: Id },
        success: function (response) {
            setTimeout(function () {
                Swal.close();
                Swal.fire({
                    title: NameTitle,
                    icon: 'success',
                    text: response.description,
                    confirmButtonText: 'Отлично',

                }).then((result) => {
                    location.reload();
                })
            }, 3000)

        },
        error: function (response) {
            setTimeout(function () {
                Swal.close();
                Swal.fire({
                    title: 'Упс..что-то пошло не так',
                    text: response.responseJSON.description,
                    icon: 'error',
                    confirmButtonText: 'Понятно'
                })
            }, 3000)
        }
    });
}
//Метод разморозки
function UnFreezing(Url, ResponseTitle, Id) {
    Swal.fire({
        title: ResponseTitle,
        html: '<img src="/myIcon/spring.gif" alt="Custom Icon"><p>Пожалуйста, подождите...</p>',
        timerProgressBar: true,
        showConfirmButton: false,
        allowOutsideClick: false,
        color: '#black',

        didOpen: () => {

            $('.unfrozen-background').fadeIn();
            Swal.showLoading();
        },
        willClose: () => {
            $('.unfrozen-background').fadeOut(1400);
        }

    });

    $.ajax({
        type: 'POST',
        url: Url,
        data: { Id: Id },
        success: function (response) {
            setTimeout(function () {
                Swal.close();
                Swal.fire({
                    title: ResponseTitle,
                    icon: 'success',
                    text: response.description,
                    confirmButtonText: 'Отлично',

                }).then((result) => {
                    location.reload();
                })
            }, 3000)

        },
        error: function (response) {
            setTimeout(function () {
                Swal.close();
                Swal.fire({
                    title: 'Упс..что-то пошло не так',
                    text: response.responseJSON.description,
                    icon: 'error',
                    confirmButtonText: 'Понятно'
                })
            }, 3000)
        }
    });
}
//Функиция кнопки
function checkHistory() {
    if (window.history.length > 1) {
        $("#backButton").show();
    } else {
        $("#backButton").hide();
    }
}
