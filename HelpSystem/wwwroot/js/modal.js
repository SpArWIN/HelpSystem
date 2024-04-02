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

        },
        failure: function () {
            modal.modal('hide')
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
};
$('#SaveChanBtn').on('click', function () {
    // Получаем контекст из кнопки
    var context = $(this).data('context');

    if (context === 'profile') {
        // Выполняем соответствующее действие для профиля
        var id = $('#hiddenId').val();
        var lastName = $('#lastName').val();
        var Name = $('#firstName').val();
        var Surname = $('#patronymic').val();
        var age = $('#age').val();

        var profileData = {
            Id: id,
            Age: age,
            Surname: Surname,
            LastName: lastName,
            Name: Name
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

    var Name = $("#nameProduct").val();
    var Code = $("#inventoryCode").val();
    var Count = $("#countUnbinding").val();
    var User = $("#UserId").val();
    
    var Data = {
        UserId : User,
        ProductName: Name,
        InventoryCode :Code,
        CountBinding: Count,
        WarehouseId: currentWarehouseId
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
function UnbindingProduct(ProfileId, NameProduct, Code, CountUnbinding) {
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

    var data = {
        ProfileId: ProfileId,
        NameProduct: NameProduct,
        Code: Code,
        CountUnbinding: CountUnbinding
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
            url: '/Invoice/CreateInvoiceProducts', // URL для отправки данных формы
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

//Загрузка таблицы списка товаров на складе через ajax
function initializeWarehouseProductTable(warehouseId) {
    
   return  $("#WarehouseProductTable").DataTable({
        ajax: {
            url: "/Warehouse/GetJSONWarehouse",
            type: "GET",
            data: { id: warehouseId },
            dataSrc: "data"
        },
        language: {
            url: "//cdn.datatables.net/plug-ins/1.10.25/i18n/Russian.json",
            emptyTable: "Нет товаров для отображения"
        },
        columns: [
            { data: "nameProduct", name: "NameProduct", className:"text-center"  },
            { data: "codeProduct", name: "CodeProduct", className: "text-center" },
            { data: "totalCountWarehouse", name: "TotalCountWarehouse", className: "text-center" },
            { data: "availableCount", name: "AvailableCount", className: "text-center" },
            {

                data : function(row) {
                    return warehouseId;
                },
                sortable: false,
                render: function (data, type, row) {
                    return '<button class="btn btn-success btn-op"  data-id="' + data + '">Закрепить</button>';

                }
            }
        ],
        initComplete: function () {
            
            /*  MoveProductWarehouse();*/
            $("#WarehouseProductTable").on("click", ".btn-op", function () {
               

                BindingProdWarehouse();
            });
        }
    });
   
   
}

//функиця для заполнения данными mass select 
function MassInputWarehouse(id) {
    $.ajax({
        url: '/Warehouse/GetNotCurrentWarehouse',
        type: 'GET',
        data: { id: id }, // Передаем идентификатор склада как параметр запроса
        success: function (response) {
            if (response && response.data) {
                var warehouses = response.data;
                var selectMassWarehouse = $('#SelectMassWarehouse');
                selectMassWarehouse.empty(); // Очищаем список перед добавлением новых элементов
                warehouses.forEach(function (warehouse) {
                    selectMassWarehouse.append('<option value="' + warehouse.id + '">' + warehouse.name + '</option>');
                });
            } else {
                console.error('Ошибка при получении списка складов');
            }
        },
        error: function (xhr, status, error) {
            console.error('Произошла ошибка при выполнении запроса:', error);
        }
    });
}
//Клик события закрепление
 function BindingProdWarehouse() {
    // Так как клики многократные, остальные убираем, оставляем текущий
   
    $(document).on('click', '.btn-op',  function () {
        var nameProduct = $(this).closest('tr').find('td:eq(0)').text();
        var codeProduct = $(this).closest('tr').find('td:eq(1)').text();
        
        $('#nameProduct').val(nameProduct);
        $('#inventoryCode').val(codeProduct);
        $('#ModalBindingProduct').modal('show');
        initializeSelectUser2();
    });
}


//Метод одиночгого перемещения товара
function SingleMoveProduct(productId, productName, productCode, warehouseId) {
    var products = [];
    var Product = {
        Id: productId,
        NameProduct: productName,
        CodeProduct: productCode,
        SourceWarehouseId: currentWarehouseId,
        DestinationWarehouseId: warehouseId,
        CountTransfer: 1
    };
    products.push(Product);
    var transferData = { model: products };  // Объект с полем "model", содержащим список товаров
    Swal.fire({
        title: 'Перемещение товара',
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
        data: transferData,
        success: function (response) {
            setTimeout(function () {
                    Swal.close();
                    Swal.fire({
                        title: 'Перемещение  товара',
                        text: response.description,
                        icon: 'success',
                        confirmButtonText: 'Окей',

                    }).then((result) => {
                        location.reload();
                    });
                },
                2000);
        },
        error: function(response) {
            setTimeout(function () {
                    Swal.close();
                    Swal.fire({
                        title: 'Упс, что-то пошло не так',
                        text: response.responseJSON.description,
                        icon: 'error',
                        confirmButtonText: 'Окей'
                    });
                },
                2000);
            
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
                    icon:'success'
                }).then((result) => {
                    location.reload();
                });
            },1000);
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