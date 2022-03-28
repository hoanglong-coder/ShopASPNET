
var pg = {
    pageSize: 3,
    pageIndex: 1
}
//Thông báo
var Toast = Swal.mixin({
    toast: true,
    position: 'top-end',
    showConfirmButton: false,
    timer: 3000
});


//Main
var userController = {
    init: function () {
        //Khởi tạo theme
        userController.initTheme();
        //Show danh dánh account
        userController.listOrder();

    },
    initTheme: function () {
        $('.select2bs4').select2({
            theme: 'bootstrap4'
        })
    },
    createProvince: function () {
        $.ajax({
            dataType: "json",
            url: 'https://raw.githubusercontent.com/madnh/hanhchinhvn/master/dist/tinh_tp.json',
            success: function (data) {
                var $select = $('#tinh');
                var $select2 = $('#tinhCreate');
                $select.find('option').remove();
                $.each(data, function (key, value) {
                    $('<option>').val(value.name).text(value.name).appendTo($select);
                    $('<option>').val(value.name).text(value.name).appendTo($select2);
                });
            }
        });
    },
    listOrder: function (changePageSize) {
        $.ajax({
            url: '/admin/ship/ListShip',
            type: 'get',
            data: {
                page: pg.pageIndex,
                pageSize: pg.pageSize
            },
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var html = '';
                    var template = $('#data-template').html();
                    let totalprice = data.totalprice;
                    var dem = 0;
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            ID: value.ID,
                            Name: value.ShipName,
                            Phone: value.ShipMobile,
                            Totalprice: new Intl.NumberFormat().format(totalprice[dem]),
                            CREATEDATE: parseJsonDate(value.CreatedDate),
                            Email: value.ShipEmail,
                            Status: Status(value.Status),
                            st: value.Status
                        });
                        dem++;
                    });
                    $('#tbOrder').html(html);
                    userController.paging(data.total, function () {
                        userController.listOrder();
                    }, changePageSize)
                    userController.listOrderDetail();

                }
            }
        });
        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${(dt.getMonth() + 1).toString().padStart(2, '0')} /${dt.getDate().toString().padStart(2, '0')}/${dt.getFullYear().toString().padStart(4, '0')} ${dt.getHours().toString().padStart(2, '0')}: ${dt.getMinutes().toString().padStart(2, '0')}: ${dt.getSeconds().toString().padStart(2, '0')}`
        };
        function Status(value) {
            if (value == 0) {
                return "<span class=\"badge bg-danger\">Chưa giao hàng</span>";
            }
            if (value == 1) {
                return "<span class=\"badge bg-warning\">Đang giao hàng</span>"
            }
            if (value == 2) {
                return "<span class=\"badge bg-success\">Hoàn thành</span>"
            }
        }
    },
    listOrderDetail: function () {
        $('.modalshowsub').on('click', function () {

            var id = $(this).data('id');
            var status = $(this).data('status');
            $('#suborderdetail').modal('show')
            $.ajax({
                url: '/admin/order/ListOrderDetail',
                type: 'get',
                data: {
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {
                        var html = '';
                        var template = $('#data-tbOrderDetail').html();
                        var dem = 1;
                        $.each(data.data, function (key, value) {
                            html += Mustache.render(template, {
                                STT: dem,
                                ID: value.Product.ID,
                                Name: value.Product.Name,
                                Price: new Intl.NumberFormat().format(value.Price),
                                Quantity: value.Quantity,
                                Totalprice: new Intl.NumberFormat().format(value.TotalPrice),
                            });
                            dem++;

                        });
                        $('#tbOrderDetail').html(html);

                        var quantitysub = 0;
                        var total = 0;
                        $.each(data.data, function (key, value) {
                            quantitysub += value.Quantity;
                            total += value.TotalPrice
                        })

                        $('#quantitysub').text(quantitysub);
                        $('#total').text(new Intl.NumberFormat().format(total));
                        $('#totalprice').text(new Intl.NumberFormat().format(total));
                        if (status == 0) {
                            $(".btn-change-status").removeClass("btn btn-danger").addClass("btn btn-warning");
                            $(".btn-change-status").text("Giao hàng");

                            $(".btn-change-status").off('click').on('click', function () {
                                var check = $(".btn-change-status").text();
                                if (check == "Giao hàng") {
                                    $.ajax({
                                        url: '/admin/ship/OrderShip',
                                        type: 'post',
                                        data: {
                                            idorder: id
                                        },
                                        dataType: 'json',
                                        success: function (data) {
                                            if (data.code == 200) {
                                                $('#suborderdetail').modal('hide');
                                                Toast.fire({
                                                    icon: 'success',
                                                    title: 'Đơn hàng đang giao!.',
                                                })
                                                userController.listOrder(true);
                                            }
                                        }
                                    })
                                }
                            })
                        }
                        if (status == 1) {
                            $(".btn-change-status").removeClass("btn btn-warning").addClass("btn btn-primary");
                            $(".btn-change-status").text("Giao hàng thành công");
                            $(".btn-change-status").off('click').on('click', function () {
                                var check = $(".btn-change-status").text();
                                if (check == "Giao hàng thành công") {
                                    $.ajax({
                                        url: '/admin/ship/OrderShipSuccess',
                                        type: 'post',
                                        data: {
                                            idorder: id
                                        },
                                        dataType: 'json',
                                        success: function (data) {
                                            if (data.code == 200) {
                                                $('#suborderdetail').modal('hide');
                                                Toast.fire({
                                                    icon: 'success',
                                                    title: 'Giao hàng thành công!.',
                                                })
                                                userController.listOrder(true);
                                            }
                                        }
                                    })
                                }
                            })
                        }
                        if (status == 2) {
                            $(".btn-change-status").removeClass("btn btn-primary btn btn-warning").addClass("btn btn-success");
                            $(".btn-change-status").text("Hoàn thành");
                        }
                    }
                }
            });
        });



    },
    createAccount: function () {

        $('.btn-create').off('click').on('click', function () {
            if ($('#frmCreate').valid()) {
                var user = new Object();
                user.UserName = $("#inputUserNameCreate").val();
                user.Password = $("#inputPasswordCreate").val();
                user.Name = $("#inputNameCreate").val();
                user.Email = $("#inputEmailCreate").val();
                user.Address = $('#tinhCreate').val();
                user.Phone = $('#inputPhoneCreate').val();
                user.Status = $("[name='statusCreate']").prop('checked');
                if (user != null) {
                    $.ajax({
                        url: '/admin/user/create',
                        type: 'post',
                        datatype: 'json',
                        data: user,
                        success: function (data) {
                            if (data.code == 200) {
                                $('#modal-create').modal('hide');
                                $(':input', '#modal-create').not('#accountType').val('').removeAttr('checked')
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thêm thành công!.',
                                })
                                userController.listAccount(true);
                            } else {
                                Toast.fire({
                                    icon: 'error',
                                    title: 'Thêm thất bại!.',
                                })
                            }
                        }
                    });
                }
            }
        });
    },
    deleteAccount: function () {
        $('.btn-delete').off('click').on('click', function () {
            var id = $(this).data('id');
            $.ajax({
                url: '/admin/user/Update',
                type: 'get',
                data: {
                    id: id
                },
                success: function (data) {
                    if (data.code = 200) {
                        $('#inputUserNameDelete').val(data.data.UserName);
                        $('.btn-delete-final').off('click').on('click', function () {
                            $.ajax({
                                url: '/admin/user/delete',
                                type: 'post',
                                data: {
                                    id: id
                                },
                                success: function (data) {
                                    if (data.code == 200) {
                                        Toast.fire({
                                            icon: 'success',
                                            title: 'Xóa thành công!.',
                                        })
                                        userController.listAccount(true);
                                    } else {
                                        Toast.fire({
                                            icon: 'error',
                                            title: 'Xóa thất bại!.',
                                        })
                                    }
                                }
                            });
                        })
                    }
                }
            })
        });

    },
    editAccount: function () {
        $('.btn-edit').off('click').on('click', function () {
            var id = $(this).data('id');
            //Xem chi tiết Account
            userController.getDetailAccount(id);
            //Lưu Account
            userController.saveAccount(id);
        });
    },
    getDetailAccount: function (id) {
        $.ajax({
            url: '/admin/user/Update',
            type: 'get',
            data: {
                id: id
            },
            success: function (data) {
                if (data.code = 200) {
                    $('#inputName').val(data.data.Name);
                    $('#inputEmail').val(data.data.Email);
                    $('#inputPhone').val(data.data.Phone);
                    $('#tinh').val(data.data.Address).change();
                    $("[name='status']").prop('checked', data.data.Status);
                }

            }

        })


    },
    saveAccount: function (id) {
        $('.save').off('click').on('click', function () {
            var user = new Object();
            user.ID = id;
            user.Name = $('#inputName').val();
            user.Email = $('#inputEmail').val();
            user.Address = $('#tinh').val();
            user.Phone = $('#inputPhone').val();
            user.Status = $("[name='status']").prop('checked')
            if (user != null) {
                $.ajax({
                    type: 'post',
                    url: '/admin/user/Update',
                    data: user,
                    dataType: "json",
                    success: function (data) {
                        if (data.code == 200) {
                            Toast.fire({
                                icon: 'success',
                                title: 'Sửa thành công!.',
                            })
                            userController.listAccount();
                        } else {
                            Toast.fire({
                                icon: 'error',
                                title: 'Sửa thất bại!.'
                            })
                        }
                    }
                });
            }

        });

    },
    paging: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pg.pageSize)
        if ($('#pagination a').length === 0 || changePageSize === true) {
            $('#pagination').empty();
            $('#pagination').removeData('twbs-pagination');
            $('#pagination').unbind("page");
        }
        $('#pagination').twbsPagination({
            totalPages: totalPage,
            visiblePages: 7,
            first: 'Đầu',
            next: 'Tiếp',
            last: 'Cuối',
            prev: 'Trước',
            onPageClick: function (event, page) {
                pg.pageIndex = page;
                setTimeout(callback, 100);
            }
        });
    },
    search: function () {
        $('#search-text').on('keyup', function () {
            userController.listAccount(true)
        });
        $('#search-dropdown').change(function () {
            userController.listAccount(true)
        });

    },
    validate: function () {
        $('#frmCreate').validate({
            rules: {
                inputUserNameCreated: 'required',
                inputPasswordCreate: 'required'
            },
            messages: {
                inputUserNameCreated: 'Tài khoản không được bỏ trống',
                inputPasswordCreate: 'Mật khẩu không được bỏ trống'
            },
        })
    },
}
userController.init();

