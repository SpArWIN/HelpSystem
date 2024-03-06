//Чтобы  js код в кеш не отправляло, нужно добавлять рандомное значение при подключении
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
            modal.modal('show')
        },
        failure: function () {
            modal.modal('hide')
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
};
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
                    text: response.message,
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
                    text: response.responseJSON.message,
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
        id: $('#hiddenId').val(),
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
                    $('#StateModdal').modal('hide');
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
                    $('#StateModdal').modal('hide');
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
        erorr:function(response) {
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
    $('#SaveChanBtn').click(function () {
        SaveProfile();
    });
$('#AnswerIdB').click(function() {
    UpdateStatment();
});
